using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SettingService.Entities;

public class Application : IdentityEntity
{
    [Required]
    [MinLength(1)]
    [Column("name")]
    public required string Name { get; set; }

    public virtual ICollection<Setting>? Settings { get; set; }
}
