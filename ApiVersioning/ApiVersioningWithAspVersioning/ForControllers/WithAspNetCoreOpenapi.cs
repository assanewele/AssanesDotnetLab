#:property PublishAot=false
#:sdk Microsoft.NET.Sdk.Web
#:package Asp.Versioning.Mvc@10.0.0
#:package Asp.Versioning.Mvc.ApiExplorer@10.0.0
#:package Asp.Versioning.OpenApi@10.0.0-rc.1

using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// We don't need to customize the API versioning options for this example as we are using query string versioning.
builder.Services.AddApiVersioning()
.AddApiExplorer(options =>
{
    // Calling "AddApiExplorer" is required for OpenAPI versioning to work correctly.
    // Without this, the generated OpenAPI documents will not be versioned.

    // GroupNameFormat specifies the format of the API version.
    // Without this, versioning will use the literal group names. In our case, that would be 1.0.
    // For compatibility with the "default" /openapi/v1.json behavior from Microsoft.AspNetCore.OpenApi, we use v'VVV' so we can retrieve it using v1.json.
    // See https://github.com/dotnet/aspnet-api-versioning/wiki/Version-Format#custom-api-version-format-strings for more information about formatting API versions.
    options.GroupNameFormat = "'v'VVV";
})
.AddMvc()
// You must call "AddOpenApi" after "AddApiVersioning" to ensure you use Asp.Versioning's variant.
// This variant of "AddOpenApi" is required to properly integrate with API versioning and generate versioned OpenAPI documents.
// You can call an overload of "AddOpenApi" to customize the OpenAPI generation, just like you would with Microsoft.AspNetCore.OpenApi's "AddOpenApi".
.AddOpenApi();

var app = builder.Build();

// WithDocumentPerVersion() is an extension method provided by the Asp.Versioning.OpenApi package.
// It configures the OpenAPI endpoint to generate a separate document for each API version.
// This allows clients to retrieve documentation specific to the version of the API they are using.
// This approach is preferable compared to having to call "services.AddOpenApi()" multiple times for each version, which can lead to maintenance issues and potential misconfigurations when adding new versions.
app.MapOpenApi().WithDocumentPerVersion();

app.MapControllers();

app.Run();

// For a file-based app, paste the controller classes from
[ApiController]
[Route("api/users")]
[ApiVersion("1.0")]
public class UsersV1Controller : ControllerBase
{
    [HttpGet]
    public ActionResult<UserV1[]> Get()
    {
        return Ok(new[]
        {
            new UserV1(1, "John Doe"),
            new UserV1(2, "Alice Dewett"),
        });
    }
}

[ApiController]
[Route("api/users")]
[ApiVersion("2.0")]
public class UsersV2Controller : ControllerBase
{
    [HttpGet]
    public ActionResult<UserV2[]> Get()
    {
        return Ok(new[]
        {
            new UserV2(1, "John Doe", new DateOnly(1990, 1, 1)),
            new UserV2(2, "Alice Dewett", new DateOnly(1992, 2, 2)),
        });
    }
}

public record UserV1(int Id, string Name);
public record UserV2(int Id, string Name, DateOnly BirthDate);

// You can now retrieve your versioned OpenAPI documents at /openapi/v1.json and /openapi/v2.json for the first and second version of the API, respectively!