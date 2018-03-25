namespace Lombiq.ContentEditors.ViewModels
{
    public enum DateTimeEditorType
    {
        Date,
        Time,
        DateWithTimeZoneConversion
    }


    public class DateTimeEditorViewModel : EditorViewModel
    {
        public DateTimeEditorType EditorType { get; set; } = DateTimeEditorType.Date;
        public string DateDisplayFormat { get; set; } = "mm/dd/yy";
    }
}