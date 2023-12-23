using Game.Field.Mediators;
using Game.Services.Storage;

namespace UI.GameScreen.Data
{
    public sealed class GameScreenData
    {
        public IGameMediator GameMediator { get; }
        public IGameSessionStorage GameSessionStorage { get; }

        public GameScreenData(IGameMediator gameMediator, IGameSessionStorage gameSessionStorage)
        {
            GameMediator = gameMediator;
            GameSessionStorage = gameSessionStorage;
        }
    }
}