using System.Text.Json;
using System.Text.Json.Nodes;

// working with json so nmare of null refs, this would be awful irl but here can guarantee data integrity
#pragma warning disable CS8602
#pragma warning disable CS8604

var input = File.ReadAllText("./input.txt");
var asJson = JsonNode.Parse(input);
Console.WriteLine(Recurse(asJson));

static int Recurse(JsonNode node)
{
    var ans = 0;

    if (node.GetType() == typeof(JsonObject))
    {
        foreach (var thing in node.AsObject())
        {
            if (thing.Value is JsonValue)
            {
                if (thing.Value.GetValue<JsonElement>().ValueKind == JsonValueKind.Number)
                {
                    ans += thing.Value.GetValue<JsonElement>().GetInt32();
                }
                else if (thing.Value.GetValue<JsonElement>().ValueKind == JsonValueKind.String &&
                    thing.Value.GetValue<JsonElement>().GetString().Equals("red"))
                {
                    return 0;
                }
            }
            else
            {
                ans += Recurse(thing.Value);
            }
        }
    }
    else
    {
        foreach (var thing in node.AsArray())
        {
            if (thing is JsonValue) //.GetValue<JsonElement>().TryGetInt32(out var value))
            {
                if (thing.GetValue<JsonElement>().ValueKind == JsonValueKind.Number)
                {
                    ans += thing.GetValue<JsonElement>().GetInt32();
                }
            }
            else
            {
                ans += Recurse(thing);
            }
        }
    }

    return ans;
}