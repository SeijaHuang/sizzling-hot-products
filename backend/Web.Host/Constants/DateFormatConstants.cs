using System.Collections.Immutable;

namespace Web.Host.Common;

public static class DateFormatConstants
{
    public static readonly ImmutableArray<string> SupportedFormats = ImmutableArray.Create("dd/MM/yyyy");
}
