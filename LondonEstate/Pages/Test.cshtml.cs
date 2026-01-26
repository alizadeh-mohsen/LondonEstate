using LondonEstate.Utils.Types;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text.Json.Serialization;

public class TestModel : PageModel
{
    public List<CountryEntry> CountryList { get; set; } = new();

    public void OnGet()
    {
        // Path to your json file
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data\\countryCodes.json");
        var jsonData = System.IO.File.ReadAllText(filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        CountryList = JsonSerializer.Deserialize<List<CountryEntry>>(jsonData, options) ?? new();
    }
}