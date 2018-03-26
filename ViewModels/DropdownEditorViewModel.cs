﻿using System.Collections.Generic;
using System.Web.Mvc;

namespace Lombiq.ContentEditors.ViewModels
{
    public class DropdownEditorViewModel : EditorViewModel
    {
        public List<SelectListItem> SelectList { get; set; } = new List<SelectListItem>();


        public DropdownEditorViewModel(IEnumerable<string> values)
        {
            foreach (var value in values) SelectList.Add(new SelectListItem { Text = value, Value = value });
        }

        public DropdownEditorViewModel()
        {

        }
    }
}