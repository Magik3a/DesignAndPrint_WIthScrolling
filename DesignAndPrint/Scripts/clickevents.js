$(document).ready(function () {
    window.scrollTo(0, 0);
    $('#btnContinueWithPapers').animate({
        opacity: 1
    }, 1500);
});
// Middle buttons for scrolling 
$('#btnContinueWithPapers').click(function () {

    $(this).animate({
        opacity: 0
    }, 500);
    $('#aChoosePaper').click();
    $('#choose-paper').animate({
        opacity: 1
    }, 500, function () {
        $('#btnContinueWiTemplates').animate({
            opacity: 1
        }, 1500);
    });

});
$('#btnContinueWiTemplates').click(function () {
    console.log($(".UsPaper").hasClass('active-paper'));
    if (!ValidatePaperBeforeFinnish()) {
        return;
    }
    $(this).animate({
        opacity: 0
    }, 500);
    $('#aChooseTemplate').show();
    $('#aChooseTemplate').click();
    $('#choose-template').animate({
        opacity: 1
    }, 500, function () {
        $('#btnContinueWithPictures').animate({
            opacity: 1
        }, 1500);
    });
});
$('#btnContinueWithPictures').click(function () {
    if (!ValidateTemplateBeforeFinnish()) {
        return;
    }
    $(this).animate({
        opacity: 0
    }, 500);
    $('#aChoosePictures').show();
    $('#aChoosePictures').click();
    $('#choose-image').animate({
        opacity: 1
    }, 500, function () {
        $('#btnFinishAndPrint').animate({
            opacity: 1
        }, 1500);
    });
});
$('#btnFinishAndPrint').click(function () {
    $(this).animate({
        opacity: 0
    }, 500);
    $('#aFinishAndPrint').show();
    $('#aFinishAndPrint').click();
    $('#finishAndPrint').animate({
        opacity: 1
    }, 500);
});

// Last buttons for printing and downloading
$('#print-button').click(function () {
    $('.btn-add-image').hide();
    $(".box").removeClass("active-box");
    $("#page-for-printing").printElement({
        printMode: 'popup',
        importStyle: false,
        printContainer: false,
        debug: true
    });
});
$("#download-pdf-button").click(function () {
    var param = {
        html: $("#page-for-printing").html()
    };

    //$.post('/Home/DownloadStickers', { html: $("#page-for-printing").html() } , function (data) {
    //    console.log(data);
    //});
    var paperSize = "";
    if ($(".a4size").hasClass('active-paper')) {
        paperSize = "A4"
    }
    if ($(".UsPaper").hasClass('active-paper')) {
        paperSize = "UsPaper"
    }
    $.ajax({
        cache: false,
        url: '/Home/DownloadStickers',
        data: JSON.stringify({ html: $("#page-for-printing").html(), pagesize: paperSize }),
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {
            console.log(data);
            if (!data) {
                $("#wrapper").overhang({
                    type: "error",
                    message: "Whoops! You think you can skip steps, but actually you can't!",
                    duration: 2,
                    upper: true,
                });
                return;
            }
            console.log(data.fileGuid);
            //    window.location = '/Home/Download?fileGuid=' + data.fileGuid;
        }
    })
});
$("#button-wrapper").on('click', "#send-pdf-button", function () {
    $(this).attr("id", "send-message");
    $(this).removeClass("btn-danger");
    $(this).addClass("btn-warning");
    $("#email-user-pdf").show(500);


    $(this).html("<span class='glyphicon glyphicon-envelope'></span> Press again to sent!")
});
$("#button-wrapper").on('click', "#send-message", function () {
    if (!ValidateUserEmail($('#input-user-email-pdf').val())) {
        return;
    }
    if (!ValidatePaperBeforeFinnish()) {
        return;
    }
    if (!ValidateTemplateBeforeFinnish()) {
        return;
    }
    $(this).removeClass("btn-warning");
    $(this).addClass("btn-success btn-lg");
    $("#email-user-pdf").hide(500);

    $(this).html("<span class='glyphicon glyphicon-envelope'></span> Message is sent very successfully! <br /> Click to send another.")
    $(this).attr("id", "send-pdf-button");
    //   $(this).unbind("click").click();
});
$("#button-wrapper").on('click', "#send-pdf-button", function () {
    console.log("click email with pdf");
});

