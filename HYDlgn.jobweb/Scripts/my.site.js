String.prototype.escapeUrlStr = function () {
    if (this && this.length > 0) {
        return this.replace("&", "`3").replace("/", "`8").replace("\\", "`1").replace("!", "`2");
    }
    return "";
}

String.prototype.format = function () {
    var args = arguments;
    return this.replace(/{([0-9]+)}/g, function (match, index) {
        return typeof args[index] == 'undefined' ? match : args[index];
    });
};

$(document).ready(function () {
    $('.loading-wrapper').addClass('hide');

});