# Project Oba Staj Raporu (Detaylı)

## 3 Temmuz 2025 – Başlangıç
- Kalıcı bir `InputManager` (Girdi Yöneticisi) tekil nesnesi kuruldu; klavye eksenlerini kare başına joystick verisiyle birleştirir ve `OnMove` olayı üzerinden normalize edilmiş hareket vektörlerini yayınlayarak herhangi bir hareket bileşeninin belirli giriş cihazlarından bağımsız kalmasını sağlar.【F:Assets/Scripts/Input/InputManager.cs†L4-L83】
- Dokunmaya uygun bir `VirtualJoystick` (Sanal Kumanda) sunuldu; sürükleme hareketini arka plan yarıçapına sıkıştırır, basılı tutulduğu sürece normalize vektörleri giriş merkezine sürekli besler ve mobilde hayalet girişleri önlemek için bırakıldığında kolu temizler.【F:Assets/Scripts/UI/VirtualJoystick.cs†L4-L80】
- Modüler bir `CameraController` (Kamera Denetleyicisi) eklendi; oyuncuyu yumuşak damp/lerp geçişleriyle takip eder, ortografik bir ofseti korur ve ScriptableObject ayarlarıyla hız olaylarına abone olarak uyarlanabilir yakınlaşma/uzaklaşma temposu sağlar.【F:Assets/Scripts/Camera/CameraController.cs†L3-L160】

## 5 Temmuz 2025 – Mobil kontrollerde cilalama
- Sanal joystick’in işaretçi işleme akışı sağlamlaştırıldı; sürükleme ofsetlerini UI uzayında yeniden hesaplar, kolun hareketini sınırlar ve parmak pedin kenarını terk etse bile giriş akışının stabil kalması için normalize vektörler yazar.【F:Assets/Scripts/UI/VirtualJoystick.cs†L35-L68】
- Dokunmatik vektörlerin birikmemesi için giriş merkezinde joystick niyeti her karede sıfırlandı; mobil giriş yoksa yalnızca klavye yedekleri devreye girer, böylece dokunmatik cihazlarda “takılı yürüyüş” senaryoları önlenir.【F:Assets/Scripts/Input/InputManager.cs†L36-L83】

## 7 Temmuz 2025 – Hareket ayarı
- Tüm hareket çağrıları varlık durum kontrolleriyle korundu; ölü veya devre dışı aktörler anında durur, böylece yenilgi sonrası navmesh ajanlarının sürüklenmesi önlenirken dünya- ve transform-tabanlı hedefler sunulmaya devam edilir.【F:Assets/Scripts/Movement/MovementController.cs†L63-L154】
- Hareket başlatma merkezileştirildi; istatistik varlıkları (ScriptableObject) çalışma zamanından önce doğrulanır ve bir tanım eksik olduğunda ayrıntılı tanılama sunulur; bu sayede veri odaklı hareket tanımları senkron kalır.【F:Assets/Scripts/Movement/MovementController.cs†L35-L61】

