/*global $,alert,console,document,AddAntiForgeryToken,kendo,window,refreshGrid */

function refreshData(gotoFirstPage) {
    'use strict';

    if (gotoFirstPage === true) {
        $("#kendoUserGrid").data("kendoGrid").dataSource.page(1);
    } else {
        $("#kendoUserGrid").data("kendoGrid").dataSource.read();
        $("#kendoUserGrid").data("kendoGrid").refresh();
    }
}

function editUserCommand(e) {
    //'use strict';

    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr"));
    window.location.href = 'User/Edit/' + dataItem.Id;

}

function disableUserCommand(e) {
    //'use strict';

    e.preventDefault();
    var dataItem = this.dataItem($(e.currentTarget).closest("tr")),
        userId = dataItem.Id;
    $("#DisableUserDialog").append("<div></div>");

    $("#DisableUserDialog div")
        .addClass("dialog")
        .appendTo("body")
        .dialog({
            title: "Disable User?",
            close: function () { $(this).remove(); },
            modal: true,
            width: 450,
            left: 0,
            resizable: false,
            buttons: {
                "Disable": function () {
                    var self = this;
                    $.ajax({
                        context: document.body,
                        data: $.addAntiForgeryToken({ id: userId }),
                        dataType: 'json',
                        error: function (xmlHttpRequest, status, error) {
                            alert("An error occurred whilst trying to contact the website");
                        },
                        success: function (data) {
                            if (data.success === true) {
                                refreshData(false);
                                $(self).dialog("close");
                            } else {
                                alert("An error occurred: " + data.message);
                                $(self).dialog("close");
                            }
                        },
                        type: 'POST',
                        url: 'User/Disable'
                    });
                },
                Cancel: function () {
                    $(this).dialog("close");
                }
            }
        })
            .load('User/Disable/' + userId);
}

$(document).ready(function () {
    'use strict';

    var usersData = new kendo.data.DataSource({
        transport: {
            read: {
                url: 'User/Read/',
                dataType: "json",
                data: function () {
                        var parameter = {
                            searchText: $('#quickFindCriteria').val()
                        };
                    // Get text for filter bar
                    return parameter;
                }
            }
        },
        schema: {
            data: "data", // records are returned in the "data" field of the response
            total: "total" // total number of records is in the "total" field of the response
        },
        pageSize: 10,
        serverPaging: true,
        serverFiltering: true,
        serverSorting: true
    });

    $("#kendoUserGrid").kendoGrid({
        dataBound: function () {
            var userList = $(this)[0]._data,
                user,
                isEnabled;
            for (user in userList) {
                isEnabled = userList[user].Enabled === true;
                if (!isEnabled) {
                    var uid = userList[user].uid;
                    $.each($('#kendoUserGrid tr'), function () {
                        if ($(this).data("uid") === uid) {
                            $(this).find('a.disableUser').addClass('disabled').on('click', function () {
                                return false;
                            });
                        }
                    });

                }
            }

        },
        dataSource: usersData,
        groupable: false,
        sortable: true,
        scrollable: false,
        pageable: {
            refresh: true,
            pageSizes: [5, 10, 20, 50, 100, 200]
        },
        columns: [
            { field: "FullName", sortable: false, title: "Name" },
            { field: "Id", hidden: true },
            { field: "UserName", title: "User Name"},
            { field: "TelNoMobile", title: "Mobile No" },
            {
                field: "Enabled",
                type: "Boolean",
                editable: false,
                sortable: false,
                title: "Enabled",
                template: "<input type='checkbox' #= Enabled ? checked='checked': '' # class='chkbx' disabled='disabled' />",
                attributes: { style: "text-align: center;" }
            },
            {
                field: "Approved",
                type: "Boolean",
                editable: false,
                sortable: false,
                title: "Approved",
                template: "<input type='checkbox' #= Approved ? checked='checked': '' # class='chkbx' disabled='disabled' />",
                attributes: { style: "text-align: center;" }
            },
            {
                command: [
                    { text: "Edit", className: 'editUser', click: editUserCommand },
                    { text: "Disable", className: 'disableUser', click: disableUserCommand }
                ],
                title: " ",
                attributes: { style: "text-align: center;" }
            }
        ]
    });

    $('#kendoUserGrid tbody').on('click', ':checkbox', function (e) {
        e.stopPropagation();
    });

});

