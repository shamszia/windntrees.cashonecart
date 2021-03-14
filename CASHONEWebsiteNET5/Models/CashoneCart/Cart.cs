using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Models.CashoneCart
{
    [Serializable]
    public class Cart
    {
        public string UserId { get; set; }
        public string SessionId { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string Email { get; set; }
        public string Cell { get; set; }
        public string Status { get; set; }

        [JsonIgnore]
        public List<CartItem> Items = new List<CartItem>();

        public Cart()
        {

        }

        public Cart(string id)
        {
            SessionId = id;
        }
    }
}
