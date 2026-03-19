namespace RadicalFeedbackApp.Helpers
{
    public static class Session
    {
        public static int IdUtilisateur { get; set; }
        public static string Login { get; set; }
        public static string Role { get; set; }

        public static bool EstAdmin => Role == "Admin";
        public static bool EstExpert => Role == "Expert";

        public static void Clear()
        {
            IdUtilisateur = 0;
            Login = null;
            Role = null;
        }
    }
}