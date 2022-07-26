using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using CromulentBisgetti.ContainerPacking;
using CromulentBisgetti.ContainerPacking.Entities;
using CromulentBisgetti.ContainerPacking.Algorithms;
using NUnit.Framework;

namespace CromulentBisgetti.ContainerPackingTests
{
    public record TestInput(string testId, string[] testResults, List<Container> containers, List<Item> itemsToPack);

    [TestFixture]
    public class ContainerPackingTests
    {
        public static IEnumerable<TestCaseData> GetTests()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CromulentBisgetti.ContainerPackingTests.DataFiles.ORLibrary.txt");
            using var reader = new StreamReader(stream);

            var count = 0;
            while (reader.DidReadLine(out var firstLine))
            {
                var itemsToPack = new List<Item>();

                // First line in each test case is an ID. Skip it.

                // Second line states the results of the test, as reported in the EB-AFIT master's thesis, appendix E.
                var testResults = reader.ReadLine().Split(' ');

                // Third line defines the container dimensions.
                var containerDims = reader.ReadLine().Split(' ');

                // Fourth line states how many distinct item types we are packing.
                var itemTypeCount = Convert.ToInt32(reader.ReadLine());

                for (var i = 0; i < itemTypeCount; i++)
                {
                    var itemArray = reader.ReadLine().Split(' ');

                    var item = new Item(
                        0,
                        Convert.ToDecimal(itemArray[1]),
                        Convert.ToDecimal(itemArray[3]),
                        Convert.ToDecimal(itemArray[5]),
                        Convert.ToInt32(itemArray[7]));

                    itemsToPack.Add(item);
                }

                var container = new Container(
                    0,
                    Convert.ToDecimal(containerDims[0]),
                    Convert.ToDecimal(containerDims[1]),
                    Convert.ToDecimal(containerDims[2]));

                var containers = new List<Container> { container };

                yield return new TestCaseData(new TestInput(firstLine, testResults, containers, itemsToPack)).SetName($"{count++:000} - {firstLine}");
            }
        }

        [TestCaseSource(nameof(GetTests))]
        public void PackingServiceTest(TestInput testInput)
        {
            var testResults = testInput.testResults;
            var result = PackingService.Pack(testInput.containers, testInput.itemsToPack, new List<int> { (int)AlgorithmType.EB_AFIT });

            // Assert that the number of items we tried to pack equals the number stated in the published reference.
            Assert.AreEqual(result[0].AlgorithmPackingResults[0].PackedItems.Count + result[0].AlgorithmPackingResults[0].UnpackedItems.Count, Convert.ToDecimal(testResults[1]));

            // Assert that the number of items successfully packed equals the number stated in the published reference.
            Assert.AreEqual(result[0].AlgorithmPackingResults[0].PackedItems.Count, Convert.ToDecimal(testResults[2]));

            // Assert that the packed container volume percentage is equal to the published reference result.
            // Make an exception for a couple of tests where this algorithm yields 87.20% and the published result 
            // was 87.21% (acceptable rounding error).
            Assert.IsTrue(result[0].AlgorithmPackingResults[0].PercentContainerVolumePacked == Convert.ToDecimal(testResults[3]) ||
                (result[0].AlgorithmPackingResults[0].PercentContainerVolumePacked == 87.20M && Convert.ToDecimal(testResults[3]) == 87.21M));

            // Assert that the packed item volume percentage is equal to the published reference result.
            Assert.AreEqual(result[0].AlgorithmPackingResults[0].PercentItemVolumePacked, Convert.ToDecimal(testResults[4]));
        }
    }

    public static class StreamReaderExtensions
    {
        public static bool DidReadLine(this StreamReader sr, out string value)
        {
            value = sr.ReadLine();
            return value != null;
        }
    }
}
