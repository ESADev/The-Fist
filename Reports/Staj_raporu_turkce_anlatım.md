Proje amacım bir mobil oyun geliştrimek. Takımımda sadece ben varım ve mentrolerimle düzenli toplantılar sayesinde görüşüp fikirlerini alıyorum. Teknoloji temeli: Unity, C#…

Yazılımsal anlamda amacım, büyütülürken zorlanılmayan, kolay ölçeklendirilebilir, büyüdükçe spagettiye dönüşmeyen bir mimariyi korumak ama aynı zamanda ortaya çalışan ve tamamlanmış bir ürün çıkartmak.

### 30 June 2025

Başlangıç

- Kalıcı bir InputManager (Girdi Yöneticisi) tekil nesnesi kuruldu; klavye eksenlerini kare başına joystick verisiyle birleştirir ve OnMove olayı üzerinden normalize edilmiş hareket vektörlerini yayınlayarak herhangi bir hareket bileşeninin belirli giriş cihazlarından bağımsız kalmasını sağlar.
    - Yani klavye, gamepad veya ekrandaki sanal bir joystick’in hep beraber kullanabileceği modüler bir sistem kurdum. Bu sayede oyunu çıkarttığımda ekrandaki joystiği kullanabilecekken oyunu bilgisayarda geliştirirken de klavye ile hareketi sağlıyorum.
- Dokunmaya uygun bir VirtualJoystick (Sanal Kumanda) sunuldu; sürükleme hareketini arka plan yarıçapına sıkıştırır, basılı tutulduğu sürece normalize vektörleri giriş merkezine sürekli besler ve mobilde hayalet girişleri önlemek için bırakıldığında kolu temizler.
    - Yani ekranın ortasında beklenen şekilde çalışan normal bir sanal joystick yaptım, InputManager ile birbirine bağımlı değiller, sadece anonsları dinleyerek çalışıyorlar.
- Modüler bir CameraController (Kamera Denetleyicisi) eklendi; oyuncuyu yumuşak damp/lerp geçişleriyle takip eder, ortografik bir ofseti korur ve ScriptableObject ayarlarıyla hız olaylarına abone olarak uyarlanabilir yakınlaşma/uzaklaşma temposu sağlar.
    - Burada kamera ayarlarını bir scriptable objectte tutuyorduk yani data driven mı deniyordu buna öyle bir şey. Ayrıca en temel şekilde anlatmak gerekirse kameramız izometrik ortografik bir kameradır.

### 3 July 2025

Mobil kontrollerde cilalama

- Sanal joystick’in işaretçi işleme akışı sağlamlaştırıldı; sürükleme ofsetlerini UI uzayında yeniden hesaplar, kolun hareketini sınırlar ve parmak pedin kenarını terk etse bile giriş akışının stabil kalması için normalize vektörler yazar.
    - Daha robust bir joystick yaptık yani
- Dokunmatik vektörlerin birikmemesi için giriş merkezinde joystick niyeti her karede sıfırlandı; mobil giriş yoksa yalnızca klavye yedekleri devreye girer, böylece dokunmatik cihazlarda “takılı yürüyüş” senaryoları önlenir.
    - Hareketi sağlayan şeylere anons yapan tek bir sistem yok ve yedek sistemler sayesinde mobil giriş yokken klavyeden de hareket sağlanabiliyordu.

### 4 July 2025

Hareket ayarı

- Tüm hareket çağrıları varlık durum kontrolleriyle korundu; ölü veya devre dışı aktörler anında durur, böylece yenilgi sonrası navmesh ajanlarının sürüklenmesi önlenirken dünya- ve transform-tabanlı hedefler sunulmaya devam edilir.
- Hareket başlatma merkezileştirildi; istatistik varlıkları (ScriptableObject) çalışma zamanından önce doğrulanır ve bir tanım eksik olduğunda ayrıntılı tanılama sunulur; bu sayede veri odaklı hareket tanımları senkron kalır.
- Çekirdek sistemler sprinti
- Burada devasa bir PR dalgası yaşandı. Bu PR dalgası, her sınıf, sistem, tüm alanları ve işlevleri, aralarındaki etkileşimler vb. ile sistemin büyük resmini oldukça uzun bir planlama ve çizim sürecinden geçtiğim için ortaya çıktı. Olay tetikleyici sistemlere ve daha birçok şeye odaklandım.
    - Fotoğrafı ekledim: “BigPictureOverview.jpg”
