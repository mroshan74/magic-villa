using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Logging;
using MagicVilla_VillaApi.Models.DTO;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaApi.Controllers;

// [Route("api/[controller]")]
[Route("api/villas")]
[ApiController]
public class VillaApiController : ControllerBase
{
    // Default Logging using DI
    // private readonly ILogger<VillaApiController> _logger;
    //
    // public VillaApiController(ILogger<VillaApiController> logger)
    // {
    //     _logger = logger;
    // }
    
    // Custom Logging implemented 
    private readonly ILogging _logger;
    
    public VillaApiController(ILogging logger)
    {
        _logger = logger;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<VillaDTO>> GetVillas()
    {
        _logger.LogInformation("Getting all villas");
        return Ok(VillaStore.villaList);
    }
    
    // [HttpGet("id")]
    // [ProducesResponseType(200, Type = typeof(VillaDTO)]
    [HttpGet("{id:int}", Name = "GetVilla")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<VillaDTO> GetVilla(int id)
    {
        if (id == 0)
        {
            return BadRequest();
        }

        var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
        if (villa == null)
        {
            return NotFound();
        }

        return Ok(villa);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDto)
    {
        // if (!ModelState.IsValid)
        // {
        //     return BadRequest(ModelState);
        // }
        if (VillaStore.villaList.FirstOrDefault(v => v.Name.ToLower() == villaDto.Name.ToLower()) != null)
        {
            ModelState.AddModelError("CustomError", "Villa already exist");
            return BadRequest(ModelState);
        }
        if (villaDto is null)
        {
            return BadRequest(villaDto);
        }

        if (villaDto.Id < 0)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        villaDto.Id = VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;
        VillaStore.villaList.Add(villaDto);
        _logger.LogInformation("New villa created with id: " + villaDto.Id);
        return CreatedAtRoute("GetVilla",new { id = villaDto.Id }, villaDto);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpDelete("{id:int}", Name = "DeleteVilla")]
    public IActionResult DeleteVilla(int id)
    {
        if (id == 0)
        {
            return BadRequest();
        }

        var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
        if (villa == null)
        {
            return NotFound();
        }

        VillaStore.villaList.Remove(villa);
        return NoContent();
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("{id:int}", Name = "UpdateVilla")]
    public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDto)
    {
        if (villaDto == null | id != villaDto.Id)
        {
            return BadRequest();
        }

        var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
        if (villa == null)
        {
            return NotFound();
        }

        villa.Name = villaDto.Name;
        villa.Sqft = villaDto.Sqft;
        villa.Occupancy = villaDto.Occupancy;

        return NoContent();
    }
    
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPatch("{id:int}", Name = "UpdateVilla")]
    public IActionResult PatchVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
    {
        if (patchDTO == null || id == 0)
        {
            return BadRequest();
        }

        var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
        if (villa == null)
        {
            return NotFound();
        }
        
        patchDTO.ApplyTo(villa, ModelState);
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        return NoContent();
    }
}