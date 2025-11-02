### 30 June 2025

Başlangıç

- Kalıcı bir InputManager (Girdi Yöneticisi) tekil nesnesi kuruldu; klavye eksenlerini kare başına joystick verisiyle birleştirir ve OnMove olayı üzerinden normalize edilmiş hareket vektörlerini yayınlayarak herhangi bir hareket bileşeninin belirli giriş cihazlarından bağımsız kalmasını sağlar.
    - Yani klavye, gamepad veya ekrandaki sanal bir joystick’in hep beraber kullanabileceği modüler bir sistem kurdum. Bu sayede oyunu çıkarttığımda ekrandaki joystiği kullanabilecekken oyunu bilgisayarda geliştirirken de klavye ile hareketi sağlıyorum.
- Dokunmaya uygun bir VirtualJoystick (Sanal Kumanda) sunuldu; sürükleme hareketini arka plan yarıçapına sıkıştırır, basılı tutulduğu sürece normalize vektörleri giriş merkezine sürekli besler ve mobilde hayalet girişleri önlemek için bırakıldığında kolu temizler.
    - Yani ekranın ortasında beklenen şekilde çalışan normal bir sanal joystick yaptım, InputManager ile birbirine bağımlı değiller, sadece anonsları dinleyerek çalışıyorlar.
- Modüler bir CameraController (Kamera Denetleyicisi) eklendi; oyuncuyu yumuşak damp/lerp geçişleriyle takip eder, ortografik bir ofseti korur ve ScriptableObject ayarlarıyla hız olaylarına abone olarak uyarlanabilir yakınlaşma/uzaklaşma temposu sağlar.

### 3 July 2025

Mobil kontrollerde cilalama

- Sanal joystick’in işaretçi işleme akışı sağlamlaştırıldı; sürükleme ofsetlerini UI uzayında yeniden hesaplar, kolun hareketini sınırlar ve parmak pedin kenarını terk etse bile giriş akışının stabil kalması için normalize vektörler yazar.
- Dokunmatik vektörlerin birikmemesi için giriş merkezinde joystick niyeti her karede sıfırlandı; mobil giriş yoksa yalnızca klavye yedekleri devreye girer, böylece dokunmatik cihazlarda “takılı yürüyüş” senaryoları önlenir.

### 4 July 2025

Hareket ayarı

- Tüm hareket çağrıları varlık durum kontrolleriyle korundu; ölü veya devre dışı aktörler anında durur, böylece yenilgi sonrası navmesh ajanlarının sürüklenmesi önlenirken dünya- ve transform-tabanlı hedefler sunulmaya devam edilir.
- Hareket başlatma merkezileştirildi; istatistik varlıkları (ScriptableObject) çalışma zamanından önce doğrulanır ve bir tanım eksik olduğunda ayrıntılı tanılama sunulur; bu sayede veri odaklı hareket tanımları senkron kalır.
- Çekirdek sistemler sprinti
- Burada devasa bir PR dalgası yaşandı. Bu PR dalgası, her sınıf, sistem, tüm alanları ve işlevleri, aralarındaki etkileşimler vb. ile sistemin büyük resmini oldukça uzun bir planlama ve çizim sürecinden geçtiğim için ortaya çıktı. Olay tetikleyici sistemlere ve daha birçok şeye odaklandım.

### 7 July 2025

- Çekirdek sistemler sprinti
- Proje genelinde GameEvents (Oyun Olayları) otobüsü tanıtıldı; savaş, kaynak, ilerleme ve hız bildirimlerini yükselterek gevşek bağlı sistemlerin (UI, ses, VFX) sert referanslar olmadan tepki vermesini sağlar.
- Başlangıç bakiyelerini yükleyen, satın alımları koşullandıran ve bakiye değişikliklerini yayınlayan ResourceManager (Kaynak Yöneticisi) tekil nesnesi kuruldu; binalar ve düşen nesneler için veri odaklı ekonomi döngüsünü güçlendirir.

### 10 July 2025

- Çekirdek sistemler sprinti
- Entity (Varlık) merkezi eklendi; tek bir CharacterDefinitionSO üzerinden fraksiyon, sağlık, hareket, saldırı, hedefleme ve etkileşim bileşenlerini bir araya getirerek herhangi bir prefab için güvenilir başlatma sağlar.
- Saha karar verme süreci ayrıştırıldı: AutoInteractor (Otomatik Etkileştirici) taktik hedefleri seçer, AIMovementBrain (Yapay Zeka Hareket Beyni) stratejik yürüyüşleri yönetir ve Attacker (Saldıran) bekleme süresi takibiyle profil odaklı komboları yürütür; böylece AI birimlerine modüler savaş zekâsı kazandırılır.
- Genel akış GameManager tekil nesnesi ve sahneye bağlı LevelManager ile başlatıldı; seviye seçimi, sahne yükleme ve kazan/kaybet tetiklerini yeni olay sistemi etrafında bağlar.

### 11 July 2025

