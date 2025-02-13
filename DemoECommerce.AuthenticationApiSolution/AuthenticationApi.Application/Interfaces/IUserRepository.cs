using AuthenticationApi.Application.DTOs;
using ECommerce.SharedLibrary.Responses;

namespace AuthenticationApi.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<Response> Register(AppUserDto appUserDto);
        Task<Response> Login(LoginDto loginDto);
        Task<UserDetailsDto> GetUser(int userId);
    }
}
