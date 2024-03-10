using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using T3.Match.Api.Documents;

namespace T3.Match.Api.Services;

public class MatchEventPolymorphicTypeResolver : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);

        var baseType = typeof(MatchEvent);
        if (jsonTypeInfo.Type != baseType) return jsonTypeInfo;
        
        var polymorphismOptions = new JsonPolymorphismOptions
        {
            TypeDiscriminatorPropertyName = "$type",
            IgnoreUnrecognizedTypeDiscriminators = true,
            UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
        };
            
        // Get all derived types
        var derivedTypes = baseType.Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(baseType))
            .Where(t => !t.IsAbstract)
            .ToList();
        foreach (var derivedType in derivedTypes)
        {
            var typeDiscriminator = CreateTypeDiscriminator(derivedType, baseType);
            var entry = new JsonDerivedType(derivedType, typeDiscriminator);
            polymorphismOptions.DerivedTypes.Add(entry);
        }
        
        jsonTypeInfo.PolymorphismOptions = polymorphismOptions;

        return jsonTypeInfo;
    }

    private static string CreateTypeDiscriminator(Type derivedType, Type baseType)
    {
        var typeDiscriminator = derivedType.Name;
        typeDiscriminator = typeDiscriminator.Substring(baseType.Name.Length);
        typeDiscriminator = typeDiscriminator.Substring(0, 1).ToLower() + typeDiscriminator.Substring(1);
        return typeDiscriminator;
    }
}