using SettingService.DataModel;
using SettingService.FrontModels;

namespace SettingService.Services.Extensions;

public static class ApplicationModelExtensions
{
    public static ApplicationFrontModel MapToFrontModel(this Application application)
    {
        return new ApplicationFrontModel
        {
            Key = application.Id,
            Name = application.Name,
            Description = application.Description
        };
    }

    public static Application MapFromFrontModel(this ApplicationFrontModel applicationFrontModel)
    {
        return new Application
        {
            Id = applicationFrontModel.Key ?? 0,
            Name = applicationFrontModel.Name,
            Description = applicationFrontModel.Description
        };
    }

    public static bool EqualTo(this Application application, ApplicationFrontModel frontModel)
    {
        return application.Name == frontModel.Name &&
            application.Description == frontModel.Description;
    }
}
