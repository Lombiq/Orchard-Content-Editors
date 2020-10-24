﻿using Piedone.HelpfulExtensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Lombiq.ContentEditors.ViewModels
{
    public class CheckboxListEditorViewModel : FieldViewModel, IElementValueFilterRelationshipSelector, IParentElementValueDependency
    {
        public List<CheckboxListFieldItemViewModel> Items { get; set; } = new List<CheckboxListFieldItemViewModel>();
        public bool EnableSelectAll { get; set; }
        public bool IsExpandable { get; set; }
        public bool IsCollapsedByDefault { get; set; }
        public bool IsSearchEnabled { get; set; } = true;

        #region IElementValueFilterRelationshipSelector implementation

        public bool IsFilterRelationShipSelectorEnabled { get; set; }
        public FilterRelationship[] EnabledFilterRelationShips { get; set; } = (FilterRelationship[])Enum.GetValues(typeof(FilterRelationship));

        #endregion

        #region IParentElementValueDependency implementation

        public string ParentElementSelector { get; set; }
        public Dictionary<string, IEnumerable<string>> ParentElementValueHierarchy { get; set; } = new Dictionary<string, IEnumerable<string>>();

        #endregion

        public CheckboxListEditorViewModel(string technicalName, IEnumerable<string> values)
            : this(new NameValueCollection(), technicalName, values) { }

        public CheckboxListEditorViewModel(
            NameValueCollection queryString, string technicalName, IEnumerable<string> values)
        {
            var checkedItems = queryString.GetQueryStringParameterValues(technicalName) ?? Enumerable.Empty<string>();

            foreach (var value in values)
                Items.Add(new CheckboxListFieldItemViewModel
                {
                    Value = value,
                    Checked = checkedItems?.Contains(value) ?? false
                });

            TechnicalName = technicalName;
        }

        public CheckboxListEditorViewModel(string technicalName, IEnumerable<KeyValuePair<string, string>> valueLabelDictionary)
            : this(new NameValueCollection(), technicalName, valueLabelDictionary) { }

        public CheckboxListEditorViewModel(
            NameValueCollection queryString, string technicalName, IEnumerable<KeyValuePair<string, string>> valueLabelDictionary)
        {
            var checkedItems = queryString.GetQueryStringParameterValues(technicalName) ?? Enumerable.Empty<string>();

            foreach (var valueLabelItem in valueLabelDictionary)
                Items.Add(new CheckboxListFieldItemViewModel
                {
                    Value = valueLabelItem.Key?.ToString(),
                    Label = valueLabelItem.Value?.ToString(),
                    Checked = checkedItems?.Contains(valueLabelItem.Key.ToString()) ?? false
                });

            TechnicalName = technicalName;
        }
    }

    public class CheckboxListFieldItemViewModel
    {
        private string _label;
        public string Label
        {
            get => string.IsNullOrEmpty(_label) ? Value : _label;
            set => _label = value;
        }

        public string Value { get; set; }

        public bool Checked { get; set; }

        public bool Disabled { get; set; }
    }
}