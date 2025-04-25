document.addEventListener('DOMContentLoaded', function () {
    // Selección de elementos
    const tabButtons = document.querySelectorAll('.tab-button');
    const tabContents = document.querySelectorAll('.tab-content');
    const tabIndicator = document.querySelector('.tab-indicator');

    // Función para activar una pestaña
    function activateTab(tabId) {
        // Actualizar botones
        tabButtons.forEach(btn => {
            btn.classList.remove('active');
            if (btn.getAttribute('data-tab') === tabId) {
                btn.classList.add('active');
            }
        });

        // Actualizar contenidos con animación
        tabContents.forEach(content => {
            if (content.id === tabId) {
                content.classList.add('active');
                content.style.opacity = '0';
                content.style.transform = 'translateY(10px)';
                setTimeout(() => {
                    content.style.opacity = '1';
                    content.style.transform = 'translateY(0)';
                }, 50);
            } else {
                content.classList.remove('active');
            }
        });

        // Mover indicador con animación
        const activeBtn = document.querySelector(`.tab-button[data-tab="${tabId}"]`);
        if (activeBtn) {
            const btnRect = activeBtn.getBoundingClientRect();
            const containerRect = activeBtn.parentElement.getBoundingClientRect();

            tabIndicator.style.transition = 'all 0.3s ease-out';
            tabIndicator.style.width = `${btnRect.width}px`;
            tabIndicator.style.left = `${btnRect.left - containerRect.left}px`;
        }
    }

    // Event listeners para las pestañas
    tabButtons.forEach(button => {
        button.addEventListener('click', function () {
            const tabId = this.getAttribute('data-tab');
            activateTab(tabId);
        });
    });

    // Activar la primera pestaña al cargar
    activateTab('pendientes');

    // Manejo de expansión de tarjetas
    const caseCards = document.querySelectorAll('.case-card');
    caseCards.forEach(card => {
        const header = card.querySelector('.case-header');
        const body = card.querySelector('.case-body');
        const expandIcon = card.querySelector('.expand-icon');

        header.addEventListener('click', function () {
            // Cerrar otras tarjetas abiertas
            caseCards.forEach(otherCard => {
                if (otherCard !== card && otherCard.classList.contains('expanded')) {
                    otherCard.classList.remove('expanded');
                    animateCardClose(otherCard);
                }
            });

            // Alternar estado de la tarjeta actual
            if (card.classList.contains('expanded')) {
                card.classList.remove('expanded');
                animateCardClose(card);
            } else {
                card.classList.add('expanded');
                animateCardOpen(card);
            }
        });
    });

    // Animación para abrir tarjeta
    function animateCardOpen(card) {
        const body = card.querySelector('.case-body');
        const content = card.querySelector('.case-content');
        const expandIcon = card.querySelector('.expand-icon');

        // Preparar animación
        body.style.maxHeight = '0';
        content.style.opacity = '0';
        content.style.transform = 'translateY(20px)';
        expandIcon.style.transform = 'rotate(0deg)';

        // Forzar reflow
        void body.offsetHeight;

        // Ejecutar animación
        body.style.maxHeight = `${body.scrollHeight}px`;
        setTimeout(() => {
            content.style.opacity = '1';
            content.style.transform = 'translateY(0)';
            expandIcon.style.transform = 'rotate(180deg)';
        }, 10);
    }

    // Animación para cerrar tarjeta
    function animateCardClose(card) {
        const body = card.querySelector('.case-body');
        const content = card.querySelector('.case-content');
        const expandIcon = card.querySelector('.expand-icon');

        content.style.opacity = '0';
        content.style.transform = 'translateY(20px)';
        expandIcon.style.transform = 'rotate(0deg)';

        setTimeout(() => {
            body.style.maxHeight = '0';
        }, 200);
    }

    // Animación inicial de las tarjetas
    animateCards();

    function animateCards() {
        const cards = document.querySelectorAll('.case-card');
        cards.forEach((card, index) => {
            card.style.opacity = '0';
            card.style.transform = 'translateY(20px)';
            setTimeout(() => {
                card.style.transition = 'all 0.3s ease-out';
                card.style.opacity = '1';
                card.style.transform = 'translateY(0)';
            }, index * 100);
        });
    }
});