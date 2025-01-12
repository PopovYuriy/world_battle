namespace App.Modules.Matchmaking
{
    public interface IMatchmaker
    {
        void CreateGame();
        void JoinGame(string keyword);
    }
}