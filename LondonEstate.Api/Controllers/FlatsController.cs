using LondonEstate.Core.Dtos;
using LondonEstate.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace LondonEstate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class FlatsController : ControllerBase
    {
        private readonly IFlatService flatService;

        public FlatsController(IFlatService flatService)
        {
            this.flatService = flatService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var flats = await flatService.GetAllFlatsAsync();
            return Ok(flats);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var flat = await flatService.GetFlatAsync(id);
            if (flat == null) return NotFound();
            return Ok(flat);
        }

        [HttpGet("online/{onlineName}")]
        public async Task<IActionResult> GetByOnlineName(string onlineName)
        {
            var flat = await flatService.GetFlatByOnlineNameAsync(onlineName);
            if (flat == null) return NotFound();
            return Ok(flat);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FlatDto flatDto)
        {
            if (flatDto == null) return BadRequest();
            var created = await flatService.CreateFlat(flatDto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] FlatDto flatDto)
        {
            if (flatDto == null || id != flatDto.Id) return BadRequest();
            var exists = await flatService.FlatExists(id);
            if (!exists) return NotFound();
            var result = await flatService.UpdateFlat(flatDto);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var exists = await flatService.FlatExists(id);
            if (!exists) return NotFound();
            await flatService.DeleteFlat(id);
            return NoContent();
        }

        [HttpPost("backup")]
        public async Task<IActionResult> Backup()
        {
            await flatService.BackupAsync();
            return NoContent();
        }

        [HttpPost("recover")]
        public async Task<IActionResult> Recover()
        {
            await flatService.RecoverAsync();
            return NoContent();
        }

        [HttpGet("exists/{id:guid}")]
        public async Task<IActionResult> Exists(Guid id)
        {
            var exists = await flatService.FlatExists(id);
            return Ok(exists);
        }

        [HttpPost("import")]
        public async Task<IActionResult> UpdateByImport([FromBody] BookingImportDto booking)
        {
            if (booking == null) return BadRequest();
            var result = await flatService.UpdateFlatByImportAsync(booking);
            return Ok(result);
        }

        [HttpPut("checkin")]
        public async Task<IActionResult> UpdateForCheckin([FromBody] FlatDto flat)
        {
            if (flat == null) return BadRequest();
            await flatService.UpdateFlatForCheckinAsync(flat);
            return NoContent();
        }
    }
}
