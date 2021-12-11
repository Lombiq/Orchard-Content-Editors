using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace Lombiq.ContentEditors.Models
{
    public class AsyncEditorUpdateResult
    {
        public ModelStateDictionary ModelState { get; set; }
        public Func<ValueTask<string>> RenderedEditorShapeFactory { get; set; }
    }
}
