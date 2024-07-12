using AisStreamService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//create a connectionstring to the database using the environment variables: DBname, DBServer, DBuser, DBpassword

var connectionString = $"Server={Environment.GetEnvironmentVariable("DBServer")};Database={Environment.GetEnvironmentVariable("DBname")};User={Environment.GetEnvironmentVariable("DBUser")};Password={Environment.GetEnvironmentVariable("DBpassword")};";


builder.Services.AddDbContext<AisDbContext>(options =>
    options.UseMySql(connectionString,
        new MariaDbServerVersion(new Version(10, 6, 5))));

builder.Services.AddHostedService<AisStreamService.Services.AisBackgroundService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AisDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();