## 10 Temmuz 2025 – Çekirdek sistemler sprinti
- Proje genelinde `GameEvents` (Oyun Olayları) otobüsü tanıtıldı; savaş, kaynak, ilerleme ve hız bildirimlerini yükselterek gevşek bağlı sistemlerin (UI, ses, VFX) sert referanslar olmadan tepki vermesini sağlar.【F:Assets/Scripts/Core/GameEvents.cs†L4-L153】
- Başlangıç bakiyelerini yükleyen, satın alımları koşullandıran ve bakiye değişikliklerini yayınlayan `ResourceManager` (Kaynak Yöneticisi) tekil nesnesi kuruldu; binalar ve düşen nesneler için veri odaklı ekonomi döngüsünü güçlendirir.【F:Assets/Scripts/Core/ResourceManager.cs†L1-L120】
- `Entity` (Varlık) merkezi eklendi; tek bir `CharacterDefinitionSO` üzerinden fraksiyon, sağlık, hareket, saldırı, hedefleme ve etkileşim bileşenlerini bir araya getirerek herhangi bir prefab için güvenilir başlatma sağlar.【F:Assets/Scripts/Entity/Entity.cs†L4-L137】【F:Assets/Scripts/Data/CharacterDefinitionSO.cs†L4-L115】
- Saha karar verme süreci ayrıştırıldı: `AutoInteractor` (Otomatik Etkileştirici) taktik hedefleri seçer, `AIMovementBrain` (Yapay Zeka Hareket Beyni) stratejik yürüyüşleri yönetir ve `Attacker` (Saldıran) bekleme süresi takibiyle profil odaklı komboları yürütür; böylece AI birimlerine modüler savaş zekâsı kazandırılır.【F:Assets/Scripts/Interaction/AutoInteractor.cs†L6-L197】【F:Assets/Scripts/AI/AIMovementBrain.cs†L4-L195】【F:Assets/Scripts/Combat/Attacker.cs†L1-L139】
- Genel akış `GameManager` tekil nesnesi ve sahneye bağlı `LevelManager` ile başlatıldı; seviye seçimi, sahne yükleme ve kazan/kaybet tetiklerini yeni olay sistemi etrafında bağlar.【F:Assets/Scripts/Core/GameManager.cs†L7-L121】【F:Assets/Scripts/Core/LevelManager.cs†L4-L111】

## 11 Temmuz 2025 – Sunum ve etkileşim yükseltmeleri
- Karakter tanımları; maliyet, animasyon, hedefleme ve geçersiz kılma kancalarıyla genişletildi; böylece tek bir varlık, birim başına prefab, UI portresi, ses ve VFX seçimlerini yönetebilir.【F:Assets/Scripts/Data/CharacterDefinitionSO.cs†L11-L115】
- `EntityAnimator` sağlandı; hareket hızı, saldırı, hasar, yükseltme ve kilit açma olaylarını dinleyerek animasyon grafiklerinin oyun durum değişimleriyle senkron kalmasını sağlar.【F:Assets/Scripts/Animation/EntityAnimator.cs†L8-L198】
- Yükseltilebilir `BuildingSlot`lar uygulandı; onay arayüzü sunar, kaynak harcar, prefab üretir ve SFX/VFX olaylarına bağlanır; böylece üste etkileşimli inşa alanları etkinleşir.【F:Assets/Scripts/Buildings/BuildingSlot.cs†L6-L181】

## 12 Temmuz 2025 – Boru hattı canlı tutma
- Daha büyük yeniden düzenlemeler gözden geçirilirken CI otomasyonunu çalışır halde tutmak için boş bir commit itildi; takip eden birleştirmeler için derleme tazeliği korundu.【d147ea†L60-L63】

## 14 Temmuz 2025 – Savaş güvenilirliği
- Hareket denetleyicisi boyunca `Continue` komutu ve ölüm kapıları eklendi; AI kesintilerden sonra yollarına devam edebilir ancak etkin olmayan varlıklar diriltilmeden kalır.【F:Assets/Scripts/Movement/MovementController.cs†L88-L154】
- `Attacker` bekleme sürelerini günceller, hedefleri yeniden edinir ve saldırı başlangıç/etki olaylarını açığa çıkarır; böylece savaşçılar için daha öngörülebilir zamanlama kancaları sağlanır.【F:Assets/Scripts/Combat/Attacker.cs†L40-L139】

## 15 Temmuz 2025 – Etkileşim mimarisi ve kamera hissi
- Otomatik etkileştirici; edinim/kayıp olayları yayınlayacak, düşmanca etkileşimleri önceliklendirecek ve profil bayraklarına göre etkileşim/yükseltme/kilit açma iş akışlarında kademeli ilerleyecek şekilde rafine edildi.【F:Assets/Scripts/Interaction/AutoInteractor.cs†L122-L197】
- Kamera denetleyici, yeni hız-değişim olayına bağlandı; hızlı oyuncu hareketi otomatik olarak yakınlaşmayı genişletirken, boşta kalma sürelerinde sinematik bir tempoyla yumuşakça merkezlenir.【F:Assets/Scripts/Camera/CameraController.cs†L21-L160】【F:Assets/Scripts/Core/GameEvents.cs†L37-L120】

