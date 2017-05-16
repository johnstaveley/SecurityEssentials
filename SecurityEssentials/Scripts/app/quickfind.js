/*global $,alert,console,document,AddAntiForgeryToken,kendo,window,refreshData */
/*
Following are the functions to enable quick sort in a form, it relies on a function called refreshData existing which
will cause the datagrid which is being searched to be refreshed. It also relies on the following HTML:
<div class="filterBar quickFind">
    <ul class="actionLinks">
        <li><a id="clearquickfind" style="display: none">clear</a><span id="disableclear">clear</span></li>
    </ul>
    <p>
        <label for="quickFindCriteria">Quick Find:</label><input type="text" id="quickFindCriteria" placeholder="enter some search criteria" />
    </p>
</div>
*/

$(document).ready(function() {

    // Clear quick find between page loads so that quick find criteria are not persisted between loading of the form
    $("#quickFindCriteria").val("");

    // load initial values of quickFind
    var quickFindPreviousValue = $("#quickFindCriteria").val(),
        quickFindChanged = false;

    $("#quickFindCriteria").bind("keyup change",
        function() {
            // check value has changed
            if (quickFindPreviousValue !== $(this).val()) {
                quickFindChanged = true;
                refreshData(true);
            }
            // if user clears out current text then reset search
            if (quickFindChanged && $(this).val().length === 0) {
                quickFindPreviousValue = "";
                quickFindChanged = false;
                refreshData();
            }
            if ($(this).val().length >= 1) {
                $("#clearquickfind").show();
                $("#disableclear").hide();
            } else {
                $("#clearquickfind").hide();
                $("#disableclear").show();
            }
            quickFindPreviousValue = $(this).val();
        });

    $("#clearquickfind").click(function() {
        $("#quickFindCriteria").val("");
        refreshData();
        $("#clearquickfind").hide();
        $("#disableclear").show();
        return false;
    });

});