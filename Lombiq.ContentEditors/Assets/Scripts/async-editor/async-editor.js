/* eslint-disable import/no-unresolved -- ESLint does not know where to find external modules. */
import { createApp, defineComponent } from 'vue';
import { createRouter, createWebHistory } from 'vue-router';
/* eslint-enable import/no-unresolved */

class AsyncEditorApiClient {
    constructor(parameters) {
        this.apiUrl = parameters.apiUrl;
        this.asyncEditorId = parameters.asyncEditorId;
        this.providerName = parameters.providerName;
        this.contentType = parameters.contentType;
    }

    async fetchEditor(callback, contentId, editorGroup, nextEditorGroup, requestOptions, raiseEvent) {
        try {
            const response = await fetch(this.createUrl(contentId, editorGroup, nextEditorGroup), requestOptions);
            const data = await response.json();
            const success = data.type !== 'Error';

            callback(success, data);
            if (!success) return;

            if (raiseEvent) {
                const submittedEditorEvent = new CustomEvent('asyncEditorSubmittedEditor', {
                    bubbles: true,
                    cancelable: true,
                    detail: { asyncEditor: window.asyncEditor },
                });
                document.dispatchEvent(submittedEditorEvent);
            }
        }
        catch (error) {
            callback(false, error);
        }
    }

    loadEditor(contentId, editorGroup, callback) {
        return this.fetchEditor(callback, contentId, editorGroup);
    }

