// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", () => {
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
});
