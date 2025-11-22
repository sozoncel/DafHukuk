// Navbar Kaydýrma Efekti
window.addEventListener("scroll", () => {
    const header = document.getElementById("site-header");
    if (!header) return;

    if (window.scrollY > 50) header.classList.add("scrolled");
    else header.classList.remove("scrolled");
});
