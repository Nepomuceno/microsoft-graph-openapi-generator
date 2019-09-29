using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.OData;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace specs_generator
{
    
    class Program
    {
        private static HttpClient _client = new HttpClient();
        static async Task Main(string[] args)
        {
            await GenerateOpenApiDescription("v1.0", "./generated");
            await GenerateOpenApiDescription("beta", "./generated");

        }
        public static async Task  GenerateOpenApiDescription(string version, string outputhPath)
        {
            string url = $"https://graph.microsoft.com/{version}/$metadata";
            IEdmModel model = await GetEdmModel(url);
            OpenApiConvertSettings settings = new OpenApiConvertSettings
            {
                VerifyEdmModel = false,
                EnableOperationId = false,
            };
            OpenApiDocument document = model.ConvertToOpenApi(settings);
            var outputJSON = document.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
            Directory.CreateDirectory(outputhPath);
            File.WriteAllText(Path.Join(outputhPath, $"{version}.json"),outputJSON);
            var outputYAML = document.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);
            File.WriteAllText(Path.Join(outputhPath, $"{version}.yaml"), outputYAML);
        }

        public static async Task<IEdmModel> GetEdmModel(string url)
        {
            string content = await _client.GetStringAsync(url);
            IEdmModel model = CsdlReader.Parse(XElement.Parse(content).CreateReader());
            return model;
        }
    }

    
}
