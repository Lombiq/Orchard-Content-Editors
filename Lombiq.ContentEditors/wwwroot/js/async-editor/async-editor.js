"use strict";

function ownKeys(object, enumerableOnly) { var keys = Object.keys(object); if (Object.getOwnPropertySymbols) { var symbols = Object.getOwnPropertySymbols(object); if (enumerableOnly) { symbols = symbols.filter(function (sym) { return Object.getOwnPropertyDescriptor(object, sym).enumerable; }); } keys.push.apply(keys, symbols); } return keys; }

function _objectSpread(target) { for (var i = 1; i < arguments.length; i++) { var source = arguments[i] != null ? arguments[i] : {}; if (i % 2) { ownKeys(Object(source), true).forEach(function (key) { _defineProperty(target, key, source[key]); }); } else if (Object.getOwnPropertyDescriptors) { Object.defineProperties(target, Object.getOwnPropertyDescriptors(source)); } else { ownKeys(Object(source)).forEach(function (key) { Object.defineProperty(target, key, Object.getOwnPropertyDescriptor(source, key)); }); } } return target; }

function _defineProperty(obj, key, value) { if (key in obj) { Object.defineProperty(obj, key, { value: value, enumerable: true, configurable: true, writable: true }); } else { obj[key] = value; } return obj; }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } }

function _createClass(Constructor, protoProps, staticProps) { if (protoProps) _defineProperties(Constructor.prototype, protoProps); if (staticProps) _defineProperties(Constructor, staticProps); return Constructor; }

/* global Vue */

/* global VueRouter */

/* global DomParser */
if (!window.asyncEditor) window.asyncEditor = {
  editors: []
};

var AsyncEditorApiClient = /*#__PURE__*/function () {
  function AsyncEditorApiClient(parameters) {
    _classCallCheck(this, AsyncEditorApiClient);

    this.apiUrl = parameters.apiUrl;
    this.asyncEditorId = parameters.asyncEditorId;
    this.providerName = parameters.providerName;
    this.contentType = parameters.contentType;
  }

  _createClass(AsyncEditorApiClient, [{
    key: "loadEditor",
    value: function loadEditor(contentId, editorGroup, callback) {
      return fetch(this.createUrl(contentId, editorGroup)).then(function (response) {
        return response.json();
      }).then(function (data) {
        return callback(true, data);
      })["catch"](function (error) {
        return callback(false, error);
      });
    }
  }, {
    key: "submitEditor",
    value: function submitEditor(contentId, editorGroup, nextEditorGroup, formData, callback) {
      return fetch(this.createUrl(contentId, editorGroup, nextEditorGroup), {
        method: 'post',
        headers: {
          'Content-Type': 'application/x-www-form-urlencoded;charset=UTF-8'
        },
        body: new URLSearchParams(formData)
      }).then(function (response) {
        return response.json();
      }).then(function (data) {
        return callback(true, data);
      }).then(function () {
        var submittedEditorEvent = new CustomEvent('asyncEditorSubmittedEditor', {
          bubbles: true,
          cancelable: true,
          detail: {
            asyncEditor: window.asyncEditor
          }
        });
        document.dispatchEvent(submittedEditorEvent);
      })["catch"](function (error) {
        return callback(false, error);
      });
    }
  }, {
    key: "createUrl",
    value: function createUrl(contentId, editorGroup, nextEditorGroup) {
      var url = new URL(this.apiUrl, document.baseURI);
      var query = {
        asyncEditorId: this.asyncEditorId,
        providerName: this.providerName,
        contentType: this.contentType
      };
      if (contentId) query.contentId = contentId;
      if (editorGroup) query.editorGroup = editorGroup;
      if (nextEditorGroup) query.nextEditorGroup = nextEditorGroup;
      url.search = new URLSearchParams(query).toString();
      return url;
    }
  }]);

  return AsyncEditorApiClient;
}();

var router = new VueRouter(); // Making the Vue object more readable.
// eslint-disable-next-line object-shorthand

