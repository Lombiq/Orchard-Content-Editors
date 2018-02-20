using Orchard;

namespace Lombiq.EditorGroups.Services
{
    public interface IEditorGroupsProviderAccessor : IDependency
    {
        IEditorGroupsProvider GetProvider(string contentType);
    }
}