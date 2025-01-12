namespace App.Services.Database.Utils
{
    public static class DatabasePathProvider
    {
        public static string Games { get; }
        public static string PendingGames { get; }
        public static string Users { get; }
        public static string UserGames { get; }

        static DatabasePathProvider()
        {
            Games = GeneratePath("games");
            PendingGames = GeneratePath("pending");
            Users = GeneratePath("users");
            UserGames = GeneratePath("usersGames");
        }
        
        public static string GeneratePath(params string[] nodes)
        {
            var result = "/";
            foreach (var node in nodes)
                result += node + "/";

            return result;
        }
    }
}