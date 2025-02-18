using System;

public class Donation
{
    public int Id { get; set; }
    public int user_id { get; set; }
    public decimal amount { get; set; }
    public DateTime donation_date { get; set; }

    public Donation(int userId, decimal amount)
    {
        this.user_id = userId;
        this.amount = amount;
    }
}
