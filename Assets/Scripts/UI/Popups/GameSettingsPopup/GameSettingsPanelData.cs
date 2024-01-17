using Game.Services.Storage;

namespace UI.Popups.GameSettingsPopup
{
    public sealed class GameSettingsPanelData
    {
        public IGameSessionStorage GameSessionStorage { get; }

        public GameSettingsPanelData(IGameSessionStorage gameSessionStorage)
        {
            GameSessionStorage = gameSessionStorage;
        }
    }
}