## 17 Temmuz 2025 – Sağlık çubuğu ve animasyon uyumu
- DOTween tabanlı `HealthBarAnimationHandler` mantığı eklendi; sağlık kaynaklarını otomatik bulur, dolum/etki çubuklarını animeler, kaynak değişimlerinde sarsar ve tam sağlıktayken otomatik gizler; daha temiz HUD geri bildirimi sağlar.【F:Assets/Scripts/UI/HealthBarAnimationHandler.cs†L5-L171】
- `EntityAnimator` geliştirildi; Playables tabanlı saldırı harmanlama, olay abonelikleri ve normalize hız örneklemesi ile lokomosyon, hasar ve ölüm klipleri varlıklar arasında senkronize kalır.【F:Assets/Scripts/Animation/EntityAnimator.cs†L31-L206】

## 21 Temmuz 2025 – Savaş ve bina modülerleştirme
- Bina yuvası hattı yeniden inşa edildi; kaynak harcar, yıkım/yükseltme animasyonları oynatır ve doğan sağlık bileşenlerine dinleyiciler bağlar; çok aşamalı yapıları ve geriye düşürmeleri zarifçe destekler.【F:Assets/Scripts/Buildings/BuildingSlot.cs†L123-L200】
- Menzilli saldırı tanımları, yeniden kullanılabilir bina animasyon klipleri ve kule işleyicileri eklendi; yeni prefablar, özel betiklere ihtiyaç duymadan paylaşılan saldıran hattından yararlanabilir.【F:Assets/Scripts/Combat/Attacker.cs†L140-L200】
- Veri odaklı üreticiler, doğurucular ve kule denetleyicileri (örn. `ResourceGenerator`, `ArcherTowerHandler`, doğurma hızı yöneticileri) uygulandı; üs savunmalarını ve pasif gelir döngülerini çeşitlendirir.【F:Assets/Scripts/Buildings/ResourceGenerator.cs†L4-L78】【F:Assets/Scripts/Buildings/UnitSpawner/IncreasingLevelSpawnRateManager.cs†L1-L32】

## 25 Temmuz 2025 – Varlık içe aktarma
- Oyun içi model prefabları projeye alındı; karakter ve yapı tanımları üretim sanat varlıklarına referans verir hale getirildi; veri odaklı profiller yeni görsel kütüphaneyle hizalandı.【F:Assets/Scripts/Data/CharacterDefinitionSO.cs†L35-L105】

## 27 Temmuz 2025 – Prefab üretimi
- Çevre prop prefabları ve kategori verisi yazıldı; dünya üretim tarifeleri savaş alanı çevresine seçilmiş dekor varyasyonlarını yerleştirebilir.【F:Assets/Scripts/Editor/World Generation/WorldGenerationRecipeSO.cs†L1-L120】

## 28 Temmuz 2025 – Çevre giydirme ve dünya üretimi
- Tarifeleri okuyan, çizgi segmentleri boyunca Perlin süzgeçli yerleşim örnekleyen ve oluşturulan prop’ları kolay geri alma için gruplandıran `LineObjectPlacerWindow` editörü oluşturuldu; seviye giydirme iş akışlarını hızlandırır.【F:Assets/Scripts/Editor/World Generation/LineObjectPlacerWindow.cs†L6-L170】
- Yerleştirici tarafından kullanılan yoğunluklar, varyantlar ve dağılım eğrilerini tanımlamak için yardımcı editör araçları (rastgele dönüşüm yöneticileri, prop profilleri) eklendi.【F:Assets/Scripts/Editor/World Generation/PropProfile.cs†L1-L160】

