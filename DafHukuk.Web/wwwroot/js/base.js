/* GENEL FONKSÝYONLAR */

document.addEventListener('DOMContentLoaded', function () {

    /* NAVBAR KAYDIRMA ETKÝSÝ */
    window.addEventListener("scroll", () => {
        const header = document.getElementById("site-header");
        if (!header) return;

        if (window.scrollY > 50) {
            header.classList.add("scrolled");
        } else {
            header.classList.remove("scrolled");
        }
    });

    /* KATEGORÝ RENKLENDÝRME */

    function colorizeCategoryButtons() {
        document.querySelectorAll(".content-tag-button").forEach(btn => {
            let text = btn.innerText.trim();

            let bgColor = "#dfb899";
            let txtColor = "white";

            if (text === "Hizmetlerimiz") {
                bgColor = "#366C80";
            }
            else if (text === "Duyurular") {
                bgColor = "#5c7341";
            }
            else if (text === "Etkinlikler") {
                bgColor = "#78514A";
            }

            btn.style.background = bgColor;
            btn.style.color = txtColor;
            btn.style.borderColor = bgColor;

        });
    }

    const observer = new MutationObserver(() => {
        colorizeCategoryButtons();
    });

    observer.observe(document.body, { childList: true, subtree: true });
    setTimeout(colorizeCategoryButtons, 200);

    /* MOBÝL MENÜ */
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

        const mobileLinks = mobileMenu.querySelectorAll("a");
        mobileLinks.forEach(link => {
            link.addEventListener("click", closeMobileMenu);
        });
    }

    /* MASAÜSTÜ ARAMA */
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

    /* MOBÝL ARAMA */
    const mobileSearchTrigger = document.getElementById("mobile-search-trigger");
    const mobileSearchOverlay = document.getElementById("mobile-search-overlay");
    const mobileSearchClose = document.getElementById("mobile-search-close");
    const mobileSearchInput = document.getElementById("mobile-search-input");
    const mobileMenuBtnElement = document.getElementById("mobile-menu-btn");

    if (mobileSearchTrigger && mobileSearchOverlay) {
        mobileSearchTrigger.addEventListener("click", (e) => {
            e.preventDefault();
            e.stopPropagation();

            closeMobileMenu();

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

    /* ARAMA FONKSÝYONU */
    function performSearch(query, source) {
        console.log(`Arama yapýlýyor (${source}):`, query);

        alert(`Arama: "${query}" (Kaynak: ${source})`);
    }

    /* ESC TUÞU ÝÞLEMCÝSÝ */
    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape") {
            if (desktopSearchBar && desktopSearchBar.classList.contains("active")) {
                desktopSearchBar.classList.remove("active");
                if (desktopSearchInput) {
                    desktopSearchInput.value = "";
                }
            }

            if (mobileSearchOverlay && mobileSearchOverlay.classList.contains("active")) {
                mobileSearchOverlay.classList.remove("active");
                body.style.overflow = "";
                if (mobileSearchInput) {
                    mobileSearchInput.value = "";
                }
            }

            if (mobileMenu && mobileMenu.classList.contains("open")) {
                closeMobileMenu();
            }
        }
    });

    /* PENCERE BOYUTLANDIRMA ÝÞLEMCÝSÝ */
    let resizeTimer;
    window.addEventListener("resize", () => {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(() => {
            if (window.innerWidth > 950 && mobileMenu && mobileMenu.classList.contains("open")) {
                closeMobileMenu();
            }
        }, 250);
    });

    /* ALT BÝLGÝ AKORDÝYON */
    const footerTitles = document.querySelectorAll('.footer-column-title[data-target]');

    footerTitles.forEach(title => {
        title.addEventListener('click', function (e) {
            e.preventDefault();

            if (window.innerWidth <= 950) {
                const targetId = this.getAttribute('data-target');
                const targetElement = document.getElementById(targetId);

                if (targetElement) {
                    this.classList.toggle("active");
                    targetElement.classList.toggle("open");
                }
            }
        });
    });

});

/* ÝÇERÝK ÞABLONU */
const MAX_VISIBLE = 6;
let currentCategory = 0;


/* KATEGORÝ FÝLTRELEME */
function filterCategory(categoryId) {
    currentCategory = categoryId;

    const cards = document.querySelectorAll('.content-card-wrapper');
    const showAllButton = document.getElementById("showAllButton");
    const noContentMessage = document.getElementById("noContentMessage");

    let visibleCount = 0;
    let categoryItemCount = 0;

    cards.forEach(card => {
        const cat = parseInt(card.getAttribute("data-category"));
        const isMatch = (categoryId === 0 || cat === categoryId);

        if (isMatch) {
            categoryItemCount++;

            if (visibleCount < MAX_VISIBLE) {
                card.style.display = "block";
            } else {
                card.style.display = "none";
            }

            visibleCount++;
        } else {
            card.style.display = "none";
        }
    });

    if (categoryItemCount === 0) {
        noContentMessage.style.display = "block";
        showAllButton.style.display = "none";
    } else {
        noContentMessage.style.display = "none";
        showAllButton.style.display =
            categoryItemCount > MAX_VISIBLE ? "inline-block" : "none";
    }

    document.querySelectorAll('#categoryTabs .nav-link').forEach(el => {
        el.classList.remove("active");
    });

    const activeTab = [...document.querySelectorAll('#categoryTabs .nav-link')]
        .find(x => x.getAttribute("onclick") === `filterCategory(${categoryId})`);

    if (activeTab) activeTab.classList.add("active");
}


/* TÜMÜNÜ GÖSTER */
function showAll() {
    const cards = document.querySelectorAll('.content-card-wrapper');

    cards.forEach(card => {
        const cat = parseInt(card.getAttribute("data-category"));
        const isMatch = (currentCategory === 0 || cat === currentCategory);

        if (isMatch) {
            card.style.display = "block";
        }
    });

    document.getElementById("showAllButton").style.display = "none";
}


/* YÜKLEMEDE FÝLTRE UYGULA */
document.addEventListener("DOMContentLoaded", () => {
    filterCategory(0);
});

/* SAYFA ÖNBELLEK ÝÞLEMCÝSÝ */
window.addEventListener("pageshow", (event) => {
    if (event.persisted) {
        filterCategory(currentCategory || 0);
    }
});