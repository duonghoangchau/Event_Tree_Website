document.addEventListener("DOMContentLoaded", function () {
    var currentUrl = window.location.pathname;
    if (currentUrl === "/") {
        currentUrl = "/trang-chu";
    }
    var navLinks = document.querySelectorAll(".navbar-nav a");
    navLinks.forEach(function (link) {
        if (link.getAttribute("href") === currentUrl) {
            link.parentNode.classList.add("active");
        }
    });
});