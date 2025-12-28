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

    /* KATEGORİ RENKLENDİRME (Case-Insensitive + MutationObserver) */
    function colorizeCategoryButtons() {
        document.querySelectorAll(".content-tag-button").forEach(btn => {
            let text = btn.innerText.trim().toLowerCase();

            let bgColor = "#dfb899";
            let txtColor = "white";

            switch (text) {
                case "hizmetlerimiz":
                case "services":
                case "our services":
                case "خدماتنا":
                case "خدمات":
                    bgColor = "#366C80";
                    break;
                case "duyurular":
                case "announcements":
                case "الإعلانات":
                case "إعلانات":
                    bgColor = "#5c7341";
                    break;
                case "etkinlikler":
                case "events":
                case "الفعاليات":
                case "فعاليات":
                    bgColor = "#78514A";
                    break;
                case "yayınlar":
                case "yayinlar":
                case "publications":
                case "المنشورات":
                case "منشورات":
                    bgColor = "#dfb899";
                    break;
                default:
                    bgColor = "#3b82f6";
                    break;
            }

            btn.style.background = bgColor;
            btn.style.color = txtColor;
        });
    }

    /* MutationObserver - DOM değişikliklerini izle */
    const observer = new MutationObserver(() => {
        colorizeCategoryButtons();
    });

    observer.observe(document.body, {
        childList: true,
        subtree: true
    });

    /* İlk yükleme + gecikme ile tekrar kontrol */
    colorizeCategoryButtons();
    setTimeout(colorizeCategoryButtons, 200);
    setTimeout(colorizeCategoryButtons, 500);
    setTimeout(colorizeCategoryButtons, 1000);


    /* SAYICI ANİMASYONU (Counter Animation) */
    function startCounter(target) {
        const dataTarget = parseInt(target.getAttribute("data-target"));
        const hasPlus = target.innerText.trim().endsWith('+');

        let count = 0;
        const duration = 2000;
        const stepTime = 10;
        const step = dataTarget / (duration / stepTime);

        const counter = setInterval(() => {
            count += step;

            if (count >= dataTarget) {
                clearInterval(counter);
                count = dataTarget;
                target.innerText = count + (hasPlus ? '+' : '');
            } else {
                target.innerText = Math.floor(count);
            }
        }, stepTime);
    }

    const counterObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const counterElement = entry.target;
                startCounter(counterElement);
                observer.unobserve(counterElement);
            }
        });
    }, {
        threshold: 0.5
    });

    document.querySelectorAll(".counter-number").forEach(counter => {
        counterObserver.observe(counter);
    });

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

        const path = window.location.pathname.toLowerCase();
        let searchUrl = '/arama';

        if (path.startsWith('/en/') || path === '/en') {
            searchUrl = '/en/search';
        } else if (path.startsWith('/ar/') || path === '/ar') {
            searchUrl = '/ar/search';
        }

        window.location.href = `${searchUrl}?q=${encodeURIComponent(query.trim())}`;
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


    let currentCategory = 0;
    const MAX_VISIBLE = 9;

    window.filterCategory = function (categoryId) {
        const cards = document.querySelectorAll('.content-card-wrapper');
        const noContent = document.getElementById("noContentMessage");
        const showAllButton = document.getElementById("showAllButton");

        if (!noContent || !showAllButton || cards.length === 0) {
            return;
        }

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

        /* Kategori değişikliğinde renklendirmeyi tetikle */
        setTimeout(colorizeCategoryButtons, 100);
    };

    window.showAll = function () {
        const cards = document.querySelectorAll('.content-card-wrapper');
        const showAllButton = document.getElementById("showAllButton");

        if (!showAllButton || cards.length === 0) {
            return;
        }

        cards.forEach(card => {
            const cat = parseInt(card.dataset.category);
            if (currentCategory === 0 || cat === currentCategory) {
                card.style.display = "block";
            }
        });

        document.getElementById("showAllButton").style.display = "none";

        /* Tümünü göster'de renklendirmeyi tetikle */
        setTimeout(colorizeCategoryButtons, 100);
    };

    setTimeout(() => window.filterCategory?.(0), 250);


    function setupFooterToggle() {
        const toggleButtons = document.querySelectorAll('.footer-column-title[data-target]');

        toggleButtons.forEach(button => {
            const targetId = button.getAttribute('data-target');
            const targetMenu = document.getElementById(targetId);

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

/* =========================================================
    ADMİN MOBİL MENÜ
========================================================= */

document.addEventListener('DOMContentLoaded', function () {
    const adminMobileMenuBtn = document.getElementById("admin-mobile-menu-btn");
    const adminSidebar = document.getElementById("admin-sidebar");
    const adminMobileOverlay = document.getElementById("admin-mobile-overlay");
    const body = document.body;

    if (!adminMobileMenuBtn || !adminSidebar || !adminMobileOverlay) {
        return;
    }

    function openAdminMenu() {
        adminSidebar.classList.add("open");
        adminMobileOverlay.classList.add("active");
        adminMobileMenuBtn.classList.add("active");
        body.style.overflow = "hidden";
    }

    function closeAdminMenu() {
        adminSidebar.classList.remove("open");
        adminMobileOverlay.classList.remove("active");
        adminMobileMenuBtn.classList.remove("active");
        body.style.overflow = "";
    }

    function toggleAdminMenu() {
        adminSidebar.classList.contains("open")
            ? closeAdminMenu()
            : openAdminMenu();
    }

    adminMobileMenuBtn.addEventListener("click", (e) => {
        e.stopPropagation();
        toggleAdminMenu();
    });

    adminMobileOverlay.addEventListener("click", closeAdminMenu);

    adminSidebar.querySelectorAll("a, button").forEach(link => {
        link.addEventListener("click", (e) => {
            if (link.classList.contains("admin-lang-link")) {
                return;
            }
            closeAdminMenu();
        });
    });

    window.addEventListener("resize", () => {
        if (window.innerWidth > 767) {
            closeAdminMenu();
        }
    });
});

/* =========================================================
    DİL DEĞİŞTİRME YARDIMCI FONKSİYONU
========================================================= */

(function () {
    const path = window.location.pathname.toLowerCase();
    let detectedLang = null;

    if (path.startsWith('/en/') || path === '/en') {
        detectedLang = 'en';
    } else if (path.startsWith('/ar/') || path === '/ar') {
        detectedLang = 'ar';
    } else if (path.startsWith('/tr/') || path === '/tr') {
        detectedLang = 'tr';
    }

    const currentCookie = document.cookie
        .split('; ')
        .find(row => row.startsWith('user_language='))
        ?.split('=')[1];

    if (detectedLang && currentCookie !== detectedLang) {
        document.cookie = `user_language=${detectedLang}; path=/; max-age=31536000; SameSite=Lax`;
    }
})();

window.goToTurkish = function () {
    let currentPath = window.location.pathname;

    currentPath = currentPath.replace(/^\/(en|ar)\//, '/').replace(/^\/(en|ar)$/, '/');

    document.cookie = 'user_language=tr; path=/; max-age=31536000; SameSite=Lax';

    window.location.href = currentPath;
};

/* =========================================================
   BLAZOR IMAGE UPLOAD - DRAG & DROP
========================================================= */

window.blazorImageUpload = {
    dropZoneHandlers: {},

    initDragDrop: function (dropZoneId, inputFileId) {
        const dropZone = document.getElementById(dropZoneId);
        const inputFile = document.getElementById(inputFileId);

        if (!dropZone || !inputFile) {
            console.warn('Drop zone veya input file bulunamadı:', dropZoneId, inputFileId);
            return;
        }

        const handlers = {
            dragenter: (e) => {
                e.preventDefault();
                e.stopPropagation();
                dropZone.classList.add('drag-over');
            },
            dragover: (e) => {
                e.preventDefault();
                e.stopPropagation();
            },
            dragleave: (e) => {
                e.preventDefault();
                e.stopPropagation();
                if (e.target === dropZone) {
                    dropZone.classList.remove('drag-over');
                }
            },
            drop: (e) => {
                e.preventDefault();
                e.stopPropagation();
                dropZone.classList.remove('drag-over');

                const files = e.dataTransfer?.files;
                if (files && files.length > 0) {
                    const dataTransfer = new DataTransfer();
                    dataTransfer.items.add(files[0]);
                    inputFile.files = dataTransfer.files;

                    const event = new Event('change', { bubbles: true });
                    inputFile.dispatchEvent(event);
                }
            },
            click: () => {
                inputFile.click();
            }
        };

        dropZone.addEventListener('dragenter', handlers.dragenter);
        dropZone.addEventListener('dragover', handlers.dragover);
        dropZone.addEventListener('dragleave', handlers.dragleave);
        dropZone.addEventListener('drop', handlers.drop);
        dropZone.addEventListener('click', handlers.click);

        this.dropZoneHandlers[dropZoneId] = { dropZone, handlers };
    },

    disposeDragDrop: function (dropZoneId) {
        const data = this.dropZoneHandlers[dropZoneId];
        if (!data) return;

        const { dropZone, handlers } = data;

        dropZone.removeEventListener('dragenter', handlers.dragenter);
        dropZone.removeEventListener('dragover', handlers.dragover);
        dropZone.removeEventListener('dragleave', handlers.dragleave);
        dropZone.removeEventListener('drop', handlers.drop);
        dropZone.removeEventListener('click', handlers.click);

        delete this.dropZoneHandlers[dropZoneId];
    }
};