# NSchemaRegistry
Simple Schema Registry based on https://github.com/RicoSuter/NJsonSchema

Example schema is located under _NSchemaRegistry/Schemas/Accounts/AccountChanged_

```csharp
var payload = new
{
    event_id = Guid.NewGuid(),
    event_name = "AccountChanged",
    event_version = 1,
    data = new
    {
        PublicId = Guid.NewGuid(),
        Username = "Johndoe",
        Role = "user"
    }
};
            
var validationResult = await SchemaValidator.Validate(JsonSerializer.Serialize(payload), "Accounts.AccountChanged", 1);
Console.WriteLine($"Validation result: {validationResult.Success};");

```

When creating a new schema it's neccessary to enable the "Copy To Output Directory" property for a json file in Visual Studio
