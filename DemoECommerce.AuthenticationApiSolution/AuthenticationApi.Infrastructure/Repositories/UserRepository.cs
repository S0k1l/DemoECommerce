using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using ECommerce.SharedLibrary.Logs;
using ECommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public class UserRepository(AuthenticationDbContext context, IConfiguration config) : IUserRepository
    {
        public async Task<AppUser> GetByEmail(string email)
        {

            try
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

                return user is not null ? user : null!;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                throw new Exception("Error occurred while retrieving user");
            }

        }
        public async Task<UserDetailsDto> GetUser(int userId)
        {
            try
            {
                var user = await context.Users.FindAsync(userId);

                return user is not null ? new UserDetailsDto(
                    user.Id,
                    user.Name!,
                    user.PhoneNumber!,
                    user.Address!,
                    user.Email!,
                    user.Role!) 
                    : null!;
            }
            catch (Exception ex) 
            {
                LogException.LogExceptions(ex);

                throw new Exception("Error occurred while retrieving user");
            }
        }

        public async Task<Response> Login(LoginDto loginDto)
        {
            var user = await GetByEmail(loginDto.Email);
            if (user is null)
                return new Response(false, "Invalid credentials");

            bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);

            if(!verifyPassword)
                return new Response(false, "Invalid credentials");

            string token = GenerateToken(user);

            return new Response(true, token);
        }

        private string GenerateToken(AppUser user)
        {
            var key = Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);

            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, user.Name!),
                new (ClaimTypes.Email, user.Email!),
            };

            if (!string.IsNullOrEmpty(user.Role))
                claims.Add(new(ClaimTypes.Role, user.Role!));

            var token = new JwtSecurityToken(
                issuer : config["Authentication:Issuer"],
                audience : config["Authentication:Audience"],
                claims : claims,
                expires : null,
                signingCredentials : credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Response> Register(AppUserDto appUserDto)
        {
            try
            {
                var user = await GetByEmail(appUserDto.Email);

                if (user is not null)
                    return new Response(false, "User with this Email already exist");

                var result = context.Users.Add(new AppUser
                {
                    Name = appUserDto.Name,
                    PhoneNumber = appUserDto.PhoneNumber,
                    Address = appUserDto.Address,
                    Email = appUserDto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(appUserDto.Password),
                    Role = appUserDto.Role,
                }).Entity;

                await context.SaveChangesAsync();

                return result.Id > 0 
                    ? new Response(true, "User registered successfully") 
                    : new Response(false, "Invalid data provided");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);

                return new Response(false, "Error occurred while register new user");
            }
        }
    }
}
