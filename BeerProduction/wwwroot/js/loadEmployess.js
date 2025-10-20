
async function loadEmployees() {
    const err = document.getElementById('error');
    err.textContent = '';
    try {
        const res = await fetch('/api/employee');
        if (!res.ok) throw new Error('HTTP ' + res.status);
        const data = await res.json();

        const tbody = document.querySelector('#emp-table tbody');
        tbody.innerHTML = '';
        for (const e of data) {
            const tr = document.createElement('tr');
            tr.innerHTML =
                `<td>${e.id}</td><td>${e.name}</td><td>${e.role}</td><td>${e.hiredOn ?? ''}</td>`;
            tbody.appendChild(tr);
        }
    } catch (ex) {
        console.error(ex);
        err.textContent = 'Failed to load employees: ' + ex.message;
    }
}
document.addEventListener('DOMContentLoaded', loadEmployees);
