
// Reset Data
function ResetData() {
 
    $(":text").val("");
    $("textarea").val("");
    $(".txtBox").val("");
    
    //$(".hiddenField").val("");
    //$(".SetFocus").focus();
   
 
}

function RemoveTxtChk() {
    $('.chk').remove();
    $('.txt').remove();

}
//For removing operational & required message after triggering other event
function ClearBorderRequiredMsg() {
    $(".RequiredField").css("border", "1px Solid #D2D6DE");
    $(".txtBox").css("border", "1px Solid #D2D6DE");//Clear Red Color     
    $("#MessageText").html(""); //Clear operation Message  
}


