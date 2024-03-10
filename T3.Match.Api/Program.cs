using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using T3.Match.Api.Documents;
using T3.Match.Api.Dto;
using T3.Match.Api.Services;

BsonClassMap.RegisterClassMap<MatchEvent>();
BsonClassMap.RegisterClassMap<MatchEventScore>();
BsonClassMap.RegisterClassMap<MatchEventServiceChange>();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IMongoClient>(_ => new MongoClient("mongodb://root:example@localhost:27017"));
builder.Services.AddTransient<IMongoDatabase>(x => x.GetRequiredService<IMongoClient>().GetDatabase("t3"));
builder.Services.AddTransient<MatchService>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();


app.MapPost("/match", ([FromServices] MatchService service,
    [FromBody] CreateMatchRequest request) => service.Create(request));
app.MapGet("/match/{id}", ([FromServices] MatchService service, string id) => service.Get(id));

app.MapPost("/match/{id}/Score", ([FromServices] MatchService service,
    string id,
    [FromBody] ScoreChangeRequest request) => service.AddScoreChange(id, request));

app.MapPost("/match/{id}/ServiceChange", ([FromServices] MatchService service,
    string id,
    [FromBody] ServiceChangeRequest request) => service.AddServiceChange(id, request));

app.Run();

public class ScoreChangeRequest
{
    public string? previousId { get; set; }
    public string Score { get; set; }
}

public class ServiceChangeRequest
{
    public string? previousId { get; set; }
    public string Server { get; set; }
    public string Receiver { get; set; }
}