using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class AccountController(AppDbContext appDbContext, ITokenService tokenService) : BaseApiController
    {

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (await EmailExists(registerDto.Email))
                return BadRequest("Email Taken!");

            using var hmac = new HMACSHA512();
            var user = new AppUser
            {
                DisplayName = registerDto?.DisplayName,
                Email = registerDto?.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto?.Password)),
                PasswordSalt = hmac.Key

            };
            appDbContext.Users.Add(user);
            await appDbContext.SaveChangesAsync();
            return user.ToDto(tokenService);
        }


        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
        {
            var user = await appDbContext.Users.SingleOrDefaultAsync(x => x.Email == loginDto.Email);
            if (user == null) return Unauthorized("Invalid email address");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for (var i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("invalid password");
            }
             return user.ToDto(tokenService);
        }


        private async Task<bool> EmailExists(string email)
        {
            return await appDbContext.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
        }
    }
}
