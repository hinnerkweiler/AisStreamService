using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AisStreamService.Data;
using AisStreamService.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace AisStreamService.Services
{
    public class AisBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AisBackgroundService> _logger;
        private readonly string _apiUrl = "wss://stream.aisstream.io/v0/stream";
        private readonly string _apiKey;

        public AisBackgroundService(IServiceProvider serviceProvider, ILogger<AisBackgroundService> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _apiKey = System.Environment.GetEnvironmentVariable("AIS_API_KEY") ?? "";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var websocket = new ClientWebSocket();
                    await websocket.ConnectAsync(new Uri(_apiUrl), stoppingToken);
                    _logger.LogInformation("Connected to AISStream WebSocket.");
                    
                    var area = Environment.GetEnvironmentVariable("Area") ?? "-90,-180,90,180";
                    var areaBoundary = area.Split(",").Select(double.Parse).ToArray();

                    var mmsiList = Environment.GetEnvironmentVariable("Boats");
                    var boundingBox = new double[][] { new double[] { areaBoundary[0], areaBoundary[1] }, new double[] { areaBoundary[2], areaBoundary[3] } };
                    
                    var subscriptionMessage = new
                    {
                        APIKey = _apiKey,
                        BoundingBoxes = new[] { boundingBox },
                        FilterMessageTypes = new[] { "PositionReport", "StandardClassBPositionReport", "ShipStaticData"},
                        FiltersShipMMSI = mmsiList.Split(",")
                    };

                    var message = JsonSerializer.Serialize(subscriptionMessage);
                    var buffer = Encoding.UTF8.GetBytes(message);
                    await websocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, stoppingToken);
                    _logger.LogInformation("Subscription message sent.");

                    var responseBuffer = new byte[1024 * 4];

                    while (websocket.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
                    {
                        var result = await websocket.ReceiveAsync(new ArraySegment<byte>(responseBuffer), stoppingToken);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            _logger.LogWarning("WebSocket connection closed by the server.");
                            await websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the server", stoppingToken);
                            break;
                        }

                        var responseString = Encoding.UTF8.GetString(responseBuffer, 0, result.Count);
                        _logger.LogInformation("AIS-A PositionReport: {responseString.ToString()}", responseString);
                        var aisStreamResponse = JsonSerializer.Deserialize<AisStreamResponse>(responseString);

                        if (aisStreamResponse?.MessageType == "StandardClassBPositionReport")
                        {
                            _logger.LogInformation("AIS-B PositionReport: {responseString.ToString()}", responseString);
                        }
                    }
                }
                catch (System.Text.Json.JsonException ex)
                {
                    _logger.LogError(ex, "An error occurred while deserializing the AIS data.");
                }
                catch (WebSocketException ex)
                {
                    _logger.LogError(ex, "An error occurred in the AIS WebSocket connection.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in the AIS background service.");
                }

                // Reconnect after a short delay
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private async Task StoreAisDataAsync(AisStreamService.Models.AisStreamResponse responseMessage)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AisDbContext>();
            
            if (responseMessage.MessageType == "StandardClassBPositionReport")
            {
                var vessel = await dbContext.Vessels.FirstOrDefaultAsync(v => v.Mmsi == responseMessage.Message.StandardClassBPositionReport.UserID);

                if (vessel == null)
                {
                    vessel = new Vessel
                    {
                        Mmsi = responseMessage.Message.StandardClassBPositionReport.UserID,
                        ShipName = responseMessage.MetaData.ShipName,
                        Latitude = responseMessage.Message.StandardClassBPositionReport.Latitude,
                        Longitude = responseMessage.Message.StandardClassBPositionReport.Longitude,
                        LastUpdated = DateTime.UtcNow,
                        Speed = responseMessage.Message.StandardClassBPositionReport.Sog,
                        Course = responseMessage.Message.StandardClassBPositionReport.Cog,
                    };
                    dbContext.Vessels.Add(vessel);
                }
                else
                {
                    vessel.Latitude = responseMessage.Message.StandardClassBPositionReport.Latitude;
                    vessel.Longitude = responseMessage.Message.StandardClassBPositionReport.Longitude;
                    vessel.LastUpdated = DateTime.UtcNow;
                }

                await dbContext.SaveChangesAsync();
            }
            else if (responseMessage.MessageType == "PositionReport")
            {
                var vessel =
                    await dbContext.Vessels.FirstOrDefaultAsync(v =>
                        v.Mmsi == responseMessage.Message.PositionReport.UserID);

                if (vessel == null)
                {
                    vessel = new Vessel
                    {
                        Mmsi = responseMessage.Message.PositionReport.UserID,
                        ShipName = responseMessage.MetaData.ShipName,
                        Latitude = responseMessage.Message.PositionReport.Latitude,
                        Longitude = responseMessage.Message.PositionReport.Longitude,
                        LastUpdated = DateTime.UtcNow,
                        Speed = responseMessage.Message.PositionReport.Sog,
                        Course = responseMessage.Message.PositionReport.Cog,

                    };
                    dbContext.Vessels.Add(vessel);
                }
                else
                {
                    vessel.Latitude = responseMessage.Message.PositionReport.Latitude;
                    vessel.Longitude = responseMessage.Message.PositionReport.Longitude;
                    vessel.LastUpdated = DateTime.UtcNow;
                }

                await dbContext.SaveChangesAsync();
            }
            else if (responseMessage.MessageType == "ShipStaticData")
            {
                var vessel = await dbContext.Vessels.FirstOrDefaultAsync(v => v.Mmsi == responseMessage.Message.ShipStaticData.UserID);

                if (vessel == null)
                {
                    vessel = new Vessel
                    {
                        Mmsi = responseMessage.Message.ShipStaticData.UserID,
                        ShipName = responseMessage.Message.ShipStaticData.Name,
                    };
                    dbContext.Vessels.Add(vessel);
                }
                else
                {
                    if (vessel.ShipName != responseMessage.Message.ShipStaticData.Name)
                    {
                        _logger.LogInformation("Updating ship name for MMSI {Mmsi} from {OldName} to {NewName}.", 
                            vessel.Mmsi, vessel.ShipName, responseMessage.Message.ShipStaticData.Name);
                        
                        vessel.ShipName = responseMessage.Message.ShipStaticData.Name;
                        vessel.LastUpdated = DateTime.UtcNow;
                    }
                }

                await dbContext.SaveChangesAsync();
            }
            {
                
            }
        }
    }
}
