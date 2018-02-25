﻿namespace Lombiq.EditorGroups.Models
{
    public class AsyncEditorResult
    {
        public int ContentItemId { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string EditorShape { get; set; }
        public string EditorGroup { get; set; }
    }
}