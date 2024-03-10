using MongoDB.Bson;

namespace T3.Accounts;

/// <summary>
/// A document that describes the user
/// </summary>
public class UserDoc
{
    public ObjectId Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}
