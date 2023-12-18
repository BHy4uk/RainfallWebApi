namespace RainfallApi.Models;

public class RainfallApiResponse
{
    public List<RainfallReadingItem> Items { get; set; }
}

public class RainfallReadingItem
{
    public string DateTime { get; set; }
    public decimal Value { get; set; }
}
