/* --- Dropdown menu: hover to open, click to pin, close on outside/ESC --- */
function setupDropdown() {
    const dropdown = document.getElementById("dropdown"); // wrapper <li>
    if (!dropdown) return;

    const btn = dropdown.querySelector(".dropBtn");       // trigger <a>
    const menu = dropdown.querySelector(".dropdown-menu");

    function openDropdown() {
        dropdown.classList.add("open");
        btn.setAttribute("aria-expanded", "true");
    }
    function closeDropdown() {
        dropdown.classList.remove("open");
        btn.setAttribute("aria-expanded", "false");
    }
    function toggleDropdown(e) {
        e.preventDefault();
        e.stopPropagation();
        dropdown.classList.contains("open") ? closeDropdown() : openDropdown();
    }

    // Click to pin/unpin
    btn.addEventListener("click", toggleDropdown);

    // Close when clicking outside
    document.addEventListener("click", (e) => {
        if (!dropdown.contains(e.target)) closeDropdown();
    });

    // Close on ESC
    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape") closeDropdown();
    });

    // Optional: prevent menu click from closing immediately (useful for links)
    if (menu) {
        menu.addEventListener("click", (e) => e.stopPropagation());
    }
}

/* --- Logout Button --- */
function setupLogout() {
    const logoutBtn = document.getElementById("logoutButton");
    if (!logoutBtn) return;

    logoutBtn.addEventListener("click", (e) => {
        e.preventDefault();
        console.log("logging out");
        // TODO: add actual logout action here
    });
}

document.addEventListener("DOMContentLoaded", () => {
    setupDropdown();
    setupLogout();
});