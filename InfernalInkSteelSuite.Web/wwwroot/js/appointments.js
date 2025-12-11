/* Infernal Ink Appointments - Logic from the Underworld */

document.addEventListener('DOMContentLoaded', function () {
    initializeOracle();
    initializeDragDrop();
    checkForClashes();
});

// --- The Oracle (Search) ---
function initializeOracle() {
    const searchTrigger = document.querySelector('[title="Consult the Oracle (Search)"]');
    if (!searchTrigger) return;

    searchTrigger.addEventListener('click', function () {
        let searchBar = document.getElementById('oracle-search-input');
        if (!searchBar) {
            createOracleInput(searchTrigger);
        } else {
            searchBar.focus();
        }
    });
}

function createOracleInput(triggerBtn) {
    const container = triggerBtn.parentElement;
    const input = document.createElement('input');
    input.id = 'oracle-search-input';
    input.type = 'text';
    input.className = 'form-control form-control-sm bg-dark text-white border-secondary ms-2';
    input.placeholder = 'Search souls...';
    input.style.width = '200px';
    input.style.animation = 'fadeIn 0.3s';

    input.addEventListener('input', function (e) {
        filterAppointments(e.target.value);
    });

    container.insertBefore(input, triggerBtn);
    input.focus();
}

function filterAppointments(query) {
    const cards = document.querySelectorAll('.soul-contract-card');
    const lowerQuery = query.toLowerCase();

    cards.forEach(card => {
        const text = card.innerText.toLowerCase();
        if (text.includes(lowerQuery)) {
            card.style.display = 'block';
        } else {
            card.style.display = 'none';
        }
    });
}

// --- Status Ritual Toggle ---
function toggleStatus(element, id) {
    // Visual toggle for now - would initiate API call
    const icon = element.querySelector('i');

    // Cycle: Pending -> Confirmed -> Completed
    if (element.classList.contains('rune-pending')) {
        element.classList.remove('rune-pending');
        element.classList.add('rune-confirmed');
        icon.className = 'bi bi-check-lg';
    } else if (element.classList.contains('rune-confirmed')) {
        element.classList.remove('rune-confirmed');
        element.classList.add('rune-completed');
        icon.className = 'bi bi-check-all';
    } else {
        // Reset or ignored, or maybe cycle back to pending?
        // element.classList.remove('rune-completed');
        // element.classList.add('rune-pending');
        // icon.className = 'bi bi-hourglass-split';
    }

    // Animate
    element.animate([
        { transform: 'scale(1)' },
        { transform: 'scale(1.2)' },
        { transform: 'scale(1)' }
    ], { duration: 300 });

    // In a real app: fetch(`/api/appointments/${id}/status`, ...);
}

// --- Clash Detection ---
function checkForClashes() {
    const cards = Array.from(document.querySelectorAll('.soul-contract-card'));
    // Simple visual clash mock (randomly flag valid conflicts if we parsed time)
    // For now, let's just highlight if any two cards look visibly overlapped in time (complex logic)
    // Or just "Simulate" a clash check on load
}

// --- Drag & Drop (Reshuffling) ---
function initializeDragDrop() {
    const container = document.querySelector('.appointments-timeline');
    if (!container) return;

    let draggedItem = null;

    container.querySelectorAll('.soul-contract-card').forEach(item => {
        item.setAttribute('draggable', true);
        item.style.cursor = 'grab';

        item.addEventListener('dragstart', function (e) {
            draggedItem = this;
            setTimeout(() => this.style.opacity = '0.5', 0);
            this.style.cursor = 'grabbing';
        });

        item.addEventListener('dragend', function (e) {
            setTimeout(() => this.style.opacity = '1', 0);
            draggedItem = null;
            this.style.cursor = 'grab';
        });

        item.addEventListener('dragover', function (e) {
            e.preventDefault();
        });

        item.addEventListener('dragenter', function (e) {
            e.preventDefault();
            this.style.borderTop = '2px solid var(--accent-color)';
        });

        item.addEventListener('dragleave', function () {
            this.style.borderTop = '';
        });

        item.addEventListener('drop', function () {
            this.style.borderTop = '';
            if (draggedItem !== this) {
                // Insert before or after based on position
                container.insertBefore(draggedItem, this);
                // Animate drop
                draggedItem.animate([
                    { transform: 'scale(1.02)' },
                    { transform: 'scale(1)' }
                ], { duration: 200 });
            }
        });
    });
}