- İlerde her sistemden kısaca bahsedeceğim:
    - Tüm sistemler birbirinden bağımsız. Event driven architecture ile tasarlandı. Data driven scriptable objectler ile tasarlandı. Bu sayede ilerleyen zamanda kod yazmadan yeni objeler tanımlamak mümkün olacak.
    - Health sistemi
        - Can yönetimi, can istatistikleri, can ile ilgili her şeyi yöneten sistem.
    - Hareket sistemi
        - Hareket edebilenleri belirleyen IMoveable interface’i
        - Oyuncu joystick ile, NPC’ler ai ile hareket edeceği için ikisini ayıran IMoveable’dan gelen iki hareket kontrolcüsü, hareket verilerini tutan scriptable object sistemi

### 7 July 2025

- Çekirdek sistemler sprinti
- Proje genelinde GameEvents (Oyun Olayları) otobüsü tanıtıldı; savaş, kaynak, ilerleme ve hız bildirimlerini yükselterek gevşek bağlı sistemlerin (UI, ses, VFX) sert referanslar olmadan tepki vermesini sağlar.
    - GameEvents
        - Oyunun genelini ilgilendiren, tam anlamıyla ortaya yapılması gereken anonsların yapıldığı ana radyo kulesi
- Başlangıç bakiyelerini yükleyen, satın alımları koşullandıran ve bakiye değişikliklerini yayınlayan ResourceManager (Kaynak Yöneticisi) tekil nesnesi kuruldu; binalar ve düşen nesneler için veri odaklı ekonomi döngüsünü güçlendirir.
    - Kaynak yönetim sistemi
        - Birden çok kaynak tipini yöneten banka sistemi.

### 10 July 2025

- Çekirdek sistemler sprinti
- Entity (Varlık) merkezi eklendi; tek bir CharacterDefinitionSO üzerinden fraksiyon, sağlık, hareket, saldırı, hedefleme ve etkileşim bileşenlerini bir araya getirerek herhangi bir prefab için güvenilir başlatma sağlar.
    - Karakter kimliği sistemi
        - Karakterin kimliğini tutan, içerisinde saldırı, can, hareket, takım, gamobject prefab gibi birçok bilgiyi barındıran id sistemi. Bir karakteri tanımlamak ve oyunda yerleştirmek veya oyunda tanımlamak için kullanılan sistem.
- Saha karar verme süreci ayrıştırıldı: AutoInteractor (Otomatik Etkileştirici) taktik hedefleri seçer, AIMovementBrain (Yapay Zeka Hareket Beyni) stratejik yürüyüşleri yönetir ve Attacker (Saldıran) bekleme süresi takibiyle profil odaklı komboları yürütür; böylece AI birimlerine modüler savaş zekâsı kazandırılır.
    - Interaction tiplerine göre Interface’ler
        - IUpgradable, IDestructible, ICollectible, IHealable, IInteractable, IUnlockable şeklinde.
    - Attacking sistemi
        - Saldırı türlerini, karakterlerin saldırı kimliğini tutan objeleri bilen ve çalıştıran; saldırılması gerektiğinde saldır komutu ile çalıştırılan sistem.
    - TargetScanner sistemi
        - Tüm karakterlerde bulunan, gözler görevi gören sistem. Etrafı kendi kabiliyet (bu bilgi de data driven) durumuna göre tarar ve bir liste tutar.
    - AIController
        - NPC’lerin hareket davranışını yöneten sistem. NPC’ler ana hedefe giderken yolda ufak hedeflerle karşılaşırsa önce onlarla etkileşime geçiyor, sonra ana hedeflerine doğru ilerlemeye devam ediyorlar.
    - Interactor sistemi
        - Aslında oyunda oyuncunun hareketi hariç diğer şeyler (yaklaştığı kişiye saldırması veya interaksiyona girmesi gibi) de yapay zeka tarafından kararlaştırıldığı için oyuncuda da bulunan otomatik interaksiyon yöneticisi
