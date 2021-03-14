using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataAccessNET5.Models
{   
    public partial class Product
    {
        [NotMapped]
        public string PicturePath { 
            get 
            {
                return "/home/getpicture/" + Code;
            }
        }
        [NotMapped]
        public string FormattedPublishedPrice
        {
            get
            {
                return PublishedPrice == null ? "0" : ((decimal)PublishedPrice).ToString("0");
            }
        }

        [NotMapped]
        public string FormattedPublishedPriceWith2DecimalPoints
        {
            get
            {
                return PublishedPrice == null ? "0.00" : ((decimal)PublishedPrice).ToString("0.00");
            }
        }

        [NotMapped]
        public string ProductName { get; set; }
        [NotMapped]
        public string PlaceName { get; set; }
        [NotMapped]
        public int AvailableQuantity { get; set; }
    }
}
