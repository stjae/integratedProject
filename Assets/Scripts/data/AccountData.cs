namespace Poly.Data
{
    [System.Serializable]
    public class AccountData
    {
        // login info
        private string email;
        private string password;

        // get, set
        public string Email { get { return email; } set { email = value; } }
        public string Password { get { return password; } set { password = value; } }

        public AccountData()
        {
            email = "";
            password = "";
        }
    }
}