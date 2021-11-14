using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using NSchemaRegistry;

namespace SchemaValidationExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
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
            
            var validationResult = await SchemaValidator.Validate(JsonSerializer.Serialize(payload, new JsonSerializerOptions {PropertyNameCaseInsensitive = true}), "Accounts.AccountChanged", 1);
            Console.WriteLine($"Validation result: {validationResult.Success};");


            var notValidPayload = new
            {
                event_id = Guid.NewGuid(),
                event_name = "AccountChanged",
                event_version = 1,
                data = new
                {
                    PublicId = Guid.NewGuid(),
                    Username = "Johndoe",
                    Role = 1
                }
            };

            var validationResult2 = await SchemaValidator.Validate(JsonSerializer.Serialize(notValidPayload), "Accounts.AccountChanged", 1);
            Console.WriteLine($"Validation result: {validationResult2.Success}; errors: [{string.Join(',', validationResult2.Errors.Select(e => $"{e.Property}: {e.Error}"))}]");
        }
    }
}