window.asyncEditor.editor = {
  template: '#async-editor-template',
  data: function data() {
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
    progress: function progress(self) {
      if (self.editorGroups.length < 1) return 0;
      return self.editorGroups.filter(function (group) {
        return group.isFilled;
      }).length / self.editorGroups.length * 100;
    },
    showProgressBar: function showProgressBar(self) {
      return self.editorGroups.length > 1;
    }
  },
  watch: {
    '$route.query': function $routeQuery() {
      this.processQuery();
    }
  },
  router: router,
  updated: function updated() {
    var self = this;

    if (self.scriptsHtml) {
      var scripts = new DOMParser().parseFromString(self.scriptsHtml, 'text/html').getElementsByTagName('script');

      for (var i = 0; i < scripts.length; i++) {
        window.eval(scripts[i].text);
      }

      self.scriptsHtml = '';
    }
  },
  methods: {
    initEditor: function initEditor(parameters) {
      var _parameters$defaultEr;

      var self = this;
      self.api = new AsyncEditorApiClient(parameters);
      self.contentId = parameters.contentId;
      self.editorGroup = parameters.editorGroup;
      self.defaultErrorText = (_parameters$defaultEr = parameters.defaultErrorText) !== null && _parameters$defaultEr !== void 0 ? _parameters$defaultEr : 'Something went wrong.';
      self.asyncEditorId = parameters.asyncEditorId;
      if (!self.processQuery()) self.loadEditor();
    },
    loadEditor: function loadEditor(editorGroup) {
      var self = this;
      self.editorGroup = editorGroup !== null && editorGroup !== void 0 ? editorGroup : self.editorGroup;
      self.api.loadEditor(self.contentId, self.editorGroup, function (success, data) {
        self.processApiData(success, data);
      });
    },
    submitEditor: function submitEditor(nextEditorGroup) {
      var self = this;
      var submittingEditorEvent = new CustomEvent('asyncEditorSubmittingEditor', {
        bubbles: true,
        cancelable: true,
        detail: {
          asyncEditor: window.asyncEditor
        }
      });
      var success = document.dispatchEvent(submittingEditorEvent);

      if (success) {
        self.api.submitEditor(self.contentId, self.editorGroup, nextEditorGroup, new FormData(self.$refs.editorForm), function (success, data) {
          self.processApiData(success, data);
        });
      }
    },
    processApiData: function processApiData(success, data) {
      var self = this;

      if (success) {
        var shouldUpdateQuery = self.contentId !== data.contentId || self.editorGroup !== data.editorGroup;
        self.errorText = '';
        self.validationSummaryHtml = data.validationSummaryHtml;
        self.contentId = data.contentId;
        self.editorHtml = data.editorHtml;
        self.editorGroup = data.editorGroup;
        self.editorGroups = data.editorGroups;
        self.scriptsHtml = data.scriptsHtml;
        self.message = data.message;
        window.asyncEditor.editors[self.asyncEditorId].contentId = data.contentId;
        if (shouldUpdateQuery) self.updateQuery();
      } else {
        self.errorText = self.defaultErrorText;
      }
    },
    updateQuery: function updateQuery() {
      var self = this;

      var query = _objectSpread({}, self.$route.query);

      query[self.asyncEditorId + '.contentId'] = self.contentId;
      query[self.asyncEditorId + '.editorGroup'] = self.editorGroup;
      router.push({
        path: '/',
        query: query
      });
    },
    processQuery: function processQuery() {
      var self = this;
      var shouldLoadEditor = false;
      var contentIdKey = self.asyncEditorId + '.contentId';

      if (self.$route.query.hasOwnProperty(contentIdKey) && self.$route.query[contentIdKey] !== self.contentId) {
        self.contentId = self.$route.query[contentIdKey];
        shouldLoadEditor = true;
      }

      var editorGroupKey = self.asyncEditorId + '.editorGroup';

      if (self.$route.query.hasOwnProperty(editorGroupKey) && self.$route.query[editorGroupKey] !== self.editorGroup) {
        self.editorGroup = self.$route.query[editorGroupKey];
        shouldLoadEditor = true;
      }

      if (shouldLoadEditor) {
        self.loadEditor();
      }

      return shouldLoadEditor;
    },
    isCurrentGroup: function isCurrentGroup(editorGroup) {
      return editorGroup === this.editorGroup;
    },
    isFirstGroup: function isFirstGroup(editorGroup) {
      var _self$editorGroups$at;

      var self = this;
      return ((_self$editorGroups$at = self.editorGroups.at(0)) === null || _self$editorGroups$at === void 0 ? void 0 : _self$editorGroups$at.name) === (editorGroup !== null && editorGroup !== void 0 ? editorGroup : self.editorGroup);
    },
    isLastGroup: function isLastGroup(editorGroup) {
      var _self$editorGroups$at2;

      var self = this;
      return ((_self$editorGroups$at2 = self.editorGroups.at(-1)) === null || _self$editorGroups$at2 === void 0 ? void 0 : _self$editorGroups$at2.name) === (editorGroup !== null && editorGroup !== void 0 ? editorGroup : self.editorGroup);
    },
    getPreviousEditor: function getPreviousEditor(editorGroup) {
      var editorGroups = this.editorGroups.map(function (group) {
        return group.name;
      });
      var index = editorGroups.indexOf(editorGroup !== null && editorGroup !== void 0 ? editorGroup : this.editorGroup);
      return editorGroups[index - 1];
    },
    getNextEditor: function getNextEditor(editorGroup) {
      var editorGroups = this.editorGroups.map(function (group) {
        return group.name;
      });
      var index = editorGroups.indexOf(editorGroup !== null && editorGroup !== void 0 ? editorGroup : this.editorGroup);
      return editorGroups[index + 1];
    }
  }
};

window.initAsyncEditor = function (asyncEditorId, parameters) {
  if (!parameters) return window.asyncEditor.editors[asyncEditorId];
  window.asyncEditor.editors[asyncEditorId] = new Vue({
    el: parameters.element,
    data: {
      id: asyncEditorId
    },
    mounted: function mounted() {
      parameters.asyncEditorId = asyncEditorId;
      this.$refs.editor.initEditor(parameters);
    },
    components: {
      'async-editor': window.asyncEditor.editor
    },
    template: '<async-editor ref="editor"></async-editor>'
  });
};