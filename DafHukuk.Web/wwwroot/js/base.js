/* GENEL FONKSİYONLAR */

document.addEventListener('DOMContentLoaded', function () {

    /* NAVBAR KAYDIRMA ETKİSİ */
    window.addEventListener("scroll", () => {
        const header = document.getElementById("site-header");
        if (!header) return;

        if (window.scrollY > 50) {
            header.classList.add("scrolled");
        } else {
            header.classList.remove("scrolled");
        }
    });

    /* KATEGORİ RENKLENDİRME (Case-Insensitive olarak optimize edildi) */
    function colorizeCategoryButtons() {
        document.querySelectorAll(".content-tag-button").forEach(btn => {
            // Metni küçük harfe çevirerek karşılaştırma yapılır
            let text = btn.innerText.trim().toLowerCase();

            let bgColor = "#dfb899"; // Yayınlar/Varsayılan
            let txtColor = "white";

            switch (text) {
                case "hizmetlerimiz":
                    bgColor = "#366C80";
                    break;
                case "duyurular":
                    bgColor = "#5c7341";
                    break;
                case "etkinlikler":
                    bgColor = "#78514A";
                    break;
                case "yayınlar": // Hem ç/ş/ğ/ü gibi harflerle hem de düz harflerle uyumlu olması için sadece küçük harf versiyonu kontrol edilir.
                case "yayinlar":
                    bgColor = "#dfb899";
                    break;
                default:
                    bgColor = "#3b82f6"; // Varsayılan renk
                    break;
            }

            btn.style.background = bgColor;
            btn.style.color = txtColor;
        });
    }



    /* SAYICI ANİMASYONU (Counter Animation) */
    function startCounter(target) {
        const dataTarget = parseInt(target.getAttribute("data-target"));
        // + işareti olsun mu olmasın mı diye bakmak için son karakteri kontrol et
        const hasPlus = target.innerText.trim().endsWith('+');

        let count = 0;
        const duration = 2000; // 2 saniye
        const stepTime = 10;
        const step = dataTarget / (duration / stepTime);

        const counter = setInterval(() => {
            count += step;

            if (count >= dataTarget) {
                clearInterval(counter);
                count = dataTarget; // Tam hedef değere ulaşıldığından emin ol
                // Eğer + işareti varsa, son haliyle ekle
                target.innerText = count + (hasPlus ? '+' : '');
            } else {
                // Animasyon sırasında + işareti ekleme (Sadece sayı artsın)
                target.innerText = Math.floor(count);
            }
        }, stepTime);
    }

    const counterObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            // Eğer sayaç ekranda görünüyorsa
            if (entry.isIntersecting) {
                const counterElement = entry.target;
                startCounter(counterElement);
                // Animasyon bir kere çalıştıktan sonra gözlemlemeyi durdur
                observer.unobserve(counterElement);
            }
        });
    }, {
        threshold: 0.5 // Sayaç elementinin %50'si görünür olduğunda tetikle
    });

    // Sayfadaki tüm sayaç elementlerini gözlemlemeye başla
    document.querySelectorAll(".counter-number").forEach(counter => {
        counterObserver.observe(counter);
    });

    const observer = new MutationObserver(() => colorizeCategoryButtons());
    observer.observe(document.body, { childList: true, subtree: true });
    setTimeout(colorizeCategoryButtons, 200);

    /* MOBİL MENÜ */
    const mobileMenuBtn = document.getElementById("mobile-menu-btn");
    const mobileMenu = document.getElementById("mobile-menu");
    const mobileOverlay = document.getElementById("mobile-overlay");
    const mobileMenuClose = document.getElementById("mobile-menu-close");
    const logo = document.querySelector(".logo");
    const body = document.body;

    function openMobileMenu() {
        if (!mobileMenu) return;

        mobileMenu.classList.add("open");
        mobileOverlay.classList.add("active");
        mobileMenuBtn?.classList.add("active");

        if (window.innerWidth <= 550) {
            logo.style.display = "none";
        } else {
            logo.style.filter = "blur(2px)";
        }

        body.style.overflow = "hidden";
    }

    function closeMobileMenu() {
        if (!mobileMenu) return;

        mobileMenu.classList.remove("open");
        mobileOverlay.classList.remove("active");
        mobileMenuBtn?.classList.remove("active");

        logo.style.display = "";
        logo.style.filter = "";
        body.style.overflow = "";
    }

    function toggleMobileMenu() {
        mobileMenu.classList.contains("open")
            ? closeMobileMenu()
            : openMobileMenu();
    }

    if (mobileMenuBtn && mobileMenu && mobileOverlay) {
        mobileMenuBtn.addEventListener("click", e => {
            e.stopPropagation();
            toggleMobileMenu();
        });

        mobileMenuClose?.addEventListener("click", closeMobileMenu);
        mobileOverlay.addEventListener("click", closeMobileMenu);

        mobileMenu.querySelectorAll("a").forEach(link =>
            link.addEventListener("click", closeMobileMenu)
        );
    }

    /* ARAMA SİSTEMİ */
    function performSearch(query) {
        if (!query || query.trim().length < 2) return;
        window.location.href = `/arama?q=${encodeURIComponent(query.trim())}`;
    }

    const searchTrigger = document.getElementById("search-trigger");
    const desktopSearchBar = document.getElementById("desktop-search-bar");
    const desktopSearchClose = document.getElementById("desktop-search-close");
    const desktopSearchInput = document.getElementById("desktop-search-input");

    if (searchTrigger && desktopSearchBar) {
        searchTrigger.addEventListener("click", e => {
            e.preventDefault();
            desktopSearchBar.classList.toggle("active");

            if (desktopSearchBar.classList.contains("active")) {
                setTimeout(() => desktopSearchInput.focus(), 100);
            }
        });

        desktopSearchClose?.addEventListener("click", () => {
            desktopSearchBar.classList.remove("active");
        });

        desktopSearchInput.addEventListener("keypress", e => {
            if (e.key === "Enter") performSearch(desktopSearchInput.value);
        });
    }

    /* MOBİL ARAMA */
    const mobileSearchTrigger = document.getElementById("mobile-search-trigger");
    const mobileSearchOverlay = document.getElementById("mobile-search-overlay");
    const mobileSearchClose = document.getElementById("mobile-search-close");
    const mobileSearchInput = document.getElementById("mobile-search-input");

    if (mobileSearchTrigger && mobileSearchOverlay) {
        mobileSearchTrigger.addEventListener("click", () => {
            closeMobileMenu();
            mobileSearchOverlay.classList.add("active");
            setTimeout(() => mobileSearchInput.focus(), 150);
        });

        mobileSearchClose?.addEventListener("click", () => {
            mobileSearchOverlay.classList.remove("active");
        });

        mobileSearchInput.addEventListener("keypress", e => {
            if (e.key === "Enter") performSearch(mobileSearchInput.value);
        });
    }

    /* İÇERİK FİLTRELEME (Sadece Blazor'un olmadığı statik sayfalar için geçerlidir) */
    let currentCategory = 0;
    const MAX_VISIBLE = 9;

    window.filterCategory = function (categoryId) {
        const cards = document.querySelectorAll('.content-card-wrapper');
        const noContent = document.getElementById("noContentMessage");
        const showAllButton = document.getElementById("showAllButton");

        currentCategory = categoryId;

        let visible = 0;
        let total = 0;

        cards.forEach(card => {
            const cat = parseInt(card.dataset.category);
            const match = (categoryId === 0 || cat === categoryId);

            if (match) {
                total++;
                if (visible < MAX_VISIBLE) {
                    card.style.display = "block";
                    visible++;
                } else {
                    card.style.display = "none";
                }
            } else {
                card.style.display = "none";
            }
        });

        noContent.style.display = total === 0 ? "block" : "none";
        showAllButton.style.display = total > MAX_VISIBLE ? "inline-block" : "none";

        document.querySelectorAll('#categoryTabs .nav-link')
            .forEach(x => x.classList.remove("active"));

        const activeTab = [...document.querySelectorAll('#categoryTabs .nav-link')]
            .find(x => x.getAttribute("onclick") === `filterCategory(${categoryId})`);

        activeTab?.classList.add("active");
    };

    window.showAll = function () {
        const cards = document.querySelectorAll('.content-card-wrapper');

        cards.forEach(card => {
            const cat = parseInt(card.dataset.category);
            if (currentCategory === 0 || cat === currentCategory) {
                card.style.display = "block";
            }
        });

        document.getElementById("showAllButton").style.display = "none";
    };

    setTimeout(() => window.filterCategory?.(0), 250);

    /* ALT BİLGİ (FOOTER) MOBİL AÇILIR MENÜ */
    function setupFooterToggle() {
        // data-target özniteliğine sahip tüm başlıkları seçer
        const toggleButtons = document.querySelectorAll('.footer-column-title[data-target]');

        toggleButtons.forEach(button => {
            // Tıklanacak başlık ve hedef menü (ul elementi)
            const targetId = button.getAttribute('data-target');
            const targetMenu = document.getElementById(targetId);

            // Sadece mobil boyutta çalışması için ekran genişliği kontrolü yapabiliriz
            // Ancak, CSS'deki @media query zaten görünürlüğü yönettiği için doğrudan toggle mantığı daha basittir.

            if (targetMenu) {
                button.addEventListener('click', () => {
                    const isActive = button.classList.toggle('active');

                    targetMenu.classList.toggle('open', isActive);
                });
            }
        });
    }

    setupFooterToggle();
});