// Validation on elements
function ValidatePaperBeforeFinnish() {
    if (!$(".a4size").hasClass('active-paper') && !$(".UsPaper").hasClass('active-paper')) {
        $("#wrapper").overhang({
            type: "warn",
            message: "You have to choose paper size!",
            duration: 2,
            upper: true
        });
        return false;
    }
    return true;
};
function ValidateTemplateBeforeFinnish() {
    if (!$(".template").hasClass('active-template')) {
        $("#wrapper").overhang({
            type: "warn",
            message: "You have to choose template!",
            duration: 2,
            upper: true
        });
        return false;
    }
    return true;
};
function ValidateUserEmail(email) {
    var testEmail = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    if (!testEmail.test(email)) {
        $("#wrapper").overhang({
            type: "warn",
            message: "This email is not appropriate, can you check what is going on!",
            duration: 2,
            upper: true
        });
        return false;
    }
    return true;
}

// FUnctions for adding classes to active elements
function AddImageToPlaceHolder(elem) {

    if ($(elem).hasClass('active-box')) {
        return;
    };
    $('.btn-add-image').hide();

    $(".box").removeClass("active-box");
    $(elem).addClass("active-box");


    var attr = $(".active-box").find(".btn-add-image").attr('style');
    console.log(attr);

    if (attr == "display: none;") {
        $('.active-box .btn-add-image').show();
    } else {
        $('.active-box .btn-add-image').hide();
    }

    //$(".active-box .panel-body").html("<a href='#' class='btn btn-default btn-add-image'>Add image to this place </a> ")
};

function TemplateClick(elem) {
    if ($(elem).hasClass('active-template')) {
        return;
    };
    $(".template").removeClass("active-template");
    $(elem).addClass("active-template");
    console.log($('.active-template').parent().data('templatename'));
    var param = {
        templateName: $('.active-template').parent().data('templatename')
    };
    $.get("/Home/ChangeTemplate", { templateName: $('.active-template').parent().data('templatename') })
  .done(function (data) {
      $('#page-for-printing').html(data);
      var fourColumnHeight = $(".template-box-square  .panel-body").width() * 2;

      var width = $('.placeholder .panel').offsetParent().width();
      var parentWidth = $('#page-for-printing').width();
      // console.log(width);
      // console.log(parentWidth);

      var height = $('.placeholder .panel').offsetParent().height();
      var parentHeight = $('#page-for-printing > .placeholder').offsetParent().height();
      console.log(height);
      console.log(parentHeight);

      var percentWidth = 100 * width / parentWidth;
      var percentHeight = 100 * fourColumnHeight / parentHeight;

      console.log(percentHeight);
      $("#page-for-printing  .panel-body").height(fourColumnHeight);

      $(".placeholder").css('width', percentWidth + '%');


  });
};

$(".papersizes .panel").click(function () {
    $(".papersizes .panel").removeClass("active-paper");
    $(this).addClass("active-paper");
})


// Email Modal Sent Completed
function EmailSendComplete(data) {
    console.log(data);
    if (data.responseText == "true") {
        $("body").overhang({
            type: "success",
            message: "Woohoo! Thanks For your message!"
        });


        $("#modal-body-email form").hide(500);
          $("#modal-body-email .alert").delay(2000).show(500);
    }
    else {
         $("#wrapper").overhang({
            type: "warn",
            message: "Something is happening with the server, try again later!",
            duration: 2,
            upper: true
        });
        return false;
    }
}