using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json;

namespace Json.Config.Transform.Services
{
    public class TransformationService : ITransformationService
    {
        private readonly Regex regex = new Regex(@"(add|replace|remove|copy|move|test)\s?:\s?(/.*)\s=>(.*)", RegexOptions.Multiline);

        private IConsole _console;

        public TransformationService(IConsole console)
        {
            _console = console;
        }

        public async Task ApplyAsync(string sourceFilePath, string configFilePath, bool displayResult)
        {
            try
            {
                // Check
                if (string.IsNullOrEmpty(sourceFilePath))
                    throw new ArgumentNullException(nameof(sourceFilePath));
                if (string.IsNullOrEmpty(configFilePath))
                    throw new ArgumentNullException(nameof(configFilePath));
                _console.WriteLine("Parameters :");
                _console.WriteLine($"  - Source File Path : {sourceFilePath}");
                _console.WriteLine($"  - Config File Path : {configFilePath}");

                // Get source file content
                var configContent = await File.ReadAllTextAsync(configFilePath);

                // Get operations
                var matches = regex.Matches(configContent);
                JsonPatchDocument patchDocument = new JsonPatchDocument();
                foreach (Match match in matches)
                {
                    var opType = match.Groups[1].Value;
                    var path = match.Groups[2].Value;
                    var value = match.Groups[3].Value.Trim(' ', '\r');
                    object objectValue = null;
                    try
                    {
                        objectValue = JsonConvert.DeserializeObject(value);
                    }
                    catch (Exception)
                    {
                        objectValue = value;
                    }
                    patchDocument.Operations.Add(new Operation
                    {
                        op = opType,
                        path = path,
                        value = objectValue
                    });
                }

                // Apply transformation
                string jsonContent = await File.ReadAllTextAsync(sourceFilePath);
                var jsonObject = JsonConvert.DeserializeObject(jsonContent);
                var errors = new Dictionary<string, string>();
                patchDocument.ApplyTo(jsonObject, err => { errors.Add($"{err.Operation.op} : {err.Operation.path}", err.ErrorMessage); });

                // Log errors
                if (errors.Any())
                {
                    _console.WriteLine();
                    _console.WriteLine("Errors :");
                    foreach (var error in errors)
                    {
                        _console.WriteLine($"  - {error.Key}");
                        _console.WriteLine();
                        _console.WriteLine($"    {error.Value}");
                        _console.WriteLine();
                    }
                }

                // Save file
                jsonContent = JsonConvert.SerializeObject(jsonObject);
                await File.WriteAllTextAsync(sourceFilePath, jsonContent);

                // Log result
                if (displayResult)
                {
                    _console.WriteLine("Result :");
                    _console.WriteLine();
                    using (var stringReader = new StringReader(jsonContent))
                    using (var stringWriter = new StringWriter())
                    {
                        var jsonReader = new JsonTextReader(stringReader);
                        var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
                        jsonWriter.WriteToken(jsonReader);
                        var formatted = stringWriter.ToString();
                        formatted.Split("\r\n").ToList().ForEach(_ => _console.WriteLine(_));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new TransformationException("Unable to apply transformation. See inner exceptions for more details", ex);
            }
        }
    }
}