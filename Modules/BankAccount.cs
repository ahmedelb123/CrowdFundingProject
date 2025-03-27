using System;

public class BankAccount
{
    public int Id {get; set;}
    public int UserId{get; set;}
    public int PostId{get; set;}
    public string HolderName{get; set;}
    public long CardNumber{get; set;}
    public int SecretNumber{get; set;}
    public string ExpiryDate{get; set;}
    public DateTime CreatedAt = DateTime.UtcNow;

    // Constructor
    public BankAccount(int userId, int postId, string holderName, long cardNumber, int secretNumber, string expiryDate)
    {
        this.UserId = userId;
        this.PostId = postId;
        this.HolderName = holderName;
        this.CardNumber = cardNumber;
        this.SecretNumber = secretNumber;
        this.ExpiryDate = expiryDate;
        this.CreatedAt = DateTime.UtcNow;
    }
}
