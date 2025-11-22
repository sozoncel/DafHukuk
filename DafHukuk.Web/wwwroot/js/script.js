// Navbar Kaydýrma Efekti
window.addEventListener("scroll", () => {
    const header = document.getElementById("site-header");
    if (!header) return;

    if (window.scrollY > 50) {
        header.classList.add("scrolled");
    } else {
        header.classList.remove("scrolled");
    }
});

// Mobile Menü Aç/Kapa
const mobileMenuBtn = document.getElementById("mobile-menu-btn");
const mobileMenu = document.getElementById("mobile-menu");
const mobileOverlay = document.getElementById("mobile-overlay");
const mobileMenuClose = document.getElementById("mobile-menu-close");
const body = document.body;

if (mobileMenuBtn && mobileMenu && mobileOverlay) {
    // Menüyü aç/kapa
    mobileMenuBtn.addEventListener("click", (e) => {
        e.stopPropagation();
        toggleMobileMenu();
    });

    // Close button'a týklandýðýnda menüyü kapat
    if (mobileMenuClose) {
        mobileMenuClose.addEventListener("click", (e) => {
            e.stopPropagation();
            closeMobileMenu();
        });
    }

    mobileOverlay.addEventListener("click", () => {
        closeMobileMenu();
    });

    const mobileLinks = mobileMenu.querySelectorAll("a");
    mobileLinks.forEach(link => {
        link.addEventListener("click", () => {
            closeMobileMenu();
        });
    });
}

function toggleMobileMenu() {
    const isOpen = mobileMenu.classList.contains("open");

    if (isOpen) {
        closeMobileMenu();
    } else {
        openMobileMenu();
    }
}

function openMobileMenu() {
    mobileMenu.classList.add("open");
    mobileOverlay.classList.add("active");
    mobileMenuBtn.classList.add("active");
    body.style.overflow = "hidden"; 
}

function closeMobileMenu() {
    mobileMenu.classList.remove("open");
    mobileOverlay.classList.remove("active");
    mobileMenuBtn.classList.remove("active");
    body.style.overflow = ""; 
}

// ESC tuþu ile menüyü kapat
document.addEventListener("keydown", (e) => {
    if (e.key === "Escape" && mobileMenu.classList.contains("open")) {
        closeMobileMenu();
    }
});

let resizeTimer;
window.addEventListener("resize", () => {
    clearTimeout(resizeTimer);
    resizeTimer = setTimeout(() => {
        if (window.innerWidth > 950 && mobileMenu.classList.contains("open")) {
            closeMobileMenu();
        }
    }, 250);
});

/* =========================================================
   FOOTER ACCORDION (BLAZOR UYUMLU - data-target LOGÝÐÝ)
========================================================= */

document.addEventListener('click', function (e) {

    const title = e.target.closest('.footer-column-title');

    if (title && window.innerWidth <= 950 && title.hasAttribute('data-target')) {

        const targetId = title.getAttribute('data-target');
        const nextElement = document.getElementById(targetId);

        if (nextElement && nextElement.classList.contains("footer-links")) {
            title.classList.toggle("active"); 
            nextElement.classList.toggle("open"); 
        }
    }
});