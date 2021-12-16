using Lombiq.ContentEditors.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.ContentEditors.Services
{
    public interface IAsyncEditorProvider<T>
        where T : class
    {
        string Name { get; }

        Task<IEnumerable<AsyncEditorGroupDescriptor<T>>> DescribeEditorGroupsAsync();
        Task<string> RenderEditorAsync(AsyncEditorContext<T> context);
        Task<AsyncEditorUpdateResult> UpdateEditorAsync(AsyncEditorContext<T> context);
    }

    public static class AsyncEditorProviderExtensions
    {
        public static async Task<bool> CanRenderEditorGroupAsync<T>(this IAsyncEditorProvider<T> provider, AsyncEditorContext<T> context)
            where T : class
        {
            var groupDescriptor = (await provider.DescribeEditorGroupsAsync())
                ?.FirstOrDefault(descriptor => descriptor.Name == context.EditorGroup);

            if (groupDescriptor == null) return false;
            return await groupDescriptor.IsAccessibleAsync(context);
        }

        public static async Task<IEnumerable<string>> GetAllEditorGroupsAsync<T>(this IAsyncEditorProvider<T> provider)
            where T : class =>
            (await provider.DescribeEditorGroupsAsync()).Select(descriptor => descriptor.Name);
    }
}
