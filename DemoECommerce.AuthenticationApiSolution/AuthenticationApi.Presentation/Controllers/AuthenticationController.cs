using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using ECommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IUserRepository userRepository) : ControllerBase
    {
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDetailsDto>> GetUser(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid user id");

            var user = await userRepository.GetUser(id);

            return user is not null ? Ok(user) : NotFound("User not found");
        }

        [HttpPost("register")]
        public async Task<ActionResult<Response>> Register(AppUserDto appUserDto)
        {
            if(!ModelState.IsValid) 
                return BadRequest(ModelState);

            var result = await userRepository.Register(appUserDto);

            return result.Flag ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<Response>> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await userRepository.Login(loginDto);

            return result.Flag ? Ok(result) : BadRequest(result);
        }
    }
}
