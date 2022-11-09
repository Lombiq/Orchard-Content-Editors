using Lombiq.ContentEditors.Samples.Models;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Builders;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using System;
using static Lombiq.ContentEditors.Samples.Constants.ContentTypes;

namespace Lombiq.ContentEditors.Samples.Migrations;

public class EmployeeMigrations : DataMigration
{
    private readonly IContentDefinitionManager _contentDefinitionManager;

    public EmployeeMigrations(IContentDefinitionManager contentDefinitionManager) => _contentDefinitionManager = contentDefinitionManager;

    public int Create()
    {
        _contentDefinitionManager.AlterPartDefinition(nameof(AsyncEditorEmployeePart), part => part
            .WithField(nameof(AsyncEditorEmployeePart.Name), ConfigureField<TextField>(name: "Name"))
            .WithField(nameof(AsyncEditorEmployeePart.Position), ConfigureField<TextField>(name: "Position"))
            .WithField(nameof(AsyncEditorEmployeePart.Office), ConfigureField<TextField>(name: "Office Location"))
            .WithField(nameof(AsyncEditorEmployeePart.Age), ConfigureField<NumericField>(name: "Age"))
            .WithField(nameof(AsyncEditorEmployeePart.StartDate), ConfigureField<DateField>(name: "Start Date"))
            .WithField(nameof(AsyncEditorEmployeePart.Salary), ConfigureField<NumericField>(name: "Salary")));

        _contentDefinitionManager.AlterTypeDefinition(Employee, type => type
            .Listable()
            .WithPart(nameof(AsyncEditorEmployeePart)));

        return 1;
    }

    private static Action<ContentPartFieldDefinitionBuilder> ConfigureField<T>(string name) =>
        field => field.OfType(typeof(T).Name).WithDisplayName(name);
}
