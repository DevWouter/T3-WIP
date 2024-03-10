using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using T3.Accounts.Api.Dto;
using T3.Accounts.Dto;
using T3.Accounts.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IMongoClient>(_ => new MongoClient("mongodb://root:example@localhost:27017"));
builder.Services.AddTransient<IMongoDatabase>(x => x.GetRequiredService<IMongoClient>().GetDatabase("t3"));
builder.Services.AddTransient<AccountService>();
builder.Services.Configure<AccountService.Settings>(builder.Configuration.GetSection(nameof(AccountService)));


var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/accounts", async (
    [FromServices] AccountService service
) => (await service.GetAll()).Select(AccountExtension.ToDto));

app.MapPost("/accounts", async (
    [FromServices] AccountService service,
    [FromBody] CreateAccountDto body
) => (await service.Create(body.Username, body.Password)).ToDto());

app.MapPost("/accounts/{id}", async (
    [FromServices] AccountService service,
    [FromRoute] string id
) => (await service.GetById(id)).ToDto());


app.MapPost("/login", async (
    [FromServices] AccountService service,
    [FromBody] LoginDto body
) => await service.Login(body.Username, body.Password));

app.MapPost("/verify", async (
    [FromServices] AccountService service,
    [FromBody] T3Token token
) => await service.Verify(token));

app.Run();