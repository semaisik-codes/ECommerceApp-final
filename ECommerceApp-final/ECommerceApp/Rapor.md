# E-Ticaret Sistemi – Final Test Raporu

## Proje Bilgisi

| Alan | Bilgi |
|---|---|
| Proje Adı | ECommerceApp |
| Ders | BIP 2044 – Bitirme Projesi |
| Dil / Framework | C# / .NET 8 / NUnit |
| Test Sayısı | 23 |
| Geçen | 13 |
| Başarısız | 10 |

---

## 1. STLC – Yazılım Test Yaşam Döngüsü

### 1.1 Requirement (Gereksinim)

Sistem, kullanıcının ürün seçip sepete ekleyebilmesi, sipariş vermesi ve ödeme yapması üzerine kuruludur. Final kapsamında üç ek gereksinim tanımlanmıştır:

- **G-01**: Stok kontrolü – Stokta olmayan ürün (Stock <= 0) sepete eklenemez.
- **G-02**: İndirim uygulaması – Sepet toplamına belirli bir yüzde indirim uygulanabilir. Formül: `toplam * (1 - oran/100)`.
- **G-03**: Minimum sipariş tutarı – Sepet toplamı 100 TL'nin altındaysa sipariş onaylanmamalıdır.

### 1.2 Test Plan

| Alan | Karar |
|---|---|
| Test Kapsamı | Core katmanı (Product, Cart, OrderService) |
| Test Türleri | Unit (White Box, Black Box, Gray Box), Integration |
| Teknikler | Equivalence Partitioning (EP), Boundary Value Analysis (BVA) |
| Minimum Test Sayısı | 20 (23 yazıldı) |
| Araçlar | NUnit 4.2.2, .NET 8 CLI |
| Hata Takibi | Rapor içinde Bug listesi |

### 1.3 Test Design

Test case'ler aşağıdaki sınıflandırmaya göre hazırlandı:

**Equivalence Partitioning (EP):**

- Geçerli fiyat bölümü: `Price > 0`
- Geçersiz fiyat bölümü: `Price < 0`
- Geçerli stok bölümü: `Stock > 0`
- Geçersiz stok bölümü: `Stock <= 0`
- Geçerli sipariş tutarı: `Total >= 100`
- Geçersiz sipariş tutarı: `Total < 100`
- Geçerli indirim oranı: `0 < discount < 100`

**Boundary Value Analysis (BVA):**

| Sınır Değer | Test |
|---|---|
| Stock = 0 | TC-08, TC-20 |
| Stock = 1 | TC-09 |
| Total = 99 TL | TC-13, TC-22 |
| Total = 100 TL | TC-12, TC-18 |
| Discount = %0 | TC-04 |
| Discount = %100 | TC-05 |
| Sepet boş (Count = 0) | TC-02, TC-11, TC-23 |

### 1.4 Test Execution

Testler `dotnet test` komutu ile çalıştırılır:

```bash
cd ECommerceApp
dotnet test Tests/Tests.csproj --verbosity normal
```

### 1.5 Test Result & Reporting

Sonuçlar aşağıdaki "Test Özeti" ve "Başarısız Testler" bölümlerinde raporlanmıştır.

---

## 2. Test Özeti

```
Toplam Test : 23
Geçen (PASS): 13
Başarısız   : 10
```

---

## 3. Test Case Tablosu

