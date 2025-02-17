using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class UserHandler
{
    private const string EXIST_ACCOUNT_WITH_THIS_EMAIL = "There is already an account with this email";
    private const string ACCOUNT_CREATED = "The account is created successfully";

    private readonly AppDbContext _dbContext;

    public UserHandler(AppDbContext context)
    {
        _dbContext = context;
    }

    public async Task<ResponseDto> CreateUser(string name, string surname, string email, string password)
    {
        // Check if user already exists
        bool userExists = await _dbContext.Users.AnyAsync(u => u.email == email);
        if (userExists)
        {
            return new ResponseDto { status = false, message = EXIST_ACCOUNT_WITH_THIS_EMAIL };
        }

        // Create new user
        User newUser = new User(name, surname, email, password);

        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync();

        return new ResponseDto { status = true, message = ACCOUNT_CREATED };
    }
    public async Task<ResponseDto> Login(string email, string password){
        User? userExist = await _dbContext.Users.FirstOrDefaultAsync(u => u.email == email);

        if(userExist == null){
            return new ResponseDto {status = false, message = "This account dont exists"};
        }
        if(userExist.password_hash != password){
            return new ResponseDto {status = false, message = "Your email or password is incorrect"};
        }

        return new ResponseDto {id = userExist.id, status = true, message = "You logged in successfully"};

    }
}