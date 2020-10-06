﻿using Lombiq.ContentEditors.ViewModels;
using Orchard.DisplayManagement;
using System;

namespace Orchard.ContentManagement.Drivers
{
    public static class ContentPartDriverExtensions
    {
        public static ContentShapeResult InputFieldDisplayShape<TContent>(
            this ContentPartDriver<TContent> driver,
            string shapeName,
            IShapeFactory shapeFactory,
            Func<InputFieldViewModel> viewModelFactory) where TContent : ContentPart, new() =>
                driver.ContentShape(shapeName, () =>
                    (shapeFactory as dynamic).Lombiq_Fields_InputField(ViewModel: viewModelFactory()));

        public static ContentShapeResult TextBoxEditorShape<TContent>(
            this ContentPartDriver<TContent> driver,
            string shapeName,
            IShapeFactory shapeFactory,
            Func<EditorViewModel> viewModelFactory) where TContent : ContentPart, new() =>
                driver.ContentShape(shapeName, () =>
                    (shapeFactory as dynamic).Lombiq_Editors_TextboxEditor(ViewModel: viewModelFactory()));
    }
}