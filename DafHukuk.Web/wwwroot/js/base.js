/**
 * BASE.JS - Global Navbar & Footer Functionality
 * Bu dosya tüm sayfalarda yüklenmelidir
 */

document.addEventListener('DOMContentLoaded', function () {

    /* =========================================================
       NAVBAR SCROLL EFFECT
    ========================================================= */
    window.addEventListener("scroll", () => {
        const header = document.getElementById("site-header");
        if (!header) return;

        if (window.scrollY > 50) {
            header.classList.add("scrolled");
        } else {
            header.classList.remove("scrolled");
        }
    });

    /* =========================================================
       MOBILE MENU
    ========================================================= */
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
        mobileMenuBtn.classList.add("active");

        // Logo blur efekti
        if (window.innerWidth <= 550 && logo) {
            logo.style.display = "none";
        } else if (logo) {
            logo.style.filter = "blur(2px)";
        }

        body.style.overflow = "hidden";
    }

    function closeMobileMenu() {
        if (!mobileMenu) return;
        mobileMenu.classList.remove("open");
        mobileOverlay.classList.remove("active");
        mobileMenuBtn.classList.remove("active");

        // Logo'yu normale döndür
        if (logo) {
            logo.style.display = "";
            logo.style.filter = "";
        }

        body.style.overflow = "";
    }

    function toggleMobileMenu() {
        if (!mobileMenu) return;
        const isOpen = mobileMenu.classList.contains("open");
        isOpen ? closeMobileMenu() : openMobileMenu();
    }

    // Mobile Menu Event Listeners
    if (mobileMenuBtn && mobileMenu && mobileOverlay) {
        mobileMenuBtn.addEventListener("click", (e) => {
            e.stopPropagation();
            toggleMobileMenu();
        });

        if (mobileMenuClose) {
            mobileMenuClose.addEventListener("click", (e) => {
                e.stopPropagation();
                closeMobileMenu();
            });
        }

        mobileOverlay.addEventListener("click", closeMobileMenu);

        // Menü linklerine týklandýðýnda menüyü kapat
        const mobileLinks = mobileMenu.querySelectorAll("a");
        mobileLinks.forEach(link => {
            link.addEventListener("click", closeMobileMenu);
        });
    }

    /* =========================================================
       DESKTOP SEARCH
    ========================================================= */
    const searchTrigger = document.getElementById("search-trigger");
    const desktopSearchBar = document.getElementById("desktop-search-bar");
    const desktopSearchClose = document.getElementById("desktop-search-close");
    const desktopSearchInput = document.getElementById("desktop-search-input");

    if (searchTrigger && desktopSearchBar) {
        searchTrigger.addEventListener("click", (e) => {
            e.preventDefault();
            e.stopPropagation();

            desktopSearchBar.classList.toggle("active");

            if (desktopSearchBar.classList.contains("active")) {
                setTimeout(() => {
                    if (desktopSearchInput) {
                        desktopSearchInput.focus();
                    }
                }, 100);
            }
        });
    }

    if (desktopSearchClose) {
        desktopSearchClose.addEventListener("click", (e) => {
            e.preventDefault();
            desktopSearchBar.classList.remove("active");
            if (desktopSearchInput) {
                desktopSearchInput.value = "";
            }
        });
    }

    // Desktop Search - Enter ile arama
    if (desktopSearchInput) {
        desktopSearchInput.addEventListener("keypress", (e) => {
            if (e.key === "Enter") {
                const searchValue = desktopSearchInput.value.trim();
                if (searchValue) {
                    performSearch(searchValue, 'desktop');
                }
            }
        });
    }

    /* =========================================================
       MOBILE SEARCH
    ========================================================= */
    const mobileSearchTrigger = document.getElementById("mobile-search-trigger");
    const mobileSearchOverlay = document.getElementById("mobile-search-overlay");
    const mobileSearchClose = document.getElementById("mobile-search-close");
    const mobileSearchInput = document.getElementById("mobile-search-input");
    const mobileMenuBtnElement = document.getElementById("mobile-menu-btn");

    if (mobileSearchTrigger && mobileSearchOverlay) {
        mobileSearchTrigger.addEventListener("click", (e) => {
            e.preventDefault();
            e.stopPropagation();

            closeMobileMenu(); // Önce mobil menüyü kapat

            setTimeout(() => {
                mobileSearchOverlay.classList.add("active");
                body.style.overflow = "hidden";
                setTimeout(() => {
                    if (mobileSearchInput) {
                        mobileSearchInput.focus();
                        mobileMenuBtnElement.style.display = "none";
                    }
                }, 100);
            }, 400);
        });
    }

    if (mobileSearchClose) {
        mobileSearchClose.addEventListener("click", (e) => {
            e.preventDefault();
            mobileSearchOverlay.classList.remove("active");
            body.style.overflow = "";
            if (mobileSearchInput) {
                mobileSearchInput.value = "";
                mobileMenuBtnElement.style.display = "";
            }
        });
    }

    // Mobile Search - Enter ile arama
    if (mobileSearchInput) {
        mobileSearchInput.addEventListener("keypress", (e) => {
            if (e.key === "Enter") {
                const searchValue = mobileSearchInput.value.trim();
                if (searchValue) {
                    performSearch(searchValue, 'mobile');
                }
            }
        });
    }

    /* =========================================================
       SEARCH FUNCTION (Özelleþtirilebilir)
    ========================================================= */
    function performSearch(query, source) {
        console.log(`Arama yapýlýyor (${source}):`, query);

        // TODO: Kendi arama mantýðýnýzý buraya ekleyin
        // Örnek: window.location.href = `/search?q=${encodeURIComponent(query)}`;

        // Geçici olarak console'a yazdýr
        alert(`Arama: "${query}" (Kaynak: ${source})`);
    }

    /* =========================================================
       ESC KEY HANDLER
    ========================================================= */
    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape") {
            // Desktop search kapatma
            if (desktopSearchBar && desktopSearchBar.classList.contains("active")) {
                desktopSearchBar.classList.remove("active");
                if (desktopSearchInput) {
                    desktopSearchInput.value = "";
                }
            }

            // Mobile search kapatma
            if (mobileSearchOverlay && mobileSearchOverlay.classList.contains("active")) {
                mobileSearchOverlay.classList.remove("active");
                body.style.overflow = "";
                if (mobileSearchInput) {
                    mobileSearchInput.value = "";
                }
            }

            // Mobile menu kapatma
            if (mobileMenu && mobileMenu.classList.contains("open")) {
                closeMobileMenu();
            }
        }
    });

    /* =========================================================
       WINDOW RESIZE HANDLER
    ========================================================= */
    let resizeTimer;
    window.addEventListener("resize", () => {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(() => {
            // Büyük ekrana geçildiðinde mobil menüyü kapat
            if (window.innerWidth > 950 && mobileMenu && mobileMenu.classList.contains("open")) {
                closeMobileMenu();
            }
        }, 250);
    });

    /* =========================================================
       FOOTER ACCORDION (Mobile Only)
    ========================================================= */
    const footerTitles = document.querySelectorAll('.footer-column-title[data-target]');

    footerTitles.forEach(title => {
        title.addEventListener('click', function (e) {
            e.preventDefault();

            // Sadece mobilde çalýþ
            if (window.innerWidth <= 950) {
                const targetId = this.getAttribute('data-target');
                const targetElement = document.getElementById(targetId);

                if (targetElement) {
                    // Toggle active ve open class'larý
                    this.classList.toggle("active");
                    targetElement.classList.toggle("open");
                }
            }
        });
    });

}); // DOMContentLoaded end