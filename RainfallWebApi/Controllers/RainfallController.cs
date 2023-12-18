using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RainfallApi.Models;

namespace RainfallWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RainfallController : ControllerBase
{
    private const string RainfallApiBaseUrl = "https://environment.data.gov.uk/flood-monitoring/id/stations/";

    [HttpGet("id/{stationId}/readings")]
    public async Task<IActionResult> GetRainfallReadings(string stationId, [FromQuery] int count = 10)
    {
        string rainfallApiUrl = $"{RainfallApiBaseUrl}{stationId}/readings?_limit={count}";

        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(rainfallApiUrl);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                // Assuming the structure of the response is similar to RainfallApiResponse
                var rainfallApiResponse = JsonConvert.DeserializeObject<RainfallReadingResponse>(responseBody);

                return Ok(rainfallApiResponse);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }
            else
            {
                return StatusCode(500, new ErrorResponse { Message = "Internal server error" });
            }
        }
    }
}
