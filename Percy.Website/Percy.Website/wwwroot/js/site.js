
/* ==========================================
   Percy.Website
   Global JavaScript
   ========================================== */

document.addEventListener("DOMContentLoaded", () => {

    initializeCardCollections();

});

/* ==========================================
   Reusable Card Collection Navigation
   ========================================== */

function initializeCardCollections() {

    const collections = document.querySelectorAll(
        ".percy-card-collection"
    );

    collections.forEach(collection => {

        const track = collection.querySelector(
            ".percy-card-collection-track"
        );

        const previousButton = collection.querySelector(
            ".percy-card-nav-prev"
        );

        const nextButton = collection.querySelector(
            ".percy-card-nav-next"
        );

        if (!track) {
            return;
        }

        const getScrollDistance = () => {

            const card = track.querySelector(
                ".percy-project-card"
            );

            if (!card) {
                return track.clientWidth;
            }

            const styles = window.getComputedStyle(track);
            const gap = parseFloat(styles.columnGap) || 0;

            return card.getBoundingClientRect().width + gap;
        };

        const updateNavigation = () => {

            const maxScroll = Math.max(
                0,
                track.scrollWidth - track.clientWidth
            );

            const tolerance = 2;

            if (previousButton) {
                previousButton.disabled =
                    track.scrollLeft <= tolerance;
            }

            if (nextButton) {
                nextButton.disabled =
                    track.scrollLeft >= maxScroll - tolerance;
            }
        };

        if (previousButton) {

            previousButton.addEventListener("click", () => {

                track.scrollBy({
                    left: -getScrollDistance(),
                    behavior: window.matchMedia(
                        "(prefers-reduced-motion: reduce)"
                    ).matches ? "instant" : "smooth"
                });

            });

        }

        if (nextButton) {

            nextButton.addEventListener("click", () => {

                track.scrollBy({
                    left: getScrollDistance(),
                    behavior: window.matchMedia(
                        "(prefers-reduced-motion: reduce)"
                    ).matches ? "instant" : "smooth"
                });

            });

        }

        track.addEventListener("scroll", updateNavigation, {
            passive: true
        });

        window.addEventListener("resize", updateNavigation);

        updateNavigation();

    });

}
