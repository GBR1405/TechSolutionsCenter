
document.addEventListener('DOMContentLoaded', function () {
    
    const links = document.querySelectorAll('a:not([href^="#"]):not([target="_blank"]):not([href^="javascript:"]):not([href*="mailto:"])');

    links.forEach(link => {
        link.addEventListener('click', function (e) {
            
            if (this.href && !this.href.includes('javascript:') &&
                this.hostname === window.location.hostname) {
                e.preventDefault();
                document.body.classList.add('body-fade-out');

                
                setTimeout(() => {
                    window.location.href = this.href;
                }, 200);
            }
        });
    });
});

document.querySelectorAll('form').forEach(form => {
    form.addEventListener('submit', function (e) {
        if (!this.getAttribute('data-ajax')) {
            e.preventDefault();
            document.body.classList.add('body-fade-out');

            setTimeout(() => {
                this.submit();
            }, 200);
        }
    });
});