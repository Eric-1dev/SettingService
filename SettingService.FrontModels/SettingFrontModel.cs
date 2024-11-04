namespace SettingService.FrontModels;

public class SettingFrontModel
{
    public IReadOnlyCollection<ApplicationFrontModel> AllApplications { get; set; } = [];
    public IReadOnlyCollection<SettingItemFrontModel> Settings { get; set; } = [];
}
