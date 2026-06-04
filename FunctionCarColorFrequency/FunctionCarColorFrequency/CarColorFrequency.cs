using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

using CarColorFrequencyApi.Models;

namespace FunctionCarColorFrequency;

public class CarColorFrequency
{
    private readonly ILogger<CarColorFrequency> _logger;

    public CarColorFrequency(ILogger<CarColorFrequency> logger)
    {
        _logger = logger;
    }

    [Function("CarColorFrequency")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        await TableDataStorage.CheckIfStorageExists();
        switch (req.Method)
        {
            case "GET":
                var colorData = await TableDataStorage.GetColorData();
                return new OkObjectResult(colorData);
            case "POST":
                if (req.Body.CanSeek)
                {
                    req.Body.Position = 0;
                }

                // Set up case-insensitive matching if your JSON properties are lowercase (camelCase)
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // Deserialize directly from the Request.Body stream
                List<ColorData>? list = await JsonSerializer.DeserializeAsync<List<ColorData>>(req.Body, options);

                await TableDataStorage.UpdateColorData(list);

                return new OkObjectResult("POST request received.");
            default:
                _logger.LogWarning("C# HTTP trigger function received an unsupported request method.");
                return new BadRequestObjectResult("Unsupported request method.");
        }
    }
}