- Sunum ve etkileşim yükseltmeleri
- Karakter tanımları; maliyet, animasyon, hedefleme ve geçersiz kılma kancalarıyla genişletildi; böylece tek bir varlık, birim başına prefab, UI portresi, ses ve VFX seçimlerini yönetebilir.
- EntityAnimator sağlandı; hareket hızı, saldırı, hasar, yükseltme ve kilit açma olaylarını dinleyerek animasyon grafiklerinin oyun durum değişimleriyle senkron kalmasını sağlar.

### 14 July 2025

- Sunum ve etkileşim yükseltmeleri
- Yükseltilebilir BuildingSlotlar uygulandı; onay arayüzü sunar, kaynak harcar, prefab üretir ve SFX/VFX olaylarına bağlanır; böylece üste etkileşimli inşa alanları etkinleşir.
- Savaş güvenilirliği
- Hareket denetleyicisi boyunca Continue komutu ve ölüm kapıları eklendi; AI kesintilerden sonra yollarına devam edebilir ancak etkin olmayan varlıklar diriltilmeden kalır.
- Attacker bekleme sürelerini günceller, hedefleri yeniden edinir ve saldırı başlangıç/etki olaylarını açığa çıkarır; böylece savaşçılar için daha öngörülebilir zamanlama kancaları sağlanır.

### 17 July 2025

- Etkileşim mimarisi ve kamera hissi
- Otomatik etkileştirici; edinim/kayıp olayları yayınlayacak, düşmanca etkileşimleri önceliklendirecek ve profil bayraklarına göre etkileşim/yükseltme/kilit açma iş akışlarında kademeli ilerleyecek şekilde rafine edildi.
- Kamera denetleyici, yeni hız-değişim olayına bağlandı; hızlı oyuncu hareketi otomatik olarak yakınlaşmayı genişletirken, boşta kalma sürelerinde sinematik bir tempoyla yumuşakça merkezlenir.

### 18 July 2025

- Sağlık çubuğu ve animasyon uyumu
- DOTween tabanlı HealthBarAnimationHandler mantığı eklendi; sağlık kaynaklarını otomatik bulur, dolum/etki çubuklarını animeler, kaynak değişimlerinde sarsar ve tam sağlıktayken otomatik gizler; daha temiz HUD geri bildirimi sağlar.
- EntityAnimator geliştirildi; Playables tabanlı saldırı harmanlama, olay abonelikleri ve normalize hız örneklemesi ile lokomosyon, hasar ve ölüm klipleri varlıklar arasında senkronize kalır.
- Savaş ve bina modülerleştirme
- Bina yuvası hattı yeniden inşa edildi; kaynak harcar, yıkım/yükseltme animasyonları oynatır ve doğan sağlık bileşenlerine dinleyiciler bağlar; çok aşamalı yapıları ve geriye düşürmeleri zarifçe destekler.

### 21 July 2025

Savaş ve bina modülerleştirme

- Menzilli saldırı tanımları, yeniden kullanılabilir bina animasyon klipleri ve kule işleyicileri eklendi; yeni prefablar, özel betiklere ihtiyaç duymadan paylaşılan saldıran hattından yararlanabilir.
- Veri odaklı üreticiler, doğurucular ve kule denetleyicileri (örn. ResourceGenerator, ArcherTowerHandler, doğurma hızı yöneticileri) uygulandı; üs savunmalarını ve pasif gelir döngülerini çeşitlendirir.

### 24 July 2025

Varlık içe aktarma

- Oyun içi model prefabları projeye alındı; karakter ve yapı tanımları üretim sanat varlıklarına referans verir hale getirildi; veri odaklı profiller yeni görsel kütüphaneyle hizalandı.
- Prefab üretimi
- Çevre prop prefabları ve kategori verisi yazıldı; dünya üretim tarifeleri savaş alanı çevresine seçilmiş dekor varyasyonlarını yerleştirebilir.
- Çevre giydirme ve dünya üretimi
- Tarifeleri okuyan, çizgi segmentleri boyunca Perlin süzgeçli yerleşim örnekleyen ve oluşturulan prop’ları kolay geri alma için gruplandıran LineObjectPlacerWindow editörü oluşturuldu; seviye giydirme iş akışlarını hızlandırır.

### 25 July 2025

Çevre giydirme ve dünya üretimi

- Yerleştirici tarafından kullanılan yoğunluklar, varyantlar ve dağılım eğrilerini tanımlamak için yardımcı editör araçları (rastgele dönüşüm yöneticileri, prop profilleri) eklendi.
- Geri bildirim ve ekonomi UI yükseltmesi
- Kaynak olaylarını dinleyen, görüntülenen toplamları tween eden, delta açılır pencereleri oluşturan ve büyüklük ölçeklemesiyle UI öğelerini sarsan ResourceCounter widget’ları uygulandı; ekonomi vuruşlarını kutlar.

### 28 July 2025

Geri bildirim ve ekonomi UI yükseltmesi

