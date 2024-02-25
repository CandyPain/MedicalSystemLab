using MedLab3.Models;
using MedLab3.Models.Enums;
using MedLab3.Models.Inspection;
using MedLab3.Models.Patient;
using MedLab3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;

namespace MedLab3.Controllers
{
    [ApiController]
    [Route("api/patient")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IBannedTokenService _banTokensService;

        public PatientController(IPatientService patientService, IBannedTokenService banTokensService)
        {
            _patientService = patientService;
            _banTokensService = banTokensService;
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> NewPatient([FromBody] PatientCreateModel patient)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                var res = await _patientService.CreatePatient(patient);
                return Ok(res);
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(401, "Autorization Error");
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

        [Route("{Id}")]
        [HttpGet]
        public async Task<IActionResult> GetPatientId(Guid Id)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                var res = await _patientService.GetPatientId(Id);
                return Ok(res);
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

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetPatientsList(string? name,[FromQuery] Conclusion[]? conclusions = null, Sorting sort = 0, bool scheduledVisits = false, bool onlyMine = false, int page = 1, int size = 5)
        {
            try
            {
                if (conclusions == null || sort == null)
                {
                    throw new ArgumentException("Ошибка аргументов фильтрации");
                }
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                 var Result =  await _patientService.GetPatientsList(userId,name,conclusions,sort,scheduledVisits,onlyMine,page,size);
                return Ok(Result);
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(401, "Autorization Error");
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

        [Route("{Id}/inspections")]
        [HttpPost]
        public async Task<IActionResult> CreateInspection(Guid Id, [FromBody]InspectionCreateModel inspectionCreateModel)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var Result = await _patientService.CreateInspection(userId,Id,inspectionCreateModel);
                return Ok(Result);
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(401, "Autorization Error");
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Route("{Id}/inspections")]
        [HttpGet]
        public async Task<IActionResult> GetInspections(Guid Id, bool? grouped = false, [FromQuery]List<Guid>? icdRoots = null, int page = 1, int size = 5)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                var Result = await _patientService.GetInspections(Id,grouped,icdRoots,page,size);
                return Ok(Result);
            }
            catch (AuthenticationException ex)
            {
                return StatusCode(401, "Autorization Error");
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (DirectoryNotFoundException ex)
            {
                return StatusCode(404, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Route("{Id}/inspections/search")]
        [HttpGet]
        public async Task<IActionResult> SearchInspections(Guid Id, string? request)
        {
            try
            {
                TokenBan token = new TokenBan { BannedToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") };
                _banTokensService.CheckAuthentication(token);
                var Result = await _patientService.InspectionSearch(Id,request);
                return Ok(Result);
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
                return StatusCode(500, ex.Message);
            }
        }
    }
}
