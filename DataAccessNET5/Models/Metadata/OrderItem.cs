using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DataAccessNET5.Models
{
    public partial class OrderItem
    {
        [NotMapped]
        [IgnoreDataMember]
        public decimal LineTotal
        {
            get
            {
                return ((Cost * Quantity) - ((Discount == null ? 0 : ((decimal)Discount)) * Quantity));
            }
        }

        [NotMapped]
        [IgnoreDataMember]
        public decimal ItemDiscount
        {
            get
            {
                return (Discount == null ? 0 : ((decimal)Discount));
            }
        }
    }
}