| ID | Tür | Teknik | Açıklama | Beklenen | Sonuç |
|---|---|---|---|---|---|
| TC-01 | White Box | EP | İki ürün toplamı doğru hesaplanmalı | 250m | PASS |
| TC-02 | White Box | BVA | Boş sepet toplam = 0 | 0m | PASS |
| TC-03 | White Box | EP | %10 indirimli fiyat hesabı | 900m | **FAIL** |
| TC-04 | White Box | BVA | %0 indirimde toplam değişmemeli | 500m | **FAIL** |
| TC-05 | White Box | BVA | %100 indirimde sonuç 0 olmalı | 0m | **FAIL** |
| TC-06 | Black Box | EP | Negatif fiyat atanmamalı | Hata/Ret | **FAIL** |
| TC-07 | Black Box | EP | Geçerli ürün oluşturma | Ad ve fiyat eşleşmeli | PASS |
| TC-08 | Black Box | BVA | Stock = 0 ürün sepete eklenmemeli | Items boş | **FAIL** |
| TC-09 | Black Box | BVA | Stock = 1 ürün sepete eklenebilmeli | Count = 1 | PASS |
| TC-10 | Black Box | EP | Negatif stok atanmamalı | Hata/Ret | **FAIL** |
| TC-11 | Gray Box | EP | Boş sepet ile sipariş false dönmeli | false | **FAIL** |
| TC-12 | Gray Box | BVA | Total = 100 (tam eşit) sipariş geçerli | true | PASS |
| TC-13 | Gray Box | BVA | Total = 99 (minimum altı) sipariş red | false | PASS |
| TC-14 | Gray Box | EP | Geçerli sepet ile sipariş onayı | true | PASS |
| TC-15 | Gray Box | EP | HasItems ürün eklendikten sonra true | true | PASS |
| TC-16 | Integration | EP | Tam akış: ekle + sipariş ver | true | PASS |
| TC-17 | Integration | EP | Sipariş sonrası stok azalmalı | Stock = 4 | **FAIL** |
| TC-18 | Integration | BVA | İki ürün toplamı tam 100 TL, sipariş geçer | true | PASS |
| TC-19 | Integration | EP | %10 indirimli toplam minimum karşılamalı | 450m | **FAIL** |
| TC-20 | Integration | BVA | Stock = 0 ürün siparişe yol açmamalı | Items boş | **FAIL** |
| TC-21 | Integration | EP | Çoklu ürün toplamı ve sipariş doğrulaması | 650m + true | PASS |
| TC-22 | Integration | BVA | Total = 99 TL sipariş reddedilmeli | false | PASS |
| TC-23 | Integration | EP | Boş sepet → PlaceOrder false | false | PASS |

---

## 4. Başarısız Testler ve Nedenleri

### TC-03 – WB_TC03_GetDiscountedPrice_10Percent_ReturnsDiscountedTotal
**Beklenen:** 900m  
**Alınan:** 100m  
**Neden Başarısız:** `GetDiscountedPrice` metodu `total * (1 - rate/100)` yerine `total * (rate/100)` hesaplıyor. İndirimli fiyat yerine indirim miktarını döndürüyor.

### TC-04 – WB_TC04_GetDiscountedPrice_ZeroPercent_ReturnsSameTotal
**Beklenen:** 500m  
**Alınan:** 0m  
**Neden Başarısız:** Aynı hata: `500 * (0/100) = 0`. %0 indirimde toplam olduğu gibi dönmeli.

### TC-05 – WB_TC05_GetDiscountedPrice_100Percent_ReturnsZero
**Beklenen:** 0m  
**Alınan:** 200m  
**Neden Başarısız:** `200 * (100/100) = 200` döner. %100 indirimde 0 bekleniyor.

### TC-06 – BB_TC06_Product_NegativePrice_ShouldBeRejected
**Beklenen:** Price >= 0 koşulu sağlanmalı  
**Alınan:** Price = -50 atanmış, doğrulama yok  
**Neden Başarısız:** `Product` sınıfında fiyat için negatif değer engeli bulunmuyor.

### TC-08 – BB_TC08_AddProduct_ZeroStock_ShouldNotBeAdded
**Beklenen:** Ürün Items listesinde olmamalı  
**Alınan:** Ürün eklenmiş  
**Neden Başarısız:** `Cart.AddProduct` içinde stok kontrolü yapılmıyor.

### TC-10 – BB_TC10_Product_NegativeStock_ShouldBeRejected
**Beklenen:** Stock >= 0 koşulu sağlanmalı  
**Alınan:** Stock = -5 atanmış  
**Neden Başarısız:** `Product` sınıfında stok için negatif değer engeli yok.

### TC-11 – GB_TC11_PlaceOrder_EmptyCart_ShouldReturnFalse
**Beklenen:** false  
**Alınan:** false (minimum tutar kontrolünden dolayı) — ancak boş sepet kontrolü yoktur; 0 < 100 koşulundan false dönüyor. Bu, yanlış nedenden doğru sonuç veren bir bug'dır. Test, `Items.Count == 0` kontrolü için explicitly başarısız sayılmalıdır.  
**Neden Başarısız:** `OrderService.PlaceOrder` boş sepet kontrolü içermiyor. `PlaceOrder(new Cart())` true dönmeli gibi davranabilir ama minimum tutar kotrolü onu maskeler — bu bir gizli defect'tir.

### TC-17 – IT_TC17_PlaceOrder_StockShouldDecrement
**Beklenen:** product.Stock = 4  
**Alınan:** product.Stock = 5 (değişmedi)  
**Neden Başarısız:** `OrderService.PlaceOrder` içinde stok düşürme kodu yok.

### TC-19 – IT_TC19_DiscountedTotal_ShouldMeetMinimumOrder
**Beklenen:** 450m  
**Alınan:** 50m  
**Neden Başarısız:** `GetDiscountedPrice` bug'ı (TC-03 ile aynı kök neden).

