using System;

namespace Core
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }

        // BUG (Fault): Fiyat ve stok için negatif değer kontrolü yok.
        // Negatif fiyat veya negatif stok atanabilir; bu bir tasarım hatasıdır.
        // Doğru olması gereken: Price >= 0 ve Stock >= 0 zorunluluğu.
    }
}
