namespace App.Data.Player
{
    public interface IPlayer
    {
        string Uid { get; }
        string Name { get; }
    }

    public interface IPlayerMutable : IPlayer
    {
        void SetName(string name);
        void SetUid(string uid);
    }
}