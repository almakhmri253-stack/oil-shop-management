// Sidebar toggle
const sidebar = document.getElementById('sidebar');
const mainContent = document.getElementById('mainContent');
const sidebarToggle = document.getElementById('sidebarToggle');

// Overlay for tablet/mobile
const overlay = document.createElement('div');
overlay.id = 'sidebarOverlay';
overlay.className = 'sidebar-overlay';
document.body.appendChild(overlay);

function isTabletOrMobile() { return window.innerWidth <= 1024; }

function closeSidebar() {
    if (sidebar) sidebar.classList.remove('open');
    overlay.classList.remove('active');
}

if (sidebarToggle && sidebar) {
    sidebarToggle.addEventListener('click', () => {
        if (isTabletOrMobile()) {
            const isOpen = sidebar.classList.toggle('open');
            overlay.classList.toggle('active', isOpen);
        } else {
            sidebar.classList.toggle('collapsed');
            mainContent?.classList.toggle('expanded');
        }
    });
}

overlay.addEventListener('click', closeSidebar);

window.addEventListener('resize', () => {
    if (!isTabletOrMobile()) closeSidebar();
});

// Dark Mode
const darkToggle = document.getElementById('darkModeToggle');
const html = document.documentElement;
const savedTheme = localStorage.getItem('theme') || 'light';
html.setAttribute('data-bs-theme', savedTheme);
updateDarkIcon(savedTheme);

if (darkToggle) {
    darkToggle.addEventListener('click', () => {
        const current = html.getAttribute('data-bs-theme');
        const next = current === 'dark' ? 'light' : 'dark';
        html.setAttribute('data-bs-theme', next);
        localStorage.setItem('theme', next);
        updateDarkIcon(next);
    });
}

function updateDarkIcon(theme) {
    if (!darkToggle) return;
    const icon = darkToggle.querySelector('i');
    if (icon) icon.className = theme === 'dark' ? 'bi bi-sun-fill' : 'bi bi-moon-stars-fill';
}

// Auto-dismiss alerts
document.querySelectorAll('.alert.alert-success').forEach(el => {
    setTimeout(() => {
        const bsAlert = bootstrap.Alert.getOrCreateInstance(el);
        bsAlert?.close();
    }, 4000);
});

// Number formatting helper
function formatCurrency(amount) {
    return new Intl.NumberFormat('ar-OM', { minimumFractionDigits: 3, maximumFractionDigits: 3 }).format(amount);
}
