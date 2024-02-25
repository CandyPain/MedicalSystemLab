using MedLab3.Models;
using MedLab3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedLab3.Controllers
{
    [ApiController]
    [Route("api/dictionary")]
    public class DictionaryController : ControllerBase
    {
        private readonly IDictionaryService _DictionaryService;
        private readonly IBannedTokenService _banTokensService;

        public DictionaryController(IDictionaryService DictionaryService, IBannedTokenService banTokensService)
        {
            _DictionaryService = DictionaryService;
            _banTokensService = banTokensService;
        }


        [Route("speciality")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetSpeciality(string? name, int page = 1, int size = 5)
        {
            try
            {
                var res = await _DictionaryService.GetSpeciality(name,page,size);
                return Ok(res);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (Exception ex) { return StatusCode(500,ex.Message); }
        }

        [Route("icd10")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetMKB(string? request, int page = 1, int size = 5)
        {
            try
            {
                var res = await _DictionaryService.Search(request,page,size);
                return Ok(res);
            }
            catch (ArgumentException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }

        [Route("icd10/roots")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Roots()
        {
            try
            {
                var res = await _DictionaryService.Roots();
                return Ok(res);
            }
            catch (Exception ex) { return StatusCode(500, ex.Message); }
        }
    }
}
