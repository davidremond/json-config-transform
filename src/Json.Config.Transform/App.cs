using Json.Config.Transform.Services;
using McMaster.Extensions.CommandLineUtils;
using System;

namespace Json.Config.Transform
{
    public class App
    {
        public App(ITransformationService transformationService, IConsole console)
        {
            TransformationService = transformationService;
            Console = console;
        }

        public const string Name = "json-config-transform";
        public const string Description = "Provides commands for applying JSON configuration transformation to JSON files.";

        public IConsole Console { get; }
        protected ITransformationService TransformationService { get; }

        public int Execute(string[] args)
        {
            var app = new CommandLineApplication<App>
            {
                Name = Name
            };

            app.Conventions
                .UseDefaultConventions();

            app.Description = "Provides commands for applying JSON configuration transformation to JSON files.";

            app.Command("apply", _ =>
            {
                _.Description = "Apply tranformation configuration to a json file with a json configuration file.";

                var sourceFilePath = _.Option("-s|--source", "Specifies the source json file path to transform.", CommandOptionType.SingleValue).IsRequired();
                var configFilePath = _.Option("-c|--config", "Specifies the configuration file path.", CommandOptionType.SingleValue).IsRequired();
                var displayResult = _.Option("-r|--display-result", "Specifies if the transformation result should be displayed.", CommandOptionType.NoValue);

                _.OnExecuteAsync(async t =>
                {
                    try
                    {
                        Console.ResetColor();
                        Console.Write(Figgle.FiggleFonts.Standard.Render(App.Name));
                        Console.WriteLine();

                        await TransformationService.ApplyAsync(sourceFilePath.Value(), configFilePath.Value(), displayResult.HasValue());
                        return 0;
                    }
                    catch (TransformationException e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"ERROR : {e.Message}");
                        Console.WriteLine($"BASE-ERROR : {e.GetBaseException().Message}");
                        Console.WriteLine(e.StackTrace);
                        return 1;
                    }
                    finally
                    {
                        Console.ResetColor();
                    }
                });

                _.OnValidationError((e) =>
                {
                    Console.ResetColor();
                    Console.Write(Figgle.FiggleFonts.Standard.Render(Name));
                    Console.WriteLine();

                    _.ShowHelp();
                    return 1;
                });
            });

            app.OnExecute(() =>
            {
                Console.ResetColor();
                Console.Write(Figgle.FiggleFonts.Standard.Render(Name));
                Console.WriteLine();

                app.ShowHelp();
                return 1;
            });

            app.OnValidationError((_) =>
            {
                Console.ResetColor();
                Console.Write(Figgle.FiggleFonts.Standard.Render(Name));
                Console.WriteLine();

                app.ShowHelp();
                return 1;
            });

            return app.Execute(args);
        }
    }
}