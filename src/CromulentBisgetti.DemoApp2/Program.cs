using CromulentBisgetti.ContainerPacking;
using CromulentBisgetti.ContainerPacking.Entities;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(opts =>
{
    // Use PascalCase
    opts.SerializerOptions.PropertyNamingPolicy = null;
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapPost("/api/containerpacking", async (HttpContext context) =>
{
    var req = await context.Request.ReadFromJsonAsync<ContainerPackingRequest>();
    return PackingService.Pack(req.Containers, req.ItemsToPack, req.AlgorithmTypeIDs);
});

app.Run();

public record ContainerPackingRequest(List<Container> Containers, List<Item> ItemsToPack, List<int> AlgorithmTypeIDs);