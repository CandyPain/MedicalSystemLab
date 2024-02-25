using MedLab3.Models;
using MedLab3.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace MedLab3.Controllers
{
    [ApiController]
    [Route("api/report")]
    public class ReportController : Controller
    {
        private readonly IReportService _ReportService;
        private readonly IBannedTokenService _banTokensService;

        public ReportController(IReportService ReportService, IBannedTokenService banTokensService)
        {
            _ReportService = ReportService;
            _banTokensService = banTokensService;
        }
        [Route("icdrootsreport")]
        [HttpGet]
        public async Task<IActionResult> GetReport(DateTime start, DateTime end,[FromQuery]List<Guid>? icdRoots = null)
        {
            try
            {
                if (start == null || end == null)
                {
                    throw new ArgumentException("Bad Time");
                }
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                var res = await _ReportService.GetRootsReportAsync(start,end,icdRoots);
                return Ok(res);
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(401, "Autorization Error");
            }
            catch(ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
