using System.IO;
using LZ4;

namespace AeroSharp.Compression
{
    internal class LZ4Compressor : ICompressor
    {
        public byte[] Compress(byte[] data)
        {
            using (var dataStream = new MemoryStream(data))
            using (var compressed = new MemoryStream())
            {
                using (var zip = new LZ4Stream(compressed, LZ4StreamMode.Compress))
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
                using (var zip = new LZ4Stream(dataStream, LZ4StreamMode.Decompress))
                {
                    zip.CopyTo(decompressed);
                }

                return decompressed.ToArray();
            }
        }
    }
}
