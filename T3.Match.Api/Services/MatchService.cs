using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Driver;
using T3.Match.Api.Documents;
using T3.Match.Api.Dto;

namespace T3.Match.Api.Services;

public class MatchService
{
    private readonly IMongoDatabase _db;

    public MatchService(IMongoDatabase db)
    {
        _db = db;
    }

    private IMongoCollection<MatchDoc> Collection => _db.GetCollection<MatchDoc>("matches");
    
    public async Task<JsonElement> Create(CreateMatchRequest request)
    {
        var doc = new MatchDoc()
        {
            Id = ObjectId.GenerateNewId()
        };
        await Collection.InsertOneAsync(doc);

        return doc.ToJson();
    }

    public async Task<JsonElement> Get(string id)
    {
        var doc = await Collection.Find(x => x.Id == ObjectId.Parse(id)).FirstOrDefaultAsync();
        return doc.ToJson();
    }

    public async Task<JsonElement> AddServiceChange(string id, ServiceChangeRequest request)
    {
        MatchEvent matchEvent = new MatchEventServiceChange()
        { 
            Id = ObjectId.GenerateNewId(),
            PreviousId = request.previousId != null ? ObjectId.Parse(request.previousId) : null,
            Author = new MatchAuthor(){ DisplayName = "Wouter"},
            Timestamp = DateTime.Now,
            Servers = new []
            {
                new MatchPlayer() {DisplayName = request.Server }
            },
            Receivers = new []
            {
                new MatchPlayer() {DisplayName = request.Receiver }
            },
        };

        return await AddChange(id, matchEvent);
    }

    public async Task<JsonElement> AddScoreChange(string id, ScoreChangeRequest request)
    {
        MatchEvent matchEvent = new MatchEventScore()
        {
            Id = ObjectId.GenerateNewId(),
            PreviousId = request.previousId != null ? ObjectId.Parse(request.previousId) : null,
            Author = new MatchAuthor(){ DisplayName = "Wouter"},
            Timestamp = DateTime.Now,
            Score = request.Score,
        };
        return await AddChange(id, matchEvent);
    }

    private async Task<JsonElement> AddChange(string id, MatchEvent matchEvent)
    {
        var filter = Builders<MatchDoc>.Filter.Eq(x => x.Id, ObjectId.Parse(id));
        var update = Builders<MatchDoc>.Update.Push(x => x.Events, matchEvent);

        await Collection.UpdateOneAsync(filter, update);
        return await Get(id);
    }
}


public static class MatchExtensions
{
    public static JsonElement ToJson(this MatchDoc doc)
    {
        var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        jsonSerializerOptions.Converters.Add(new ObjectIdToStringConvertor());
        jsonSerializerOptions.TypeInfoResolver = new MatchEventPolymorphicTypeResolver();
        return JsonSerializer.SerializeToElement(doc, jsonSerializerOptions);
    }
}