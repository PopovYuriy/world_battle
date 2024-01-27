using Game.Field.Mediators;
using Game.Services.Storage;

namespace UI.GameScreen.Data
{
    public sealed class GameScreenData
    {
        public IGamePlayController GamePlayController { get; }
        public IGameSessionStorage GameSessionStorage { get; }

        public GameScreenData(IGamePlayController gamePlayController, IGameSessionStorage gameSessionStorage)
        {
            GamePlayController = gamePlayController;
            GameSessionStorage = gameSessionStorage;
        }
    }
}