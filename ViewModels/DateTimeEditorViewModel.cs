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
        public string FrontEndDateDisplayFormat { get; set; } = "MM/DD/YYYY";
        public string FrontEndDateStoreFormat { get; set; } = "YYYY-MM-DD";
        public string DateFormat { get; set; } = "yyyy-MM-dd";
        public DateTime? Minimum { get; set; }
        public DateTime? Maximum { get; set; }
    }
}