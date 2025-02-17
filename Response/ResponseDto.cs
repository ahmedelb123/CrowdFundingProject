

public class ResponseDto
{
    public required bool status { get; set; }
    public required string message { get; set; }

    public int? id {get; set;} // Here we rturn the user id when he logs in 
}
