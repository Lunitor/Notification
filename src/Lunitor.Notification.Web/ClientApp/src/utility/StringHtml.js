"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.StringHtml = void 0;
var StringHtml = /** @class */ (function () {
    function StringHtml() {
    }
    StringHtml.replaceNewLineWithBrTag = function (text) {
        return text.replace(new RegExp("\r\n", 'g'), "<br />")
            .replace(new RegExp("\n", 'g'), "<br />")
            .replace(new RegExp("\r", 'g'), "<br />");
    };
    return StringHtml;
}());
exports.StringHtml = StringHtml;
//# sourceMappingURL=StringHtml.js.map