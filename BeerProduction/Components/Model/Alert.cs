namespace BeerProduction.Components.Model;
    
    public record Alert(
        int Id,
        int UserId,
        DateTime Calltime);