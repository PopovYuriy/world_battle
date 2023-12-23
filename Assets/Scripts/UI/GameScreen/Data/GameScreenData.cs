using Game.Field;
using Game.Field.Mediators;
using Game.Services.Storage;

namespace UI.GameScreen.Data
{
    public sealed class GameScreenData
    {
        public GameField GameFieldPrefab { get; }
        public IGameMediator GameMediator { get; }
        public IGameSessionStorage GameSessionStorage { get; }
        public string OwnerId { get; }

        public GameScreenData(GameField gameFieldPrefab, IGameMediator gameMediator, IGameSessionStorage gameSessionStorage, string ownerId)
        {
            GameFieldPrefab = gameFieldPrefab;
            GameMediator = gameMediator;
            GameSessionStorage = gameSessionStorage;
            OwnerId = ownerId;
        }
    }
}