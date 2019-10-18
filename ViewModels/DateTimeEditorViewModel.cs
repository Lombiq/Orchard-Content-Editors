using System;

namespace Lombiq.ContentEditors.ViewModels
{
    public enum DateTimeEditorType
    {
        Date,
        Time,
        DateWithTime // Use this to render a Date editor that has a separate Time editor.
    }


    public class DateTimeEditorViewModel : EditorViewModel
    {
        public DateTimeEditorType EditorType { get; set; } = DateTimeEditorType.Date;

        public string BackEndDateDisplayFormat { get; set; } = "MM/dd/yyyy";
        public string FrontEndDateDisplayFormat { get; set; } = "MM/DD/YYYY";
        public string FrontEndDateStoreFormat { get; set; } = "YYYY-MM-DD";
        public DateTime? Minimum { get; set; }
        public DateTime? Maximum { get; set; }

        public string BackEndTimeDisplayFormat { get; set; } = "HH:mm";
        public string FrontEndTimeDisplayFormat { get; set; } = "HH:mm";
        public string FrontEndTimeStoreFormat { get; set; } = "hh:mm:ss A";
    }
}