- Genel akış GameManager tekil nesnesi ve sahneye bağlı LevelManager ile başlatıldı; seviye seçimi, sahne yükleme ve kazan/kaybet tetiklerini yeni olay sistemi etrafında bağlar.
    - Bir oyun ve bölüm yöneticisi yani

### 11 July 2025

- Sunum ve etkileşim yükseltmeleri
- Karakter tanımları; maliyet, animasyon, hedefleme ve geçersiz kılma kancalarıyla genişletildi; böylece tek bir varlık, birim başına prefab, UI portresi, ses ve VFX seçimlerini yönetebilir.
    - Karakterin data driven’lığını arttırdık, daha çok özellik ekledik.
- EntityAnimator sağlandı; hareket hızı, saldırı, hasar, yükseltme ve kilit açma olaylarını dinleyerek animasyon grafiklerinin oyun durum değişimleriyle senkron kalmasını sağlar.
    - Animasyonlarda da aslında her bir entity’nin yapabileceği belli animasyon state’leri olduğu için bir base animator controller’ın üzerinden sadece override eden animasyon kliplerini değiştirdik.

### 14 July 2025

- Sunum ve etkileşim yükseltmeleri
- Yükseltilebilir BuildingSlotlar uygulandı; onay arayüzü sunar, kaynak harcar, prefab üretir ve SFX/VFX olaylarına bağlanır; böylece üste etkileşimli inşa alanları etkinleşir.
    - Kaynak harcayarak bina yükseltme özelliği ekledik kısacası.
- Savaş güvenilirliği
- Hareket denetleyicisi boyunca Continue komutu ve ölüm kapıları eklendi; AI kesintilerden sonra yollarına devam edebilir ancak etkin olmayan varlıklar diriltilmeden kalır.
    - Zombie behaviour’ları engellemiş olduk.
- Attacker bekleme sürelerini günceller, hedefleri yeniden edinir ve saldırı başlangıç/etki olaylarını açığa çıkarır; böylece savaşçılar için daha öngörülebilir zamanlama kancaları sağlanır.

### 17 July 2025

- Etkileşim mimarisi ve kamera hissi
- Otomatik etkileştirici; edinim/kayıp olayları yayınlayacak, düşmanca etkileşimleri önceliklendirecek ve profil bayraklarına göre etkileşim/yükseltme/kilit açma iş akışlarında kademeli ilerleyecek şekilde rafine edildi.
    - Savaşmak yani canını kurtarmak öncelenmiş oldu, aynı zamanda bug’lar düzeltildi.
- Kamera denetleyici, yeni hız-değişim olayına bağlandı; hızlı oyuncu hareketi otomatik olarak yakınlaşmayı genişletirken, boşta kalma sürelerinde sinematik bir tempoyla yumuşakça merkezlenir.
    - Kamera hareketi daha yumuşak, dinamik ve hoş oldu.

### 18 July 2025

- Sağlık çubuğu ve animasyon uyumu
- DOTween tabanlı HealthBarAnimationHandler mantığı eklendi; sağlık kaynaklarını otomatik bulur, dolum/etki çubuklarını animeler, kaynak değişimlerinde sarsar ve tam sağlıktayken otomatik gizler; daha temiz HUD geri bildirimi sağlar.
    - Her karaktere uyacak şekilde yapıldı, sağlık anonsalırını dinliyor, yarıda kesilen edge case’leri de hesaba katıyor.
- EntityAnimator geliştirildi; Playables tabanlı saldırı harmanlama, olay abonelikleri ve normalize hız örneklemesi ile lokomosyon, hasar ve ölüm klipleri varlıklar arasında senkronize kalır.
- Savaş ve bina modülerleştirme
- Bina yuvası hattı yeniden inşa edildi; kaynak harcar, yıkım/yükseltme animasyonları oynatır ve doğan sağlık bileşenlerine dinleyiciler bağlar; çok aşamalı yapıları ve geriye düşürmeleri (downgrade) zarifçe destekler.

