const prev = document.getElementById("prev");
const next = document.getElementById("next");

const pageCount = document.getElementById("pageCount");

const btns = document.querySelectorAll('.btn-page');

btns.forEach(button => {
	button.addEventListener('click', () => {
		pageCount.value = Number(button.dataset.count);
	});
});

if (prev) {
	prev.addEventListener('click', function () {
		pageCount.value = Number(pageCount.value) - 1;
	})
}

if (next) {
	next.addEventListener('click', function () {
		pageCount.value = Number(pageCount.value) + 1;
	})
}