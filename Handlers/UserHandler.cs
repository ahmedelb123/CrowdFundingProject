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

    public async Task<ResponseDto> Login(string email, string password)
{
    User? userExist = await _dbContext.Users.FirstOrDefaultAsync(u => u.email == email);

    if (userExist == null)
    {
        return new ResponseDto { Status = false, Message = "This account doesn't exist" };
    }
    if (userExist.password_hash != password)
    {
        return new ResponseDto { Status = false, Message = "Your email or password is incorrect" };
    }

    // Generate JWT Token
    var token = GenerateJwtToken(userExist);

    return new ResponseDto
    {
        Id = userExist.id,
        Status = true,
        Message = "You logged in successfully",
        Token = token
    };
 }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:SecretKey"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
            new Claim(ClaimTypes.Email, user.email),
            new Claim("IsAdmin", user.isAdmin.ToString())
        };

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}








