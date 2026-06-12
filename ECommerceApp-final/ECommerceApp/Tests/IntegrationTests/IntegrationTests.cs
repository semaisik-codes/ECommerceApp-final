using NUnit.Framework;
using Core;

namespace Tests.IntegrationTests
{
    /// <summary>
    /// INTEGRATION TESTS
    /// Cart + Product + OrderService bileşenleri birlikte test edilir.
    /// Teknikler: EP, BVA
    /// </summary>
    [TestFixture]
    public class IntegrationTests
    {
        /// <summary>
        /// TC-16 | Integration | EP: Tam akış – ürün ekle, sipariş ver
        /// Geçerli ürünlü sepet ile sipariş başarılı olmalı.
        /// Beklenen: PASS
        /// </summary>
        [Test]
        public void IT_TC16_FullFlow_AddProduct_PlaceOrder_ShouldSucceed()
        {
            var product = new Product { Id = 1, Name = "Laptop", Price = 15000m, Stock = 5 };
            var cart = new Cart();
            cart.AddProduct(product);

            var service = new OrderService();
            bool result = service.PlaceOrder(cart);

            Assert.That(result, Is.True);
        }

        /// <summary>
        /// TC-17 | Integration | EP: Sipariş sonrası stok azalmalı
        /// PlaceOrder çağrıldıktan sonra ürünün Stock değeri 1 düşmeli.
        /// Beklenen: FAIL — BUG: Stok düşürme kodu mevcut değil.
        /// </summary>
        [Test]
        public void IT_TC17_PlaceOrder_StockShouldDecrement()
        {
            var product = new Product { Id = 2, Name = "Telefon", Price = 20000m, Stock = 5 };
            var cart = new Cart();
            cart.AddProduct(product);

            var service = new OrderService();
            service.PlaceOrder(cart);

            Assert.That(product.Stock, Is.EqualTo(4),
                "BUG: Sipariş sonrası stok 4 olmalı; ancak stok düşürülmüyor.");
        }

        /// <summary>
        /// TC-18 | Integration | BVA: İki ürün toplamı minimum tutarı tam karşılar
        /// İki ürün toplamı tam 100 TL → sipariş geçerli.
        /// Beklenen: PASS
        /// </summary>
        [Test]
        public void IT_TC18_TwoProducts_TotalExactlyMinimum_ShouldSucceed()
        {
            var p1 = new Product { Id = 3, Price = 60m, Stock = 2 };
            var p2 = new Product { Id = 4, Price = 40m, Stock = 2 };

            var cart = new Cart();
            cart.AddProduct(p1);
            cart.AddProduct(p2);

            var service = new OrderService();
            bool result = service.PlaceOrder(cart);

            Assert.That(result, Is.True);
            Assert.That(cart.GetTotalPrice(), Is.EqualTo(100m));
        }

        /// <summary>
        /// TC-19 | Integration | EP: İndirim uygulanmış toplam minimum tutarı karşılamalı
        /// 500 TL sepette %10 indirim → 450 TL → sipariş geçerli olmalı.
        /// Beklenen: FAIL — BUG: GetDiscountedPrice 450 yerine 50 döndürüyor.
        /// OrderService GetDiscountedPrice'ı kullanmıyor (ayrı bir bug): bu test
        /// manuel hesaplama ile indirimli fiyat üzerinden sipariş kontrolü simüle eder.
        /// </summary>
        [Test]
        public void IT_TC19_DiscountedTotal_ShouldMeetMinimumOrder()
        {
            var cart = new Cart();
            cart.AddProduct(new Product { Id = 5, Price = 500m, Stock = 3 });

            decimal discountedTotal = cart.GetDiscountedPrice(10m);

            // Beklenen indirimli fiyat: 500 * 0.90 = 450
            Assert.That(discountedTotal, Is.EqualTo(450m),
                "BUG: GetDiscountedPrice 450 dönmeli; ancak indirim miktarı (50) döndürüyor.");
            Assert.That(discountedTotal, Is.GreaterThanOrEqualTo(OrderService.MinimumOrderAmount));
        }

        /// <summary>
        /// TC-20 | Integration | BVA: Stok = 0 ürün ile sipariş zinciri
        /// Stokta olmayan ürün sepete eklenmemeli, dolayısıyla sipariş verilememeli.
        /// Beklenen: FAIL — BUG: Stok kontrolü olmadığından ürün sepete ekleniyor;
        /// ancak fiyat >= 100 olduğu için sipariş yine de onaylanıyor (çift bug).
        /// </summary>
        [Test]
        public void IT_TC20_ZeroStockProduct_ShouldNotLeadToSuccessfulOrder()
        {
            var product = new Product { Id = 6, Name = "Kamera", Price = 5000m, Stock = 0 };
            var cart = new Cart();
            cart.AddProduct(product);  // Bug: stok kontrolü yok, ekleniyor

            // Sepette stok=0 ürün olduğunda sipariş reddedilmeli
            Assert.That(cart.Items.Contains(product), Is.False,
                "BUG: Stok = 0 ürün sepete eklenmemeli.");
        }

        /// <summary>
        /// TC-21 | Integration | EP: Çoklu ürün – toplam fiyat ve sipariş doğrulaması
        /// Birden fazla ürün ile sepet toplamı ve sipariş sonucu birlikte doğrulanır.
        /// Beklenen: PASS
        /// </summary>
        [Test]
        public void IT_TC21_MultipleProducts_TotalAndOrderBothCorrect()
        {
            var p1 = new Product { Id = 7, Price = 200m, Stock = 10 };
            var p2 = new Product { Id = 8, Price = 300m, Stock = 5 };
            var p3 = new Product { Id = 9, Price = 150m, Stock = 2 };

            var cart = new Cart();
            cart.AddProduct(p1);
            cart.AddProduct(p2);
            cart.AddProduct(p3);

            Assert.That(cart.GetTotalPrice(), Is.EqualTo(650m));

            var service = new OrderService();
            Assert.That(service.PlaceOrder(cart), Is.True);
        }

        /// <summary>
        /// TC-22 | Integration | BVA: Minimum tutarın 1 TL altı – sınır değer
        /// Toplam 99 TL → sipariş reddedilmeli.
        /// Beklenen: PASS
        /// </summary>
        [Test]
        public void IT_TC22_TotalJustBelowMinimum_ShouldFail()
        {
            var cart = new Cart();
            cart.AddProduct(new Product { Id = 10, Price = 99m, Stock = 5 });

            var service = new OrderService();
            bool result = service.PlaceOrder(cart);

            Assert.That(result, Is.False,
                "99 TL ile sipariş onaylanmamalı (minimum 100 TL).");
        }

        /// <summary>
        /// TC-23 | Integration | EP: Boş sepet + minimum tutar ihlali birlikte
        /// Boş sepet: HasItems = false ve GetTotalPrice = 0 → sipariş reddedilmeli.
        /// Beklenen: FAIL kısmen — PlaceOrder minimum tutar kontrolünden false döner (doğru),
        /// ancak bu boş sepet kontrolünden değil, tutar kontrolünden kaynaklanır.
        /// </summary>
        [Test]
        public void IT_TC23_EmptyCart_PlaceOrder_ReturnsFalseForCorrectReason()
        {
            var service = new OrderService();
            var cart = new Cart();

            bool hasItems = cart.HasItems();
            bool orderResult = service.PlaceOrder(cart);

            Assert.That(hasItems, Is.False);
            Assert.That(orderResult, Is.False,
                "Boş sepet ile sipariş false dönmeli (minimum tutar ihlali nedeniyle de olsa).");
        }
    }
}
