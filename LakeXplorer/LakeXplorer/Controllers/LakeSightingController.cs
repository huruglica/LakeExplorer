using LakeXplorer.Models.Dto.LakeSightingDtos;
using LakeXplorer.Services.ISerices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LakeXplorer.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    [Authorize]
    public class LakeSightingController : Controller
    {
        private readonly ILakeSightingService _lakeSightingService;

        public LakeSightingController(ILakeSightingService lakeSightingService)
        {
            _lakeSightingService = lakeSightingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var lakesSighting = await _lakeSightingService.GetAll();
                return Ok(lakesSighting);
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
                var lakeSighting = await _lakeSightingService.GetById(id);
                return Ok(lakeSighting);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(int lakeId, [FromForm] LakeSightingCreateDto lakeSightingCreateDto)
        {
            try
            {
                await _lakeSightingService.Post(lakeId, lakeSightingCreateDto);
                return Ok("Posted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] LakeSightingUpdateDto lakeSightingUpdateDto)
        {
            try
            {
                await _lakeSightingService.Update(id, lakeSightingUpdateDto);
                return Ok("Updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/like")]
        public async Task<IActionResult> Like(int id)
        {
            try
            {
                await _lakeSightingService.Like(id);
                return Ok("Liked successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _lakeSightingService.Delete(id);
                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}/dislike")]
        public async Task<IActionResult> Dislike(int id)
        {
            try
            {
                await _lakeSightingService.Dislike(id);
                return Ok("Disliked successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
