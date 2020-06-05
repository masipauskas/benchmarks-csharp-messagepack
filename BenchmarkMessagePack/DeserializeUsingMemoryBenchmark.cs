using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using MessagePack;

namespace BenchmarkMessagePack
{
    [SimpleJob(RuntimeMoniker.NetCoreApp22)]
    [SimpleJob(RuntimeMoniker.NetCoreApp30)]
    [RPlotExporter]
    [MarkdownExporter]
    public class DeserializeUsingMemoryBenchmark
    {
        private byte[] _payload;
        private MessagePackSerializerOptions _options;

        [GlobalSetup]
        public void SetUp()
        {
            var objects = Enumerable
                .Range(0, 5000)
                .Select(i => new Holder {Id = i})
                .ToList();

            _options = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);

            using (var stream = new MemoryStream())
            {
                MessagePackSerializer.Serialize(stream, objects, _options);
                _payload = stream.ToArray();
            }
        }

        [Benchmark(Baseline = true)]
        public void DeserializeUsingStreams1Thread()
        {
            DeserializeUsingMemory(1);
        }

        [Benchmark]
        public void DeserializeUsingStreams2Threads()
        {
            DeserializeUsingMemory(2);
        }

        [Benchmark]
        public void DeserializeUsingStreams4Threads()
        {
            DeserializeUsingMemory(4);
        }

        [Benchmark]
        public void DeserializeUsingStreams8Threads()
        {
            DeserializeUsingMemory(8);
        }

        private IEnumerable<byte[]> GetPayloadsToDeserialize()
        {
            var chunks = 32;
            return Enumerable.Repeat(_payload, chunks);
        }

        private void DeserializeUsingMemory(int numThreads)
        {
            var options = new ParallelOptions {MaxDegreeOfParallelism = numThreads};
            var payloads = GetPayloadsToDeserialize();
            Parallel.ForEach(payloads, options, payload =>
            {
                var memory = new Memory<byte>(payload);
                var _ = MessagePackSerializer.Deserialize<List<Holder>>(memory, _options);
            });
        }
    }
}