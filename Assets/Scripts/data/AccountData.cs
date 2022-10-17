[System.Serializable]
public class AccountData
{
    // login info
    private string id;
    private string password;

    // get, set
    public string ID { get { return id; } set { id = value; } }
    public string Password { get { return password; } set { password = value; } }

    public AccountData()
    {
        id = "sample id";
        password = "sample password";
    }
}
