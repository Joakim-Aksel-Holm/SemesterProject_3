class Batch
{
    int id;
    int type;
    int quantity;
    int speed;
    DateTime manufactureDate;

    List<Batch> batches = new List<Batch>();

    public Batch(int id, int type, int quantity, int speed, DateTime expiryDate)
    {
        this.id = id;
        this.type = type;
        this.quantity = quantity;
        this.speed = speed;
        manufactureDate = DateTime.Now;
    }

    public void AddBatch(Batch batch)
    {

    }

    public void RemoveBatch(int id)
    {
    }

    public List<Batch> GetBatches()
    {
        return batches;
    }

}