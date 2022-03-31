using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Lombiq.ContentEditors.Services;

/// <summary>
/// A convenience bundle of the common dependencies in ContentItemAsyncEditorProvider services.
/// </summary>
[SuppressMessage(
    "StyleCop.CSharp.DocumentationRules",
    "SA1600:Elements should be documented",
    Justification = "There is nothing to add past what's already on the individual services' documentations.")]
[SuppressMessage(
    "StyleCop.CSharp.DocumentationRules",
    "SA1618:The documentation for type parameter 'T' is missing",
    Justification = "There is nothing to add past what's already on the individual services' documentations.")]
public interface IContentItemAsyncEditorProviderServices<T>
    where T : IAsyncEditorProvider<ContentItem>
{
    Lazy<IContentManager> ContentManager { get; }
    Lazy<IContentItemDisplayManager> ContentItemDisplayManager { get; }
    Lazy<IDisplayHelper> DisplayHelper { get; }
    Lazy<IShapeFactory> ShapeFactory { get; }
    Lazy<IUpdateModelAccessor> UpdateModelAccessor { get; }
    Lazy<IStringLocalizer<T>> StringLocalizer { get; }
    Lazy<IAuthorizationService> AuthorizationService { get; }
    Lazy<IHttpContextAccessor> HttpContextAccessor { get; }
}
