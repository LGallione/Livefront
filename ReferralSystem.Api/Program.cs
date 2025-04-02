using ReferralSystem.Api.Services;
using ReferralSystem.Api.Middleware;
using Microsoft.OpenApi.Models;

namespace ReferralSystem.Api;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Register our services
        builder.Services.AddSingleton<IThirdPartyReferralService, MockThirdPartyReferralService>();
        builder.Services.AddSingleton<IReferralService>(sp => 
            new MockReferralService(sp.GetRequiredService<IThirdPartyReferralService>()));

        // Configure CORS
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseCors();

        // Add abuse prevention middleware
        app.UseReferralAbusePrevention();

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
