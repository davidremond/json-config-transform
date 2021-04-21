using Json.Config.Transform.Services;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;

namespace Json.Config.Transform
{
    [HelpOption]
    [Command(Name = App.Name, FullName = App.Description)]
    internal class Program
    {
        public static int Main(string[] args)
        {
            using var services = new ServiceCollection()
                .AddSingleton<App, App>()
                .AddSingleton(PhysicalConsole.Singleton)
                .AddSingleton<ITransformationService, TransformationService>()
                .BuildServiceProvider();

            var app = services.GetService<App>();
            return app.Execute(args);
        }
    }
}