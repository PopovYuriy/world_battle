using System.Collections.Generic;
using App.Modules.GameSessions.Controller;

namespace App.Modules.GameSessions
{
    public interface IGameSessionsManagerInitializable
    {
        void Initialize(IEnumerable<IGameSessionController> onlineControllers);
    }
}