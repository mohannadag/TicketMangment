// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(function () {

    var placeHolderElement = $('#placeHolderHere');

    $('button[data-toggle="ajax-modal"]').click(function (event) {

        var url = $(this).data('href');
        console.log(url);

        $.get(url).done(function (data) {
            placeHolderElement.html(data);
            placeHolderElement.find('.modal').modal('show');
        })

    })


})