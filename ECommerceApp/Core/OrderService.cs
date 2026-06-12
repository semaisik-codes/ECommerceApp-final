using System;
using System.Linq;

namespace Core
{
    public class OrderService
    {
        // Minimum sipariş tutarı (iş kuralı)
        public const decimal MinimumOrderAmount = 100m;

        public bool PlaceOrder(Cart cart)
        {
            // BUG (Fault 3): Boş sepet kontrolü eksik.
            // Sepette hiç ürün olmasa bile sipariş onaylanıyor.
            // Doğrusu: if (!cart.HasItems()) return false;

            // BUG (Fault 4): Minimum sipariş tutarı kontrolü yanlış yazılmış.
            // <= yerine < kullanılmış; tam eşit olduğunda (100) sipariş geçiyor
            // fakat ödev senaryosuna göre 100'ün altında olmamalı, 100 geçerli.
            // Aşağıdaki mantık aslında doğrudur (< 100 → false), ancak
            // stok düşürmeme hatası ile birlikte entegrasyon başarısız olur.
            decimal total = cart.GetTotalPrice();
            if (total < MinimumOrderAmount)
            {
                return false;
            }

            // BUG (Fault 5): Sipariş sonrası stok düşürülmüyor.
            // Her ürünün Stock değeri 1 azaltılmalıyken hiç güncellenmez.
            // Doğrusu: foreach (var item in cart.Items) { item.Stock--; }

            return true;
        }
    }
}