- ResourceDropper yükseltildi; büyük ödemeleri akıllı kupürlere böler, düşme fiziğini rastgeleleştirir ve toplama efektlerini tetikler; kaynak sağanaklarını performanslı ve tatmin edici kılar.
- Varlık ölümünde çözünme (dissolve) işleme; mağlup birimler VFX ve SFX üretirken, ölüm işleyicisi üzerinden temizlik planlanır; savaş alanı okunabilirliği artar.
- Veri temizliği
- Seviye veri ScriptableObject’leri yeniden adlandırıldı ve düzenlendi; ilerleme varlıkları GameManager beklentileriyle hizalandı; UI seçiminden sahne yüklenirken kafa karışıklığı azaltıldı.

### 31 July 2025

Veri temizliği

- Fraksiyon sabitlemesi karakter tanımlarından kaldırıldı; daha iyi prefab yeniden kullanımı için hizalama, ekli Faction bileşenlerine devredildi.
- Bina sistemi kararlılığı
- Birim doğurucu mantığı; yapılandırılabilir doğurma-hızı yöneticileri ve hata tanılamalarıyla sağlamlaştırıldı; geç oyunda bile ağır yük altında dalga temposu korunur.

### 1 August 2025

Bina sistemi kararlılığı

- Bina yuvaları yükseltme sırasında sağlık dinleyicilerini ayıracak ve etkileşim UI’ını solduracak şekilde yamalandı; hızlı yeniden inşalarda yinelenen ölüm işlemleri önlenir.
- Koşu ortası ekonomi ve savunma
- ResourceGenerator rutinleri eklendi; birden fazla kaynak kanalını tikler, oranları saniye başı getirilerle dönüştürür ve tutarlı pasif gelir geri bildirimi için ilgili ses/görsel efektleri tetikler.
- Savunma kule davranışları ve doğurma hızı denetleyicileri sağlandı; pasif yapılar ateş hızlarını ölçekleyebilir ve merkezi kaynak defterine entegre olur.

### 4 August 2025

Ses/görsel birliği ve oyun sonu akışları

- Ses çalmayı SFXManager tekil nesnesi üzerinden merkezileştirildi; küresel olaylar, kütüphane anahtarlarına bağlanır ve birim hasarı, ölümler, yükseltmeler ve zafer/mağlubiyet için yeniden kullanılabilir ses kaynakları yaratılır.
- Aynı yaklaşım görseller için VFXManager cephesiyle yansıtıldı; parçacık, kamera sarsıntısı ve post-processing efektleri, aynı olay otobüsüne tepki veren veri odaklı eşlemeler üzerinden yönlendirilir.

### 7 August 2025

Ses/görsel birliği ve oyun sonu akışları

- Oyun sonu UI panelleri zafer/mağlubiyet olaylarına bağlandı; böylece üs savunma döngüsü, ses-görsel pekiştirmelerle cilalı kazan/kaybet ekranlarına geçer.
- Kademeli zorluk ve yeni içerik
- El işi dalga tablolarına gerek kalmadan; ölçek verilerini seviye veri eğimlerine referansla büyüten ve ölçeklenmemiş zaman üzerinde artan doğurma-hızı ölçekleyicileri eklendi.

### 8 August 2025

Kademeli zorluk ve yeni içerik

- Kule ve prestij üretici verileri genişletildi; yeni yapılar, karakter profillerinde tanımlanan merkezi ses/VFX geçersiz kılmalarını paylaşır.
- Hareket hatası düzeltmesi
- AIMovementBrain etkin olmayan varlık durumunda erken çıkacak ve komut vermeden önce hareket denetleyicilerini doğrulayacak şekilde güncellendi; sahne geçişleri sırasında olası boş referanslar önlendi.

### 11 August 2025

SFX sistemi kurulum geçişi

- Varlığa özgü SFX anahtarları, zarifçe varsayılanlara geri dönecek şekilde sonlandırıldı; tasarımcılar birim başına ayak sesi/çarpma seslerini kod değişikliği olmadan değiştirebilir.
- Dayanıklılık ve seviye içeriği
- Health bileşeninde otomatik diriltme rejenerasyonu etkinleştirildi; hasarda zamanlayıcılar sıfırlanır ve yapılandırılabilir gecikmelerden sonra sağlık geri yüklenir; uzun görevlerin akışı sürer.

### 14 August 2025

Dayanıklılık ve seviye içeriği

- Seviye 1 içeriği LevelManagera bağlandı; düşman üssünü yok etmek veya kahramanı kaybetmek zafer/mağlubiyet olaylarını ateşler ve bu olaylar UI ve kalıcılık sistemlerine kabarır.
- Ana menü ve seviye seçimi
- Kaydırılabilir LevelSelectUI oluşturuldu; GameManager olaylarını dinler, etkin göreve odaklanır ve seçili seviyeyi yüklemek için Oynat düğmesini sürer.

### 15 August 2025

Ana menü ve seviye seçimi

- LevelSelectButton davranışları eklendi; kilit/tamamlama görsellerini değiştirir, mevcut seçimi vurgular ve tıklandığında ilerleme tekiline çağrı yapar.
- Ana menü kolaylıkları sağlandı; LoadScene0Button zaman ölçeğini sıfırlar ve menü sahnesine döner; ön kapı navigasyon döngüsü tamamlandı.