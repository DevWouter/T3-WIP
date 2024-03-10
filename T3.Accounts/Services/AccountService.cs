using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using T3.Accounts.Dto;

namespace T3.Accounts.Services;

public class AccountService
{
    public class Settings
    {
        public string ServerKey { get; set; }
    }
    
    private readonly IMongoDatabase _database;
    private readonly IOptions<Settings> _settings;

    public AccountService(IMongoDatabase database, IOptions<Settings> settings)
    {
        _database = database;
        _settings = settings;
    }
    
    public async Task<List<UserDoc>> GetAll()
    {
        return await Collection.Find(Builders<UserDoc>.Filter.Empty).ToListAsync();
    }
    
    public async Task<UserDoc> GetByName(string username)
    {
        var filter = Builders<UserDoc>.Filter.Eq(x => x.Username, username);
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    private IMongoCollection<UserDoc> Collection => _database.GetCollection<UserDoc>("users");

    public async Task<UserDoc> GetById(string id)
    {
        var filter = Builders<UserDoc>.Filter.Eq(x=>x.Id, ObjectId.Parse(id));
        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<UserDoc> Create(string username, string password)
    {
        var collection = _database.GetCollection<UserDoc>("users");
        var user = new UserDoc
        {
            Id = ObjectId.GenerateNewId(),
            Username = username,
            Password = password
        };
        await collection.InsertOneAsync(user);
        return user;
    }
    
    // Delete method
    public async Task<bool> Delete(string id)
    {
        var filter = Builders<UserDoc>.Filter.Eq(x=>x.Id, ObjectId.Parse(id));
        var result = await Collection.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }

    /// <summary>
    /// If a valid username/password it will return a login.
    /// </summary>
    public async Task<T3Token> Login(string username, string password)
    {
        await Task.CompletedTask; // TODO: Replace with an actual implementation
        return new T3Token(_settings.Value.ServerKey, "fake-token");
    }

    public async Task<VerifyResponse> Verify(T3Token token)
    {
        await Task.CompletedTask; // TODO: Replace with an actual implementation
        return new VerifyResponse(token.ServerKey == _settings.Value.ServerKey);
    }
}