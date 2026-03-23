using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Reader.Common.Data.Models {
    [Index(nameof(UniqueReportID), IsUnique = true)]
    public class Report {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public long ID { get; set; }
        public required string UniqueReportID { get; set; }
        public string? OrganizationName { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string? ContactInfo { get; set; }
        public string? ReportID { get; set; }

        [InverseProperty(nameof(ReportPolicy.Report))]
        public ICollection<ReportPolicy>? Policies { get; set; }
    }
}
