using App.Data.Player;
using App.Modules.GameSessions.API.Enums;

namespace App.Modules.GameSessions.API.Data
{
    public sealed class UserResult
    {
        public Player    User { get; set; }
        public UserError Error      { get; set; }

        public UserResult(Player user)
        {
            User = user;
        }

        public UserResult(UserError error)
        {
            Error = error;
        }
    }
}