using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WindnTrees.CRUDS.Repository.Core;
using WindnTrees.ICRUDS.Standard;

namespace Application.Models.CashoneCart.Repository
{
    public abstract class CartRepositoy : CRUDLMRepository<CartItem>
    {
        #region Car_Instantiation
        protected Cart m_Cart = null;
        protected virtual void SaveCart()
        {

        }
        protected virtual Cart Cart { get; set; } 
        #endregion

        public override CartItem Create(CartItem contentObject)
        {
            var cartItem = Cart.Items.Where(l => l.ItemId == contentObject.ItemId).SingleOrDefault();
            if (cartItem == null)
            {
                if (contentObject.Quantity > 0 && contentObject.Quantity <= contentObject.AvailableQuantity)
                {
                    Cart.Items.Add(contentObject);
                    SaveCart();

                    return contentObject;
                }
                else
                {
                    throw new Exception("Add to cart product failed, invalid quantity specified.");
                }
            }
            throw new Exception("Add to cart product failed, cart already have a product with same id.");
        }

        public override CartItem Read(object id)
        {
            Guid guid = Guid.Parse((string)id);
            var cartItem = Cart.Items.Where(l => l.ItemId == guid).SingleOrDefault();
            return cartItem;
        }

        public override CartItem Update(CartItem contentObject)
        {
            if (contentObject.Quantity > 0 && contentObject.Quantity <= contentObject.AvailableQuantity)
            {
                var cartItem = Cart.Items.Where(l => l.ItemId == contentObject.ItemId).SingleOrDefault();

                //do not use Cart property after first time retrieval of m_Cart instance. 
                //Cart property makes sure that object is deserialized from session and ready for use.

                m_Cart.Items.Remove(cartItem);

                //if quantity is 0 then consider product removal from cart.
                if (contentObject.Quantity > 0)
                {
                    m_Cart.Items.Add(contentObject);
                }
                SaveCart();

                return contentObject;
            }
            else
            {
                throw new Exception("Update cart product failed, invalid quantity specified.");
            }
        }

        public override CartItem Delete(CartItem contentObject)
        {
            var cartItem = Cart.Items.Where(l => l.ItemId == contentObject.ItemId).SingleOrDefault();

            //do not use Cart property after first time retrieval of m_Cart instance. 
            //Cart property makes sure that object is deserialized from session and ready for use.

            m_Cart.Items.Remove(cartItem);

            SaveCart();

            return contentObject;
        }

        public override List<CartItem> List(SearchInput queryObject)
        {
            return Cart.Items;
        }

        public int ListTotal(SearchInput queryObject)
        {
            return Cart.Items.Count;
        }

        public int GetCartCount()
        {
            int cartItemCount = 0;
            foreach(var cartItem in Cart.Items)
            {
                cartItemCount += cartItem.Quantity;
            }
            return cartItemCount;
        }
    }
}
