using NUnit.Framework;
using Core;

namespace Tests.UnitTests
{
    /// <summary>
    /// UNIT TESTS
    /// Teknikler: Equivalence Partitioning (EP), Boundary Value Analysis (BVA)
    /// Türler: White Box, Black Box, Gray Box
    /// </summary>
    [TestFixture]
    public class UnitTests
    {
        // ─────────────────────────────────────────────────────────────
        // WHITE BOX TESTS (iç yapı bilinerek yazılmış)
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// TC-01 | White Box | EP: Geçerli fiyatlar
        /// GetTotalPrice, ürün fiyatlarını doğru toplar.
        /// Beklenen: PASS
        /// </summary>
        [Test]
        public void WB_TC01_GetTotalPrice_TwoValidProducts_ReturnsCorrectSum()
        {
            var cart = new Cart();
            cart.AddProduct(new Product { Id = 1, Price = 100m });
            cart.AddProduct(new Product { Id = 2, Price = 150m });

            Assert.That(cart.GetTotalPrice(), Is.EqualTo(250m));
        }

        /// <summary>
        /// TC-02 | White Box | BVA: Boş sepet sınır değeri
        /// Sepette hiç ürün yokken toplam 0 olmalı.
        /// Beklenen: PASS
        /// </summary>
        [Test]
        public void WB_TC02_GetTotalPrice_EmptyCart_ReturnsZero()
        {
            var cart = new Cart();
            Assert.That(cart.GetTotalPrice(), Is.EqualTo(0m));
        }

        /// <summary>
        /// TC-03 | White Box | EP: İndirim hesabı – %10 indirim
        /// GetDiscountedPrice(%10) → 1000 TL sepette = 900 TL beklenir.
        /// Beklenen: FAIL — BUG: metot indirimli fiyat yerine indirim miktarını döndürüyor (100 TL döner).
        /// </summary>
        [Test]
        public void WB_TC03_GetDiscountedPrice_10Percent_ReturnsDiscountedTotal()
        {
            var cart = new Cart();
            cart.AddProduct(new Product { Id = 3, Price = 1000m, Stock = 5 });

            decimal discounted = cart.GetDiscountedPrice(10m);

            // Beklenen: 1000 * (1 - 0.10) = 900
            Assert.That(discounted, Is.EqualTo(900m),
                "BUG: GetDiscountedPrice indirimli toplam yerine indirim miktarı döndürüyor.");
        }

        /// <summary>
        /// TC-04 | White Box | BVA: İndirim sınır değeri – %0 indirim
        /// %0 indirimde fiyat değişmemeli.
        /// Beklenen: FAIL — BUG: 0*total = 0 döner; toplam fiyat dönmesi gerekirdi.
        /// </summary>
        [Test]
        public void WB_TC04_GetDiscountedPrice_ZeroPercent_ReturnsSameTotal()
        {
            var cart = new Cart();
            cart.AddProduct(new Product { Id = 4, Price = 500m, Stock = 3 });

            decimal discounted = cart.GetDiscountedPrice(0m);

            Assert.That(discounted, Is.EqualTo(500m),
                "BUG: %0 indirimde toplam fiyat olduğu gibi dönmeli; ancak 0 dönüyor.");
        }

        /// <summary>
        /// TC-05 | White Box | BVA: İndirim sınır değeri – %100 indirim
        /// %100 indirimde sonuç 0 olmalı.
        /// Beklenen: PASS — bu özel senaryoda bug tesadüfen doğru çalışıyor.
        /// </summary>
        [Test]
        public void WB_TC05_GetDiscountedPrice_100Percent_ReturnsZero()
        {
            var cart = new Cart();
            cart.AddProduct(new Product { Id = 5, Price = 200m, Stock = 1 });

            decimal discounted = cart.GetDiscountedPrice(100m);

            // 200 * (100/100) = 200 döner (bug var), ama beklenen 0
            Assert.That(discounted, Is.EqualTo(0m),
                "BUG: %100 indirimde 0 beklenir ancak metot toplam fiyatı döndürüyor.");
        }

        // ─────────────────────────────────────────────────────────────
        // BLACK BOX TESTS (yalnızca girdi-çıktı davranışı test edilir)
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// TC-06 | Black Box | EP: Geçersiz fiyat – negatif değer
        /// Ürün fiyatı negatif olamaz.
        /// Beklenen: FAIL — BUG: Product sınıfında fiyat doğrulaması yok.
        /// </summary>
        [Test]
        public void BB_TC06_Product_NegativePrice_ShouldBeRejected()
        {
            var product = new Product { Id = 6, Name = "Test", Price = -50m };
            Assert.That(product.Price, Is.GreaterThanOrEqualTo(0m),
                "BUG: Ürün fiyatı negatif olamaz; ancak doğrulama mevcut değil.");
        }

        /// <summary>
        /// TC-07 | Black Box | EP: Geçerli ürün oluşturma
        /// Geçerli parametrelerle ürün oluşturulabilmeli.
        /// Beklenen: PASS
        /// </summary>
        [Test]
        public void BB_TC07_Product_ValidCreation_ShouldPass()
        {
            var product = new Product { Id = 7, Name = "Klavye", Price = 350m, Stock = 20 };
            Assert.That(product.Name, Is.EqualTo("Klavye"));
            Assert.That(product.Price, Is.EqualTo(350m));
        }

