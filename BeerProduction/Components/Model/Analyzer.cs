namespace BeerProduction.Components.Model;

public class Analyzer : User
{
    public string Field { get; set; } 

    public Analyzer(int userId, string userName, string email, string password, string field)
        : base(userId, userName, email, password)
    {
        Field = field;
        UserID= userId;
        UserName = userName;
        Email = email;
        Password = password;
    }
    
    
    
}