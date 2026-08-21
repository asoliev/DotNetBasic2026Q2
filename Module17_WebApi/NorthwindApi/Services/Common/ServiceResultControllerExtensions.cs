using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NorthwindApi.Services.Common;

public static class ServiceResultControllerExtensions
{
    public static ActionResult<T> ToGetActionResult<T>(this ControllerBase controller, ServiceResult<T> result)
        => HandleResult(controller, result, value => controller.Ok(value));

    public static IActionResult ToMutationActionResult(this ControllerBase controller, ServiceResult<bool> result)
        => HandleMutationResult(controller, result, controller.NoContent);

    public static ActionResult<T> ToCreatedActionResult<T>(this ControllerBase controller, ServiceResult<T> result, string actionName, object routeValues)
        => HandleResult(controller, result, value => controller.CreatedAtAction(actionName, routeValues, value));

    public static ModelStateDictionary ToModelStateDictionary(this IReadOnlyDictionary<string, string[]>? errors)
    {
        ModelStateDictionary modelState = [];

        if (errors is null)
            return modelState;

        foreach (KeyValuePair<string, string[]> error in errors)
        {
            foreach (string message in error.Value)
            {
                modelState.AddModelError(error.Key, message);
            }
        }

        return modelState;
    }

    private static ActionResult<T> HandleResult<T>(ControllerBase controller, ServiceResult<T> result, Func<T, ActionResult<T>> onSuccess)
    {
        switch (result.Status)
        {
            case ServiceResultStatus.Success:
                return onSuccess(result.Value!);
            case ServiceResultStatus.NotFound:
                return controller.NotFound();
            case ServiceResultStatus.Conflict:
                return controller.Conflict(result.Message);
            case ServiceResultStatus.ValidationFailed:
                return controller.ValidationProblem(result.Errors.ToModelStateDictionary());
            default:
                return controller.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    private static IActionResult HandleMutationResult(ControllerBase controller, ServiceResult<bool> result, Func<IActionResult> onSuccess)
    {
        switch (result.Status)
        {
            case ServiceResultStatus.Success:
                return onSuccess();
            case ServiceResultStatus.NotFound:
                return controller.NotFound();
            case ServiceResultStatus.Conflict:
                return controller.Conflict(result.Message);
            case ServiceResultStatus.ValidationFailed:
                return controller.ValidationProblem(result.Errors.ToModelStateDictionary());
            default:
                return controller.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
