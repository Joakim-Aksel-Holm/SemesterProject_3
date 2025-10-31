namespace BeerProduction.Components.Model;

public abstract class User
{
    public int UserID { get; set; }
    public string UserName { get; set; }
    public string  Email { get; set; }
    public string  Password { get; set; }

    public User(int userId, string userName, string email, string password)
    {
        UserID = userId;
        UserName = userName;
        Email = email;
        Password = password;
    }
    
    public void Login(string password, string userName)
    {
        
    }

    public void Logout()
    {
        
    }
}

