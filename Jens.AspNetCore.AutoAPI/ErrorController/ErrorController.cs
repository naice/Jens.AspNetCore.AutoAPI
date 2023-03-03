using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;

namespace Jens.AspNetCore.AutoAPI
{
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public abstract class ErrorController : ControllerBase
    {
        public const string ROUTE = "/error";

        [NonAction]
        protected IActionResult BaseResponseBadRequest(string message) 
        {
            return BadRequest(new BadRequestResponse() { Success = false, Message = message });
        }

        protected virtual IActionResult HandleError(IExceptionHandlerFeature context)
        {
            var exception = context.Error;
            var message = ResolveMessages(exception);
            return BaseResponseBadRequest(message);
        }

        [Route(ROUTE)]
        public IActionResult Error()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            if (context == null) throw new InvalidOperationException();
            return HandleError(context);
        }

        protected string ResolveMessages(Exception? exception)
        {
            if (exception == null) return "";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(exception.Message);
            sb.AppendLine(ResolveMessages(exception.InnerException));
            return sb.ToString();
        }
    }
}
