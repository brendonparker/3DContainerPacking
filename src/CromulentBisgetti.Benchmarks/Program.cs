using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CromulentBisgetti.ContainerPacking;
using CromulentBisgetti.ContainerPacking.Algorithms;
using CromulentBisgetti.ContainerPacking.Entities;

var summary = BenchmarkRunner.Run(typeof(ContainerPackingBenchmarks));

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class ContainerPackingBenchmarks
{
    [Benchmark]
    public void Benchmark1()
    {
        var containers = new List<Container>
        {
            new Container(1, 587, 233, 220)
        };
        var items = new List<Item>
        {
            new Item(1, 74, 60, 32, 19),
            new Item(2, 100, 50, 43, 19),
            new Item(3, 95, 74, 42, 25),
            new Item(4, 95, 70, 30, 21),
            new Item(5, 45, 37, 24, 22),
            new Item(6, 67, 60, 25, 19),
            new Item(7, 55, 51, 35, 20),
            new Item(8, 76, 71, 61, 21),
        };
        PackingService.Pack(containers, items, new List<int> { (int)AlgorithmType.EB_AFIT });
    }
}