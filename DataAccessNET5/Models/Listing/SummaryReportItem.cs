using System;
using System.Runtime.Serialization;

namespace DataAccessNET5.Models.Listing
{
    [DataContract]
    public class SummaryReportItem
    {
        [DataMember]
        public Nullable<DateTime> SummaryTime { get; set; }

        [IgnoreDataMember]
        public Nullable<System.DateTime> SummaryTimeLocal { get { return ((DateTime)SummaryTime).ToLocalTime(); } }

        [DataMember]
        public Nullable<long> ReferenceNumber { get; set; }

        [DataMember]
        public string Account { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Place { get; set; }

        [DataMember]
        public decimal Amount { get; set; }
    }
}
