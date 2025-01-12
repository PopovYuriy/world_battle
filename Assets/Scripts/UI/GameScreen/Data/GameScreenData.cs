using App.Modules.GameSessions.Controller;
using Game.Field.Mediators;

namespace UI.GameScreen.Data
{
    public sealed class GameScreenData
    {
        public IGamePlayController GamePlayController { get; }
        public IGameSessionController GameSessionController { get; }

        public GameScreenData(IGamePlayController gamePlayController, IGameSessionController gameSessionController)
        {
            GamePlayController = gamePlayController;
            GameSessionController = gameSessionController;
        }
    }
}