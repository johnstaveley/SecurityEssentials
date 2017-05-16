/*global $, window */

$.addAntiForgeryToken = function(data) {
    "use strict";
    data.__RequestVerificationToken =
        $("input[name=__RequestVerificationToken]").val();
    return data;
};