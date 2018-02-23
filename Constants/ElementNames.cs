namespace Lombiq.EditorGroups.Constants
{
    public static class ElementNames
    {
        public class AsyncEditor
        {
            public const string BlockName = "asyncEditor";
            public const string EditorElementName = BlockName + "__editor";
        }

        public class AsyncEditorActions
        {
            public const string BlockName = "asyncEditorActions";
            public const string PostEditorElementName = BlockName + "__postEditor";
            public const string LoadEditorElementName = BlockName + "__loadEditor";
        }

        public class AsyncEditorPager
        {
            public const string BlockName = "asyncEditorPager";
            public const string GroupEditorLinkElementName = BlockName + "__groupEditorLink";
        }

        public class AsyncEditorForm
        {
            public const string BlockName = "asyncEditorForm";
        }
    }
}