using MessagePack;

namespace BenchmarkMessagePack
{
    [MessagePackObject]
    public class Holder {
        [Key(1)]
        public int Id { get; set; }
    }
}