### 21 July 2025

Savaş ve bina modülerleştirme

- Menzilli saldırı tanımları, yeniden kullanılabilir bina animasyon klipleri ve kule işleyicileri eklendi; yeni prefablar, özel betiklere ihtiyaç duymadan paylaşılan saldıran hattından yararlanabilir.
- Veri odaklı üreticiler, doğurucular ve kule denetleyicileri (örn. ResourceGenerator, ArcherTowerHandler, doğurma hızı yöneticileri) uygulandı; üs savunmalarını ve pasif gelir döngülerini çeşitlendirir.
    - Bina çeşitleri eklendi, hiçbir ekstra mekaniği olmayan duvar görevinde binalar da eklenmiş oldu. Ayrıca savunma kulesi, kaynak madeni ve asker üreticileri yapıldı.

### 24 July 2025

Varlık içe aktarma

- Oyun içi model prefabları projeye alındı; karakter ve yapı tanımları üretim sanat varlıklarına referans verir hale getirildi; veri odaklı profiller yeni görsel kütüphaneyle hizalandı.
    - Neredeyse tüm modellerde AI 3d model generator kullanıldı. Buradaki workflow şu şekildeydi, chatgpt’ye görsel ürettir, görselin farklı açılardan görünen hallerini de ürettir. Hunyuan image to 3D model ile 3D model ve StableProjectorz ile texture’lar ürettirildi. İskelet (rigging) gerektiren karakter modelleri mixamo ile auto rig edildi. Tüm modeller oyuna eklendi ve boyutları ayarlandı. Tüm modeller şu aşamada üretilmedi, burada sadece workflow’u anlattım.
- Prefab üretimi
- Çevre prop prefabları ve kategori verisi yazıldı; dünya üretim tarifeleri savaş alanı çevresine seçilmiş dekor varyasyonlarını yerleştirebilir.
- Çevre giydirme ve dünya üretimi
- Tarifeleri okuyan, çizgi segmentleri boyunca Perlin süzgeçli yerleşim örnekleyen ve oluşturulan prop’ları kolay geri alma için gruplandıran LineObjectPlacerWindow editörü oluşturuldu; seviye giydirme iş akışlarını hızlandırır.
    - Editor içerisinde kullanılan yani geliştiricinin işini kolaylaştıran bir dünya oluşturucu kuruldu. Bu oluşturucu bir tarif (recipe) alıyor ve kuralları olan rastgeleliklerle bir seviye haritası oluşturuyor. Burada level ortasındaki ana yoldan ne kadar uzakta ne kadar ihtimalle koyulacağı ve genel olarak ne miktarda koyulacağı gibi veriler sayılar ve eğrilerle tanımlanıyor. Bu sistem seri level üretimini kolaylaştıracak.

### 25 July 2025

Çevre giydirme ve dünya üretimi

- Yerleştirici tarafından kullanılan yoğunluklar, varyantlar ve dağılım eğrilerini tanımlamak için yardımcı editör araçları (rastgele dönüşüm yöneticileri, prop profilleri) eklendi.
- Geri bildirim ve ekonomi UI yükseltmesi
- Kaynak olaylarını dinleyen, görüntülenen toplamları tween eden, delta açılır pencereleri oluşturan ve büyüklük ölçeklemesiyle UI öğelerini sarsan ResourceCounter widget’ları uygulandı; ekonomi vuruşlarını kutlar.
    - Animasyonlarla “game juice”i abartmadan arttıran robust bir sistem. Bağımsız, sadece dinleyerek çalışıyor (emin değilim bunu kontrol et).

### 28 July 2025

Geri bildirim ve ekonomi UI yükseltmesi

