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
        // Limit count to the range [1, 100]
        count = Math.Max(1, Math.Min(count, 100));

        string rainfallApiUrl = $"{RainfallApiBaseUrl}{stationId}/readings?_limit={count}";

        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(rainfallApiUrl);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var rainfallApiResponse = JsonConvert.DeserializeObject<RainfallApiResponse>(responseBody);

                var rainfallReadingResponse = new RainfallReadingResponse
                {
                    Readings = rainfallApiResponse.Items
                        .Select(item => new RainfallReading
                        {
                            DateMeasured = DateTime.Parse(item.DateTime),
                            AmountMeasured = item.Value
                        })
                        .ToArray()
                };

                return Ok(rainfallReadingResponse);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return BadRequest();
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
