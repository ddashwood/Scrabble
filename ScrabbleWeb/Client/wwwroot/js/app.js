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