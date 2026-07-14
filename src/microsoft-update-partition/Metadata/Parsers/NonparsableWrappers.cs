using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.PackageGraph.MicrosoftUpdate.Metadata.Parsers;

/// <summary>
/// A wrapper for an integer value that implements IParsable, allowing it to be parsed from a hexadecimal string representation.
/// </summary>
/// <param name="Value"></param>
public record ParsableHexInt(int Value) : IParsable<ParsableHexInt>
{
	public static ParsableHexInt Parse(string s, IFormatProvider? provider)
	{
		return new(Convert.ToInt32(s, 16));
	}

	public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ParsableHexInt result)
	{
		if (s is not null && int.TryParse(s, System.Globalization.NumberStyles.HexNumber, provider, out var parsedValue))
		{
			result = new(parsedValue);
			return true;
		}
		result = default;
		return false;
	}

	public static implicit operator int(ParsableHexInt p) => p.Value;
	public static implicit operator ParsableHexInt(int value) => new(value);
	public override string ToString() => Value.ToString("X");
}	

/// <summary>
/// A wrapper for an enum type that implements IParsable, allowing it to be parsed from a string representation.
/// </summary>
/// <typeparam name="TEnum">The enum type to be wrapped.</typeparam>
/// <param name="Value">The value of the enum.</param>
public record ParsableEnum<TEnum>(TEnum Value) : IParsable<ParsableEnum<TEnum>> where TEnum : struct, Enum
{
    public static ParsableEnum<TEnum> Parse(string s, IFormatProvider? provider)
    {
        return new(Enum.Parse<TEnum>(s));
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ParsableEnum<TEnum> result)
    {
        if (Enum.TryParse<TEnum>(s, out var parsedValue))
        {
            result = new(parsedValue);
            return true;
        }

        result = default;
        return false;
    }

    public static implicit operator TEnum(ParsableEnum<TEnum> p) => p.Value;
    public static implicit operator ParsableEnum<TEnum>(TEnum value) => new(value);
    public override string ToString() => Value.ToString();
}

/// <summary>
/// A wrapper for the Uri type that implements IParsable, allowing it to be parsed from a string representation.
/// </summary>
/// <param name="Value">The value of the Uri.</param>
public record ParsableUri(Uri Value) : IParsable<ParsableUri>
{
    public static ParsableUri Parse(string s, IFormatProvider? provider)
    {
        return new(new Uri(s));
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ParsableUri result)
    {
		result = default;

        if (s is null || 
            !Uri.TryCreate(s, UriKind.Absolute, out var uri))
        {
            return false;
        }

        result = new(uri);
        return true;
    }

    public static implicit operator Uri(ParsableUri u) => u.Value;
}