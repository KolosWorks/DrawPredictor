if (typeof KolosWorks.DrawPredicter === "undefined") {
    KolosWorks.DrawPredicter = {};
}
console.log('KolosWorks', KolosWorks);

KolosWorks.DrawPredicter.DefaultPage = KolosWorks.Util.Class(KolosWorksBase.PageControl, {
    mainContainerDiv: null,
    contentDiv: null,    
    loadingImage: null,

    construct: function (eventArgs) {
        this.mainContainerDiv = $('#' + eventArgs.mainContainerDivId);
        var contentDivId = "contentDiv";
        var doAjaxButtonId = "doAjaxButton";

        this.contentDiv = $('<div/>', {
            id: contentDivId
        }).appendTo(this.mainContainerDiv);

        $('<input/>', {
            id: doAjaxButtonId,
            type: 'button',
            value: 'Get Matches!',
            click: KolosWorks.Util.createDelegate(this, this.doAjaxButtonClickHandler)
        }).appendTo(this.mainContainerDiv);

        this.loadingImage = $('<img/>', {
            id: 'loadingImage',
            class: 'loadingImage',
            src: 'Images/Loading.gif'
        }).appendTo(this.mainContainerDiv).hide();
    },
    doAjaxButtonClickSuccessCallback: function (data) {
        this.loadingImage.hide(200);

        this.constructTableContent(data.d);
    },
    doAjaxButtonClickFailureCallback: function (eventArgs) {
        this.loadingImage.hide(200);

        alert('FAIL');
    },
    doAjaxButtonClickHandler: function (eventArgs) {
        this.loadingImage.show(200);

        KolosWorks.Util.doAjax('GetMatches', '{}', this.doAjaxButtonClickSuccessCallback, this.doAjaxButtonClickFailureCallback, this);

        //KolosWorks.doAjax('/Default.aspx', 'TestMethod', '{message: "Hello" }', this.doAjaxButtonClickSuccessCallback, this.doAjaxButtonClickFailureCallback, 'post', this);
        //KolosWorks.doAjax('/Default.aspx', 'TestMethod', '{message: "Hello" }', this.doAjaxButtonClickSuccessCallback, this.doAjaxButtonClickFailureCallback, this);
        //KolosWorks.doAjax('TestMethod', '{message: "Hello" }', this.doAjaxButtonClickSuccessCallback, this.doAjaxButtonClickFailureCallback, 'post', this);
    },
    constructTableContent: function (data) {
        for (var i = 0; i < data.length; i++) {
            var rowDiv = $('<div/>', {
                id: 'matchRow_' + i,
                class: 'matchRow'
            }).appendTo(this.contentDiv);

            rowDiv.css({ 'top': i*20 + 'px' });

            $('<div/>', {
                id: 'dateCell_' + i,
                class: 'dateCell',
                text: this.getNiceDateFormat(data[i].MatchDate)
            }).appendTo(rowDiv);

            $('<div/>', {
                id: 'homeTeamCell_' + i,
                class: 'homeTeamCell',
                text: data[i].HomeTeam
            }).appendTo(rowDiv);

            $('<div/>', {
                id: 'versusCell_' + i,
                class: 'versusCell',
                text: '-'
            }).appendTo(rowDiv);

            $('<div/>', {
                id: 'awayTeamCell_' + i,
                class: 'awayTeamCell',
                text: data[i].AwayTeam
            }).appendTo(rowDiv);

            $('<div/>', {
                id: 'drawScoreCell_' + i,
                class: 'drawScoreCell',
                text: (data[i].HomeTeamDrawRate + data[i].AwayTeamDrawRate) / 2
            }).appendTo(rowDiv);

            $('<div/>', {
                id: 'percentCell_' + i,
                class: 'percentCell',
                text: '%'
            }).appendTo(rowDiv);
        }
    },
    getNiceDateFormat: function (matchDate) {
        var parts = matchDate.split('-');
        var d = new Date(parts[2], parseInt(parts[1]) - 1, parts[0]);

        var weekDay = new Intl.DateTimeFormat('en', { weekday: 'long' }).format(d);
        var day = new Intl.DateTimeFormat('en', { day: '2-digit' }).format(d);
        var month = new Intl.DateTimeFormat('en', { month: 'short' }).format(d);
        //var year = new Intl.DateTimeFormat('en', { year: 'numeric' }).format(d);

        return `${weekDay}, ${day} ${month}`;
    },
});
