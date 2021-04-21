using System.Threading.Tasks;

namespace Json.Config.Transform.Services
{
    public interface ITransformationService
    {
        Task ApplyAsync(string sourceFilePath, string configFilePath, bool displayResult);
    }
}