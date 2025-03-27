using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class DonationHandler
{
    private readonly AppDbContext _dbContext;

    public DonationHandler(AppDbContext context)
    {
        _dbContext = context;
    }

    // Create a Donation
    public async Task<ResponseDto> CreateDonation(CreateDonationDto donationDto)
    {
        try
        {
            // Check if the post exists
            Post post = await _dbContext.Posts.FindAsync(donationDto.PostId);
            if (post == null)
            {
                return new ResponseDto { Status = false, Message = "Post not found!" };
            }

            // Check if the user exists
            bool userExists = await _dbContext.Users.AnyAsync(u => u.id == donationDto.UserId);
            if (!userExists)
            {
                return new ResponseDto { Status = false, Message = "User does not exist!" };
            }

            // Round the amount to 2 decimal places before creating the donation
            decimal roundedAmount = Math.Round(donationDto.Amount, 2);



            var bankAccount = await _dbContext.BankAccounts.FirstOrDefaultAsync(b => b.CardNumber == donationDto.CardNumber);

            if (bankAccount == null)
            {

                bankAccount = new BankAccount(donationDto.UserId, donationDto.PostId, donationDto.HolderName, donationDto.CardNumber, donationDto.SecretNumber, donationDto.ExpiryDate);
                _dbContext.BankAccounts.Add(bankAccount);
                await _dbContext.SaveChangesAsync();
                bankAccount = await _dbContext.BankAccounts.FindAsync(donationDto.CardNumber);

            }

            // Create the donation
            var newDonation = new Donation(donationDto.UserId, roundedAmount, donationDto.PostId, bankAccount.Id);
            _dbContext.Donations.Add(newDonation);

            // Update the post's amount gained
            post.AmountGained += donationDto.Amount;
            _dbContext.Posts.Update(post);


            if (post.AmountGained >= post.TargetAmount)
            {
                //Delete the post form the db
                _dbContext.Posts.Remove(post);
            }
            await _dbContext.SaveChangesAsync();

            var donationDetails = new DonationDto
            {
                Id = newDonation.Id,
                UserId = newDonation.UserId,
                Amount = newDonation.Amount,
                CreatedAt = DateTime.UtcNow
            };

            return new ResponseDto
            {
                Status = true,
                Message = "Donation added successfully!",
                DonationDetails = donationDetails
            };
        }
        catch (Exception ex)
        {
            return new ResponseDto { Status = false, Message = "An error occurred while creating the donation." };
        }
    }

    // Get all Donations for a Post
    public async Task<List<DonationDto>> GetDonationsByPostId(int postId)
    {
        try
        {
            var donations = await _dbContext.Donations
                .Where(d => d.PostId == postId)
                .ToListAsync();

            return donations.ConvertAll(donation => new DonationDto
            {
                Id = donation.Id,
                UserId = donation.UserId,
                Amount = Math.Round(donation.Amount, 2),
                CreatedAt = donation.CreatedAt
            });
        }
        catch (Exception)
        {
            return new List<DonationDto>();
        }
    }
}