- ResourceDropper yükseltildi; büyük ödemeleri akıllı kupürlere böler, düşme fiziğini rastgeleleştirir ve toplama efektlerini tetikler; kaynak sağanaklarını performanslı ve tatmin edici kılar.
    - Ödül düşürülmesi gerektiğinde bu ödülleri anlamlı miktarlarda chunk’lara ayıran bir sistem. Mesela bir düşman öldürüldüğünde düşürdüğü miktarı ayarlayan ve fizikleri modifiye eden sistem bu.
- Varlık ölümünde çözünme (dissolve) işleme; mağlup birimler VFX ve SFX üretirken, ölüm işleyicisi üzerinden temizlik planlanır; savaş alanı okunabilirliği artar.
    - Ölenler yok oluyormuş gibi görünür. Rendering yükü azalır.
- Veri temizliği
- Seviye veri ScriptableObject’leri yeniden adlandırıldı ve düzenlendi; ilerleme varlıkları GameManager beklentileriyle hizalandı; UI seçiminden sahne yüklenirken kafa karışıklığı azaltıldı.

### 31 July 2025

Veri temizliği

- Fraksiyon sabitlemesi karakter tanımlarından kaldırıldı; daha iyi prefab yeniden kullanımı için hizalama, ekli Faction bileşenlerine devredildi.
    - Aynı karakter hem düşmanda hem de dostta üretilebileceği için kimden olduğunu karakter tanımında belirlemek mantıksız oluyordu. İki tane apaynı sadece takımı farklı tanım objesi üretmek gerekiyordu. Tasarımsal bir değişiklik olarak bu yapıldı.
- Bina sistemi kararlılığı
- Birim doğurucu mantığı; yapılandırılabilir doğurma-hızı yöneticileri ve hata tanılamalarıyla sağlamlaştırıldı; geç oyunda bile ağır yük altında dalga temposu korunur.
    - Zorluğu ayarlayabilmek adına bu şekilde bir sistem eklemesi yapıldı.

### 1 August 2025

Bina sistemi kararlılığı

- Bina yuvaları yükseltme sırasında sağlık dinleyicilerini ayıracak ve etkileşim UI’ını solduracak şekilde yamalandı; hızlı yeniden inşalarda yinelenen ölüm işlemleri önlenir.
    - Binaları slot içerisinde yükslettiğimizde bina ölüyordu ve istenmeyen bir davranıştı, bu düzeltildi.
- Koşu ortası ekonomi ve savunma
- ResourceGenerator rutinleri eklendi; birden fazla kaynak kanalını tikler, oranları saniye başı getirilerle dönüştürür ve tutarlı pasif gelir geri bildirimi için ilgili ses/görsel efektleri tetikler.
    - Kaynak üreticisinin kimliği bir veya birden çok kaynak üretebilecek şekilde yine data driven olarak tasarlandı.
- Savunma kule davranışları ve doğurma hızı denetleyicileri sağlandı; pasif yapılar ateş hızlarını ölçekleyebilir ve merkezi kaynak defterine entegre olur.
    - Aynı şekilde savunma kulesi ve asker üretici binaların da karakterlerine göre ayarlamalarını içeren kimlik objeleri üretildi.

### 4 August 2025

Ses/görsel birliği ve oyun sonu akışları

- Ses çalmayı SFXManager tekil nesnesi üzerinden merkezileştirildi; küresel olaylar, kütüphane anahtarlarına bağlanır ve birim hasarı, ölümler, yükseltmeler ve zafer/mağlubiyet için yeniden kullanılabilir ses kaynakları yaratılır.
- Aynı yaklaşım görseller için VFXManager cephesiyle yansıtıldı; parçacık, kamera sarsıntısı ve post-processing efektleri, aynı olay otobüsüne tepki veren veri odaklı eşlemeler üzerinden yönlendirilir.

### 7 August 2025

Ses/görsel birliği ve oyun sonu akışları