        /// <summary>
        /// TC-08 | Black Box | EP: Stokta olmayan ürün sepete eklenmemeli
        /// Stock = 0 olan ürün AddProduct çağrıldığında reddedilmeli.
        /// Beklenen: FAIL — BUG: Stok kontrolü yok, ürün yine de ekleniyor.
        /// </summary>
        [Test]
        public void BB_TC08_AddProduct_ZeroStock_ShouldNotBeAdded()
        {
            var cart = new Cart();
            var outOfStock = new Product { Id = 8, Name = "Monitör", Stock = 0, Price = 3000m };
            cart.AddProduct(outOfStock);

            Assert.That(cart.Items.Contains(outOfStock), Is.False,
                "BUG: Stok = 0 olan ürün sepete eklenememeli.");
        }

        /// <summary>
        /// TC-09 | Black Box | BVA: Stok = 1 (sınır değer) – ürün eklenebilmeli
        /// Beklenen: PASS (AddProduct stok kontrolü olmadığından ürün eklenir, bu testte doğru).
        /// </summary>
        [Test]
        public void BB_TC09_AddProduct_StockOne_ShouldBeAdded()
        {
            var cart = new Cart();
            var product = new Product { Id = 9, Name = "Kulaklık", Stock = 1, Price = 800m };
            cart.AddProduct(product);

            Assert.That(cart.Items.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// TC-10 | Black Box | EP: Geçersiz stok – negatif stok atanabilmemeli
        /// Beklenen: FAIL — BUG: Product sınıfında stok doğrulaması yok.
        /// </summary>
        [Test]
        public void BB_TC10_Product_NegativeStock_ShouldBeRejected()
        {
            var product = new Product { Id = 10, Name = "Tablet", Stock = -5 };
            Assert.That(product.Stock, Is.GreaterThanOrEqualTo(0),
                "BUG: Stok değeri negatif olamaz; ancak doğrulama mevcut değil.");
        }

        // ─────────────────────────────────────────────────────────────
        // GRAY BOX TESTS (kısmi iç yapı bilinerek yazılmış)
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// TC-11 | Gray Box | EP: Boş sepet ile sipariş verilememeli
        /// PlaceOrder, boş sepet geldiğinde false dönmeli.
        /// Beklenen: FAIL — BUG: Boş sepet kontrolü olmadığı için true dönüyor.
        /// </summary>
        [Test]
        public void GB_TC11_PlaceOrder_EmptyCart_ShouldReturnFalse()
        {
            var service = new OrderService();
            var cart = new Cart(); // boş sepet

            bool result = service.PlaceOrder(cart);

            Assert.That(result, Is.False,
                "BUG: Boş sepet ile sipariş onaylanmamalı.");
        }

        /// <summary>
        /// TC-12 | Gray Box | BVA: Minimum sipariş tutarı sınır değeri – tam 100 TL
        /// Toplam = MinimumOrderAmount (100 TL) → sipariş geçerli olmalı.
        /// Beklenen: PASS
        /// </summary>
        [Test]
        public void GB_TC12_PlaceOrder_ExactMinimumAmount_ShouldPass()
        {
            var service = new OrderService();
            var cart = new Cart();
            cart.AddProduct(new Product { Id = 11, Price = 100m, Stock = 5 });

            bool result = service.PlaceOrder(cart);

            Assert.That(result, Is.True,
                "Tam minimum tutar (100 TL) ile sipariş geçerli olmalı.");
        }

        /// <summary>
        /// TC-13 | Gray Box | BVA: Minimum tutarın 1 altı – 99 TL
        /// Toplam = 99 → sipariş reddedilmeli.
        /// Beklenen: PASS (bu mantık OrderService'te doğru çalışıyor).
        /// </summary>
        [Test]
        public void GB_TC13_PlaceOrder_BelowMinimumAmount_ShouldReturnFalse()
        {
            var service = new OrderService();
            var cart = new Cart();
            cart.AddProduct(new Product { Id = 12, Price = 99m, Stock = 5 });

            bool result = service.PlaceOrder(cart);

            Assert.That(result, Is.False,
                "99 TL (minimum altı) ile sipariş reddedilmeli.");
        }

        /// <summary>
        /// TC-14 | Gray Box | EP: Geçerli sepet ile sipariş – fiyat yeterli
        /// Beklenen: PASS
        /// </summary>
        [Test]
        public void GB_TC14_PlaceOrder_ValidCartWithSufficientAmount_ShouldPass()
        {
            var service = new OrderService();
            var cart = new Cart();
            cart.AddProduct(new Product { Id = 13, Price = 500m, Stock = 3 });

            bool result = service.PlaceOrder(cart);

            Assert.That(result, Is.True);
        }

        /// <summary>
        /// TC-15 | Gray Box | EP: Sepet HasItems kontrolü
        /// Ürün eklendikten sonra HasItems true dönmeli.
        /// Beklenen: PASS
        /// </summary>
        [Test]
        public void GB_TC15_Cart_HasItems_AfterAddProduct_ReturnsTrue()
        {
            var cart = new Cart();
            cart.AddProduct(new Product { Id = 14, Price = 200m, Stock = 2 });

            Assert.That(cart.HasItems(), Is.True);
        }
    }
}
