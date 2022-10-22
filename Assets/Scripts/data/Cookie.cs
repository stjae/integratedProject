namespace Poly.Data
{
    [System.Serializable]
    public class Cookie
    {
        // recent login info
        private string recentEmail;
        private string recentPassword;

        // recent save data
        private string recentSaveData;

        // get, set
        public string RecentEmail { get { return recentEmail; } set { recentEmail = value; } }
        public string RecentPassword { get { return recentPassword; } set { recentPassword = value; } }

        public string RecentSaveData { get { return recentSaveData; } set { recentSaveData = value; } }

        public Cookie()
        {
            // recent login info
            recentEmail = "";
            recentPassword = "";

            // recent save data
            recentSaveData = "";
        }
    }
}