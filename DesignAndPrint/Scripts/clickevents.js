﻿$('#print-button').click(function () {
    $('.btn-add-image').hide();
    $(".box").removeClass("active-box");
    $("#page-for-printing").printElement({ printMode: 'popup' });
});

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

    $(".template").removeClass("active-box");
    $(elem).addClass("active-box");

};
