using MedLab3.Models;
using MedLab3.Models.Inspection;
using MedLab3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using System.Security.Authentication;
using System.Security.Claims;

namespace MedLab3.Controllers
{
    [ApiController]
    [Route("api/consultation")]
    public class ConsultationController : ControllerBase
    {
        private readonly IConsultationService _ConsultationService;
        private readonly IBannedTokenService _banTokensService;

        public ConsultationController(IConsultationService ConsultationService, IBannedTokenService banTokensService)
        {
            _ConsultationService = ConsultationService;
            _banTokensService = banTokensService;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetInspections(bool? grouped = false,[FromQuery] List<Guid>? isdRoots = null,int? page = 1, int? size = 5)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var res = await _ConsultationService.GetInspections(userId,grouped,isdRoots,(int)page,(int)size);
                return Ok(res);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(401, "Autorization error");
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(404, ex.Message);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [Route("{Id}")]
        [HttpGet]
        public async Task<IActionResult> GetConsultations(Guid Id)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                var res = await _ConsultationService.GetConsultation(Id);
                return Ok(res);
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(401, "Autorization error");
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(404, ex.Message);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [Route("{Id}/comment")]
        [HttpPut]
        public async Task<IActionResult> EditComment(Guid Id,InspectionCommentCreateModel content)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _ConsultationService.EditComment(userId,Id, content);
                return Ok();
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(401, "Autorization error");
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (RankException ex)
            {
                return StatusCode(403, "Forbidden");
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(404, ex.Message);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
        [Route("comment/{Id}")]
        [HttpPost]
        public async Task<IActionResult> CreateComment(Guid Id, CommentCreateModel consultationCreateModel)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var res = await _ConsultationService.CreateComment(userId, Id, consultationCreateModel);
                return Ok(res);
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(401, "Autorization error");
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (RankException ex)
            {
                return StatusCode(403, "Forbidden");
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(404, ex.Message);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
    }
}
