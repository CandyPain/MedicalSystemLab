using MedLab3.Models;
using MedLab3.Models.Doctor;
using MedLab3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;

namespace MedLab3.Controllers
{
    [ApiController]
    [Route("api/doctor")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly IBannedTokenService _banTokensService;

        public DoctorController(IDoctorService doctorService, IBannedTokenService banTokensService)
        {
            _doctorService = doctorService;
            _banTokensService = banTokensService;
        }
        [Route("register")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] DoctorRegisterModel doctorRegisterModel)
        {
            try
            {
                var result = await _doctorService.RegisterAsync(doctorRegisterModel);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginCredentialsModel LC)
        {
            try
            {
                var result = await _doctorService.LoginAsync(LC);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [Route("logout")]
        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                await _doctorService.LogOutAsync(token);
                return Ok("LogOut successfully");
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(401, "Autorization Error");
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (Exception ex) { return StatusCode(500, "Internal server error"); }
        }

        [Route("profile")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _doctorService.GetProfileAsync(userId);
                return Ok(result);
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(404, "Not Found");
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(401, "Autorization Error");
            }
            catch (Exception ex) { return StatusCode(500, "Internal server error"); }
        }

        [Route("profile")]
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> EditProfile([FromBody] DoctorEditModel userEditModel)
        {
            try
            {
                if (userEditModel == null) { throw new DirectoryNotFoundException(); }
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _doctorService.EditProfileAsync(userEditModel, userId);
                return StatusCode(200,"OK");
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(404, "Not Found");
            }
            catch(ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(401, "Autorization Error");
            }
            catch (Exception ex) { return StatusCode(500, "Internal server error"); }
        }

    }
}
