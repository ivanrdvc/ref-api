using RefApi.Common;
using RefApi.Extensions;
using RefApi.Features.Chat;
using RefApi.Features.Config;
using RefApi.Features.Conversations;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();
builder.Services.AddHealthChecks();

builder.Services
    .AddCustomAuthentication(builder.Configuration)
    .AddDefaultOpenApi()
    .AddCustomApiVersioning()
    .AddDefaultCorsPolicy(builder.Configuration)
    .AddCustomProblemDetails();

builder.Services.AddExceptionHandler<GlobalProblemDetailsHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseDefaultOpenApi();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapChatApi();
app.MapConversationApi();
app.MapConfigApi();
app.MapHealthChecks("/healthz");

app.Run();

public partial class Program
{
}