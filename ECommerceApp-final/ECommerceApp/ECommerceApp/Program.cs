using Core;

// Demo: E-Ticaret Sistemi
var product1 = new Product { Id = 1, Name = "Laptop", Price = 15000, Stock = 5 };
var product2 = new Product { Id = 2, Name = "Mouse", Price = 250, Stock = 10 };

var cart = new Cart();
cart.AddProduct(product1);
cart.AddProduct(product2);

Console.WriteLine($"Sepet Toplamı: {cart.GetTotalPrice()} TL");
Console.WriteLine($"%10 İndirimli Fiyat: {cart.GetDiscountedPrice(10)} TL");

var orderService = new OrderService();
bool result = orderService.PlaceOrder(cart);
Console.WriteLine($"Sipariş Sonucu: {(result ? "Başarılı" : "Başarısız")}");
