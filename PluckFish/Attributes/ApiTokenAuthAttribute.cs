using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PluckFish.Components.PostgresRepositories.API;
using PluckFish.Interfaces.API;
using System.Net;

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
            var apiRepository = context.HttpContext.RequestServices.GetRequiredService<IVerificationApi>();

            var request = context.HttpContext.Request;
            string providedToken = request.Headers[headerName];

            if (string.IsNullOrEmpty(providedToken))
            {
                providedToken = request.Query["api_token"];
            }

            if (string.IsNullOrEmpty(providedToken))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (apiRepository.Verify(providedToken)) 
            {
                // Do nothing if authorized
                return;
            }

            context.Result = new UnauthorizedResult();
        }
    }
}