
function AutoFitColumn(GridID) {
    var grid = $("#" + GridID).data("kendoGrid");
    for (var i = 0; i < grid.columns.length; i++) {

        grid.autoFitColumn(i);
    }
}


function FilterToGrid(GridID, ValueField) {
    var grid = $("#" + GridID).data("kendoGrid");
    var columns = grid.columns;
    if (columns.length > 0) {
        for (var i = 0; i < columns.length; i++) {
            var col = columns[i];
            $("#" + GridID).data("kendoGrid").dataSource.filter({
                logic: "or",
                filters: [{ field: col.field, operator: "contains", value: ValueField }, ]
            });
        }
    }
}


function AddRowToGrid(GridID) {
    var addflag = 1; // For Row Add in Detail Grid using enter Key Press

    var grid = $("#" + GridID).data("kendoGrid");

    $("#" + GridID).data().kendoGrid.bind('dataBound', function () {
        this.element.find('tbody tr:first').addClass('k-state-selected');
    });
    var dataSource = $("#" + GridID).data("kendoGrid").dataSource;
    var addData = dataSource.data();
    if (addData != null) { // For Add row when Exiting Data in Grid
        for (var i = 0; i < addData.length; i++) {
            if (addData[i].val == "") {
                addflag = 0;
            }
        }
    }
    if (addflag == 1) {
        grid.addRow();
    }
}

//---------------------------------------
var dataitem = {};
var GridName = null;
function RemoveRowFromGrid(GridID, GridSource) {  
    var GridView = $("#" + GridID).data("kendoGrid");
    dataitem = (GridView.dataItem(GridView.select()));
    //  dataitem = GridSource.dataItem($(event.currentTarget).closest("tr"));
    GridName = GridSource;
    RemoveConfirmation.open();
   // GridSource.dataSource.remove(dataitem);
}
var RemoveConfirmation = $('#RemoveConfirmationWindow').kendoWindow({
    width: "30%",
    height: "10%",
    draggable: true,
    modal: true,
    title: "Do You Want To Remove This Record?"    
    }).data("kendoWindow").center();


////Handling Yes button click for master Delete Confirmation[Yes Button]
$('#btnRemoveConfirmationYes').click(function () {
     GridName.dataSource.remove(dataitem);
     RemoveConfirmation.close();
     OperationMsg();
});

//Handling No button click for grid row Delete Confirmation[No Button]
$('#btnRemoveConfirmationNo').click(function () {
    dataitem = {};
    RemoveConfirmation.close();
});

//-----------------------------------------------------


var linkURL = null;
var vID = null;
function DeleteRowFromGrid(GridID, GridSource,URL,ID) {
    var GridView = $("#" + GridID).data("kendoGrid");
    dataitem = (GridView.dataItem(GridView.select()));   
    GridName = GridSource;
    linkURL = URL;
    vID = ID;
    DeleteConfirmation.open();   
}
var DeleteConfirmation = $('#DeleteConfirmationWindow').kendoWindow({
    width: "30%",
    height: "10%",
    draggable: true,
    modal: true,
    title: "Do You Want To Delete This Record?"  
}).data("kendoWindow").center();


////Handling Yes button click for master Delete Confirmation[Yes Button]
$('#btnDeleteConfirmationYes').click(function () {   
    DeleteGridRow(linkURL, vID);   
    GridName.dataSource.remove(dataitem);
    DeleteConfirmation.close();
});

//Handling No button click for grid row Delete Confirmation[No Button]
$('#btnDeleteConfirmationNo').click(function () {
    dataitem = {};
    DeleteConfirmation.close();
});


function DeleteGridRow(linkURL, vID) {
    $.ajax({
        url: linkURL,
        type: 'GET',
        data: { "kID": vID },
        contentType: 'application/json;',
        dataType: 'json',
        success: function (response) {
            if (response == "Deletesuccess") {
                OperationMsg();
            }
            else {
                $("#MessageText").html(response.Message);
                $("#MessageText").css("color", "green");
            }
        }
    });
}