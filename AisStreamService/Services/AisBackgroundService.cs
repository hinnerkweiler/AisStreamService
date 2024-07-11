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
            _apiKey = configuration["AIS:ApiKey"];
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

                    var boundingBox = new double[][] { new double[] { -90, -180 }, new double[] { 90, 180 } };
                    var subscriptionMessage = new
                    {
                        APIKey = _apiKey,
                        BoundingBoxes = new[] { boundingBox },
                        FilterMessageTypes = new[] { "PositionReport" }
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
                        var aisStreamResponse = JsonSerializer.Deserialize<AisStreamResponse>(responseString);

                        if (aisStreamResponse?.MessageType == "PositionReport")
                        {
                            await StoreAisDataAsync(aisStreamResponse.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in the AIS background service.");
                }

                // Reconnect after a short delay
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private async Task StoreAisDataAsync(PositionReport positionReport)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AisDbContext>();
            var vessel = await dbContext.Vessels.FirstOrDefaultAsync(v => v.Mmsi == positionReport.UserID);

            if (vessel == null)
            {
                vessel = new Vessel
                {
                    Mmsi = positionReport.UserID,
                    ShipName = positionReport.ShipName,
                    Latitude = positionReport.Latitude,
                    Longitude = positionReport.Longitude,
                    LastUpdated = DateTime.UtcNow
                };
                dbContext.Vessels.Add(vessel);
            }
            else
            {
                vessel.Latitude = positionReport.Latitude;
                vessel.Longitude = positionReport.Longitude;
                vessel.LastUpdated = DateTime.UtcNow;
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
