public class CreateDonationDto
{
    public required int PostId { get; set; }
    public required decimal Amount { get; set; }
    public int UserId{get; set;}
    public string HolderName{get; set;}
    public long CardNumber{get; set;}
    public int SecretNumber{get; set;}
    public string ExpiryDate{get; set;}
}
