using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reader.Common.Data.Models {
    public class ReportPolicy {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public long ID { get; set; }
        public string? PolicyType { get; set; }
        public string? PolicyStrings { get; set; }
        public string? PolicyDomain { get; set; }
        public string? MXHosts { get; set; }
        public string? FailureDetails { get; set; }
        public int? TotalSuccessfulSessionCount { get; set; }
        public int? TotalFailureSessionCount { get; set; }

        public long ReportID { get; set; }
        [ForeignKey(nameof(ReportID))]
        [InverseProperty(nameof(Report.Policies))]
        public Report? Report { get; set; }
    }
}
