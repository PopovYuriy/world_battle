using App.Modules.GameSessions.Controller;

namespace UI.Popups.GameSettingsPopup
{
    public sealed class GameSettingsPanelData
    {
        public IGameSessionController GameSessionController { get; }

        public GameSettingsPanelData(IGameSessionController gameSessionController)
        {
            GameSessionController = gameSessionController;
        }
    }
}