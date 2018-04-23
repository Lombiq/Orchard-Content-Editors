using Orchard.Localization;
using Orchard.Localization.Services;
using Piedone.HelpfulLibraries.Libraries.DependencyInjection;
using System.Collections.Generic;

namespace Lombiq.ContentEditors.Services
{
    public class IsoFormattedShortDateFormatProviderDecoratorsModule : DecoratorsModuleBase
    {
        protected override IEnumerable<DecorationConfiguration> DescribeDecorators()
        {
            yield return DecorationConfiguration.Create<IDateTimeFormatProvider, IsoFormattedShortDateFormatProviderDecorator>();
        }
    }


    internal class IsoFormattedShortDateFormatProviderDecorator : IDateTimeFormatProvider
    {
        private readonly IDateTimeFormatProvider _decorated;

        public Localizer T { get; set; }


        public IsoFormattedShortDateFormatProviderDecorator(IDateTimeFormatProvider decorated)
        {
            _decorated = decorated;

            T = NullLocalizer.Instance;
        }


        public string ShortDateFormat => T("yyyy-MM-dd").Text;


        #region IDateTimeFormatProvider proxies without change.

        public string[] MonthNames => _decorated.MonthNames;

        public string[] MonthNamesGenitive => _decorated.MonthNamesGenitive;

        public string[] MonthNamesShort => _decorated.MonthNamesShort;

        public string[] MonthNamesShortGenitive => _decorated.MonthNamesShortGenitive;

        public string[] DayNames => _decorated.DayNames;

        public string[] DayNamesShort => _decorated.DayNamesShort;

        public string[] DayNamesMin => _decorated.DayNamesMin;

        public string ShortTimeFormat => _decorated.ShortTimeFormat;

        public string ShortDateTimeFormat => _decorated.ShortDateTimeFormat;

        public string LongDateFormat => _decorated.LongDateFormat;

        public string LongTimeFormat => _decorated.LongTimeFormat;

        public string LongDateTimeFormat => _decorated.LongDateTimeFormat;

        public IEnumerable<string> AllDateFormats => _decorated.AllDateFormats;

        public IEnumerable<string> AllTimeFormats => _decorated.AllTimeFormats;

        public IEnumerable<string> AllDateTimeFormats => _decorated.AllDateTimeFormats;

        public int FirstDay => _decorated.FirstDay;

        public bool Use24HourTime => _decorated.Use24HourTime;

        public string DateSeparator => _decorated.DateSeparator;

        public string TimeSeparator => _decorated.TimeSeparator;

        public string AmPmPrefix => _decorated.AmPmPrefix;

        public string[] AmPmDesignators => _decorated.AmPmDesignators;


        public int GetEra(string eraName) => _decorated.GetEra(eraName);

        public string GetEraName(int era) => _decorated.GetEraName(era);

        public string GetShortEraName(int era) => _decorated.GetShortEraName(era);

        #endregion
    }
}