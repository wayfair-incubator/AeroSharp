using System.IO;
using K4os.Compression.LZ4.Streams;

namespace AeroSharp.Compression
{
    internal class LZ4Compressor : ICompressor
    {
        public byte[] Compress(byte[] data)
        {
            using (var dataStream = new MemoryStream(data))
            using (var compressed = new MemoryStream())
            {
                using (var zip = LZ4Stream.Encode(compressed))
                {
                    dataStream.CopyTo(zip);
                }

                return compressed.ToArray();
            }
        }

        public byte[] Decompress(byte[] data)
        {
            using (var dataStream = new MemoryStream(data))
            using (var decompressed = new MemoryStream())
            {
                using (var zip = LZ4Stream.Decode(dataStream))
                {
                    zip.CopyTo(decompressed);
                }

                return decompressed.ToArray();
            }
        }
    }
}
