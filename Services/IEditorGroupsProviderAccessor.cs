using Orchard;

namespace Lombiq.ContentEditors.Services
{
    public interface IEditorGroupsProviderAccessor : IDependency
    {
        IEditorGroupsProvider GetProvider(string contentType);
    }
}