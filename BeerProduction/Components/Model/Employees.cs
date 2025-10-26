namespace BeerProduction.Components.Model;

public record Employees(
    int Id,
    string Name,
    string Role,
    DateTime HiredOn);