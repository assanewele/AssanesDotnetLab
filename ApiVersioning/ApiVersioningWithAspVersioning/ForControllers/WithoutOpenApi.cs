#:property PublishAot=false
#:sdk Microsoft.NET.Sdk.Web
#:package Asp.Versioning.Mvc@10.0.0

using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddApiVersioning(options =>
{
    // options.ApiVersionReader = new UrlSegmentApiVersionReader();
    // options.ApiVersionReader = ApiVersionReader.Combine(
    //     new UrlSegmentApiVersionReader(),
    //     new HeaderApiVersionReader("X-API-Version")
    // );
}

).AddMvc();

var app = builder.Build();

app.MapControllers();
app.Run();

// For a file-based app, controller classes go below app.Run()

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

/*
 Asp.Versioning supports query string versioning by default.
 You can now reach these endpoints by going to api/users?api-version=1.0
  for the first version, and api/users?api-version=2.0 for the second version!
*/