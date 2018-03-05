namespace Lombiq.ContentEditors.Models
{
    public class EditorGroupDescriptor
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public bool IsPublishGroup { get; set; }


        public override bool Equals(object other)
        {
            var otherGroup = other as EditorGroupDescriptor;

            return otherGroup == null ? base.Equals(other) : otherGroup.Name == Name;
        }

        public override int GetHashCode() => Name.GetHashCode();
    }
}