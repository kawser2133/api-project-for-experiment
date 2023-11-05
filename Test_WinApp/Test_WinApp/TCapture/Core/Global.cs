namespace Test_WinApp.TCapture.Core
{
    public static class Global
    {
        public static readonly string Version;
        public static int MaxAttempt;
        public static bool UseBestAmongMaxAttempt;
        internal static byte[] token;

        static Global()
        {
            Global.Version = "V-1.16";
            Global.MaxAttempt = 3;
            Global.UseBestAmongMaxAttempt = true;
        }
    }
}