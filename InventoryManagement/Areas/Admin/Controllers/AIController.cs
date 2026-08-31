using System.Text;
using System.Text.Json;
using Inventory.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AIController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUnitOfWork _unitOfWork;

        public AIController(
            IHttpClientFactory httpClientFactory,
            IUnitOfWork unitOfWork)
        {
            _httpClientFactory = httpClientFactory;
            _unitOfWork = unitOfWork;
        }

     
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        
        [HttpPost]
        [Route("/api/ai/inventory-advice")]
        public async Task<IActionResult> InventoryAdvice(
            [FromBody] InventoryAdviceRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.ProductName))
            {
                return BadRequest(new
                {
                    message = "Product name is required."
                });
            }

            
            var productName = request.ProductName.Trim().ToLower();

            var supply = await _unitOfWork.LabSupply.GetAsync(
                u => u.SupplyName.ToLower() == productName
            );

           
            if (supply == null)
            {
                return NotFound(new
                {
                    message = $"Product '{request.ProductName}' was not found in inventory."
                });
            }

           
            int currentStock = supply.QuantityOnHand;
            int reorderPoint = supply.ReorderPoint;

            
            var apiKey =
                Environment.GetEnvironmentVariable("GEMINI_API_KEY");

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return StatusCode(500, new
                {
                    message = "GEMINI_API_KEY is not configured."
                });
            }

           
            var prompt = $"""
                You are an AI inventory assistant for a laboratory inventory management system.

                Analyze the following inventory item:

                Product: {supply.SupplyName}
                Current Stock: {currentStock}
                Reorder Point: {reorderPoint}

                Give a short and practical inventory recommendation.

                Include:
                1. Whether the product should be reordered.
                2. Why it should or should not be reordered.
                3. A suggested reorder quantity.

                If the current stock is below the reorder point,
                calculate the minimum quantity required to reach
                the reorder point.

                Keep the response under 100 words.
                """;

            
            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = prompt
                            }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(payload);

            var client = _httpClientFactory.CreateClient();

            var url =
                "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

            using var requestMessage = new HttpRequestMessage(
                HttpMethod.Post,
                url);

            requestMessage.Headers.Add(
                "x-goog-api-key",
                apiKey);

            requestMessage.Content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            
            var response = await client.SendAsync(requestMessage);

            var responseBody =
                await response.Content.ReadAsStringAsync();

          
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(
                    (int)response.StatusCode,
                    new
                    {
                        message = "Gemini AI service request failed.",
                        details = responseBody
                    });
            }

           
            using var document =
                JsonDocument.Parse(responseBody);

            string outputText = "";

            if (document.RootElement.TryGetProperty(
                    "candidates",
                    out var candidates) &&
                candidates.GetArrayLength() > 0)
            {
                var candidate = candidates[0];

                if (candidate.TryGetProperty(
                        "content",
                        out var content) &&
                    content.TryGetProperty(
                        "parts",
                        out var parts))
                {
                    foreach (var part in parts.EnumerateArray())
                    {
                        if (part.TryGetProperty(
                                "text",
                                out var text))
                        {
                            outputText +=
                                text.GetString();
                        }
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(outputText))
            {
                return StatusCode(500, new
                {
                    message = "Gemini returned an empty response."
                });
            }

          
            return Ok(new
            {
                product = supply.SupplyName,
                currentStock = currentStock,
                reorderPoint = reorderPoint,
                recommendation = outputText
            });
        }
    }

    public class InventoryAdviceRequest
    {
        public string ProductName { get; set; } = string.Empty;
    }
}
