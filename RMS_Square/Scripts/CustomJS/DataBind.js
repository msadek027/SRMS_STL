

function SetMasterData(data) {
    $.each(data, function (key, value) {
        $('#' + key).val(value);
    });
}


function SetDetailData(GridID, data) {
    $("#" + GridID).data('kendoGrid').dataSource.data(data);
}

function CustomRowDataBound(gridName) {
    var grid = $("#" + gridName).data("kendoGrid");
    var gridData = grid.dataSource.view();
    var dl=gridData.length;

    for (var i = 0; i < dl; i++) {
        //alert(gridData[i].DateDiff);
        //get the item uid
        var currentUid = gridData[i].uid;
        //if the record fits the custom condition
        if (gridData[i].DateDiff <= 0) {
            //find the row based on the uid and the custom class
            var currenRow = grid.table.find("tr[data-uid='" + currentUid + "']");
            $(currenRow).addClass("classRed");
        }
        else if (gridData[i].DateDiff > 0 && gridData[i].DateDiff <= 15) {
            //find the row based on the uid and the custom class
            var currenRow = grid.table.find("tr[data-uid='" + currentUid + "']");
            $(currenRow).addClass("classOrange");
        }
        else if (gridData[i].DateDiff >= 16 && gridData[i].DateDiff <= 30) {
            //find the row based on the uid and the custom class
            var currenRow = grid.table.find("tr[data-uid='" + currentUid + "']");
            $(currenRow).addClass("classYellow");
        }
        else if (gridData[i].DateDiff >= 31 && gridData[i].DateDiff <= 60) {
            //find the row based on the uid and the custom class
            var currenRow = grid.table.find("tr[data-uid='" + currentUid + "']");
            $(currenRow).addClass("classGreen");
        }
    }
}
