using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PluckFish.Attributes
{
    public class ApiTokenAuthAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string headerName;

        public ApiTokenAuthAttribute(string headerName = "X-API-TOKEN")
        {
            this.headerName = headerName;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var expectedToken = "Test";

            var request = context.HttpContext.Request;
            string providedToken = request.Headers[headerName];

            if (string.IsNullOrEmpty(providedToken))
            {
                providedToken = request.Query["api_token"];
            }

            if (string.IsNullOrEmpty(providedToken) || providedToken != expectedToken)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}