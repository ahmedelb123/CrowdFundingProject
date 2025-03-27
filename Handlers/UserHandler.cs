using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;


public class UserHandler
{
    private const string EXIST_ACCOUNT_WITH_THIS_EMAIL = "There is already an account with this email";
    private const string ACCOUNT_CREATED = "The account is created successfully";
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration; 


    public UserHandler(AppDbContext context, IConfiguration configuration)
    {
        _dbContext = context;
        _configuration = configuration;
    }

public async Task<ResponseDto> CreateUser(string name, string surname, string email, string password)
{
    // Check if user already exists
    bool userExists = await _dbContext.Users.AnyAsync(u => u.email == email);
    if (userExists)
    {
        return new ResponseDto { Status = false, Message = EXIST_ACCOUNT_WITH_THIS_EMAIL };
    }

    // Create new user
    User newUser = new User(name, surname, email, password);
    _dbContext.Users.Add(newUser);
    await _dbContext.SaveChangesAsync();

    return new ResponseDto { Status = true, Message = ACCOUNT_CREATED };
}

    public async Task<ResponseDto> Login(LoginDto loginDto)
{
    User? userExist = await _dbContext.Users.FirstOrDefaultAsync(u => u.email == loginDto.Email);

    if (userExist == null)
    {
        return new ResponseDto { Status = false, Message = "This account doesn't exist" };
    }
    if (userExist.password_hash != loginDto.Password)
    {
        return new ResponseDto { Status = false, Message = "Your email or password is incorrect" };
    }

    // Generate JWT Token
    

    return new ResponseDto
    {
        Id = userExist.id,
        Status = true,
        Message = "You logged in successfully",
    };
 }

    
}








