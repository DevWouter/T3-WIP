using MongoDB.Bson;

namespace T3.Match.Api.Documents;

public class MatchDoc
{
    public ObjectId Id { get; set; }
    public MatchEvent[] Events { get; set; } = Array.Empty<MatchEvent>();
}