## 31 Temmuz 2025 – Geri bildirim ve ekonomi UI yükseltmesi
- Kaynak olaylarını dinleyen, görüntülenen toplamları tween eden, delta açılır pencereleri oluşturan ve büyüklük ölçeklemesiyle UI öğelerini sarsan `ResourceCounter` widget’ları uygulandı; ekonomi vuruşlarını kutlar.【F:Assets/Scripts/UI/InGame/ResourceCounter.cs†L5-L155】
- `ResourceDropper` yükseltildi; büyük ödemeleri akıllı kupürlere böler, düşme fiziğini rastgeleleştirir ve toplama efektlerini tetikler; kaynak sağanaklarını performanslı ve tatmin edici kılar.【F:Assets/Scripts/Resource/ResourceDropper.cs†L5-L190】
- Varlık ölümünde çözünme (dissolve) işleme; mağlup birimler VFX ve SFX üretirken, ölüm işleyicisi üzerinden temizlik planlanır; savaş alanı okunabilirliği artar.【F:Assets/Scripts/Entity/Entity.cs†L100-L137】

## 1 Ağustos 2025 – Veri temizliği
- Seviye veri ScriptableObject’leri yeniden adlandırıldı ve düzenlendi; ilerleme varlıkları `GameManager` beklentileriyle hizalandı; UI seçiminden sahne yüklenirken kafa karışıklığı azaltıldı.【F:Assets/Scripts/Core/GameManager.cs†L21-L121】
- Fraksiyon sabitlemesi karakter tanımlarından kaldırıldı; daha iyi prefab yeniden kullanımı için hizalama, ekli `Faction` bileşenlerine devredildi.【F:Assets/Scripts/Data/CharacterDefinitionSO.cs†L40-L58】

## 4 Ağustos 2025 – Bina sistemi kararlılığı
- Birim doğurucu mantığı; yapılandırılabilir doğurma-hızı yöneticileri ve hata tanılamalarıyla sağlamlaştırıldı; geç oyunda bile ağır yük altında dalga temposu korunur.【F:Assets/Scripts/Buildings/UnitSpawner/IncreasingLevelSpawnRateManager.cs†L1-L32】
- Bina yuvaları yükseltme sırasında sağlık dinleyicilerini ayıracak ve etkileşim UI’ını solduracak şekilde yamalandı; hızlı yeniden inşalarda yinelenen ölüm işlemleri önlenir.【F:Assets/Scripts/Buildings/BuildingSlot.cs†L140-L200】

## 7 Ağustos 2025 – Koşu ortası ekonomi ve savunma
- `ResourceGenerator` rutinleri eklendi; birden fazla kaynak kanalını tikler, oranları saniye başı getirilerle dönüştürür ve tutarlı pasif gelir geri bildirimi için ilgili ses/görsel efektleri tetikler.【F:Assets/Scripts/Buildings/ResourceGenerator.cs†L4-L78】
- Savunma kule davranışları ve doğurma hızı denetleyicileri sağlandı; pasif yapılar ateş hızlarını ölçekleyebilir ve merkezi kaynak defterine entegre olur.【F:Assets/Scripts/Buildings/UnitSpawner/IncreasingLevelSpawnRateManager.cs†L1-L32】【F:Assets/Scripts/Combat/Attacker.cs†L140-L200】

