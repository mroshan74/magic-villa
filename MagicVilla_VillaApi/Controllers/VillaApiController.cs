using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Models;
// using MagicVilla_VillaApi.Logging;
using MagicVilla_VillaApi.Models.DTO;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaApi.Controllers;

// [Route("api/[controller]")]
[Route("api/villas")]
[ApiController]
public class VillaApiController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    // Default Logging using DI
    // private readonly ILogger<VillaApiController> _logger;
    //
    // public VillaApiController(ILogger<VillaApiController> logger)
    // {
    //     _logger = logger;
    // }
    
    // Custom Logging implemented 
    // private readonly ILogging _logger;
    //
    // public VillaApiController(ILogging logger)
    // {
    //     _logger = logger;
    // }

    public VillaApiController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<VillaDTO>> GetVillas()
    {
        // _logger.LogInformation("Getting all villas");
        return Ok(_dbContext.Villas.ToList());
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

        var villa = _dbContext.Villas.FirstOrDefault(v => v.Id == id);
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
    public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO? villaDto)
    {
        // if (!ModelState.IsValid)
        // {
        //     return BadRequest(ModelState);
        // }
        if (_dbContext.Villas.FirstOrDefault(v => v.Name.ToLower() == villaDto.Name.ToLower()) != null)
        {
            ModelState.AddModelError("CustomError", "Villa already exist");
            return BadRequest(ModelState);
        }
        if (villaDto is null)
        {
            return BadRequest();
        }

        if (villaDto.Id < 0)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        Villa model = new()
        {
            Amenity = villaDto.Amenity,
            Details = villaDto.Details,
            ImageUrl = villaDto.ImageUrl,
            Name = villaDto.Name,
            Occupancy = villaDto.Occupancy,
            Rate = villaDto.Rate,
            Sqft = villaDto.Sqft
        };
        _dbContext.Villas.Add(model);
        _dbContext.SaveChanges();

        VillaDTO response = new()
        {
            Id = model.Id,
            Amenity = model.Amenity,
            Details = model.Details,
            ImageUrl = model.ImageUrl,
            Name = model.Name,
            Occupancy = model.Occupancy,
            Rate = model.Rate,
            Sqft = model.Sqft
        };

        // _logger.LogInformation("New villa created with id: " + villaDto.Id);
        return CreatedAtRoute("GetVilla",new { id = model.Id }, response);
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

        var villa = _dbContext.Villas.FirstOrDefault(v => v.Id == id);
        if (villa == null)
        {
            return NotFound();
        }

        _dbContext.Villas.Remove(villa);
        _dbContext.SaveChanges();
        return NoContent();
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("{id:int}", Name = "UpdateVilla")]
    public IActionResult UpdateVilla(int id, [FromBody] VillaDTO? villaDto)
    {
        if (villaDto == null || id != villaDto.Id)
        {
            return BadRequest();
        }

        var villa = _dbContext.Villas.AsNoTracking().FirstOrDefault(v => v.Id == id);
        if (villa == null || villa.Id == 0)
        {
            return NotFound();
        }
        
        Villa model = new()
        {
            Id = villaDto.Id,
            Amenity = villaDto.Amenity,
            Details = villaDto.Details,
            ImageUrl = villaDto.ImageUrl,
            Name = villaDto.Name,
            Occupancy = villaDto.Occupancy,
            Rate = villaDto.Rate,
            Sqft = villaDto.Sqft
        };

        _dbContext.Villas.Update(model);
        _dbContext.SaveChanges();

        return NoContent();
    }
    
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPatch("{id:int}", Name = "UpdateVilla")]
    public IActionResult PatchVilla(int id, JsonPatchDocument<VillaDTO>? patchDto)
    {
        if (patchDto == null || id == 0)
        {
            return BadRequest();
        }

        var villa = _dbContext.Villas.FirstOrDefault(v => v.Id == id);
        if (villa == null)
        {
            return NotFound();
        }
        
        VillaDTO modelDTO = new()
        {
            Amenity = villa.Amenity,
            Details = villa.Details,
            ImageUrl = villa.ImageUrl,
            Name = villa.Name,
            Occupancy = villa.Occupancy,
            Rate = villa.Rate,
            Sqft = villa.Sqft
        };

        patchDto.ApplyTo(modelDTO, ModelState);

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        
        Villa model = new Villa()
        {
            Amenity = modelDTO.Amenity,
            Details = modelDTO.Details,
            ImageUrl = modelDTO.ImageUrl,
            Name = modelDTO.Name,
            Occupancy = modelDTO.Occupancy,
            Rate = modelDTO.Rate,
            Sqft = modelDTO.Sqft
        };

        _dbContext.Villas.Update(model);
        _dbContext.SaveChanges();

        return NoContent();
    }
}