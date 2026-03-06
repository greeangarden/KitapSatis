using KitapSatis.Api.Models;

namespace KitapSatis.Api.Data
{
    public static class DbSeeder
    {
        public static void SeedAdmin(AppDbContext context)
        {
            if (!context.Users.Any(u => u.Username == "admin"))
            {
                context.Users.Add(new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
                    Role = "Admin",
                    IsActive = true
                });

            }
            if (!context.Users.Any(u => u.Username == "user1"))
            {
                context.Users.Add(new User
                {
                    Username = "user1",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("1234"),
                    Role = "User",
                    IsActive = true
                });
            }
            context.SaveChanges();

                // Seed Real DEU Engineering Books if empty
                if (!context.Books.Any())
                {
                    var rawText = @"
ÇEVRE MÜHENDİSLİĞİ BÖLÜMÜ
98
Genel kimya için Lab.Deneyleri Kitap
55,00 TL
153
Çevre Mühendisliğinde Fiziksel- Kimyasal Temel Süreçler
50,00 TL
234
Çevre Mühendisliğinde Biyoprosesler
110,00 TL
240
Atıksu Arıtma Sistemlerinin Tasarım Esasları C-I
80,00 TL
241
Atıksu Arıtma Sistemlerinin Tasarım Esasları C-II
80,00 TL
255
Arıtma Çamurlarının İşlenmesi
60,00 TL
265
Katı Atık Toplama Taş. Bertaraf sis.
55,00 TL
266
Atıksu Stabili Havuz ve Mekanik Havuz Lagün Taşıma-İnş -İşlt Esas
55,00 TL
280
Anaerobik Arıtma
50,00 TL
305
Atmosfer Kimyası
55,00 TL
316
Çevre Mühendisliğinde Mikrobiyolojik Uygulamalar
50,00 TL
320
Aktif Çamur Sürecinin Tasarım Uygulamaları Cilt I
60,00 TL
321
Aktif Çamur Sürecinin Tasarım Uygulamaları Cilt II
60,00 TL
325
Teknik İngilizce
55,00 TL
341
Endüstriyel atık suların yönetilmesi
55,00 TL
ELEKTRİK ELEKTRONİK MÜHENDİSLİĞİ BÖLÜMÜ
293
Elektrik  Makinalarının Temel İlkeleri
55,00 TL
ENDÜSTRİ MÜHENDİSLİĞİ BÖLÜMÜ
52
Üretim Fonksiyonları
50,00 TL
200
İşletme Yöntemi ve Organizasyonu
50,00 TL
201
Tesis Planlama
YOK
245
Üretim Planlama
60,00 TL
İNŞAAT MÜHENDİSLİĞİ BÖLÜMÜ
212
Sukuvveti tes.Sayısal Örnekle
50,00 TL
218
Su Getirme ve Kanalizasyon Tes.Sayısal Örnekler
YOK
294
Yapı ve Deprem Mühendisliğinde Matris Yöntemler
190,00 TL
307
Malzeme Bilgisi (İnş.Müh.İçin.)
100,00 TL
324
Kıyı Mühendisliğinde  Sayısal Uygulamalar
100,00 TL
327
Su Kaynaklarını Geliştirilmesi
55,00 TL
328
Su Kuvveti Su yapıları Cilt IX
55,00 TL
332
Fiziksel Modeller
50,00 TL
333
Fransızca
50,00 TL
334
Beton
260,00 TL
335
Sayısal uygulamalı Akışkanlar Mekaniği
YOK
338
Diferansiyel denklemler
190,00 TL
340
Mühendis ve Mimar Sinan
100,00 TL
342
Geçmişten Günümüze Dünya Su Yapıları
160,00 TL
343
Taşıyıcı Sistemlerin Dinamik Analizi
420,00 TL
345
Bilisayar Programlama (matlab Uygulamalı)
320,00 TL
JEOFİZİK MÜHENDİSLİĞİ BÖLÜMÜ
339
Jeofizik Sinyal Analizi
100,00 TL
347
Potansiyel Teori Jeofizik Uygulamaları
270,00 TL
JEOLOJİ MÜHENDİSLİĞİ BÖLÜMÜ
63
Mineroloji-Genel Mineroloji (Cilt I )
55,00 TL
81
Kömür
110,00 TL
253
Maden Jeolojisi Uygulama kılavuzu
50,00 TL
285
Volkanik lastik Kayaclar
50,00 TL
299
Kayaçların Radyometrik Yaş Tayininin Ana Yöntemleri
50,00 TL
302
Cevher Mikroskopisi ve Petrografisi
YOK
306
Jeotermalde Yerbilimsel  Uygulamalar
100,00 TL
323
Kil Mineralleri (Ders Notları)
50,00 TL
344
Mühendislik Jeolojisi
500,00 TL
MADEN MÜHENDİSLİĞİ BÖLÜMÜ
33
Kömür Teknolojisi
200,00 TL
145
Tünel Ve Kuyu Açma
155,00 TL
177
Kaya Mekaniği
120,00 TL
223
Maden işletme Ekonomisi
100,00 TL
256
Açık İşletme Tekniği
190,00 TL
258
Madenlerde Su atımı ve Pompalar
50,00 TL
296
Madenlerde Nakliyat
55,00 TL
309
Yer altı Maden Makinaları Ve Mekanizasyonu
70,00 TL
312
Maden Mühendisleri için Ölçme Tekniği
190,00 TL
319
Madenciliğe Giriş
YOK
336
Yer altı havalandırması
110,00 TL
337
Yer altı madencilik Yöntemleri
80,00 TL
MAKİNA MÜHENDİSLİĞİ BÖLÜMÜ
2
Teknik Resim  I
113
Kaldırma Makinaları
50,00 TL
244
Mukavemet I
YOK
313
Sistem Dinamiği ve Otomatik Kontrol
110,00 TL
1007
Staj Defteri
50,00 TL
TEKSTİL MÜHENDİSLİĞİ BÖLÜMÜ
315
Örmecilik Esasları
50,00 TL
326
Tekstil Mekaniğinin Temelleri
110,00 TL
346
Giysi Kalıpçılığı-1
150,00 TL
TEMEL BİLİMLER
89
Matematik I
103
Matematik-I (Lineer Cebir)
50,00 TL
151
Analitik Geometri
55,00 TL
268
Sayısal Çözümleme
YOK
304
Sayısal Çözümleme ve Örnekler
170,00 TL
331
Matematık ııı
YOK
DIŞ YAYINLAR
İş ve zaman etüdü   (9789754412079)
60,00 TL
DIG020
Genel kimya için Lab.Deneyleri
50,00 TL
DIG021
Fizik Lab II
35,00 TL
DIG022
Fizik lab.I
YOK
DIG023
Tekstil Kimya
35,00 TL";

                    // Use context.Database.ExecuteSqlRaw directly if needed but since EF tracks, better use entities
                    // In this context, we will clear current categories to re-parse them from rawText cleanly
                    context.Categories.RemoveRange(context.Categories);
                    context.SaveChanges();

                    var lines = rawText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(l => l.Trim()).ToList();
                    
                    Category currentCategory = null;
                    string pendingCode = null;
                    string pendingName = null;

                    foreach (var line in lines)
                    {
                        if (line.EndsWith("BÖLÜMÜ") || line == "TEMEL BİLİMLER" || line == "DIŞ YAYINLAR")
                        {
                            currentCategory = new Category { Name = line };
                            context.Categories.Add(currentCategory);
                            context.SaveChanges(); // get ID immediately
                            pendingCode = null;
                            pendingName = null;
                            continue;
                        }

                        if (currentCategory == null) continue;

                        if (System.Text.RegularExpressions.Regex.IsMatch(line, @"^\d+$") || line.StartsWith("DIG") || line.StartsWith("İş ve zaman")) 
                        {
                            if (line.StartsWith("İş ve zaman")) {
                                pendingCode = "N/A";
                                pendingName = line;
                            } else {
                                pendingCode = line;
                            }
                        }
                        else if (line.Contains("TL") || line == "YOK" || line == "113")
                        {
                            if (line == "113") {
                                pendingName = "Teknik Resim I";
                                pendingCode = "2";
                                continue;
                            }

                            decimal price = 0;
                            int stock = 0;
                            bool isActive = true;

                            if (line == "YOK")
                            {
                                stock = 0;
                                isActive = false;
                            }
                            else
                            {
                                var priceStr = line.Replace(" TL", "").Replace(",", ".");
                                if (decimal.TryParse(priceStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal p))
                                {
                                    price = p;
                                    stock = new Random().Next(10, 50); // Given random stock for demo
                                }
                            }

                            if (pendingName != null)
                            {
                                context.Books.Add(new Book
                                {
                                    Name = pendingName,
                                    Price = price,
                                    StockQuantity = stock,
                                    MinStockLevel = 5,
                                    CategoryId = currentCategory.Id,
                                    IsActive = isActive,
                                    Author = "DEÜ " + currentCategory.Name.Replace(" BÖLÜMÜ", ""), 
                                    Description = pendingCode != "N/A" ? $"Yayın No: {pendingCode}" : "Açıklama bulunmuyor.",
                                    ImageUrl = "" // Optional field fallback 
                                });
                                pendingCode = null;
                                pendingName = null;
                            }
                        }
                        else 
                        {
                            pendingName = line;
                            if (pendingCode == null) pendingCode = "N/A"; 
                        }
                    }
                    context.SaveChanges();
                }
        }

    }
}
