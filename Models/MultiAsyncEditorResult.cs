using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lombiq.ContentEditors.Models
{
    public class MultiAsyncEditorResult
    {
        public int Id { get; set; }
        public dynamic ContentToDisplay { get; set; }
        public string SuccessMessage { get; set; }
        public bool SuccessfulSave { get; set; }
    }
}