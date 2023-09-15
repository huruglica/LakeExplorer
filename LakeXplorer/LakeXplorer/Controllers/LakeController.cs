using LakeXplorer.Models.Dto.LakeDtos;
using LakeXplorer.Models.Dto.LakeSightingDtos;
using LakeXplorer.Services.ISerices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LakeXplorer.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    [Authorize]
    public class LakeController : Controller
    {
        private readonly ILakeService _lakeService;

        public LakeController(ILakeService lakeService)
        {
            _lakeService = lakeService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var lakes = await _lakeService.GetAll();
                return Ok(lakes);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var lake = await _lakeService.GetById(id);
                return Ok(lake);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromForm] LakeCreateDto lakeCreateDto)
        {
            try
            {
                await _lakeService.Post(lakeCreateDto);
                return Ok("Posted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromForm] LakeUpdateDto lakeUpdateDto)
        {
            try
            {
                await _lakeService.Update(id, lakeUpdateDto);
                return Ok("Updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/add-lake-sighting")]
        public async Task<IActionResult> AddLakeSighting(int id, [FromForm] LakeSightingCreateDto lakeSightingCreateDto)
        {
            try
            {
                await _lakeService.AddLakeSighting(id, lakeSightingCreateDto);
                return Ok("Added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _lakeService.Delete(id);
                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
