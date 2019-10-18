using System;

namespace Lombiq.ContentEditors.ViewModels
{
    public enum DateTimeEditorType
    {
        Date,
        Time,
        DateWithTimeZoneConversion,
        TimeWithTimeZoneConversion
    }


    public class DateTimeEditorViewModel : EditorViewModel
    {
        public DateTimeEditorType EditorType { get; set; } = DateTimeEditorType.Date;

        public string DateFormat { get; set; } = "yyyy-MM-dd";
        public string FrontEndDateDisplayFormat { get; set; } = "MM/DD/YYYY";
        public string FrontEndDateStoreFormat { get; set; } = "YYYY-MM-DD";
        public DateTime? Minimum { get; set; }
        public DateTime? Maximum { get; set; }

        public string TimeFormat { get; set; } = "hh:mm:ss tt";
        public string FrontEndTimeDisplayFormat { get; set; } = "hh:mm A";
        public string FrontEndTimeStoreFormat { get; set; } = "hh:mm:ss A";
    }
}