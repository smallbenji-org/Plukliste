document.addEventListener('DOMContentLoaded', function () {
    const burgerMenu = document.querySelector('.burger-menu');
    const navContainer = document.querySelector('.nav-container');

    if (burgerMenu && navContainer) {
        burgerMenu.addEventListener('click', function () {
            const isVisible = navContainer.classList.toggle('show');

            if (isVisible) {
                burgerMenu.innerHTML = '&times;';
                burgerMenu.style.fontSize = '3rem';
            } else {
                burgerMenu.innerHTML = '&#9776;';
                burgerMenu.style.fontSize = '2.5rem';
            }
        });
    }
});