using WebStart.Dto;
using WebStart.Data;

using Microsoft.EntityFrameworkCore;
namespace WebStart.Services
{
    public interface IUserService
    {
        Task Add(UserInfoDto policyHolderDto);
        Task Delete(int id);
        Task<List<UserInfoDto>> GetAll();
        Task<UserInfoDto> GetById(int id);
        Task Update(UserInfoDto policyHolderDto);

        Task<LoginDto> ValidateUser(string email, string password);
        string GenerateToken(LoginDto user);
        Task<UserInfoDto> GetPolicyHolderByEmailAsync(string email);
    }

    public class UserService : IUserService
    {
        private readonly WebStartContext context;

        public UserService(WebStartContext context)
        {
            this.context = context;
        }

        public async Task<List<UserInfoDto>> GetAll()
        {
            List<UserInfoDto> policyHolderDtos = [];
            await foreach (var policyHolderTable in context.Userinfos)
            {
                var policyHolderDto = ConvertToDto(policyHolderTable);
                policyHolderDtos.Add(policyHolderDto);
            }
            return policyHolderDtos;
        }

        public async Task Delete(int id)
        {
            var found = await context.Userinfos.FirstOrDefaultAsync((policyHolderTable) =>
                policyHolderTable.Id == id);
            if (found != null)
            {
                context.Userinfos.Remove(found);
                await context.SaveChangesAsync();
                return;
            }
            throw new NullReferenceException();
        }

        public async Task Add(UserInfoDto policyHolderDto)
        {
            Userinfo policyHolderTable = new();
            ConvertToTable(policyHolderDto, policyHolderTable);
            context.Userinfos.Add(policyHolderTable);
            await context.SaveChangesAsync();
            return;
        }

        public async Task Update(UserInfoDto policyHolderDto)
        {
            var found = await context.Userinfos.FirstOrDefaultAsync((policyHolderTable) =>
                policyHolderTable.Id == policyHolderDto.Id);
            if (found != null)
            {
                ConvertToTable(policyHolderDto, found);
                await context.SaveChangesAsync();
                return;
            }
            throw new NullReferenceException();
        }

        public async Task<UserInfoDto> GetById(int id)
        {
            var found = await context.Userinfos.FirstOrDefaultAsync((policyHolderTable) =>
                policyHolderTable.Id == id);

            if (found != null)
                return ConvertToDto(found);

            throw new NullReferenceException();
        }

        private UserInfoDto ConvertToDto(Userinfo policyHolderTable)
        {
            UserInfoDto policyHolderDto = new()
            {
                Id = policyHolderTable.Id,
                Name = policyHolderTable.Name,
                Address = policyHolderTable.Address,
                Email = policyHolderTable.Email,
                Password = policyHolderTable.Password,
                Phone = policyHolderTable.PhoneNumber,
               
            };
            return policyHolderDto;
        }

        private void ConvertToTable(UserInfoDto policyHolderDto, Userinfo policyHolderTable)
        {
            policyHolderTable.Id = policyHolderDto.Id;
            policyHolderTable.Name = policyHolderDto.Name;
            policyHolderTable.Address = policyHolderDto.Address;
            policyHolderTable.Email = policyHolderDto.Email;
            policyHolderTable.Password = policyHolderDto.Password;
            policyHolderTable.PhoneNumber = policyHolderDto.Phone;
           
            return;
        }
        public async Task<LoginDto> ValidateUser(string email, string password)
        {

            // Fetch the user by email
            var user = await context.Userinfos.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (user == null)
            {
                return null;
            }


            // Verify the password (assuming passwords are stored as hashes)
            if (user.Password != password)
            {
                return null;
            }
            var userDTo = new LoginDto
            {

                Email = user.Email,


            };
            return userDTo;
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }
            return true;
        }


        // Optional: Implement this method if you want to use JWT for authentication
        public string GenerateToken(LoginDto user)
        {
            return null;
            // Implementation for generating a JWT token
        }

        public async Task<UserInfoDto> GetPolicyHolderByEmailAsync(string email)
        {
            return await context.Userinfos
                .Where(ph => ph.Email == email)
                .Select(ph => new UserInfoDto
                {
                    Id = ph.Id,
                    Email = ph.Email
                    // map other necessary fields
                })
                .FirstOrDefaultAsync();
        }
    }
}