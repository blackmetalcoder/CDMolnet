function inlogg(){
    var x = document.getElementById('username').value;
    var y = document.getElementById('password').value;
    if (x == null || x == "") {
        alert("Fyll i användarnamn & lösen");
        return false;
    }
    var serviceURL = 'http://cdmolnet.se/CDService.asmx/loggaIn';
    //var serviceURL = 'http://localhost:1239//CDService.asmx/loggaIn';
    $.ajax({
        type: "POST",
        url: serviceURL,
        data: "{usernamn:'"+ x + "', password: '" + x + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: successFunc,
        error: errorFunc
    });

    function successFunc(data, status) {
        // parse it as object
        if (data.d == 'NO')
        {
            alert('Fel användar uppgifter försök igen');
        }
        else {
            var str = data.d;
            var splitted = str.split(";", 2);
            alert('Välkommen ' + splitted[0]);
            localStorage.inloggad = 'Ja';
            localStorage.userID = splitted[1];
            localStorage.User = splitted[0];
            document.getElementById('username').value = "";
            document.getElementById('password').value = "";
        }
        
    }

    function errorFunc() {
        alert('fel');
    }
}
function checkinlogg(){
    if (localStorage.inloggad == 'Ja')
    {
        //alert('Inloggad');
    }
    else
    {
        alert('Logga in eller skapa ett konto så kommer du sidan !!');
        window.location.href("index.html");
    }
}
function nyUser() {
    var fnamn = document.getElementById('txtFnamn').value;
    var enamn = document.getElementById('txtEnamn').value;
    var usernamn = document.getElementById('txtUsername').value;
    var password = document.getElementById('txtPassword').value;
    var epost = document.getElementById('txtEpost').value;
    var svar = " ";
    var kor = true;
    //Nu kollar vi så att allt är rätt i fyllt!!
    //Förnamn
    if (fnamn == null || fnamn == "")
    {
        svar = "Förnamn saknas, ";
        kor = false;
    }
    //Efternamn
    if (enamn == null || enamn == "") {
        svar = svar + "Efternamn saknas, ";
        kor = false;
    }
    //Usernamn
    if (usernamn == null || usernamn == "") {
        svar = svar + "Användarnamn saknas, ";
        kor = false;
    }
    //Password
    if (password == null || password == "" || password.length != 8 ) {
        svar = svar + "Lösenord saknas, eller inte 8 tecken långt, ";
        kor = false;
    }
    //Epost
    var atpos = epost.indexOf("@");
    var dotpos = epost.lastIndexOf(".");
    if (atpos < 1 || dotpos < atpos + 2 || dotpos + 2 >= epost.length) {
        svar = svar +  "Ingen epost-adress";
        kor = false;
    }
    //Slut koll***********************************
    if (kor == false)
    {
        alert(svar);
        return false;
    }
    var serviceURL = 'http://cdmolnet.se/CDService.asmx/addUser';
    //var serviceURL = 'http://localhost:1239//CDService.asmx/addUser';
    $.ajax({
        type: "POST",
        url: serviceURL,
        data: "{fnamn:'" + fnamn + "', enamn: '" + enamn + "', epost: '" + epost + "', usernamn: '" + usernamn + "', password: '" + password + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: successFunc,
        error: errorFunc
    });

    function successFunc(data, status) {
        // parse it as object
        if (data.d == 'NO') {
            alert('Fel användar uppgifter försök igen');
        }
        else {
            alert('Välkommen ' + data.d);
            localStorage.inloggad = 'Ja';
            //alert(localStorage.inloggad);
        }

    }

    function errorFunc() {
        alert('fel');
    }
}
function finnsUser() {

    var serviceURL = 'http://cdmolnet.se/CDService.asmx/finnsUser';
     var usernamn = document.getElementById('txtUsername').value;
    //Usernamn
     if (usernamn == null || usernamn == "") {
         alert("inget Användarnamn angivet!!");
         return false;
     }
    $.ajax({
        type: "POST",
        url: serviceURL,
        data: "{userNamn:'" + usernamn + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: successFunc,
        error: errorFunc
    });

    function successFunc(data, status) {
        var r;
        r = data.d;
        if (r == 'true')
        {
            test1();
        }
        else
        {
            test2();
        }
       
    }
    function errorFunc() {
        alert('fel');
    }
      
 }
 function test1() {
     alert('Användarnamn upptaget, ange nytt !!');
 }
 function test2() {
     nyUser();
 }
 function senasteCD() {
     var userId = localStorage.userID;
     url = 'http://cdmolnet.se/CDService.asmx/senasteInnlagda?userId=' + userId; 
     //url = 'http://localhost:1239/CDService.asmx/senasteInnlagda?userId=' + userId;
     var source =
 {
     type: "GET",
     datatype: "json",
     datafields: [
         { name: 'Bild', type: 'string' },
         { name: 'Artist', type: 'string' },
         { name: 'Album', type: 'string' },
         { name: 'Media', type: 'string' }        
     ],
     url: url,
     cache: false,
     root: 'data'
 };

     var dataAdapter1 = new $.jqx.dataAdapter(source, {
         contentType: 'application/json; charset=utf-8',
         downloadComplete: function (data, textStatus, jqXHR) {
             return data.d;
             
         },
         loadError: function (jqXHR, textStatus, errorThrown) {
             alert('HTTP status code: ' + jqXHR.status + '\n' +
                   'textStatus: ' + textStatus + '\n' +
                   'errorThrown: ' + errorThrown);
             alert('HTTP message body (jqXHR.responseText): ' + '\n' + jqXHR.responseText);
         }
     }
         );
     var imagerenderer = function (row, datafield, value) {
         return '<img style="margin-left: 5px;" height="60" width="60" src="' + value + '"/>';
     }
     $("#DataGridSenaste").jqxGrid(
     {
         width: 500,
         height: 300,
         source: dataAdapter1,
         pageable: false,
         autoheight: false,
         showheader: true,
         rowsheight: 60,
         theme: 'bootstrap',
         columnsresize: true,
         columns: [
           { text: 'Cover', columngroup: 'alla', datafield: 'Bild', width: 60, cellsrenderer: imagerenderer },
           { text: 'Artist', columngroup: 'alla', datafield: 'Artist', width: 200, cellsalign: 'left' },
           { text: 'Album', columngroup: 'alla', datafield: 'Album', width: 200, cellsalign: 'left' },       
         ],
         columngroups: 
     [
       { text: '30 Senaste inlagda', align: 'center', name: 'alla' }
     ]
     });
 }
 function AntalPerAr() {
     var userId = localStorage.userID;
     var source = {};
     $.ajax({
         type: 'GET',
         dataType: 'json',
         async: false,
         url: 'http://cdmolnet.se/CDService.asmx/AntalperAr',
         //url: 'http://localhost:1239/CDService.asmx/AntalperAr',
         data: { 'userId': userId },
         cache: false,
         contentType: 'application/json; charset=utf-8',
         success: function (data) {
             source = $.parseJSON(data.d);
         },
         error: function (err) {
             alert(err.statusText);
         }
     });
     // prepare jqxChart settings
     var settings = {
         title: "Antal per år",
         //description: "Time spent in vigorous exercise by activity",
         enableAnimations: true,
         showLegend: true,
         padding: { left: 10, top: 5, right: 10, bottom: 5 },
         titlePadding: { left: 90, top: 0, right: 0, bottom: 10 },
         source: source,
         categoryAxis:
             {
                 text: 'Category Axis',
                 textRotationAngle: 90,
                 dataField: 'Ar',
                 showTickMarks: true,
                 valuesOnTicks: false,
                 tickMarksInterval: 1,
                 tickMarksColor: '#888888',
                 unitInterval: 1,
                 gridLinesInterval: 1,
                 gridLinesColor: '#888888',
                 axisSize: 'auto'
             },
         colorScheme: 'scheme05',
         backgroundColor: '#eeeeee',
         seriesGroups:
             [
                 {
                     type: 'line',
                     showLabels: true,
                     symbolType: 'circle',
                     valueAxis:
                     {
                         unitInterval: 10,
                         minValue: 0,
                         maxValue: 100,
                         description: 'Antal per år',
                         axisSize: 'auto',
                         tickMarksColor: '#888888'
                     },
                     series: [
                             { dataField: 'antal', displayText: 'Antal per år' }
                     ]
                 }
             ]
     };
     // setup the chart
     $('#LinePerAr').jqxChart(settings);
 }
 function AntalPerMedia() {
     var userId = localStorage.userID;
     var source = {};
     $.ajax({
         type: 'GET',
         dataType: 'json',
         async: false,
         url: 'http://cdmolnet.se/CDService.asmx/media',
         //url: 'http://localhost:1239/CDService.asmx/media',
         data: { 'userId': userId },
         cache: false,
         contentType: 'application/json; charset=utf-8',
         success: function (data) {
             source = $.parseJSON(data.d);
         },
         error: function (err) {
             alert(err.statusText);
         }
     });
     // prepare jqxChart settings
     var settings = {
         title: "Fördelning CD/Vinyl",
         description: "(source: CDMOLNET)",
         enableAnimations: true,
         showLegend: true,
         padding: { left: 5, top: 5, right: 5, bottom: 5 },
         titlePadding: { left: 0, top: 0, right: 0, bottom: 10 },
         source: source,
         colorScheme: 'scheme01',
         backgroundColor: '#eeeeee',
         seriesGroups:
             [
                 {
                     type: 'pie',
                     showLabels: true,
                     series:
                         [
                             {
                                 dataField: 'varde',
                                 displayText: 'media',
                                 labelRadius: 120,
                                 initialAngle: 15,
                                 radius: 95,
                                 centerOffset: 0
                             }
                         ]
                 }
             ]
     };
     // setup the chart
     $('#pieMedia').jqxChart(settings);
 }
 function totStat() {
     var userId = localStorage.userID;
     var source = {};
     $.ajax({
         type: 'GET',
         dataType: 'json',
         async: false,
         url: 'http://cdmolnet.se/CDService.asmx/minStat',
         //url: 'http://localhost:1239/CDService.asmx/minStat',
         data: { 'userId': userId },
         cache: false,
         contentType: 'application/json; charset=utf-8',
         success: function (data) {
             source = $.parseJSON(data.d);
         },
         error: function (err) {
             alert(err.statusText);
         }
     });
     // prepare jqxChart settings
     var settings = {
         title: "Min samling",
         //description: "Statistics for 2011",
         showLegend: true,
         enableAnimations: true,
         backgroundColor: '#eeeeee',
         padding: { left: 20, top: 5, right: 20, bottom: 5 },
         titlePadding: { left: 90, top: 0, right: 0, bottom: 10 },
         source: source,
         categoryAxis:
             {
                 dataField: 'Text',
                 showGridLines: true,
                 textRotationAngle: 90,
                 flip: false
             },
         colorScheme: 'scheme01',
         seriesGroups:
             [
                 {
                     type: 'column',
                     orientation: 'horizontal',
                     columnsGapPercent: 100,
                     showLabels: true,
                     toolTipFormatSettings: { thousandsSeparator: ',' },
                     valueAxis:
                     {
                         flip: true,
                         unitInterval: 50,
                         maxValue: 3000,
                         displayValueAxis: true,
                         formatFunction: function (value) {
                             return parseInt(value / 1000000);
                         }
                     },
                     series: [
                             { dataField: 'Antal', displayText: 'Antal per kategori' }
                     ]
                 }
             ]
     };
 $('#MinSamling').jqxChart(settings);
 }
 function sparaPie() {
     $('#pieMedia').jqxChart('saveAsJPEG', 'myChart.jpeg');
 }
 function getArtister() {
     var userId = "1";//localStorage.userID;
     url = 'http://cdmolnet.se/CDService.asmx/getArtist?userId=' + userId; 
     //url = 'http://localhost:1239/CDService.asmx/getArtist?userId=' + userId;
     var source =
 {
     type: "GET",
     datatype: "json",
     datafields: [
         { name: 'artist', type: 'string' }
     ],
     url: url,
     cache: false,
     root: 'data'
 };

     var dataAdapter1 = new $.jqx.dataAdapter(source, {
         contentType: 'application/json; charset=utf-8',
         downloadComplete: function (data, textStatus, jqXHR) {
             return data.d;

         },
         loadError: function (jqXHR, textStatus, errorThrown) {
             alert('HTTP status code: ' + jqXHR.status + '\n' +
                   'textStatus: ' + textStatus + '\n' +
                   'errorThrown: ' + errorThrown);
             alert('HTTP message body (jqXHR.responseText): ' + '\n' + jqXHR.responseText);
         }
     }
         );
     var imagerenderer = function (row, datafield, value) {
         return '<img style="margin-left: 5px;" height="60" width="60" src="' + value + '"/>';
     }
     $("#DataGridSenaste").jqxGrid(
     {
         width: 400,
         height: 600,
         source: dataAdapter1,
         pageable: false,
         autoheight: false,
         showheader: true,
         sortable: true,
         rowsheight: 30,
         theme: 'bootstrap',
         columnsresize: true,
         showfilterrow: true,
         filterable: true,
         columns: [
           //{ text: 'Cover', columngroup: 'alla', datafield: 'Bild', width: 60, cellsrenderer: imagerenderer },
           { text: 'Artist', columngroup: 'alla', datafield: 'artist', width: 250, cellsalign: 'left' },
           {
               text: 'Visa', datafield: 'Edit', columntype: 'button', cellsrenderer: function () { return "Visa"; }, buttonclick: function (row) {
                   var dataRecord = $("#DataGridSenaste").jqxGrid('getrowdata', row);
                   //getAlbums(dataRecord.artist);
                   getAlbumTracks(dataRecord.artist);
                   getArtistinfo(dataRecord.artist);
                   getTopalbum(dataRecord.artist);
                   getLikanadeArtister(dataRecord.artist);
               }
           }
         ]
     });
 }
 function getAlbums(artist) {
     var userId = localStorage.userID;
     url = 'http://cdmolnet.se/CDService.asmx/getAlbum?userId=' + userId + '&Artist="' + artist + '"';
     //url = 'http://localhost:1239/CDService.asmx/getAlbum?userId=' + userId + '&Artist="' + artist + '"';
     var source =
 {
     type: "GET",
     datatype: "json",
     datafields: [
         { name: 'Cover', type: 'string' },
         { name: 'artist', type: 'string' },
         { name: 'album', type: 'string' },
         { name: 'Media', type: 'string' },
         { name: 'Ar', type: 'string' },
         { name: 'Kategori', type: 'string' },
         { name: 'discID', type: 'string' }
     ],
     url: url,
     cache: false,
     root: 'data'
 };

     var dataAdapter1 = new $.jqx.dataAdapter(source, {
         contentType: 'application/json; charset=utf-8',
         downloadComplete: function (data, textStatus, jqXHR) {
             return data.d;

         },
         loadError: function (jqXHR, textStatus, errorThrown) {
             alert('HTTP status code: ' + jqXHR.status + '\n' +
                   'textStatus: ' + textStatus + '\n' +
                   'errorThrown: ' + errorThrown);
             alert('HTTP message body (jqXHR.responseText): ' + '\n' + jqXHR.responseText);
         }
     }
         );
     var imagerenderer = function (row, datafield, value) {
         return '<img style="margin-left: 5px;" height="60" width="60" src="' + value + '"/>';
     }
     $("#DataGridAlbum").jqxGrid(
     {
         width: 750,
         height: 550,
         source: dataAdapter1,
         pageable: false,
         autoheight: false,
         showheader: true,
         rowsheight: 60,
         theme: 'bootstrap',
         columnsresize: true,
         columns: [
           { text: 'Cover', datafield: 'Cover', width: 60, cellsrenderer: imagerenderer },
           { text: 'Album', datafield: 'album', width: 200, cellsalign: 'left' },
           { text: 'Media', datafield: 'Media', width: 60, cellsalign: 'left' },
           { text: 'Kategori', datafield: 'Kategori', width: 100, cellsalign: 'left' },
           { text: 'Utgiven', datafield: 'Ar', width: 60, cellsalign: 'left' },
           { text: 'DiscID', datafield: 'discID', width: 200, cellsalign: 'left' }
    ]
     });
     $('#DataGridAlbum').on('rowselect', function (event) {
         //alert("Row with bound index: " + event.args.rowindex + " has been selected");
         var dataRecord = $("#DataGridAlbum").jqxGrid('getrowdata', event.args.rowindex);
         getTracks(dataRecord.discID);
     });
 }
 function getTracks(DiscID) {
     var userId = localStorage.userID;
     url = 'http://cdmolnet.se/CDService.asmx/getTracks?userId=' + userId + '&DiscID="' + DiscID + '"';
     //url = 'http://localhost:1239/CDService.asmx/getTracks?userId=' + userId + '&DiscID="' + DiscID + '"';
     var source =
 {
     type: "GET",
     datatype: "json",
     datafields: [
         { name: 'track', type: 'string' },
         { name: 'nr', type: 'number' }
     ],
     url: url,
     cache: false,
     root: 'data'
 };

     var dataAdapter1 = new $.jqx.dataAdapter(source, {
         contentType: 'application/json; charset=utf-8',
         downloadComplete: function (data, textStatus, jqXHR) {
             return data.d;

         },
         loadError: function (jqXHR, textStatus, errorThrown) {
             alert('HTTP status code: ' + jqXHR.status + '\n' +
                   'textStatus: ' + textStatus + '\n' +
                   'errorThrown: ' + errorThrown);
             alert('HTTP message body (jqXHR.responseText): ' + '\n' + jqXHR.responseText);
         }
     }
         );
     var imagerenderer = function (row, datafield, value) {
         return '<img style="margin-left: 5px;" height="60" width="60" src="' + value + '"/>';
     }
     $("#DataGridTrack").jqxGrid(
     {
         width: 750,
         height: 300,
         source: dataAdapter1,
         pageable: false,
         autoheight: false,
         showheader: true,
         rowsheight: 30,
         theme: 'bootstrap',
         columnsresize: true,
         columns: [
           { text: 'Spår.nr', datafield: 'nr', width: 55, cellsalign: 'left' },
           { text: 'Track', datafield: 'track', width: 400, cellsalign: 'left' }
           
         ]
     });
 }
 function getArtistinfo(artist) {
     //var serviceURL = 'http://localhost:1239/CDService.asmx/artistInfo';
     var serviceURL = 'http://cdmolnet.se/CDService.asmx/artistInfo';
     $.ajax({
         type: "POST",
         url: serviceURL,
         data: "{artist:'" + artist + "'}",
         contentType: "application/json; charset=utf-8",
         dataType: "json",
         success: successFunc,
         error: errorFunc
     });

     function successFunc(data, status) {
         // parse it as object        
             var str = data.d;
         //alert(str);
             $('#Artistinfo').empty();
             $('#Artistinfo').append(str)
         }


     function errorFunc() {
         alert('fel');
     }
 }
 function getTopalbum(artist) {
     //var serviceURL = 'http://localhost:1239/CDService.asmx/topAlbum';
     var serviceURL = 'http://cdmolnet.se/CDService.asmx/topAlbum';
     $.ajax({
         type: "POST",
         url: serviceURL,
         data: "{artist:'" + artist + "'}",
         contentType: "application/json; charset=utf-8",
         dataType: "json",
         success: successFunc,
         error: errorFunc
     });
     function successFunc(data, status) {      
         var str = data.d;
         $('#topAlbum').empty();
         $('#topAlbum').append(str);
     }


     function errorFunc() {
         alert('fel');
     }
 }
 function getLikanadeArtister(artist) {
     //var serviceURL = 'http://localhost:1239/CDService.asmx/artistLiknande';
     var serviceURL = 'http://cdmolnet.se/CDService.asmx/artistLiknande';
     $.ajax({
         type: "POST",
         url: serviceURL,
         data: "{artist:'" + artist + "'}",
         contentType: "application/json; charset=utf-8",
         dataType: "json",
         success: successFunc,
         error: errorFunc
     });

     function successFunc(data, status) {
         // parse it as object        
         var str = data.d;
         //alert(str);
         $('#likandeArtister').empty();
         $('#likandeArtister').append(str);
     }


     function errorFunc() {
         alert('fel');
     }
 }
 function tom() {
     $('#topAlbum').empty();
     $('#likandeArtister').empty();
 }
 function getTracksByArtist(DiscID, Artist) {
     var userId = localStorage.userID;
     url = 'http://cdmolnet.se/CDService.asmx/getTracksByArtist?userId=' + userId + '&Artist="' + Artist + '"';
     //url = 'http://localhost:1239/CDService.asmx/getTracks?userId=' + userId + '&DiscID="' + DiscID + '"';
     var source =
 {
     type: "GET",
     datatype: "json",
     datafields: [
         { name: 'track', type: 'string' },
         { name: 'nr', type: 'number' }
     ],
     url: url,
     cache: false,
     root: 'data'
 };

     var dataAdapter1 = new $.jqx.dataAdapter(source, {
         contentType: 'application/json; charset=utf-8',
         downloadComplete: function (data, textStatus, jqXHR) {
             return data.d;

         },
         loadError: function (jqXHR, textStatus, errorThrown) {
             alert('HTTP status code: ' + jqXHR.status + '\n' +
                   'textStatus: ' + textStatus + '\n' +
                   'errorThrown: ' + errorThrown);
             alert('HTTP message body (jqXHR.responseText): ' + '\n' + jqXHR.responseText);
         }
     }
         );
     var imagerenderer = function (row, datafield, value) {
         return '<img style="margin-left: 5px;" height="60" width="60" src="' + value + '"/>';
     }
     $("#DataGridTrack").jqxGrid(
     {
         width: 750,
         height: 300,
         source: dataAdapter1,
         pageable: false,
         autoheight: false,
         showheader: true,
         rowsheight: 30,
         theme: 'bootstrap',
         columnsresize: true,
         columns: [
           { text: 'Spår.nr', datafield: 'nr', width: 55, cellsalign: 'left' },
           { text: 'Track', datafield: 'track', width: 400, cellsalign: 'left' }

         ]
     });
 }
 function getAlbumTracks(Artist) {
     var userId = "1";//localStorage.userID;
     //alert(Artist);
     var source = {
         type: "GET",
         datatype: "json",
         datafields: [
         { name: 'Cover', type: 'string' },
         { name: 'artist', type: 'string' },
         { name: 'album', type: 'string' },
         { name: 'Media', type: 'string' },
         { name: 'Ar', type: 'string' },
         { name: 'Kategori', type: 'string' },
         { name: 'discID', type: 'string' }
         ],
         url: 'http://cdmolnet.se/CDService.asmx/getAlbum?userId="' + userId + '"&Artist="' + Artist + '"',
         //url: 'http://localhost:1239/CDService.asmx/getAlbum?userId="' + userId + '"&Artist="' + Artist + '"',
         cache: false,
         root: 'data'
     };

     //Preparing the data for use
     var dataAdapter = new $.jqx.dataAdapter(source, {
         contentType: 'application/json; charset=utf-8',
         downloadComplete: function (data, textStatus, jqXHR) {
             return data.d;
         }
     }
     );
     //Detail grid***************************************************************
     var orders = {};
     var orderSource = {
         type: "GET",
         datatype: "json",
         datafields: [
             { name: 'nr', type: 'int' },
             { name: 'track', type: 'string' },
             { name: 'DiscID', type: 'string' }
         ],
         url: 'http://cdmolnet.se/CDService.asmx/getTracksByArtist?userId=' + userId + '&Artist="' + Artist + '"',
         //url: 'http://localhost:1239/CDService.asmx/getTracksByArtist?userId=' + userId + '&Artist="' + Artist + '"',
         cache: false,
         root: 'data'
     };
     var OrdersAdapter = new $.jqx.dataAdapter(orderSource, {
         contentType: 'application/json; charset=utf-8', autoBind: true,
         downloadComplete: function (data, textStatus, jqXHR) {

             orders.records = data.d;
             return data.d;
         },
             loadError: function (jqXHR, textStatus, errorThrown) {
                 alert('HTTP status code: ' + jqXHR.status + '\n' +
                       'textStatus: ' + textStatus + '\n' +
                       'errorThrown: ' + errorThrown);
                 alert('HTTP message body (jqXHR.responseText): ' + '\n' + jqXHR.responseText);
             }
     }
     );

     // create nested grid.
     var initrowdetails = function (index, parentElement, gridElement, record) {
         var id = record.discID.toString();
         var grid = $($(parentElement).children()[0]);
         var filtergroup = new $.jqx.filter();
         var filter_or_operator = 1;
         var filtervalue = id;
         var filtercondition = 'equal';
         var filter = filtergroup.createfilter('stringfilter', filtervalue, filtercondition);
         // fill the orders depending on the id.
         var ordersbyid = [];
         for (var m = 0; m < orders.records.length; m++) {
             var result = filter.evaluate(orders.records[m]["discID"]);
             if (result)
                 ordersbyid.push(orders.records[m]);
         }

         var orderssource = {
             datafields: [
                 { name: 'nr', type: 'int' },
                { name: 'track', type: 'string' },
                { name: 'discID', type: 'string' }
             ],
             id: 'discID',
             localdata: ordersbyid
         }

         if (grid != null) {
             grid.jqxGrid({
                 source: orderssource, width: 600, height: 900, showaggregates: false, showstatusbar: true,
                 columns: [
                   { text: 'Track.no', datafield: 'nr', width: 75 },
                   { text: 'Track', datafield: 'track', width: 250 }
                 ]
             });
         }
     }
     //Detail grid*****************************************************************
     var imagerenderer = function (row, datafield, value) {
         return '<img style="margin-left: 5px;" height="60" width="60" src="' + value + '"/>';
     }
     $("#DataGridAlbum").jqxGrid({
         source: dataAdapter,
         theme: 'bootstrap',
         rowdetails: true,
         rowsheight: 35,
         initrowdetails: initrowdetails,
         rowdetailstemplate: { rowdetails: "<div id='grid' style='margin: 10px;'></div>", rowdetailsheight: 220, rowdetailshidden: true },
         ready: function () {
             $("#DataGridAlbum").jqxGrid('showrowdetails');
         },
         columns: [
           { text: 'Cover', datafield: 'Cover', width: 60, cellsrenderer: imagerenderer },
           { text: 'Album', datafield: 'album', width: 200, cellsalign: 'left' },
           { text: 'Media', datafield: 'Media', width: 60, cellsalign: 'left' },
           { text: 'Kategori', datafield: 'Kategori', width: 100, cellsalign: 'left' },
           { text: 'Utgiven', datafield: 'Ar', width: 60, cellsalign: 'left' },
           { text: 'DiscID', datafield: 'discID', width: 200, cellsalign: 'left' }
         ]
     });
 }
 