namespace RainfallApi.Models;

public class ErrorResponse
{
    public string Message { get; set; }
    public ErrorDetail[] Detail { get; set; }
}
