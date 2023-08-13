using Lombiq.ContentEditors.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.ContentEditors.Services;

/// <summary>
/// Manages operations like rendering or updating editors related to an async editor.
/// </summary>
/// <typeparam name="T">Type of the object being edited by an async editor.</typeparam>
public interface IAsyncEditorProvider<T>
    where T : class
{
    /// <summary>
    /// Gets the technical name of the async editor.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Provides information about the editor groups.
    /// </summary>
    /// <returns>A list of editor group information.</returns>
    Task<IEnumerable<AsyncEditorGroupDescriptor<T>>> DescribeEditorGroupsAsync();

    /// <summary>
    /// Returns the HTML code of an editor that can be rendered in an async editor.
    /// </summary>
    /// <param name="context">Information about the current editing context such as the ID of the editor and the object being edited.</param>
    /// <returns>HTML code that can be rendered in an async editor.</returns>
    Task<string> RenderEditorAsync(AsyncEditorContext<T> context);

    /// <summary>
    /// Updates an editor by binding the fields to the given data model.
    /// </summary>
    /// <param name="context">Information about the current editing context such as the ID of the editor and the object being edited.</param>
    /// <returns>Result of the editor update including potential validation result and helpers to render the updated editor.</returns>
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
