using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaApi.Models.DTO;

public class VillaDTO
{
    public int Id { get; set; }
    [Required]
    [MaxLength(30)]
    public string Name { get; set; }
    public DateTime CreatedDate { get; set; }
    public int Sqft { get; set; }
    public int Occupancy { get; set; }
}