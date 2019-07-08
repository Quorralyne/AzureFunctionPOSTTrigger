#r "Newtonsoft.Json"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
    log.LogInformation("C# HTTP trigger function processed a request.");

    //Pull out the header values passed into the request
    var headers = req.Headers;
    if(!headers.TryGetValues("token", out var token))
    {
        return req.CreateResponse(HttpStatusCode.BadRequest);
    }
    if(!headers.TryGetValues("client_id", out var client_idd))
    {
        return req.CreateResponse(HttpStatusCode.BadRequest);
    }
    if(!headers.TryGetValues("client_secret", out var client_secret))
    {
        return req.CreateResponse(HttpStatusCode.BadRequest);
    }
    var accessToken = token.First();
    var clientId = client_id.First();
    var clientSecret = client_secret.First();

    //Call the Okta introspection API to validate the token.
    var baseUrl = "https://dev-414346.okta.com/oauth2/default/v1/introspect";

    var content = new FormUrlEncodedContent(new[]
    {
        new KeyValuePair<string, string>("token", accessToken),
        new KeyValuePair<string, string>("token_type_hint", "access_token"),
        new KeyValuePair<string, string>("client_id", clientId),
        new KeyValuePair<string, string>("client_secret", clientSecret)
    });
    var response = await _httpClient.PostAsync(baseUrl, content);
    var result = await response.Content.ReadAsStringAsync();

    log.LogInformation("C# HTTP trigger function processed an external API call to Okta.");

    //Based on the token validation from Okta, return a response
    if(result.IsSuccess)
    {
        return req.CreateResponse(HttpStatusCode.OK, "Hello, you have access to this API")
    }
    else
    {
        return req.CreateResponse(HttpStatusCode.Forbidden, "Sorry, you do not have access to this API")
    }
}
