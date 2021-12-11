using Lombiq.ContentEditors.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lombiq.ContentEditors.Services
{
    public interface IAsyncEditorProvider<T>
        where T : class
    {
        string Name { get; }

        Task<IEnumerable<AsyncEditorGroup>> GetOrderedEditorGroupsAsync(AsyncEditorContext<T> context);
        Task<bool> CanRenderEditorGroupAsync(AsyncEditorContext<T> context);
        Task<string> RenderEditorGroupAsync(AsyncEditorContext<T> context);
        Task<ModelStateDictionary> UpdateEditorAsync(AsyncEditorContext<T> context);
    }
}
