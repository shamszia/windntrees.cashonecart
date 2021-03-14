using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Principal;

namespace Application.Models.CashoneCart.Repository
{
    public class SessionCartRepository : CartRepositoy
    {
        public ISession Session { get; set; }
        public IPrincipal Principal { get; set; }

        protected override Cart Cart {

            get
            {
                m_Cart = (Cart)Utility.ObjectFormatter.GetInstanceObject(Session.Get("CASHONECart"));
                if (m_Cart == null)
                {
                    m_Cart = new Cart(Session.Id);
                    Session.Set("CASHONECart", Utility.ObjectFormatter.GetInstanceBytes(m_Cart));
                }
                return m_Cart;
            }
        }

        protected override void SaveCart()
        {
            if (m_Cart != null)
            {
                Session.Set("CASHONECart", Utility.ObjectFormatter.GetInstanceBytes(m_Cart));
            }
        }

        public SessionCartRepository()
        {
            
        }

        public Cart EmptyCart([FromBody]Cart cart)
        {
            Session.Remove("CASHONECart");
            return this.Cart;
        }

        public Cart GetCart(string id)
        {
            return this.Cart;
        }

        public Cart UpdateCart(Cart cart)
        {
            if (this.Cart.SessionId != Session.Id)
            {
                Session.Remove("CASHONECart");
                throw new Exception("Session is not valid, try your cart again.");
            }

            this.Cart.Title = cart.Title;
            this.Cart.Company = cart.Company;
            this.Cart.Address = cart.Address;
            this.Cart.City = cart.City;
            this.Cart.Country = cart.Country;
            this.Cart.Email = cart.Email;
            this.Cart.Cell = cart.Cell;
            this.Cart.PostalCode = cart.PostalCode;
            this.Cart.Status = cart.Status;
            this.Cart.UserId = Principal.Identity.Name;

            Session.Set("CASHONECart", Utility.ObjectFormatter.GetInstanceBytes(this.Cart));

            return this.Cart;
        }
    }
}
