window.asyncEditor = { editors: [] };

class AsyncEditorApiClient {
    constructor(parameters) {
        this.apiUrl = parameters.apiUrl;
        this.asyncEditorId = parameters.asyncEditorId;
        this.providerName = parameters.providerName;
        this.contentType = parameters.contentType;
    }

    loadEditor(contentId, editorGroup, callback) {
        return fetch(this.createUrl(contentId, editorGroup))
            .then((response) => response.json())
            .then((data) => {
                callback(true, data);
            })
            .catch((error) => {
                callback(false, error);
            });
    }

    submitEditor(contentId, editorGroup, nextEditorGroup, formData, callback) {
        for (var pair of formData.entries()) {
            console.log(pair[0]+ ', ' + pair[1]);
        }
        return fetch(this.createUrl(contentId, editorGroup, nextEditorGroup), {
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

    createUrl(contentId, editorGroup, nextEditorGroup) {
        const url = new URL(this.apiUrl, document.baseURI);
        const query = {
            asyncEditorId: this.asyncEditorId,
            providerName: this.providerName,
            contentType: this.contentType,
        };
        if (contentId) query.contentId = contentId;
        if (editorGroup) query.editorGroup = editorGroup;
        if (nextEditorGroup) query.nextEditorGroup = nextEditorGroup;
        url.search = new URLSearchParams(query).toString();

        return url;
    }
}

const router = new VueRouter();

window.asyncEditor.editor = {
    template: '#async-editor-template',
    data() {
        return {
            asyncEditorId: '',
            api: null,
            message: '',
            errorText: '',
            contentId: '',
            editorHtml: '',
            validationSummaryHtml: '',
            editorGroup: '',
            editorGroups: [],
            defaultErrorText: '',
            scriptsHtml: ''
        };
    },
    computed: {
        progress(self) {
            if (self.editorGroups.length < 1) return 0;
            return self.editorGroups.filter(group => group.isFilled).length / self.editorGroups.length * 100;
        },
        showProgressBar(self) {
            return self.editorGroups.length > 1;
        }
    },
    watch: {
        '$route.query': function(newVal, oldVal) {
            this.processQuery();
        }
    },
    router,
    updated() {
        const self = this;
        if (self.scriptsHtml) {
            self.scriptsHtml.match(/(?<=<script>).*?(?=<\/script>)/gms).forEach(function (match) {
                window.eval(match);
            });
            self.scriptsHtml = '';
        }
    },
    methods: {
        initEditor(parameters) {
            const self = this;

            self.api = new AsyncEditorApiClient(parameters)
            self.contentId = parameters.contentId;
            self.editorGroup = parameters.editorGroup;
            self.defaultErrorText = parameters.defaultErrorText ?? 'Something went wrong.';
            self.asyncEditorId = parameters.asyncEditorId;

            if (!self.processQuery()) self.loadEditor();
        },
        loadEditor(editorGroup) {
            const self = this;

            self.editorGroup = editorGroup ?? self.editorGroup;

            self.api.loadEditor(
                self.contentId,
                self.editorGroup,
                (success, data) => { self.processApiData(success, data); });
        },
        submitEditor(nextEditorGroup) {
            const self = this;

            self.api.submitEditor(
                self.contentId,
                self.editorGroup,
                nextEditorGroup,
                new FormData(self.$refs.editorForm),
                (success, data) => { self.processApiData(success, data); });
        },
        processApiData(success, data) {
            const self = this;

            if (success) {
                const shouldUpdateQuery = self.contentId !== data.contentId || self.editorGroup !== data.editorGroup;

                self.errorText = '';
                self.validationSummaryHtml = data.validationSummaryHtml;
                self.contentId = data.contentId;
                self.editorHtml = data.editorHtml;
                self.editorGroup = data.editorGroup;
                self.editorGroups = data.editorGroups;
                self.scriptsHtml = data.scriptsHtml;
                self.message = data.message;

                if (shouldUpdateQuery) self.updateQuery();
            }
            else {
                self.errorText = self.defaultErrorText;
            }
        },
        updateQuery() {
            const self = this;

            const query = { ...self.$route.query };
            query[self.asyncEditorId + '.contentId'] = self.contentId;
            query[self.asyncEditorId + '.editorGroup'] = self.editorGroup;
            router.push({ path: '/', query: query});
        },
        processQuery() {
            const self = this;

            let shouldLoadEditor = false;

            const contentIdKey = self.asyncEditorId + '.contentId';
            if (self.$route.query.hasOwnProperty(contentIdKey) &&
                self.$route.query[contentIdKey] !== self.contentId)
            {
                self.contentId = self.$route.query[contentIdKey];
                shouldLoadEditor = true;
            }

            const editorGroupKey = self.asyncEditorId + '.editorGroup';
            if (self.$route.query.hasOwnProperty(editorGroupKey) &&
                self.$route.query[editorGroupKey] !== self.contentId)
            {
                self.editorGroup = self.$route.query[editorGroupKey];
                shouldLoadEditor = true;
            }

            if (shouldLoadEditor) {
                self.loadEditor();
            }

            return shouldLoadEditor;
        },
        isCurrentGroup(editorGroup) {
            return editorGroup === this.editorGroup;
        },
        isLastGroup(editorGroup) {
            return this.editorGroups.at(-1)?.name === (editorGroup ?? this.editorGroup)
        },
        getNextEditor(editorGroup) {
            const editorGroups = this.editorGroups.map(group => group.name);
            const index = editorGroups.indexOf(editorGroup ?? this.editorGroup);
            return editorGroups[index + 1];
        }
    },
};

function initAsyncEditor(asyncEditorId, parameters) {
    if (!parameters) return;

    window.asyncEditor.editors[asyncEditorId] = new Vue({
        el: parameters.element,
        mounted() {
            parameters.asyncEditorId = asyncEditorId;
            this.$refs.editor.initEditor(parameters);
        },
        components: {
            "async-editor": window.asyncEditor.editor
        },
        template: '<async-editor ref="editor"></async-editor>',
    });
}
