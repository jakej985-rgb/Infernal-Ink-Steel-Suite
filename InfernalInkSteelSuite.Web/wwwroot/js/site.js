// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Infernal Interaction Controller

function showOmen(message, type = 'info') {
    // Types: 'success', 'error', 'info'
    const omenContainer = document.getElementById('omen-container');
    if (!omenContainer) {
        const container = document.createElement('div');
        container.id = 'omen-container';
        document.body.appendChild(container);
    }

    const omen = document.createElement('div');
    omen.classList.add('omen-toast', `omen-${type}`);

    let icon = 'bi-info-circle';
    if (type === 'success') icon = 'bi-check-circle';
    if (type === 'error') icon = 'bi-exclamation-triangle';

    omen.innerHTML = `
        <div class="omen-icon"><i class="bi ${icon}"></i></div>
        <div class="omen-body">${message}</div>
        <div class="omen-close"><i class="bi bi-x"></i></div>
    `;

    document.getElementById('omen-container').appendChild(omen);

    // Animation In
    setTimeout(() => omen.classList.add('show'), 10);

    // Auto Dismiss
    setTimeout(() => {
        omen.classList.remove('show');
        setTimeout(() => omen.remove(), 300);
    }, 5000);

    // Manual Dismiss
    omen.querySelector('.omen-close').addEventListener('click', () => {
        omen.classList.remove('show');
        setTimeout(() => omen.remove(), 300);

        // --- Soul Binding (Accent Color) ---
        function bindSoul(color) {
            document.documentElement.style.setProperty('--accent-color', color);
            document.documentElement.style.setProperty('--primary-color', color); // Fallback for Bootstrap primary if not var'd
            localStorage.setItem('soul-color', color);

            if (window.showOmen) showOmen('Soul bound successfully.', 'success');
        }

        // Load Soul on Init
        (function () {
            const savedSoul = localStorage.getItem('soul-color');
            if (savedSoul) {
                document.documentElement.style.setProperty('--accent-color', savedSoul);
                document.documentElement.style.setProperty('--primary-color', savedSoul);
            }
        })();
