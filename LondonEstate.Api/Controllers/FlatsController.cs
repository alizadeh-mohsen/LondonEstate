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

        #region Flats

        [HttpGet("all")]
        public async Task<ActionResult<List<FlatDto>>> GetAllFlats()
        {
            var flats = await flatService.GetAllFlatsAsync();
            return Ok(flats);
        }

        [HttpGet("info")]
        public async Task<ActionResult<List<FlatDto>>> GetAllFlatsInfo()
        {
            var flats = await flatService.GetAllFlatsInfoAsync();
            return Ok(flats);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<FlatDto>> GetFlat(Guid id)
        {
            var flat = await flatService.GetFlatAsync(id);
            if (flat == null) return NotFound();
            return Ok(flat);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFlat([FromBody] FlatDto flatDto)
        {
            if (flatDto == null) return BadRequest();
            var created = await flatService.CreateFlat(flatDto);
            return CreatedAtAction(nameof(GetFlat), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Updateflat(Guid id, [FromBody] FlatDto flatDto)
        {
            if (flatDto == null || id != flatDto.Id) return BadRequest();
            var exists = await flatService.FlatExists(id);
            if (!exists) return NotFound();
            var result = await flatService.UpdateFlat(flatDto);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteFlat(Guid id)
        {
            var exists = await flatService.FlatExists(id);
            if (!exists) return NotFound();
            await flatService.DeleteFlat(id);
            return NoContent();
        }
        #endregion

        #region Bookings

        [HttpGet("bookings")]
        public async Task<ActionResult<List<BookingDto>>> GetBookingsAsync()
        {
            var flats = await flatService.GetBookingsAsync();
            return Ok(flats);
        }

        [HttpGet("booking/{id:guid}")]
        public async Task<ActionResult<BookingDto>> GetBooking(Guid id)
        {
            var flat = await flatService.GetBookingAsync(id);
            if (flat == null) return NotFound();
            return Ok(flat);
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportBookings([FromBody] BookingImportDto booking)
        {
            if (booking == null) return BadRequest();
            var result = await flatService.ImportBookingsAsync(booking);
            return Ok(result);
        }

        [HttpPut("checkin")]
        public async Task<IActionResult> UpdateBooking([FromBody] BookingDto flat)
        {
            if (flat == null) return BadRequest();
            await flatService.UpdateBookingAsync(flat);
            return NoContent();
        }

        #endregion

        //[HttpGet("online/{onlineName}")]
        //public async Task<IActionResult> GetByOnlineName(string onlineName)
        //{
        //    var flat = await flatService.GetFlatByOnlineNameAsync(onlineName);
        //    if (flat == null) return NotFound();
        //    return Ok(flat);
        //}

        #region Backup and restore


        [HttpPost("backup")]
        public async Task<IActionResult> Backup()
        {
            await flatService.BackupAsync();
            return NoContent();
        }

        [HttpPost("recover")]
        public async Task<IActionResult> Restore()
        {
            await flatService.RestoreAsync();
            return NoContent();
        }

        [HttpGet("exists/{id:guid}")]
        public async Task<IActionResult> Exists(Guid id)
        {
            var exists = await flatService.FlatExists(id);
            return Ok(exists);
        }

        #endregion

    }
}
