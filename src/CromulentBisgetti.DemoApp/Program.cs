using CromulentBisgetti.ContainerPacking;
using CromulentBisgetti.ContainerPacking.Entities;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

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
    if (req == null) return Results.BadRequest();
    return Results.Ok(PackingService.Pack(req.Containers, req.ItemsToPack, req.AlgorithmTypeIDs));
});

app.MapPost("/api/containerpacking/multibox", async (HttpContext context) =>
{
    var req = await context.Request.ReadFromJsonAsync<ContainerPackingRequest>();
    if (req == null) return Results.BadRequest();

    var all = new List<ContainerPackingResult>();
    do
    {
        var results = PackingService.Pack(req.Containers, req.ItemsToPack, req.AlgorithmTypeIDs);
        var bestBox = results.OrderByDescending(x => x.AlgorithmPackingResults[0].PercentContainerVolumePacked).First();
        all.Add(bestBox);

        foreach (var packedItem in bestBox.AlgorithmPackingResults[0].PackedItems)
        {
            var item = req.ItemsToPack.First(x => x.ID == packedItem.ID);
            item.Quantity -= packedItem.Quantity;
        }

        req = req with { ItemsToPack = req.ItemsToPack.Where(x => x.Quantity > 0).ToList() };
    } while (!all.Last().AlgorithmPackingResults[0].IsCompletePack);
    return Results.Ok(all);
});

app.Run();

public record ContainerPackingRequest(List<Container> Containers, List<Item> ItemsToPack, List<int> AlgorithmTypeIDs);