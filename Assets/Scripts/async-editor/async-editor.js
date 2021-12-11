window.asyncEditor = { editors: [] };

const apiClient = {
    apiUrl: '',
    providerId: '',
    loadEditor(contentId, editorGroup, callback) {
        return fetch(this.apiUrl + '/' + contentId + '/' + editorGroup)
            .then((response) => response.json())
            .then((data) => {
                callback(true, data);
            })
            .catch((error) => {
                callback(false, error);
            });
    },
    submitEditor(contentId, editorGroup, formData, callback) {
        for (var pair of formData.entries()) {
            console.log(pair[0]+ ', ' + pair[1]);
        }
        return fetch(this.apiUrl + '/' + contentId + '/' + editorGroup, {
            method: 'post',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded;charset=UTF-8'
            },
            body: new URLSearchParams(formData) })
            .then((response) => response.json())
            .then((data) => {
                callback(true, data);
            })
            .catch((error) => {
                callback(false, error);
            });
    }
}

window.asyncEditor.editor = {
    template: '#async-editor-template',
    data() {
        return {
            api: apiClient,
            providerId: '',
            contentId: '',
            editorGroup: '',
            editorHtml: '',
            errorText: '',
            isLoading: false,
        };
    },
    methods: {
        initEditor(apiUrl, providerId, contentId, editorGroup) {
            const self = this;

            self.api.apiUrl = apiUrl;
            self.api.providerId = providerId;
            self.contentId = contentId;
            self.editorGroup = editorGroup;

            self.loadEditor();
        },
        loadEditor(editorGroup, callback) {
            const self = this;

            self.editorGroup = editorGroup ?? self.editorGroup;
            self.isLoading = true;

            self.api.loadEditor(self.contentId, self.editorGroup, (success, data) => {
                self.isLoading = false;
                if (success) {
                    self.editorHtml = data.editorHtml;
                    self.editorGroup = data.editorGroup;
                }
                else {
                    self.errorText = data;
                }
            });
        },
        saveAndLoadEditor(editorGroup) {
            const self = this;

            if (editorGroup !== self.editorGroup) {
                self.submitEditor(() => self.loadEditor(editorGroup));
            }
            else {
                self.loadEditor(editorGroup);
            }
        },
        submitEditor(callback) {
            const self = this;

            console.log(self.editorGroup);
            self.api.submitEditor(self.contentId, self.editorGroup, new FormData(self.$refs.editorForm), (success, data) => {
                callback();
            });
        },
    },
};

function initAsyncEditor(appId, parameters) {
    if (!parameters) return;

    window.asyncEditor.editors[appId] = new Vue({
        el: parameters.element,
        data: {
            appId: appId,
        },
        methods: {
        },
        mounted() {
            this.$refs.editor.initEditor(
                parameters.apiUrl,
                parameters.providerId,
                parameters.contentId,
                parameters.editorGroup);
        },
        components: {
            "async-editor": window.asyncEditor.editor
        },
        template: '<async-editor ref="editor"></async-editor>',
    });
}
