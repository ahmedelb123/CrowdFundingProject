public class CreateDonationDto
{
    public required int PostId { get; set; }
    public required decimal Amount { get; set; }
    public required int UserId{get; set;}
    public required string HolderName{get; set;}
    public required long CardNumber{get; set;}
    public required int SecretNumber{get; set;}
    public required string ExpiryDate{get; set;}
}
