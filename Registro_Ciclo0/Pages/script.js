// script.js
$(document).ready(function () {


    $("#imageCarousel").carousel();

    $(".carousel-control-prev").click(function () {
        $("#imageCarousel").carousel("prev");
    });
    $(".carousel-control-next").click(function () {
        $("#imageCarousel").carousel("next");
    });


    $(".carousel-indicators li").click(function () {
        var goTo = $(this).data('slide-to');
        $("#imageCarousel").carousel(goTo);
    });
});


var countDownDate = new Date("Jan 5, 2024 15:37:25").getTime();

var x = setInterval(function () {


    var now = new Date().getTime();


    var distance = countDownDate - now;


    var days = Math.floor(distance / (1000 * 60 * 60 * 24));
    var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
    var seconds = Math.floor((distance % (1000 * 60)) / 1000);


    document.getElementById("demo").innerHTML = days + "d " + hours + "h "
        + minutes + "m " + seconds + "s ";

    if (distance < 0) {
        clearInterval(x);
        document.getElementById("demo").innerHTML = "EXPIRED";
    }
}, 1000);

function validatePassword() {
    var password = document.getElementById("Input_Password").value;
    var confirmPassword = document.getElementById("confirm_password").value;
    var message = document.getElementById("password_message");

    if (password != confirmPassword) {
        message.textContent = "Las contraseñas no son iguales. ";
        return false;
    } else {
        message.textContent = "";
        return true;
    }
}

