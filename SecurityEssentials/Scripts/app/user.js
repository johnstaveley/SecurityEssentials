/*global $,alert,window,document */

$(document).ready(function () {
    'use strict';

    var isOwnProfile = Boolean($('#IsOwnProfile').val() === "True" ? true : false);
    var isAdministrator = Boolean($('#IsAdministrator').val() === "True" ? true : false);

    if (isOwnProfile) {
        $('#User_Enabled').attr('disabled', 'disabled');
    }

    if (!isAdministrator) {
        $('#User_UserName').attr('disabled', 'disabled');
    }

});
