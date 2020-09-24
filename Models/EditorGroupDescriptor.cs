namespace Lombiq.ContentEditors.Models
{
    public class EditorGroupDescriptor
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public bool IsPublishGroup { get; set; }


        public override bool Equals(object other) =>
            other is EditorGroupDescriptor otherGroup ? otherGroup.Name == Name : base.Equals(other);

        public override int GetHashCode() => Name.GetHashCode();
    }
}