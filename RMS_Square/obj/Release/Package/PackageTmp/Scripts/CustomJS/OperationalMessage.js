function ValidationMsg() {
    $("#MessageText").show(500).css("margin", "0 1px 20px 0").html("This field is required!").delay(800).fadeOut(10000);
    $("#MessageText").css("color", "Red");
}
function AcknowledgeMsg() {
    $("#MessageText").show(500).css("margin", "0 1px 20px 0").html("Data not found!").delay(800).fadeOut(10000);
    $("#MessageText").css("color", "Red");
}

function OperationMsg(mode) {
    
    switch(mode) {
        case "I":
            $("#MessageText").show(500).css("margin", "0 1px 20px 0", "color", "green").html("Saved Successfully!").delay(800).fadeOut(10000);
            $("#MessageText").css("color", "green");
            break;
        case "U":
            $("#MessageText").show(500).css("margin", "0 1px 20px 0", "color", "green").html("Updated Successfully!").delay(800).fadeOut(10000);
            $("#MessageText").css("color", "green");
            break;
        case "No":
            $("#MessageText").show(500).css("margin", "0 1px 20px 0", "color", "green").html("Not Saved!").delay(800).fadeOut(10000);
            $("#MessageText").css("color", "green");
            break;
        case "D":
            $("#MessageText").show(500).css("margin", "0 1px 20px 0", "color", "red").html("Deleted Successfully!").delay(800).fadeOut(10000);
            $("#MessageText").css("color", "green");
            break;
        case "NC":
            return "No Changes to Save!!!";
            break;
        case "NoDel":
            $("#MessageText").show(500).css("margin", "0 1px 20px 0", "color", "red").html("Not Deleted!").delay(800).fadeOut(10000);
            $("#MessageText").css("color", "green");
            break;
        case "SDN":
            return "Please Provide Document Name for Upload File.";
        case "FI":
            return "First Save Information then upload file.";
        case "FUS":
            return "File uploaded successfully.";
        case "UFW":
            return "Upload file size should be within 25 MB.";
        case "FLI":
            return "FileName length should be within 100 characters.";
        case "FUE":
            return "File could not be uploaded.";
        case "FD":
            return "File Deleted Successfully.";
        case "FND":
            return "File Cannot Deleted.";
        case "NRF":
            return "No Record Found!";
        case "DGR"://delete grid row
            return "Select Row First then click Delete!";
        case "DD"://data delete
            return "Deleted Successfully!";
        case "GRR"://Grid  row remove
            return "Removed Successfully!";
        case "DDF"://data deleted failed
            return "Deleted Failed!";
        default :
            $("#MessageText").show(500).css("margin", "0 1px 20px 0", "color", "red").html("No Info!").delay(800).fadeOut(10000);
            $("#MessageText").css("color", "green");
            break;
    }
   
}
function WarningMsg() {
    $("#MessageText").show(500).css("margin", "0 1px 20px 0").html("Duplicate Data!").delay(800).fadeOut(10000);
    $("#MessageText").css("color", "Red");
}
function ErrorFrmServerMsg(msgValue) {   
    $("#MessageText").show(500).css("margin", "0 1px 20px 0").html(msgValue).delay(800).fadeOut(10000);
    $("#MessageText").css("color", "Red");
}
function ErrorFrmClientMsg() {
    $("#MessageText").show(500).css("margin", "0 1px 20px 0").html("Error occured!").delay(800).fadeOut(10000);
    $("#MessageText").css("color", "Red");
}
function CompletedMsg() {
    $("#MessageText").show(500).css("margin", "0 1px 20px 0").html("Process Completed!").delay(800).fadeOut(10000);
    $("#MessageText").css("color", "Red");
}