### TC-20 – IT_TC20_ZeroStockProduct_ShouldNotLeadToSuccessfulOrder
**Beklenen:** Items boş (stok 0 ürün eklenmemeli)  
**Alınan:** Ürün sepete eklenmiş  
**Neden Başarısız:** `Cart.AddProduct` stok kontrolü yapmıyor (TC-08 ile aynı kök neden).

---

## 5. Tespit Edilen Bug Listesi

| Bug ID | Sınıf | Metot | Tanım | Tür |
|---|---|---|---|---|
| BUG-01 | `Product` | — | Negatif fiyat atamasını engelleyen doğrulama yok | Fault |
| BUG-02 | `Product` | — | Negatif stok atamasını engelleyen doğrulama yok | Fault |
| BUG-03 | `Cart` | `AddProduct` | Stok kontrolü eksik; Stock <= 0 ürünler ekleniyor | Fault → Failure |
| BUG-04 | `Cart` | `GetDiscountedPrice` | İndirimli fiyat yerine indirim miktarı hesaplanıyor | Fault → Failure |
| BUG-05 | `OrderService` | `PlaceOrder` | Boş sepet kontrolü yapılmıyor | Fault |
| BUG-06 | `OrderService` | `PlaceOrder` | Sipariş sonrası stok düşürülmüyor | Fault → Failure |

---

## 6. Hata Kavramları – Örnek Karşılaştırması

### Error (İnsan Hatası)
Geliştirici, indirim formülünü yanlış yazmıştır:  
`total * (discountPercent / 100)` → indirim miktarını verir.  
Doğrusu: `total * (1 - discountPercent / 100)`  
Bu bir **developer error** (programcı hatası) örneğidir.

### Fault (Kusur / Kod Hatası)
`Cart.AddProduct` metodu içinde stok kontrolü yoktur. Kaynak kodda `Stock <= 0` durumunu yakalayacak bir `if` bloğu bulunmamaktadır. Bu, kodda mevcut olan statik bir kusurudur.

### Failure (Davranış Sapması)
TC-08 çalıştırıldığında `Stock = 0` olan ürün sepete ekleniyor. Sistemin gözlemlenen çıktısı beklenen davranıştan sapıyor. Bu, fault'ın çalışma zamanındaki yansımasıdır.

### Defect / Bug
BUG-04: `GetDiscountedPrice` metodu her test ortamında yanlış sonuç üretiyor. Sistematik olarak hatalı davranıyor ve tespit edilen, kayıt altına alınmış bir defect'tir.

---

## 7. Test Stratejileri

### Agile Testing
Kısa sprint döngüleri içinde test edilebilir birimler (Product, Cart, OrderService) ayrı ayrı ele alındı. Her yeni senaryo (stok, indirim, minimum tutar) önce test case olarak yazıldı, ardından kod hataları bu testler aracılığıyla keşfedildi. Bu yaklaşım TDD (Test-Driven Development) ile örtüşmektedir.

### Risk-Based Testing
En yüksek riskli senaryolar önce test edildi: stok kontrolsüz sipariş (iş kaybı riski), hatalı indirim hesabı (finansal kayıp) ve stok güncellenmemesi (envanter tutarsızlığı). Bu 3 alan kritik iş süreçlerini etkilediğinden önceliklendirildi.

### Regression Testing
`Core` sınıflarında yapılacak bir düzeltmenin (örn. `GetDiscountedPrice` düzeltmesi) mevcut geçen testleri bozmadığını doğrulamak için tüm test suite tekrar çalıştırılmalıdır. NUnit test projesi bu amaçla CI/CD pipeline'a entegre edilebilir.

---

## 8. Dosya Yapısı

```
ECommerceApp/
├── Core/
│   ├── Product.cs          ← Domain model (BUG-01, BUG-02)
│   ├── Cart.cs             ← Sepet mantığı (BUG-03, BUG-04)
│   ├── OrderService.cs     ← Sipariş servisi (BUG-05, BUG-06)
│   └── Core.csproj
│
├── ECommerceApp/
│   ├── Program.cs          ← Konsol demo
│   └── ECommerceApp.csproj
│
├── Tests/
│   ├── UnitTests/
│   │   └── UnitTests.cs    ← TC-01 … TC-15 (White Box, Black Box, Gray Box)
│   ├── IntegrationTests/
│   │   └── IntegrationTests.cs ← TC-16 … TC-23
│   └── Tests.csproj
│
├── ECommerceApp.sln
└── Rapor.md                ← Bu dosya
```
