// Used to prevent accidental scrolling when screen state changes
var scrollPreventer = {
    previousTop: 0,

    saveTop: function (elId) {
        var rect = document.getElementById(elId).getBoundingClientRect();
        this.previousTop = rect.top;
        console.log("Saved " + this.previousTop);
    },
    restoreTop: function (elId) {
        var rect = document.getElementById(elId).getBoundingClientRect();
        var currentTop = rect.top;
        window.scrollBy(0, currentTop - this.previousTop);
        console.log("Restored " + this.previousTop);
    }
}

// iPhones seem to disconnect when leaving the web browser. This re-connects
// when the browser is re-opened
function reconnectIphone() {
    if (navigator.platform && /iPad|iPhone|iPod/.test(navigator.platform)) {
        DotNet.invokeMethodAsync('ScrabbleWeb.Client', 'RefreshConnectionAsync');
    }
}