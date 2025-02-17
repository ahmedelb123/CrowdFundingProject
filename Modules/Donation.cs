using System;

public class Donation
{
    public int Id { get; set; }
    public int UserId { get; set; } 
    public decimal Amount { get; set; }
    public DateTime DonationDate { get; set; } 

    public Donation(int userId, decimal amount)
    {
        this.UserId = userId;
        this.Amount = amount;
    }
}
