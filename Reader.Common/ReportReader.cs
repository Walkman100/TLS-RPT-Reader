using System;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Threading.Tasks;

namespace Reader.Common {
    public static class ReportReader {
        private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions() {
            UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow,
        };

        public static async Task<Report> Read(Stream stream) {
            if (!stream.CanSeek) {
                var ms = new MemoryStream((int)stream.Length);
                await stream.CopyToAsync(ms);
                await stream.DisposeAsync();
                stream = ms;
            }

            using (stream) {
                stream.Position = 0;

                var buffer = new byte[2];
                if (await stream.ReadAsync(buffer.AsMemory()) == 2 &&
                    buffer[0] == 0x1F && buffer[1] == 0x8B // GZIP header (1F 8B)
                ) {
                    stream.Position = 0;
                    using var gZipStream = new GZipStream(stream, CompressionMode.Decompress);
                    return await read(gZipStream);

                } else {
                    stream.Position = 0;
                    return await read(stream);
                }
            }
        }

        private static async Task<Report> read(Stream stream) {
            var rtn = await JsonSerializer.DeserializeAsync<Report>(stream, jsonOptions);
            ArgumentNullException.ThrowIfNull(rtn);
            return rtn;
        }
    }
}
