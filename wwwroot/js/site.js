// Add custom scripts here

$(document).ready(function () {
    // Smooth scrolling for anchor links
    $("a[href^='#']").on('click', function (e) {
        e.preventDefault();
        var target = $(this.getAttribute('href'));
        if (target.length) {
            $('html, body').stop().animate({
                scrollTop: target.offset().top - 100
            }, 1000);
        }
    });

    // Add Bootstrap validation classes
    $("form").on("submit", function () {
        $(this).addClass("was-validated");
    });

    // Prevent default form submission for AJAX if needed
    $(".ajax-form").on("submit", function (e) {
        e.preventDefault();
        // Add your AJAX logic here
    });

    // Show/Hide password toggle
    $(".toggle-password").on("click", function () {
        $(this).toggleClass("fa-eye fa-eye-slash");
        var input = $($(this).attr("toggle"));
        if (input.attr("type") == "password") {
            input.attr("type", "text");
        } else {
            input.attr("type", "password");
        }
    });

    // Dismiss alerts after 5 seconds
    setTimeout(function () {
        $(".alert").fadeOut("slow");
    }, 5000);

    // Initialize tooltips
    $('[data-toggle="tooltip"]').tooltip();

    // Initialize popovers
    $('[data-toggle="popover"]').popover();
});

// Form validation helper
function validateEmail(email) {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email);
}

function validatePassword(password) {
    return password.length >= 6;
}

function showLoading(element) {
    $(element).prev().addClass("spinner-show");
}

function hideLoading(element) {
    $(element).prev().removeClass("spinner-show");
}
