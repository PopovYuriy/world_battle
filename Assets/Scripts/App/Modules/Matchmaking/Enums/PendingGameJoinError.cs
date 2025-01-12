namespace App.Modules.Matchmaking.Enums
{
    public enum PendingGameJoinError : byte
    {
        None                = 0,
        DoesNotExist        = 1,
        IsAlreadyCreated    = 2,
        Expired             = 3,
        IsAlreadyJoining    = 4,
        CannotCreateSession = 5,
    }
}