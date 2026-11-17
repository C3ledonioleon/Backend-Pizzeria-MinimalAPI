// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", () => {
	const navbarToggle = document.querySelector(".navbar-toggle");
	const navbarLinks = document.querySelector(".navbar-links");

	if (navbarToggle && navbarLinks) {
		const closeMenu = () => {
			navbarToggle.classList.remove("is-open");
			navbarLinks.classList.remove("is-open");
			navbarToggle.setAttribute("aria-expanded", "false");
			navbarToggle.setAttribute("aria-label", "Abrir menú");
		};

		navbarToggle.addEventListener("click", () => {
			const isOpen = navbarLinks.classList.toggle("is-open");
			navbarToggle.classList.toggle("is-open", isOpen);
			navbarToggle.setAttribute("aria-expanded", String(isOpen));
			navbarToggle.setAttribute("aria-label", isOpen ? "Cerrar menú" : "Abrir menú");
		});

		navbarLinks.querySelectorAll("a").forEach((link) => {
			link.addEventListener("click", closeMenu);
		});

		document.addEventListener("keydown", (event) => {
			if (event.key === "Escape") {
				closeMenu();
				navbarToggle.focus();
			}
		});

		window.addEventListener("resize", () => {
			if (window.innerWidth > 768) {
				closeMenu();
			}
		});
	}

	const pizzas = document.querySelectorAll(".pizza-reveal");

	if (!("IntersectionObserver" in window)) {
		pizzas.forEach((pizza) => pizza.classList.add("is-visible"));
		return;
	}

	const observer = new IntersectionObserver((entries, currentObserver) => {
		entries.forEach((entry) => {
			if (!entry.isIntersecting) {
				return;
			}

			entry.target.classList.add("is-visible");
			currentObserver.unobserve(entry.target);
		});
	}, { threshold: 0.15 });

	pizzas.forEach((pizza) => observer.observe(pizza));

	const pizzaToast = document.querySelector(".pizza-toast");

	if (pizzaToast) {
		let toastTimer;

		const closeToast = () => {
			clearTimeout(toastTimer);
			pizzaToast.classList.add("is-leaving");
			window.setTimeout(() => pizzaToast.remove(), 350);
		};

		pizzaToast.querySelector(".pizza-toast__close")?.addEventListener("click", closeToast);
		toastTimer = window.setTimeout(closeToast, 5000);
	}
});
