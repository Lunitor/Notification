export class StringHtml {
    static replaceNewLineWithBrTag(text: string): string {
        return text.replace(new RegExp("\r\n", 'g'), "<br />")
            .replace(new RegExp("\n", 'g'), "<br />")
            .replace(new RegExp("\r", 'g'), "<br />");
    }
}