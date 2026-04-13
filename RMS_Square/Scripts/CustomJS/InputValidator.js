function CheckUnSavedData() {
    $(".box-body").change(function () {
        changeAny = 1;
    });
    $(".txtBox").change(function () {
        changeAny = 1;
    });
}



// Check Numeric field which can't be more than 10 digit & not required
function CheckNumericLength(valu, length) {
    var exe = /^[0-9]+$/;
    if (valu == "") {
        return true; //if input is empty.
    }
    else if (valu.search(exe) == -1) {
        return false;
    }
    else if (valu.length > length) {
        return false;
    }
    return true;
}


// Check Numeric field which can't be more than 10 digit & required
function CheckRequiredNumericLength(val, length) {
    var ex = /^[0-9]+$/;
    if (val == "") {
        return false;
    }
    if (val.search(ex) == -1) {
        return false;
    }
    else if (val.length > length) {
        return false;
    }
    else
        return true;
}



// Check Grid Mail Address
function checkEmail(val) {
    //var emailFormat = /^[A-Z0-9._%+-]+@@[A-Z0-9.-]+\.[A-Z]{2,4}$/i;
    var pattern = new RegExp(/^[+a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/i);
    if (val == null) {
        return true; //if input is empty.
    }
    else if (pattern.test(val) == false) {
        return false;
    }
    return true;
}


function InputValidation(InputID) {
    var Cnt = 0;
    if (jQuery.trim($('#' + InputID).val()).length == 0) {
        $('#' + InputID).css("border", "1px solid red");
        Cnt = 1;
    }
    return Cnt;
};
function InputValidationMaxLth(InputID, length) {
    var Cnt = 0;
    if ((jQuery.trim($('#' + InputID).val()).length == 0) || (jQuery.trim($('#' + InputID).val()).length > length)) {
        $('#' + InputID).css("border", "1px solid red");
        Cnt = 1;
    }
    return Cnt;
}


function ClearRequiredTextBoxRedColor() {
    $(".txtBox").on('input', function () {
        $(".txtBox").css("border-color", ""); //Clear Red Color
    });

    $(".txtBox").on('change', function () {
        $(".txtBox").css("border-color", ""); //Clear Red Color
    });

}






// Change Event in all TextBox
function CheckUnSavedOnlyMasterData() {
    jQuery(":text").on('input', function () {
        isValid = 1;
        $("#MessageText").html("");
        if (jQuery.trim($(".RequiredField").val()).length > 0)
            $(".RequiredField").css("border-color", "white");
    });
    jQuery("textarea").on('input', function () {
        isValid = 1;
        $("#MessageText").html("");
        if (jQuery.trim($(".RequiredField").val()).length > 0)
            $(".RequiredField").css("border-color", "white");
    });
    //$(".CommonDropDown").change(function () {
    //    isValid = 1;
    //    $("#MessageText").html("");
    //    if (jQuery.trim($(".RequiredField").val()).length > 0)
    //        $(".RequiredField").css("border-color", "white");
    //});
    $(".noChangeTypeText").change(function () {
        isValid = 0;
        $("#MessageText").html("");
        if (jQuery.trim($(".RequiredField").val()).length > 0)
            $(".RequiredField").css("border-color", "white");
    });
    jQuery(".date").change('input', function () {
        isValid = 1;
        $("#MessageText").html("");
        if (jQuery.trim($(".RequiredField").val()).length > 0)
            $(".RequiredField").css("border-color", "white");
    });
}

// Change Event in all TextBox
function CheckUnSavedMasterDetailData() {

    jQuery(":text").on('input', function () {
        isValid = 1;
        $("#MessageText").html("");
        if (jQuery.trim($(".RequiredField").val()).length > 0)
            $(".RequiredField").css("border-color", "white");
    });

    jQuery("textarea").on('input', function () {
        isValid = 1;
        $("#MessageText").html("");
        if (jQuery.trim($(".RequiredField").val()).length > 0)
            $(".RequiredField").css("border-color", "white");
    });
    $(".CommonDropDown").change(function () {
        isValid = 1;
        $("#MessageText").html("");
        if (jQuery.trim($(".RequiredField").val()).length > 0)
            $(".RequiredField").css("border-color", "white");
    });
    $(".noChangeTypeText").change(function () {
        isValid = 0;
        $("#MessageText").html("");
        if (jQuery.trim($(".RequiredField").val()).length > 0)
            $(".RequiredField").css("border-color", "white");
    });
    jQuery(".date").change('input', function () {
        isValid = 1;
        $("#MessageText").html("");
        if (jQuery.trim($(".RequiredField").val()).length > 0)
            $(".RequiredField").css("border-color", "white");
    });
    //// Change Event in PageDetailGrid Detail Grid
    //jQuery("#PageDetailGrid").on('input', function () {
    //    isValid = 1;
    //});
    // Change Event in PageDetailGrid Detail Grid
    jQuery(".MainGrid").on('input', function () {
        isValid = 1;
    });
}




//Date Formate

function parseDate(str) {
    var splitstr = str.split('/')
    return new Date(splitstr[2], splitstr[1] - 1, splitstr[0]);
}

function daydiff(first, second) {
    return (second - first) / (1000 * 60 * 60 * 24)
}
function CheckDiffTwoDates(first1, second1) {
    var Diffdate = daydiff(parseDate($('#' + first1).val()), parseDate($('#' + second1).val()));
    if (Diffdate < 0) {
        $("#MessageText").html("Please Enter Data Properly");
        $("#MessageText").css("color", "red");
        $('#' + second1).css("border-color", "red");
        saveStatus = 1
    }
}

function CheckDiffSysAndOneInpDates(first1, second1) {
    var Diffdate = daydiff(parseDate(first1), parseDate($('#' + second1).val()));
    if (Diffdate < 0) {
        $("#MessageText").html("Please Enter Data Properly");
        $("#MessageText").css("color", "red");
        $('#' + second1).css("border-color", "red");
        saveStatus = 1
    }
}
function getCurrentDate2() {
    var date = new Date();

    var yaer = date.getFullYear();
    var month = date.getMonth() + 1;
    if (month < 10) {
        month = "0" + month
    }
    var day = date.getDate();
    if (day < 10) {
        day = "0" + day
    }
    return day + "/" + month + "/" + yaer;
}

function getAdvancedDate() {
    var date = new Date();

    var yaer = date.getFullYear();
    var month = date.getMonth() + 1;
    if (month < 10) {
        month = "0" + month
    }
    var day = date.getDate();
    if (day < 10) {
        day = "0" + day
    }
    return new Date(day + "/" + month + "/" + yaer);
}

function getString() {
    return "sss";
}