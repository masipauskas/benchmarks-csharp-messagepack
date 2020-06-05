using BenchmarkDotNet.Running;

namespace BenchmarkMessagePack
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<DeserializeUsingStreamsBenchmark>();
            BenchmarkRunner.Run<DeserializeUsingMemoryBenchmark>();
        }
    }
}