using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class Cart
    {
        public List<Product> Items { get; set; } = new List<Product>();

        public void AddProduct(Product product)
        {
            // BUG (Fault 1): Stok kontrolü eksik.
            // Stock == 0 olan ürünler sepete ekleniyor.
            // Doğrusu: if (product.Stock <= 0) throw veya return olmalı.
            Items.Add(product);
        }

        public decimal GetTotalPrice()
        {
            return Items.Sum(p => p.Price);
        }

        public decimal GetDiscountedPrice(decimal discountPercent)
        {
            // BUG (Fault 2): İndirim hesaplaması yanlış.
            // Toplam fiyatın discountPercent kadarı ÇIKARILMASI gerekirken
            // burada discountPercent/100 ile çarpılıp aynen döndürülüyor,
            // yani sadece indirim miktarı döner; indirimli fiyat dönmez.
            // Doğrusu: return total * (1 - discountPercent / 100);
            decimal total = GetTotalPrice();
            return total * (discountPercent / 100);  // <-- BUG
        }

        public bool HasItems()
        {
            return Items.Count > 0;
        }
    }
}
