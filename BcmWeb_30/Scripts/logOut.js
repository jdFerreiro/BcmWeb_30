var clicked = false;

function CheckBrowser() {
    if (clicked == false) {
        //Browser closed   
    } else {
        //redirected
        clicked = false;
    }
}

function bodyUnload() {
    if (clicked == false)//browser is closed
    {
        location = window.location;
        var url = location.protocol + "//" + location.host + "/" + "account/LogOff";
        window.location.href = url;
        //request.open("GET", "Account/LogOut", false);
        //request.send();
    }
}

function GetRequest() {
    var xmlhttp;
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else {// code for IE6, IE5
        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
    }
    return xmlhttp;
}