## 8 Ağustos 2025 – Ses/görsel birliği ve oyun sonu akışları
- Ses çalmayı `SFXManager` tekil nesnesi üzerinden merkezileştirildi; küresel olaylar, kütüphane anahtarlarına bağlanır ve birim hasarı, ölümler, yükseltmeler ve zafer/mağlubiyet için yeniden kullanılabilir ses kaynakları yaratılır.【F:Assets/Scripts/Audio/SFXManager.cs†L4-L186】
- Aynı yaklaşım görseller için `VFXManager` cephesiyle yansıtıldı; parçacık, kamera sarsıntısı ve post-processing efektleri, aynı olay otobüsüne tepki veren veri odaklı eşlemeler üzerinden yönlendirilir.【F:Assets/Scripts/VFX/VFXManager.cs†L4-L180】
- Oyun sonu UI panelleri zafer/mağlubiyet olaylarına bağlandı; böylece üs savunma döngüsü, ses-görsel pekiştirmelerle cilalı kazan/kaybet ekranlarına geçer.【F:Assets/Scripts/Core/LevelManager.cs†L48-L111】【F:Assets/Scripts/UI/EndGame/EndGamePanelsHandler.cs†L1-L160】

## 13 Ağustos 2025 – Kademeli zorluk ve yeni içerik
- El işi dalga tablolarına gerek kalmadan; ölçek verilerini seviye veri eğimlerine referansla büyüten ve ölçeklenmemiş zaman üzerinde artan doğurma-hızı ölçekleyicileri eklendi.【F:Assets/Scripts/Buildings/UnitSpawner/IncreasingLevelSpawnRateManager.cs†L1-L32】
- Kule ve prestij üretici verileri genişletildi; yeni yapılar, karakter profillerinde tanımlanan merkezi ses/VFX geçersiz kılmalarını paylaşır.【F:Assets/Scripts/Data/CharacterDefinitionSO.cs†L81-L115】【F:Assets/Scripts/Audio/SFXManager.cs†L184-L200】

## 14 Ağustos 2025 – Hareket hatası düzeltmesi
- `AIMovementBrain` etkin olmayan varlık durumunda erken çıkacak ve komut vermeden önce hareket denetleyicilerini doğrulayacak şekilde güncellendi; sahne geçişleri sırasında olası boş referanslar önlendi.【F:Assets/Scripts/AI/AIMovementBrain.cs†L107-L138】

## 15 Ağustos 2025 – SFX sistemi kurulum geçişi
- Varlığa özgü SFX anahtarları, zarifçe varsayılanlara geri dönecek şekilde sonlandırıldı; tasarımcılar birim başına ayak sesi/çarpma seslerini kod değişikliği olmadan değiştirebilir.【F:Assets/Scripts/Audio/SFXManager.cs†L184-L200】

## 18 Ağustos 2025 – Dayanıklılık ve seviye içeriği
- `Health` bileşeninde otomatik diriltme rejenerasyonu etkinleştirildi; hasarda zamanlayıcılar sıfırlanır ve yapılandırılabilir gecikmelerden sonra sağlık geri yüklenir; uzun görevlerin akışı sürer.【F:Assets/Scripts/Combat/Health.cs†L39-L176】
- Seviye 1 içeriği `LevelManager`a bağlandı; düşman üssünü yok etmek veya kahramanı kaybetmek zafer/mağlubiyet olaylarını ateşler ve bu olaylar UI ve kalıcılık sistemlerine kabarır.【F:Assets/Scripts/Core/LevelManager.cs†L48-L111】

## 21 Ağustos 2025 – Ana menü ve seviye seçimi
- Kaydırılabilir `LevelSelectUI` oluşturuldu; `GameManager` olaylarını dinler, etkin göreve odaklanır ve seçili seviyeyi yüklemek için Oynat düğmesini sürer.【F:Assets/Scripts/UI/LevelSelect/LevelSelectUI.cs†L7-L135】
- `LevelSelectButton` davranışları eklendi; kilit/tamamlama görsellerini değiştirir, mevcut seçimi vurgular ve tıklandığında ilerleme tekiline çağrı yapar.【F:Assets/Scripts/UI/LevelSelect/LevelSelectButton.cs†L5-L58】
- Ana menü kolaylıkları sağlandı; `LoadScene0Button` zaman ölçeğini sıfırlar ve menü sahnesine döner; ön kapı navigasyon döngüsü tamamlandı.【F:Assets/Scripts/UI/LoadScene0Button.cs†L4-L29】
