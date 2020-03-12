
$("#envoi").css("display", "none");
$("#retour").css("display", "none");

$("#continuer").click(function () {
    $("#debut").css("display", "none");
    $("#suite").css("display", "block");
    $("#retour").css("display", "block");
    $("#continuer").css("display", "none");
    $("#envoi").css("display", "block");
});

$("#retour").click(function () {
    $("#debut").css("display", "block");
    $("#suite").css("display", "none");
    $("#retour").css("display", "none");
    $("#continuer").css("display", "block");
    $("#envoi").css("display", "none");
});