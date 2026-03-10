using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
                    try {
                        return await read(gZipStream);
                    } catch (JsonException) {
                        Console.WriteLine("Failed to parse JSON:");
                        stream.Position = 0;
                        var sr = new StreamReader(gZipStream);
                        Console.WriteLine(sr.ReadToEnd());

                        throw;
                    }

                } else {
                    stream.Position = 0;
                    try {
                        return await read(stream);
                    } catch (JsonException) {
                        Console.WriteLine("Failed to parse JSON:");
                        stream.Position = 0;
                        var sr = new StreamReader(stream);
                        Console.WriteLine(sr.ReadToEnd());

                        throw;
                    }
                }
            }
        }

        private static async Task<Report> read(Stream stream) {
            var rtn = await JsonSerializer.DeserializeAsync<Report>(stream, jsonOptions);
            ArgumentNullException.ThrowIfNull(rtn);
            return rtn;
        }

        public static Data.Models.Report ConvertToDB(Report report) =>
            new Data.Models.Report() {
                OrganizationName = report.OrganizationName,
                StartDate = report.DateRange?.Start,
                EndDate = report.DateRange?.End,
                ContactInfo = report.ContactInfo,
                ReportID = report.ReportID,
                Policies = report.Policies?.Select(container => new Data.Models.ReportPolicy() {
                    PolicyType = container.Policy?.PolicyType,
                    PolicyStrings = container.Policy?.PolicyString == null ? null : string.Join(';', container.Policy?.PolicyString ?? []),
                    PolicyDomain = container.Policy?.PolicyDomain,
                    MXHosts = container.Policy?.MXHost == null ? null : string.Join(';', container.Policy?.MXHost ?? []),
                    FailureDetails = container.FailureDetails == null ? null : string.Join(';', container.FailureDetails),
                    TotalSuccessfulSessionCount = container.Summary?.TotalSuccessfulSessionCount,
                    TotalFailureSessionCount = container.Summary?.TotalFailureSessionCount,
                }).ToList(),
            };
    }
}
