public class DonationDto
{
    public required int Id { get; set; }
    public required int UserId { get; set; }
    public required decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}
