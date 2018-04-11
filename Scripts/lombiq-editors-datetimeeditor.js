/**
 * @summary     Lombiq - Editors - DateTime Editor
 * @description Workaround for using Moment.js date formatting for jQuery DatePicker.
 * @version     1.0
 * @file        lombiq-editors-datetimeeditor.js
 * @author      Lombiq Technologies Ltd.
 */

// See: https://stackoverflow.com/questions/24500726/replace-the-jquery-datepicker-dateformat-with-momentjs-parsing-and-format/24500727#24500727
$.datepicker.parseDate = function (format, value) {
    return moment(value, format).toDate();
};

$.datepicker.formatDate = function (format, value) {
    return moment(value).format(format);
};