public class Batch
{
    public int Id { get; }
    public int BeerType { get; }
    public int Quantity { get; }
    public int Speed { get; }
    public DateTime ManufactureDate { get; }
    public DateTime ExpiryDate { get; }
    public Batch(int id, int beerType, int quantity, int speed, DateTime expiryDate)
    {
        Id = id;
        BeerType = beerType;
        Quantity = quantity;
        Speed = speed;
        ManufactureDate = DateTime.Now;
        ExpiryDate = ManufactureDate.AddDays(365);
    }
}