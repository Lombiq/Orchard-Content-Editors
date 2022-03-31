using Lombiq.ContentEditors.Services;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;

namespace Lombiq.ContentEditors.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a content item async editor provider to the service collection.
    /// </summary>
    public static void AddContentItemAsyncEditorProvider<TProvider>(this IServiceCollection services)
        where TProvider : class, IAsyncEditorProvider<ContentItem> =>
        services.AddScoped<IAsyncEditorProvider<ContentItem>, TProvider>();

    /// <summary>
    /// Adds an async editor provider to the service collection while maintaining the call chain for the
    /// content part builder.
    /// </summary>
    public static ContentPartOptionBuilder WithAsyncEditor<TProvider>(this ContentPartOptionBuilder builder)
        where TProvider : class, IAsyncEditorProvider<ContentItem>
    {
        builder.Services.AddScoped<IAsyncEditorProvider<ContentItem>, TProvider>();

        return builder;
    }
}
