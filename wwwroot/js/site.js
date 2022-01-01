// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function CopyToClipboard(id) {
	var copyText = document.getElementById(id);
	copyText.select();
	copyText.setSelectionRange(0, 99999);
	navigator.clipboard.writeText(copyText.value);
}

$(document).ready(function () {
	$('[data-toggle="popover"]').popover();
});