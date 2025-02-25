
using System.Text.Json;
using System.Web;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json.Serialization;


class Program
{
    static async Task Main()
    {
        // Prompt user for input
        Console.WriteLine("Enter a company name or WikiData ID (e.g., 'Skyscanner' or 'Q1319169'): ");
        string input = Console.ReadLine()?.Trim();

        string wikiID = input.StartsWith("Q") ? input : await SearchWikiData(input);

        if (!string.IsNullOrEmpty(wikiID))
        {
            await GetCompanyHierarchy(wikiID);
        }
        else
        {
            Console.WriteLine("No valid WikiData ID found.");
        }

        Console.ReadLine();
    }

    // SearchWikiData method
    static async Task<string> SearchWikiData(string searchQuery)
    {
        string apiUrl = $"https://www.wikidata.org/w/api.php?action=wbsearchentities&search={HttpUtility.UrlEncode(searchQuery)}&language=en&format=json&type=item";

        using HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(apiUrl);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error: Unable to fetch data. Status Code: {response.StatusCode}");
            return string.Empty; // Return an empty string if there is an error
        }

        string jsonResponse = await response.Content.ReadAsStringAsync();
        var searchResult = JsonSerializer.Deserialize<WikiSearchResult>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (searchResult != null && searchResult.Search != null && searchResult.Search.Count > 0)
        {
            foreach (var item in searchResult.Search)
            {
                Console.WriteLine($"ID: {item.Id}, Title: {item.Title}, Description: {item.Description}");
            }
            await Task.Delay(2000);
            return searchResult.Search[0].Id; // Return the first ID found
        }
        else
        {
            Console.WriteLine("Deserialization failed or no results found.");
        }

        return string.Empty; // Ensure we always return a value
    }

    // GetCompanyHierarchy method
    static async Task GetCompanyHierarchy(string companyID)
    {
        if (!companyID.StartsWith("wd:"))
        {
            companyID = $"wd:{companyID}";
        }
        string sparqlQuery = $@"
        SELECT ?company ?companyLabel ?parent ?parentLabel WHERE {{
            ?company wdt:P31 wd:Q4830453;  // company
                     wdt:P749* ?parent.   // parent
            FILTER(?company = wd:{companyID})
            SERVICE wikibase:label {{ bd:serviceParam wikibase:language ""en"". }}
        }}";

        
        string endpoint = "https://query.wikidata.org/sparql?format=json&query=" + HttpUtility.UrlEncode(sparqlQuery);


        using HttpClient client = new HttpClient();
        HttpResponseMessage response = await client.GetAsync(endpoint);

        // Check if the response is successful
        //if (!response.IsSuccessStatusCode)
        //{
        //    string errorContent = await response.Content.ReadAsStringAsync();
        //    Console.WriteLine($"Error: Unable to fetch data. Status Code: {response.StatusCode}. Details: {errorContent}");
        //    return;  // Exit early if the request fails
        //}

        // Read the response content as a string
        string jsonResponse = await response.Content.ReadAsStringAsync();

        // Print the raw response to diagnose the issue
        Console.WriteLine("Raw response: ");
        Console.WriteLine(jsonResponse);

        // Attempt to deserialize the response
        try
        {
            var result = JsonSerializer.Deserialize<SparqlResult>(jsonResponse);
            if (result?.Results?.Bindings?.Length > 0)
            {
                Console.WriteLine("\nCompany Hierarchy:");
                foreach (var binding in result.Results.Bindings)
                {
                    Console.WriteLine($"{binding.CompanyLabel.Value} -> {binding.ParentLabel.Value}");
                }
            }
            else
            {
                Console.WriteLine("No parent company found.");
            }
        }
        catch (JsonException jsonEx)
        {
            // Handle JSON parsing errors
            Console.WriteLine($"Error deserializing JSON: {jsonEx.Message}");
        }
        await Task.Delay(2000);
    }
}

// Define your WikiSearchResult and SparqlResult classes here
public class WikiSearchResult
{
    public SearchInfo SearchInfo { get; set; }
    public List<SearchItem> Search { get; set; }
}

public class SearchInfo
{
    public string Search { get; set; }
}

public class SearchItem
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}



public class Binding
{
    public Label CompanyLabel { get; set; }
    public Label ParentLabel { get; set; }
}

public class Label
{
    public string Value { get; set; }
}




   
    public class Display
    {
        [JsonPropertyName("label")]
        public Label Label { get; set; }

        [JsonPropertyName("description")]
        public Description Description { get; set; }
    }

   

    public class Description
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; }
    }
    public class SparqlResult
    {
        public SparqlHead Head { get; set; }
        public SparqlResults Results { get; set; }
    }

    public class SparqlHead { public string[] Vars { get; set; } }

    public class SparqlResults
    {
        public SparqlBinding[] Bindings { get; set; }
    }

    public class SparqlBinding
    {
        public SparqlValue Company { get; set; }
        public SparqlValue CompanyLabel { get; set; }
        public SparqlValue Parent { get; set; }
        public SparqlValue ParentLabel { get; set; }
    }

    public class SparqlValue { public string Value { get; set; } }
