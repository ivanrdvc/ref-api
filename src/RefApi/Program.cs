using RefApi.Common;
using RefApi.Extensions;
using RefApi.Features.Chat;
using RefApi.Features.Config;
using RefApi.Features.Conversations;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();
builder.Services.AddHealthChecks();

// Add API-related services
builder.Services
    .AddCustomAuthentication(builder.Configuration)
    .AddCustomApiVersioning()
    .AddDefaultCorsPolicy(builder.Configuration)
    .AddCustomProblemDetails()
    .AddOpenApi();

builder.Services.AddExceptionHandler<GlobalProblemDetailsHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

var versionSet = app.NewApiVersionSet().Build();

app.MapChatApiV1(versionSet);
app.MapConversationApiV1(versionSet);
app.MapConfigApiV1(versionSet);
app.MapHealthChecks("/healthz");

app.Run();

public partial class Program
{
}