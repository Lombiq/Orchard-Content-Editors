namespace Lombiq.EditorGroups.Constants
{
    public static class ElementNames
    {
        public static class AsyncEditorActions
        {
            public const string BlockName = "asyncEditorActions";
            public const string PostEditorElementName = BlockName + "__postEditor";
            public const string LoadEditorElementName = BlockName + "__loadEditor";
        }

        public static class AsyncEditorLoader
        {
            public const string BlockName = "asyncEditorLoader";
            public const string ProcessingIndicatorElementName = BlockName + "__processingIndicator";
            public const string EditorPlaceholderElementName = BlockName + "__editor";
        }
    }
}