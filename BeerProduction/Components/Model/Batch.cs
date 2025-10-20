public class Batch
{
    public int Id { get; }
    public int Type { get; }
    public int Quantity { get; }
    public int Speed { get; }
    public DateTime ManufactureDate { get; }
    public DateTime ExpiryDate { get; }
    public Batch(int id, int type, int quantity, int speed, DateTime expiryDate)
    {
        this.Id = id;
        this.Type = type;
        this.Quantity = quantity;
        this.Speed = speed;
        ManufactureDate = DateTime.Now;
        ExpiryDate = ManufactureDate.AddDays(365);
    }
}