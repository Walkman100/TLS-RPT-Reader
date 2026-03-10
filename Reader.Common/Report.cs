using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Reader.Common {
    public class Report {
        [JsonPropertyName("organization-name")]
        public string? OrganizationName { get; set; }
        [JsonPropertyName("date-range")]
        public DateRange? DateRange { get; set; }
        [JsonPropertyName("contact-info")]
        public string? ContactInfo { get; set; }
        [JsonPropertyName("report-id")]
        public string? ReportID { get; set; }
        [JsonPropertyName("policies")]
        public List<PolicyContainer>? Policies { get; set; }
    }

    public class DateRange {
        [JsonPropertyName("start-datetime")]
        public DateTime? Start { get; set; }
        [JsonPropertyName("end-datetime")]
        public DateTime? End { get; set; }
    }
    public class PolicyContainer {
        [JsonPropertyName("policy")]
        public PolicyValue? Policy { get; set; }
        [JsonPropertyName("summary")]
        public PolicySummary? Summary { get; set; }
        [JsonPropertyName("failure-details")]
        public List<object>? FailureDetails { get; set; }
    }
    public class PolicyValue {
        [JsonPropertyName("policy-type")]
        public string? PolicyType { get; set; }
        [JsonPropertyName("policy-string")]
        public List<string>? PolicyString { get; set; }
        [JsonPropertyName("policy-domain")]
        public string? PolicyDomain { get; set; }
        [JsonPropertyName("mx-host")]
        public List<string>? MXHost { get; set; }
    }
    public class PolicySummary {
        [JsonPropertyName("total-successful-session-count")]
        public int? TotalSuccessfulSessionCount { get; set; }
        [JsonPropertyName("total-failure-session-count")]
        public int? TotalFailureSessionCount { get; set; }
    }
}
