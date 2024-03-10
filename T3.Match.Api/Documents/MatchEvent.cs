using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace T3.Match.Api.Documents;

// [JsonDerivedType(typeof(MatchEventScore), typeDiscriminator: "score")]
// [JsonDerivedType(typeof(MatchEventServiceChange), typeDiscriminator: "serviceChange")]
// [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
public abstract class MatchEvent
{
    public required ObjectId Id { get; set; }
    public required ObjectId? PreviousId { get; set; }
    public required DateTime Timestamp { get; set; }
    public required MatchAuthor Author { get; set; }
}

public class MatchEventScore : MatchEvent
{
    public required string Score { get; set; }
}

public class MatchEventServiceChange : MatchEvent
{
    public required MatchPlayer[] Servers { get; set; }
    public required MatchPlayer[] Receivers { get; set; }
}