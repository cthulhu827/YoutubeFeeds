async function getCurrentTab() {
    let queryOptions = { active: true, lastFocusedWindow: true };
    let [tab] = await chrome.tabs.query(queryOptions);
    return tab;
};

function VM_ButtonWithStatus(url, requestBuilder, buttonSelector, spanSelector, statusTexts) {
    var self = this;

    self.url = url;
    self.requestBuilder = requestBuilder;
    self.buttonSelector = buttonSelector;
    self.spanSelector = spanSelector;
    self.statusTexts = statusTexts;

    self.applyClasses = function(newStatus) {
        const knownStatuses = ["ok", "fail", "loading"];
        for (var theStatus of knownStatuses) {
            if (theStatus == newStatus)
                $(self.spanSelector).addClass("status-" + theStatus);
            else
                $(self.spanSelector).removeClass("status-" + theStatus);
        }
    };
    
    self.resetStatusByTimeout = function() {
        setTimeout(function () { self.setStatus(""); }, 2000);
    };

    self.setStatus = function(newStatus) {
        self.applyClasses(newStatus);
        switch (newStatus) {
            case "loading":
                $(self.spanSelector).text(self.statusTexts[1]);
                break;
            case "ok":
                $(self.spanSelector).text(self.statusTexts[2]);
                self.resetStatusByTimeout();
                break;
            case "fail":
                $(self.spanSelector).text(self.statusTexts[3]);
                self.resetStatusByTimeout();
                break;
            default:
                $(self.spanSelector).text(self.statusTexts[0]);
                break;
        }
    };

    self.click = function() {
        self.setStatus("loading")
        var request = requestBuilder();
        $.ajax({
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            'type': 'POST',
            'url': self.url,
            'data': JSON.stringify(request),
            'dataType': 'json',
            'success': function (response) {
                self.setStatus("ok");
            },
            'error': function () {
                self.setStatus("fail");
            }
        });
    };

    $(self.buttonSelector).on("click", self.click);
};

let tab = await getCurrentTab();

var btnSubscribe = new VM_ButtonWithStatus(
    "http://localhost:5000/api/videos/subscribe_channel",
    function () {
        return { "videoUrl": tab.url };
    },
    "#subscribe_button",
    "#subscribe_span",
    [
        "Click the button to subscribe to the channel",
        "Subscribing to the channel...",
        "Successfully subscribed to the channel",
        "Failed to subscribe to the channel"
    ]);

var btnMarkAsViewed = new VM_ButtonWithStatus(
    "http://localhost:5000/api/videos/update_status",
    function () {
        return { "videoUrl": tab.url, "status": 2 };
    },
    "#viewed_button",
    "#viewed_span",
    [
        "Click the button to mark the video as viewed",
        "Marking video as viewed...",
        "Successfully marked video as viewed",
        "Failed to mark video as viewed"
    ]);