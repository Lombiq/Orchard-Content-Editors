namespace Lombiq.ContentEditors.ViewModels
{
    public interface IElementValueFilterRelationshipSelector
    {
        bool IsFilterRelationShipSelectorEnabled { get; set; }
        FilterRelationship[] EnabledFilterRelationShips { get; set; }
    }

    public enum FilterRelationship
    {
        Or,
        And
    }
}