using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using NJsonSchema;

namespace NSchemaRegistry
{
    public static class SchemaValidator
    {
        private const string SchemasFolder = "Schemas";

        public static async Task<SchemaValidationResult> Validate(string data, string type, int version = 1)
        {
            var schema = await LoadSchema(type, version);
            var validationErrors = schema.Validate(data);

            if (validationErrors.Any())
            {
                return new SchemaValidationResult
                {
                    Success = false, 
                    Errors = validationErrors.Select(t => new SchemaValidationError(t.Property, t.Kind.ToString("G"))).ToList()
                };
            }

            return new SchemaValidationResult {Success = true};
        }

        private static async Task<JsonSchema> LoadSchema(string type, int version)
        {
            var delimiter = type.IndexOfAny(new[] { '.', '_' });
            var domain = type[..delimiter];
            var schemaTypeName = type[(delimiter + 1)..];

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), SchemasFolder);
            path = Path.Combine(path, domain);
            path = Path.Combine(path, schemaTypeName);
            path = Path.Combine(path, $"{version}.json");
            string jsonSchema;
            try
            {
                jsonSchema = File.ReadAllText(path);
            }
            catch (Exception e)
            {
                throw new SchemaLoadException($"Error reading schema {type} v{version}", e);
            }
            
            return await JsonSchema.FromJsonAsync(jsonSchema);
        }
    }

    public class SchemaLoadException : Exception
    {
        public override string Message { get; }
        public SchemaLoadException(string message)
        {
            Message = message;
        }
        public SchemaLoadException(string message, Exception e) : base(message, e) { }
    }

    public class SchemaValidationResult
    {
        public bool Success { get; set; }
        public IReadOnlyList<SchemaValidationError> Errors { get; set; } = new List<SchemaValidationError>(0);
    }

    public class SchemaValidationError
    {
        public SchemaValidationError(string property, string error)
        {
            Property = property;
            Error = error;
        }

        public string Property { get; set; }
        public string Error { get; set; }
    }
}
