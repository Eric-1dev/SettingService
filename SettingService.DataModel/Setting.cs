using SettingService.Contracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SettingService.DataModel;

public class Setting : IdentityEntity
{
    [Required]
    [MinLength(1)]
    [Column("name")]
    public required string Name { get; set; }

    [Column("value")]
    public string? Value { get; set; }

    [Required]
    [Column("setting_type_id")]
    public required SettingTypeEnum Type { get; set; }

    public virtual ICollection<Application>? Applications { get; set; }
}
