using Lombiq.ContentEditors.Samples.Models;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Builders;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using System;
using static Lombiq.ContentEditors.Samples.Constants.ContentTypes;

namespace Lombiq.ContentEditors.Samples.Migrations;

// This is the migration class for the Employee content type. Nothing specific to async editors here.
public class EmployeeMigrations : DataMigration
{
    private readonly IContentDefinitionManager _contentDefinitionManager;

    public EmployeeMigrations(IContentDefinitionManager contentDefinitionManager) =>
        _contentDefinitionManager = contentDefinitionManager;

    public int Create()
    {
        _contentDefinitionManager.AlterPartDefinition(nameof(AsyncEditorEmployeePart), part => part
            .WithField(nameof(AsyncEditorEmployeePart.Name), ConfigureField<TextField>("Name"))
            .WithField(nameof(AsyncEditorEmployeePart.Position), ConfigureField<TextField>("Position"))
            .WithField(nameof(AsyncEditorEmployeePart.Office), ConfigureField<TextField>("Office Location"))
            .WithField(nameof(AsyncEditorEmployeePart.Age), ConfigureField<NumericField>("Age"))
            .WithField(nameof(AsyncEditorEmployeePart.StartDate), ConfigureField<DateField>("Start Date"))
            .WithField(nameof(AsyncEditorEmployeePart.Salary), ConfigureField<NumericField>("Salary")));

        _contentDefinitionManager.AlterTypeDefinition(Employee, type => type
            .Listable()
            .WithPart(nameof(AsyncEditorEmployeePart)));

        return 1;
    }

    private static Action<ContentPartFieldDefinitionBuilder> ConfigureField<T>(string name) =>
        field => field.OfType(typeof(T).Name).WithDisplayName(name);
}

// NEXT STATION: Navigation/ContentEditorsSamplesNavigationProvider.cs
