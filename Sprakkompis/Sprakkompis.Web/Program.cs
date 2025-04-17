using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;
using Sprakkompis.Core.Entities;
using Sprakkompis.Infrastructure.Data;
using Sprakkompis.Web.Components;
using Sprakkompis.Web.Features;
using Sprakkompis.Web.Features.Identity.Login;
using Sprakkompis.Web.Features.Identity.Logout;
using Sprakkompis.Web.Features.Identity.Register;
using System.Text.Json.Serialization;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMudServices();

var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Program");


var pgUser = Environment.GetEnvironmentVariable("PG_USER") ?? "postgres";
var pgPass = Environment.GetEnvironmentVariable("PG_PASS") ?? "";
logger.LogInformation($"Using database credentials - User: {pgUser}, Password length: {(pgPass?.Length ?? 0)}");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (connectionString is not null)
{
    connectionString = connectionString
        .Replace("${PG_USER}", pgUser)
        .Replace("${PG_PASS}", pgPass);

    var maskedConnectionString = connectionString.Replace(pgPass, "********");
    logger.LogInformation($"Connection string: {maskedConnectionString}");
}
else
{
    logger.LogError("Connection string 'DefaultConnection' not found in configuration");
}


// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseNpgsql(connectionString));

builder.Services.AddIdentityApiEndpoints<ApplicationUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;

    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();



builder.Services.AddAuthenticationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
builder.Services.AddAuthorization();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddOpenApi();
builder.Services.AddHttpClient<RegisterService>();
builder.Services.AddHttpClient<LoginService>();
builder.Services.AddHttpClient<LogoutService>();

var app = builder.Build();

// Configure the HTTP request pipeline.


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/api/identity")
    .MapIdentityApi<ApplicationUser>()
    .WithTags("Identity");

app.MapOpenApi();
app.MapScalarApiReference();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
    

app.MapEndpoints<Program>();

app.Run();
