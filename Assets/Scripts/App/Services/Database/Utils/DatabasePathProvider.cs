namespace App.Services.Database.Utils
{
    public sealed class DatabasePathProvider
    {
        private const string GamesNode = "games";
        private const string PendingGameNode = "pending";

        private const string UsersNode = "users";
        private const string UsersGamesNode = "usersGames";

        public string Games { get; }
        public string PendingGames { get; }
        public string Users { get; }
        public string UserGames { get; }

        public DatabasePathProvider()
        {
            Games = GeneratePath(GamesNode);
            PendingGames = GeneratePath(PendingGameNode);
            Users = GeneratePath(UsersNode);
            UserGames = GeneratePath(UsersGamesNode);
        }
        
        public string GeneratePath(params string[] nodes)
        {
            var result = "/";
            foreach (var node in nodes)
                result += node + "/";

            return result;
        }
    }
}