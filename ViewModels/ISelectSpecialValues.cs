namespace Lombiq.ContentEditors.ViewModels
{
    /// <summary>
    /// Indicates special values within a list of selectable values.
    /// </summary>
    public interface ISelectSpecialValues
    {
        /// <summary>
        /// Id of the selectable value that indicates that no meaningful value can be selected.
        /// </summary>
        string NoneValueId { get; set; }

        /// <summary>
        /// Id of the selectable value that indicates that a custom value can be entered.
        /// </summary>
        string OtherValueId { get; set; }
    }
}
