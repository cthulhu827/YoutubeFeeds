async function getCurrentTab() {
    let queryOptions = { active: true, lastFocusedWindow: true };
    let [tab] = await chrome.tabs.query(queryOptions);
    return tab;
};

function applyClasses(newStatus) {
    const knownStatuses = ["ok", "fail", "loading"];
    for (var theStatus of knownStatuses) {
        if (theStatus == newStatus)
            $("#the_span").addClass("status-" + theStatus);
        else
            $("#the_span").removeClass("status-" + theStatus);
    }
};

function resetStatusByTimeout() {
    setTimeout(function () { setStatus(""); }, 2000);
};

function setStatus(newStatus) {
    applyClasses(newStatus);
    switch (newStatus) {
        case "loading":
            $("#the_span").text("Subscribing to the channel...");
            break;
        case "ok":
            $("#the_span").text("Successfully subscribed to the channel");
            resetStatusByTimeout();
            break;
        case "fail":
            $("#the_span").text("Failed to subscribe to the channel");
            resetStatusByTimeout();
            break;
        default:
            $("#the_span").text("Click the button to subscribe to the channel");
            break;
    }
};

function subscribe() {
    setStatus("loading")
    var request = { "videoUrl": tab.url };
    $.ajax({
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        'type': 'POST',
        'url': "http://localhost:5000/api/videos/subscribe_channel",
        'data': JSON.stringify(request),
        'dataType': 'json',
        'success': function (response) {
            setStatus("ok");
        },
        'error': function () {
            setStatus("fail");
        }
    });
};

let tab = await getCurrentTab();

$("#the_button").on("click", subscribe);