/*global $,alert,window,document */

$(document).ready(function () {
    'use strict';

    var isOwnProfile = Boolean($('#IsOwnProfile').val() === "True" ? true : false);
    var isAdministrator = Boolean($('#IsAdministrator').val() === "True" ? true : false);

    if (isOwnProfile) {
        $('#User_Approved').prop('disabled', true);
        $('#User_EmailVerified').prop('disabled', true);
        $('#User_Enabled').prop('disabled', true);
    }

    if (!isAdministrator) {
        $('#User_UserName').prop('disabled', true);
    }

});
