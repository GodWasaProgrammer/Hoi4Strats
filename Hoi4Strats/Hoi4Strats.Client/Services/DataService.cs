using SharedProj;
using SharedProj.DBModels;
using System.Text.Json;

namespace Hoi4Strats.Client.Services;

public class DataService
{
    private List<GuideModel> _approvedGuides = [];
    public List<GuideModel> ApprovedGuides { get { return _approvedGuides; } }


    private List<GuideModel> _rejectedGuides = [];
    public List<GuideModel> RejectedGuides { get { return _rejectedGuides; } }

    private List<GuideModel> _underReviewGuides = [];
    public List<GuideModel> UnderReviewGuides { get { return _underReviewGuides; } }

    private List<GuideModel> _landTemplates = [];
    public List<GuideModel> LandTemplates { get { return _landTemplates; } }

    private List<GuideModel> _airTemplates = [];
    public List<GuideModel> AirTemplates { get { return _airTemplates; } }

    public List<GuideModel> _seaTemplates = [];
    public List<GuideModel> SeaTemplates { get { return _seaTemplates; } }

    private List<GuideModel> _countryGuides = [];
    public List<GuideModel> CountryGuides { get { return _countryGuides; } }

    public bool Built { get { return _built; } }
    private bool _built { get; set; }
    public DataService()
    {
        
    }

    public async Task BuildLists()
    {
        if (!_built)
        {
            try
            {
                var x = await FetchAllGuides();
                _approvedGuides = [];
                _rejectedGuides = [];
                _underReviewGuides = [];

                foreach (var guide in x)
                {
                    if (guide.Status == Review.Approved)
                    {
                        _approvedGuides.Add(guide);
                    }
                    if (guide.Status == Review.Rejected)
                    {
                        _rejectedGuides.Add(guide);
                    }
                    if (guide.Status == Review.UnderReview)
                    {
                        _underReviewGuides.Add(guide);
                    }
                }
                _built = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}, Failed to build lists in DataService");
            }
        }
        else
        {
            Console.WriteLine("DataService Already Initialized");
        }
        if (Built)
        {

            var landtemplates = _approvedGuides.FindAll(x => x.GuideType == GuideTypes.LandTemplate);
            _landTemplates = landtemplates;

            var airtemplates = _approvedGuides.FindAll(x => x.GuideType == GuideTypes.AirTemplate);
            _airTemplates = airtemplates;

            var seatemplates = _approvedGuides.FindAll(x => x.GuideType == GuideTypes.NavalTemplate);

            var countryguides = _approvedGuides.FindAll(x => x.GuideType == GuideTypes.Country);
            _countryGuides = countryguides;
        }
    }

    private async Task<List<GuideModel>> FetchAllGuides()
    {
        var client = new HttpClient { BaseAddress = new Uri("https://localhost:7141/") }; // Ange korrekt basadress för API:et

        try
        {
            var response = await client.GetAsync("/get-guides");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var guides = JsonSerializer.Deserialize<List<GuideModel>>(jsonData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (guides != null)
                {
                    Console.WriteLine($"Fetched {guides.Count} guides for inspector.");
                    Guides = guides;
                    return guides;
                }
            }
            else
            {
                Console.WriteLine($"Failed to fetch guides: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception when fetching guides: {ex.Message}");
        }
        // fallback
        return new List<GuideModel>();
    }
    private List<GuideModel>? Guides { get; set; }

}
