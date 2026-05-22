#:property PublishAot=false
#:sdk Microsoft.NET.Sdk.Web
#:package Asp.Versioning.Http@10.0.0

using Asp.Versioning;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
    {
        //By default, Query strin versioning is used
        //So, we don't need to add it if it's what we want to use, we can avoid adding it explicitly
        options.ApiVersionReader = new QueryStringApiVersionReader("api-version");

        //Set the default API version to 1.0
        options.DefaultApiVersion = new ApiVersion(2, 0);
        // If the user does not specify a version, you can let the API use the default version
        // This is disabled by default.
        options.AssumeDefaultVersionWhenUnspecified = true;
    }
);

var app = builder.Build();

var usersApi = app.NewVersionedApi("Users");

var usersv1 = usersApi.MapGroup("api/users").HasApiVersion(1.0);
var usersv2 = usersApi.MapGroup("api/users").HasApiVersion(2.0);

usersv1.MapGet("", () => TypedResults.Ok(new[]
{
    new UserV1(1, "John Doe"),
    new UserV1(2, "Alice Dewett"),
}));

usersv2.MapGet("", () => TypedResults.Ok(new[]
{
    new UserV2(1, "John Doe", new DateOnly(1990, 1, 1)),
    new UserV2(2, "Alice Dewett", new DateOnly(1992, 2, 2)),
}));

app.Run();

record UserV1(int Id, string Name);
record UserV2(int Id, string Name, DateOnly BirthDate);

//  Reach these endpoints by going to api/users?api-version=1.0 for the first version, and api/users?api-version=2.0 for the second version!