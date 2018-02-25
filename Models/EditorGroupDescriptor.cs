using System.Collections.Generic;

namespace Lombiq.EditorGroups.Models
{
    public class EditorGroupDescriptor
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public bool PublishGroup { get; set; }


        public override bool Equals(object other)
        {
            var otherGroup = other as EditorGroupDescriptor;
            if (otherGroup == null) return base.Equals(other);

            return otherGroup.Name == Name;
        }

        public override int GetHashCode() =>
            Name.GetHashCode();
    }
}