using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SettingService.DataModel;

public class Application : IdentityEntity
{
    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    [Column("name")]
    public required string Name { get; set; }

    [MaxLength(512)]
    [Column("description")]
    public string? Description { get; set; }

    public virtual ICollection<Setting>? Settings { get; set; }
}
