/*global $,alert,window,document */

$(document).ready(function () {
    'use strict';

    var isOwnProfile = Boolean($('#IsOwnProfile').val() === "True" ? true : false);

    if (isOwnProfile) {
        $('.isEnabled').attr('disabled', 'disabled');
    }

});
