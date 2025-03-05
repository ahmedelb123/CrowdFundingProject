public class Donation
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public Donation() {}
    public Donation(int userId, decimal amount)
    {
        UserId = userId;
        Amount = amount;
    }
}
