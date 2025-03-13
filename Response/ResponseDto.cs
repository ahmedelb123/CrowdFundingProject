public class ResponseDto
{
    public required bool Status { get; set; }    
    public required string Message { get; set; }  

    public int? Id { get; set; }   
    public string? Token { get; set; }  

    public int? PostId { get; set; }  
    public PostDto? PostDetails { get; set; }  

    public DonationDto? DonationDetails { get; set; }  
}

