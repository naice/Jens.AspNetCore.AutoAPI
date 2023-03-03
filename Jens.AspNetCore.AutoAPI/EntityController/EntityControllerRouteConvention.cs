using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Jens.AspNetCore.AutoAPI;

public class EntityControllerRouteConvention : IControllerModelConvention
{
    private readonly EntityControllerConfigs _controllerConfigs;

    public EntityControllerRouteConvention(EntityControllerConfigs controllerConfigs)
    {
        _controllerConfigs = controllerConfigs;
    }

    private EntityControllerConfig? GetConfig(Type controllerType) =>
        _controllerConfigs.FirstOrDefault(x => x.ControllerType == controllerType);

    public void Apply(ControllerModel controllerModel)
    {
        IActionRouteConfig? actionRouteConfig = GetConfig(controllerModel.ControllerType);
        if (actionRouteConfig == null)
            return;
        var controllerActionName = nameof(IEntityControllerAction<object>.ControllerAction);
        var controllerAction = controllerModel.Actions.FirstOrDefault(a => a.ActionName == controllerActionName);
        if (controllerAction == null)
            throw new InvalidOperationException($"Could not find {controllerActionName} on configured controller {controllerModel.ControllerType}");
        controllerAction.Selectors.Clear();
        var selectorModel = new SelectorModel
        {
            AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(actionRouteConfig.Route)),
        };
        selectorModel.ActionConstraints.Add(
            new HttpMethodActionConstraint(new [] { "POST"})
        );

        actionRouteConfig.AuthorizationConfigs.ForAll(authCfg => 
            selectorModel.EndpointMetadata.Add(
                new AuthorizeAttribute()
                { 
                    AuthenticationSchemes = authCfg.AuthenticationSchemes,
                    Policy = authCfg.Policy,
                    Roles = authCfg.Roles,
                }
            )
        );
        
        controllerAction.Selectors.Add(selectorModel);
    }
}