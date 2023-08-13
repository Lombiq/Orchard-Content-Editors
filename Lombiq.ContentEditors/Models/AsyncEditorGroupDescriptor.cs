using System;
using System.Threading.Tasks;

namespace Lombiq.ContentEditors.Models;

public class AsyncEditorGroupDescriptor<T>
    where T : class
{
    public string Name { get; set; }
    public string DisplayText { get; set; }
    public bool IsPublishGroup { get; set; }
    public Func<AsyncEditorContext<T>, ValueTask<bool>> IsAccessibleAsync { get; set; }
    public Func<AsyncEditorContext<T>, ValueTask<bool>> IsFilledAsync { get; set; }
}
