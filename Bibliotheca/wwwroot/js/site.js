var alertBox = document.getElementById("alertBox");
var alertClose = document.getElementById("alertClose");

function hideAlert() {
	alertBox.classList.add("alert-box-hidden");
}

if (alertBox) {
	setTimeout(hideAlert, 5000);
	alertClose.addEventListener("click", hideAlert);
}
