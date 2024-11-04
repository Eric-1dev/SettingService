using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SettingService.DataModel;

public class Application : NamedEntity
{
    [MaxLength(512)]
    [Column("description")]
    public string? Description { get; set; }

    public virtual ICollection<Setting> Settings { get; set; } = new List<Setting>();
}
