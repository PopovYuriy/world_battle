using System.Threading.Tasks;

namespace Core.Commands
{
    public interface ICommand
    {
        void Execute();
    }

    public interface ICommandAsync
    {
        Task Execute();
    }
}