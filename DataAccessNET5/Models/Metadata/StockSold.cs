using DataAccessNET5.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DataAccessNET5.Models
{
    [MetadataType(typeof(StockSoldMetaData))]
    public partial class StockSold
    {
        [IgnoreDataMember]
        public decimal Total { get; set; }
    }

    [DataContract]
    public partial class StockSoldMetaData
    {
        [IgnoreDataMember]
        public virtual Place Place { get; set; }
        [IgnoreDataMember]
        public virtual Product Product { get; set; }
        [IgnoreDataMember]
        public virtual SalesTransaction SalesTransaction { get; set; }
    }
}
