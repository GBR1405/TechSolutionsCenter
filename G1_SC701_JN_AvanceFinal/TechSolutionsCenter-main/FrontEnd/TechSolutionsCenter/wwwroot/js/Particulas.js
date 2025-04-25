document.addEventListener('DOMContentLoaded', function () {
    const particlesContainer = document.querySelector('.particles');
    const particleCount = 50; // Número de partículas

    // Crear partículas
    for (let i = 0; i < particleCount; i++) {
        createParticle();
    }

    function createParticle() {
        const particle = document.createElement('div');
        particle.classList.add('particle');

        // Posición horizontal aleatoria
        const randomX = Math.random() * 100;
        particle.style.setProperty('--random-x', `${randomX}vw`);

        // Tamaño aleatorio entre 2px y 6px
        const size = 2 + Math.random() * 4;
        particle.style.width = `${size}px`;
        particle.style.height = `${size}px`;

        // Opacidad aleatoria
        const opacity = 0.3 + Math.random() * 0.7;
        particle.style.opacity = opacity;

        // Duración de animación aleatoria (entre 10s y 20s)
        const duration = 10 + Math.random() * 10;
        particle.style.animationDuration = `${duration}s`;

        // Retraso inicial aleatorio
        const delay = Math.random() * -20;
        particle.style.animationDelay = `${delay}s`;

        particlesContainer.appendChild(particle);

        // Reiniciar la partícula cuando termine su animación
        particle.addEventListener('animationiteration', function () {
            this.style.left = `${Math.random() * 100}vw`;
        });
    }
});