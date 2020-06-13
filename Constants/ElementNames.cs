namespace Lombiq.ContentEditors.Constants
{
    public static class ElementNames
    {
        public static class AsyncEditorActions
        {
            public const string BlockName = "asyncEditorActions";

            public const string PostEditorElementName = BlockName + "__postEditor";
            public const string LoadEditorElementName = BlockName + "__loadEditor";
        }
        
        public static class AsyncEditorSubmitActions
        {
            public const string BlockName = "asyncEditorActions";

            public const string ActionElementName = BlockName + "__action";
            
            public const string PreviousActionElementName = BlockName + "__previous";
            public const string SaveActionElementName = BlockName + "__save";
            public const string NextActionElementName = BlockName + "__next";
            public const string PublishActionElementName = BlockName + "__publish";
            public const string CancelActionElementName = BlockName + "__cancel";
            public const string ResumeLaterActionElementName = BlockName + "__resumeLater"; // When saving without validation.
        }

        public static class AsyncEditorLoader
        {
            public const string BlockName = "asyncEditorLoader";

            public const string ProcessingIndicatorElementName = BlockName + "__processingIndicator";
            public const string MessagePlaceholderElementName = BlockName + "__message";
            public const string EditorPlaceholderElementName = BlockName + "__editor";
        }

        public static class LoadingIndicator
        {
            public const string BlockName = "loadingIndicator";

            public const string ContentElementName = BlockName + "__content";
            public const string HeaderElementName = BlockName + "__header";
            public const string ProgressBarElementName = BlockName + "__progressBar";
        }
    }
}