using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add HttpClient and CharacterService
builder.Services.AddHttpClient<ICharacterService, CharacterService>();
builder.Services.AddTransient<ICharacterService, CharacterService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins");

app.MapGet("/characters", async (ICharacterService characterService) =>
{
    var characters = await characterService.GetCharactersAsync();
    return Results.Ok(characters);
})
.WithName("GetCharacters")
.WithOpenApi();

app.Run();

// Define the service and controller logic here
public interface ICharacterService
{
    Task<IEnumerable<Character>> GetCharactersAsync();
}

public class CharacterService : ICharacterService
{
    private readonly HttpClient _httpClient;
    private readonly string apiKey = "c6467f7a89d87f238836985b8267535a";
    private readonly string hash = "611722bd23b83c986232861375be041d";

    public CharacterService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Character>> GetCharactersAsync()
    {
        var response = await _httpClient.GetStringAsync($"https://gateway.marvel.com:443/v1/public/characters?ts=1&apikey={apiKey}&hash={hash}");
        var data = JObject.Parse(response);
        return data["data"]["results"].ToObject<IEnumerable<Character>>();
    }
}

public class Character
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Modified { get; set; }
    public Thumbnail Thumbnail { get; set; }
    public Comics Comics { get; set; }
    public Series Series { get; set; }
    public Stories Stories { get; set; }
    public Events Events { get; set; }
}

public class Thumbnail
{
    public string Path { get; set; }
    public string Extension { get; set; }
}

public class Comics
{
    public int Available { get; set; }
    public string CollectionURI { get; set; }
    public List<ComicItem> Items { get; set; }
}

public class ComicItem
{
    public string ResourceURI { get; set; }
    public string Name { get; set; }
}

public class Series
{
    public int Available { get; set; }
    public string CollectionURI { get; set; }
    public List<SeriesItem> Items { get; set; }
}

public class SeriesItem
{
    public string ResourceURI { get; set; }
    public string Name { get; set; }
}

public class Stories
{
    public int Available { get; set; }
    public string CollectionURI { get; set; }
    public List<StoryItem> Items { get; set; }
}

public class StoryItem
{
    public string ResourceURI { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
}

public class Events
{
    public int Available { get; set; }
    public string CollectionURI { get; set; }
    public List<EventItem> Items { get; set; }
}

public class EventItem
{
    public string ResourceURI { get; set; }
    public string Name { get; set; }
}
