using MedLab3.Models;
using MedLab3.Models.Inspection;
using MedLab3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Authentication;
using System.Security.Claims;

namespace MedLab3.Controllers
{
    [ApiController]
    [Route("api/inspection")]
    public class InspectionController : ControllerBase
    {
        private readonly IInspectionService _inspectionService;
        private readonly IBannedTokenService _banTokensService;

        public InspectionController(IInspectionService inspectionService, IBannedTokenService banTokensService)
        {
            _inspectionService = inspectionService;
            _banTokensService = banTokensService;
        }

        [Route("{Id}")]
        [HttpGet]
        public async Task<IActionResult> GetInspection(Guid Id)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                var result = await _inspectionService.GetInspectionAsync(Id);
                return Ok(result);
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(401, "Autorization Error");
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(404, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [Route("{Id}")]
        [HttpPut]
        public async Task<IActionResult> EditInspection(Guid Id, [FromBody] InspectionEditModel editModel)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _inspectionService.EditInspectionAsync(Id, editModel,userId);
                return Ok();
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(401, "Autorization Error");
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(404, ex.Message);
            }
            catch(ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch(RankException ex)
            {
                return StatusCode(403, "Forbidden");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error saving changes: {ex.Message}");
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }

            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [Route("{Id}/chain")]
        [HttpGet]
        public async Task<IActionResult> InspectionChain(Guid Id)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var res = await _inspectionService.ChainInspectionAsync(Id);
                return Ok(res);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(401, ex.Message);
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(401, "Autorization Error");
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(404, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        /*
        [Route("MKB10")]
        [HttpPost]
        public async Task<IActionResult> MKB()
        {
            await _inspectionService.MKBInit();
            return Ok();
        }
        */
        [Route("HasMail")]
        [HttpPost]
        public async Task<IActionResult> MKB(Guid id)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                await _inspectionService.HasMail(id);
                return Ok();
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(404, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        
        
    }
}
