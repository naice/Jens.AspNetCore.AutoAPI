using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Jens.AspNetCore.AutoAPI;

public class EntityControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    private readonly EntityControllerConfigs _controllerConfigs;

    public EntityControllerFeatureProvider(EntityControllerConfigs controllerConfigs)
    {
        _controllerConfigs = controllerConfigs;
    }

    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        foreach (var config in _controllerConfigs)
        {
             feature.Controllers.Add(config.ControllerType.GetTypeInfo());
        }
    }
}
