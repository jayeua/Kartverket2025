document.addEventListener('DOMContentLoaded', function () {
    const slides = document.querySelectorAll('.bg-slide');
    let current = 0;
    let timer;

    function showSlide(idx) {
        slides.forEach((slide, i) => {
            slide.classList.toggle('active', i === idx);
        });
        current = idx;
    }

    function changeBg(dir) {
        let next = (current + dir + slides.length) % slides.length;
        showSlide(next);
        resetTimer();
    }

    function nextSlide() {
        changeBg(1);
    }

    function resetTimer() {
        clearInterval(timer);
        timer = setInterval(nextSlide, 7000);
    }

    // Start automatic slideshow
    timer = setInterval(nextSlide, 7000);

    document.querySelector('.bg-arrow.left').addEventListener('click', function () { changeBg(-1); });
    document.querySelector('.bg-arrow.right').addEventListener('click', function () { changeBg(1); });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'ArrowLeft') changeBg(-1);
        if (e.key === 'ArrowRight') changeBg(1);
    });
});