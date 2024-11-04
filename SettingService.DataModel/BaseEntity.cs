using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SettingService.DataModel;

public class NamedEntity : IdentityEntity
{
    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    [Column("name")]
    public required string Name { get; set; }
}
