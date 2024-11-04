using SettingService.Contracts;
using SettingService.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SettingService.DataModel;

public class Setting : NamedEntity
{
    [Required]
    [MinLength(1)]
    [Column("description")]
    public required string Description { get; set; }


    [Column("value")]
    public string? Value { get; set; }

    [Required]
    [Column("setting_type_id")]
    public required SettingValueTypeEnum ValueType { get; set; }

    [Required]
    [Column("is-from-external-source")]
    public bool IsFromExternalSource { get; set; }

    [Column("external-source-path")]
    public string? ExternalSourcePath { get; set; }

    [Column("external-source-key")]
    public string? ExternalSourceKey { get; set; }

    [Column("external-source-type")]
    public ExternalSourceTypeEnum? ExternalSourceType { get; set; }

    public virtual List<Application> Applications { get; set; } = [];
}
