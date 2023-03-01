using MagicVilla_VillaApi.Models.DTO;

namespace MagicVilla_VillaApi.Data;

public class VillaStore
{
    public static List<VillaDTO> villaList = new List<VillaDTO>
    {
        new VillaDTO { Id = 1, Name = "Pool View" },
        new VillaDTO { Id = 2, Name = "Beach View" }
    };
}