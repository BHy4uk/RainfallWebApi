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

                if (rainfallApiResponse.Items == null || rainfallApiResponse.Items.Count == 0)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = $"No readings found for the specified stationId: {stationId}",
                        Detail = new[]
                        {
                            new ErrorDetail
                            {
                                PropertyName = "stationId",
                                Message = $"The specified stationId '{stationId}' does not have any readings."
                            }
                        }
                    });
                }

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
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(await response.Content.ReadAsStringAsync());
                return BadRequest(errorResponse);
            }
            else
            {
                var errorResponse = new ErrorResponse
                {
                    Message = "Internal server error",
                    Detail = new[]
                    {
                        new ErrorDetail
                        {
                            PropertyName = "ServerError",
                            Message = "An unexpected server error occurred."
                        }
                    }
                };
                return StatusCode(500, errorResponse);
            }
        }
    }
}
