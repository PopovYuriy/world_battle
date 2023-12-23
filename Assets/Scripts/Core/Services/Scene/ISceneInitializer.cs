using System.Threading.Tasks;

namespace Core.Services.Scene
{
    public interface ISceneInitializer
    {
        Task InitializeAsync();
    }
}