using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filteri;

public class ValidacijaActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument == null) continue;

            var validatorTip = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (context.HttpContext.RequestServices.GetService(validatorTip) is not IValidator validator)
                continue;

            var rezultat = await validator.ValidateAsync(new ValidationContext<object>(argument));
            if (!rezultat.IsValid)
            {
                var greske = rezultat.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                context.Result = new BadRequestObjectResult(new { Poruka = "Neispravan unos.", Greske = greske });
                return;
            }
        }

        await next();
    }
}
