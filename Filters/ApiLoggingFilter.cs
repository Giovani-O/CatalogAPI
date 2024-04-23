using Microsoft.AspNetCore.Mvc.Filters;

namespace CatalogAPI.Filters
{
    public class ApiLoggingFilter : IActionFilter
    {
        private ILogger<ApiLoggingFilter> _logger;

        // Constructor with dependency injection
        public ApiLoggingFilter(ILogger<ApiLoggingFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Runs before the action
            _logger.LogInformation($"### Executando -> OnActionExecuting ###");
            _logger.LogInformation($"### ############################### ###");
            _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
            _logger.LogInformation($"ModelState: {context.ModelState.IsValid}");
            _logger.LogInformation($"### ############################### ###");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Runs after the action
            _logger.LogInformation($"### Executando -> OnActionExecuted ###");
            _logger.LogInformation($"### ############################## ###");
            _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
            _logger.LogInformation($"Status Code: {context.HttpContext.Response.StatusCode}");
            _logger.LogInformation($"### ############################## ###");
        }
    }
}