    submitEditor(contentId, editorGroup, nextEditorGroup, formData, callback) {
        const requestOptions = {
            method: 'post',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded;charset=UTF-8',
            },
            body: new URLSearchParams(formData),
        };
        return this.fetchEditor(callback, contentId, editorGroup, nextEditorGroup, requestOptions, true);
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

export default function initAsyncEditor(asyncEditorId, parameters) {
    if (!parameters) return;

    if (!window.asyncEditor) window.asyncEditor = { editors: [] };

    const basePath = window.location.pathname.split('?')[0];
    const router = createRouter({
        history: createWebHistory(basePath),
        routes: [
            {
                path: '/:pathMatch(.*)*',
                name: 'any',
                component: {
                    template: '<div></div>',
                },
            },
        ],
    });

    const asyncEditorComponent = defineComponent({
        template: '#async-editor-template',
        data: function () {
            return {
                asyncEditorId: '',
                api: null,
                message: '',
                errorText: '',
                errorJson: '',
                contentId: '',
                editorHtml: '',
                validationSummaryHtml: '',
                editorGroup: '',
                editorGroups: [],
                defaultErrorText: '',
                scriptsHtml: '',
            };
        },
        computed: {
            progress() {
                return this.editorGroups.length < 1 ? 0 : (this.editorGroups.filter((g) => g.isFilled).length / this.editorGroups.length) * 100;
            },
            showProgressBar() {
                return this.editorGroups.length > 1;
            },
        },
        watch: {
            '$route.query'() {
                this.processQuery();
            },
        },
        updated: function () {
            if (this.scriptsHtml) {
                const scripts = new DOMParser()
                    .parseFromString(this.scriptsHtml, 'text/html')
                    .getElementsByTagName('script');
                for (let i = 0; i < scripts.length; i++) {
                    const script = document.createElement('script');
                    script.text = scripts[i].text;
                    document.head.appendChild(script).parentNode.removeChild(script);
                }
                this.scriptsHtml = '';
            }
        },
        methods: {
            initEditor(editorParameters) {
                this.api = new AsyncEditorApiClient(editorParameters);
                this.contentId = editorParameters.contentId;
                this.editorGroup = editorParameters.editorGroup;
                this.defaultErrorText = editorParameters.defaultErrorText ?? 'Something went wrong.';
                this.asyncEditorId = editorParameters.asyncEditorId;

                if (!this.processQuery()) this.loadEditor();
            },
            loadEditor(editorGroup) {
                this.editorGroup = editorGroup ?? this.editorGroup;
                this.api.loadEditor(this.contentId, this.editorGroup, (success, data) => {
                    this.processApiData(success, data);
                });
            },
            submitEditor(nextEditorGroup) {
                const submittingEditorEvent = new CustomEvent('asyncEditorSubmittingEditor', {
                    bubbles: true,
                    cancelable: true,
                    detail: { asyncEditor: window.asyncEditor },
                });

                const successful = document.dispatchEvent(submittingEditorEvent);
                if (successful) {
                    this.api.submitEditor(
                        this.contentId,
                        this.editorGroup,
                        nextEditorGroup,
                        new FormData(this.$refs.editorForm),
                        (success, data) => this.processApiData(success, data)
                    );
                }
            },
            processApiData(success, data) {
                if (success) {
                    const shouldUpdateQuery = this.contentId !== data.contentId || this.editorGroup !== data.editorGroup;

                    this.errorText = '';
                    this.validationSummaryHtml = data.validationSummaryHtml;
                    this.contentId = data.contentId;
                    this.editorHtml = data.editorHtml;
                    this.editorGroup = data.editorGroup;
                    this.editorGroups = data.editorGroups;
                    this.scriptsHtml = data.scriptsHtml;
                    this.message = data.message;

                    window.asyncEditor.editors[this.asyncEditorId].contentId = data.contentId;

                    if (shouldUpdateQuery) this.updateQuery();
                }
                else {
                    this.errorJson = JSON.stringify({ error: data, string: data.toString() });
                    this.errorText = this.defaultErrorText;
                }
            },
            async updateQuery() {
                const query = { ...this.$route.query };
                query[this.asyncEditorId + '.contentId'] = this.contentId;
                query[this.asyncEditorId + '.editorGroup'] = this.editorGroup;
                await router.push({ query });
            },
            processQuery() {
                let shouldLoadEditor = false;

                const contentIdKey = this.asyncEditorId + '.contentId';
                if (Object.prototype.hasOwnProperty.call(this.$route.query, contentIdKey) &&
                    this.$route.query[contentIdKey] !== this.contentId) {
                    this.contentId = this.$route.query[contentIdKey];
                    shouldLoadEditor = true;
                }

                const editorGroupKey = this.asyncEditorId + '.editorGroup';
                if (Object.prototype.hasOwnProperty.call(this.$route.query, editorGroupKey) &&
                    this.$route.query[editorGroupKey] !== this.editorGroup) {
                    this.editorGroup = this.$route.query[editorGroupKey];
                    shouldLoadEditor = true;
                }

                if (shouldLoadEditor) this.loadEditor();
                return shouldLoadEditor;
            },
            isCurrentGroup(editorGroup) {
                return editorGroup === this.editorGroup;
            },
            isFirstGroup(editorGroup) {
                return this.editorGroups.at(0)?.name === (editorGroup ?? this.editorGroup);
            },
            isLastGroup(editorGroup) {
                return this.editorGroups.at(-1)?.name === (editorGroup ?? this.editorGroup);
            },
            getPreviousEditor(editorGroup) {
                const editorGroups = this.editorGroups.map((group) => group.name);
                const index = editorGroups.indexOf(editorGroup ?? this.editorGroup);
                return editorGroups[index - 1];
            },
            getNextEditor(editorGroup) {
                const editorGroups = this.editorGroups.map((group) => group.name);
                const index = editorGroups.indexOf(editorGroup ?? this.editorGroup);
                return editorGroups[index + 1];
            },
        },
    });

    const app = createApp({
        data: function () {
            return { id: asyncEditorId };
        },
        mounted: function () {
            parameters.asyncEditorId = asyncEditorId;
            this.$refs.editor.initEditor(parameters);
        },
        components: {
            'async-editor': asyncEditorComponent,
        },
        template: '<async-editor ref="editor"></async-editor>',
    });

    app.use(router);
    const vm = app.mount(parameters.element);

    window.asyncEditor.editors[asyncEditorId] = vm;
}
