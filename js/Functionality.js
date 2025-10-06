/*---Dropdown menu Toggle ----*/
function setupDropdown() {
    const dropdown = document.getElementById("dropdown"); // no dot, and it's an id now
    if (!dropdown) return;

    const btn = dropdown.querySelector(".dropBtn");

    function openDropdown() {
        dropdown.classList.add("open");
        btn.setAttribute("aria-expanded", "true");
    }
    function closeDropdown() {
        dropdown.classList.remove("open");
        btn.setAttribute("aria-expanded", "false");
    }

    function toggleDropdown(e) {
        e.stopPropagation();
        if (dropdown.classList.contains("open")) closeDropdown();
        else openDropdown();
    }
    btn.addEventListener("click", toggleDropdown);
}

/* --- Logout Button --- */
function setupLogout() {
    const logoutBtn = document.getElementById("logoutButton");
    if (!logoutBtn) return;

    logoutBtn.addEventListener("click", () => {
        console.log("logging out");
    });
}


document.addEventListener("DOMContentLoaded", () => {
    setupDropdown();
    setupLogout();

});

// machine image
// machine name
// machine status
// machine serial number
// machine variables:
// temperature
// Batch ID
// products per minute
// Defect products
// acceptable products


