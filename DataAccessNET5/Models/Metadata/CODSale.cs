using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessNET5.Models
{
    public partial class CODSale
    {
        [IgnoreDataMember]
        public DateTime CODTimeLocal { get { return CODTimeLocal.ToLocalTime(); } }
    }
}
