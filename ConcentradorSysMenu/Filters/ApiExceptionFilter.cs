using ConcentradorSysMenu.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ConcentradorSysMenu.Filters;

public class ApiExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _Logger;

    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
    {
        _Logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _Logger.LogError(context.ToString());

        context.Result = new ObjectResult(new Response { Status = false, Messages = new List<string> { "Erro ao processar sua requisião" } }) { StatusCode = StatusCodes.Status500InternalServerError };
    }
}