- Oyun sonu UI panelleri zafer/mağlubiyet olaylarına bağlandı; böylece üs savunma döngüsü, ses-görsel pekiştirmelerle cilalı kazan/kaybet ekranlarına geçer.
- Kademeli zorluk ve yeni içerik
- El işi dalga tablolarına gerek kalmadan; ölçek verilerini seviye veri eğimlerine referansla büyüten ve ölçeklenmemiş zaman üzerinde artan doğurma-hızı ölçekleyicileri eklendi.
    - Oyunda zorluğu belirleyen ana faktör düşmanın asker üretim hızı olduğu için bunu ayarlayan bir sistem geliştirildi.

### 8 August 2025

Kademeli zorluk ve yeni içerik

- Kule ve prestij üretici verileri genişletildi; yeni yapılar, karakter profillerinde tanımlanan merkezi ses/VFX geçersiz kılmalarını paylaşır.
    - Bir grubun vfx sfx ortaklıkları artık kimlik kartlarında tanımlanabiliyor.
- Hareket hatası düzeltmesi
- AIMovementBrain etkin olmayan varlık durumunda erken çıkacak ve komut vermeden önce hareket denetleyicilerini doğrulayacak şekilde güncellendi; sahne geçişleri sırasında olası boş referanslar önlendi.

### 11 August 2025

SFX sistemi kurulum geçişi

- Varlığa özgü SFX anahtarları, zarifçe varsayılanlara geri dönecek şekilde sonlandırıldı; tasarımcılar birim başına ayak sesi/çarpma seslerini kod değişikliği olmadan değiştirebilir.
- Dayanıklılık ve seviye içeriği
- Health bileşeninde otomatik diriltme rejenerasyonu etkinleştirildi; hasarda zamanlayıcılar sıfırlanır ve yapılandırılabilir gecikmelerden sonra sağlık geri yüklenir; uzun görevlerin akışı sürer.
    - Canın tekrar dolması sağlandı

### 14 August 2025

Dayanıklılık ve seviye içeriği

- Seviye 1 içeriği LevelManagera bağlandı; düşman üssünü yok etmek veya kahramanı kaybetmek zafer/mağlubiyet olaylarını ateşler ve bu olaylar UI ve kalıcılık sistemlerine kabarır.
- Ana menü ve seviye seçimi
- Kaydırılabilir LevelSelectUI oluşturuldu; GameManager olaylarını dinler, etkin göreve odaklanır ve seçili seviyeyi yüklemek için Oynat düğmesini sürer.

### 15 August 2025

Ana menü ve seviye seçimi

- LevelSelectButton davranışları eklendi; kilit/tamamlama görsellerini değiştirir, mevcut seçimi vurgular ve tıklandığında ilerleme tekiline çağrı yapar.
- Ana menü kolaylıkları sağlandı; LoadScene0Button zaman ölçeğini sıfırlar ve menü sahnesine döner; ön kapı navigasyon döngüsü tamamlandı.
- 5 adet artan zorlukta level oluşturuldu, oyunun demosunun sunulması adına bunlar tamamlandı ve yüklenmeye hazır hale getirildi.

### Mentorlerim’in feedback’leri

- Modellerin AI tarafından üretilmiş olması bir mobil oyun için gerekenden çok ama çok daha fazla sayıda yüze sahip olmasına sebep oldu. Bu yüzden performansın etkilenmemesi için daha optimize modellere geçilmesi gerektiği söylendi.
- Bug’ların çözülmesi ve oyunun cilalanması gerektiği söylendi.
- Kingdom Rush gibi oyuların örnek alınabileceği belirtildi

Sonuç: Boyumdan büyük bir işe kalkışmış olsam da ölçeklendirilebilir sistemler üzerine yoğunlaşmış olmak ilerde oyun için daha kolay ve hızlı sistemler geliştirebileceğim anlamına geldiği için beni bu konuda oldukça geliştirdi. Birbirinden bağımsız, büyüse de spagetti ya da level 999 jenga kulesine dönüşmeyen bir proje gerçekleştirdiğimi düşünüyorum. Ancak teknik anlamda sağlam olması bir yazılım parçasını oyun’a çevirmek için yeterli değil. Oyun Tasarımı ve oyunlarda “fun” olayının nasıl sağlanacağını öğrenmek sıradaki hedefim olacak.