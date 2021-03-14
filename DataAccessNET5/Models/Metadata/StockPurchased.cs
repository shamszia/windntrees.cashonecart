using DataAccessNET5.Models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DataAccessNET5.Models
{
    [MetadataType(typeof(StockPurchasedMetaData))]
    public partial class StockPurchased
    {
        [IgnoreDataMember]
        public decimal Total { get; set; }
    }

    [DataContract]
    public partial class StockPurchasedMetaData
    {
        [IgnoreDataMember]
        public virtual Place Place { get; set; }
        [IgnoreDataMember]
        public virtual Product Product { get; set; }
        [IgnoreDataMember]
        public virtual StockTransaction StockTransaction { get; set; }
    }
}
