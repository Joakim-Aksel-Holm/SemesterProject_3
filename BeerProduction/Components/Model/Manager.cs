namespace BeerProduction.Components.Model;

public class Manager : User
{
    public string Field { get; set; } 

    public Manager(int userId, string userName, string email, string password, string field)
        : base(userId, userName, email, password)
    {
        Field = field;
        UserID= userId;
        UserName = userName;
        Email = email;
        Password = password;
    }

    public void CreateUser(string userName, string password, string email)
    {
        
    }
            
}