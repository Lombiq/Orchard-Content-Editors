window.asyncEditor = {};

window.asyncEditor.editor = {
    template: '#async-editor-template',
    props: ['apiUrl', 'initialEditorGroup'],
    data() {
        return {
            editorHtml: '',
            errorText: '',
            editorGroup: '',
            isLoading: false,
        };
    },
    methods: {
        initEditor() {
            return this.loadEditorGroup(this.editorGroup);
        },
        loadEditorGroup(editorGroup) {
            const self = this;
            console.log('loading');
            self.editorGroup = editorGroup;
            self.isLoading = true;
            return fetch(this.apiUrl)
                .then((response) => response.json())
                .then((data) => {
                    self.editorHtml = data.editorHtml;
                    self.isLoading = false;
                })
                .catch((error) => {
                    self.errorText = error;
                    self.isLoading = false;
                });
        },
    },
};
