namespace KYCVerificationAPI.Core.Helpers;

public static class EnumHelper
{
    private static readonly Random Random = new Random();

    public static T GetRandomEnumValue<T>() where T : Enum
    {
        var enumValues = Enum.GetValues(typeof(T));
        if (enumValues.Length == 0)
        {
            throw new ArgumentException("Enum must have at least one value.", nameof(T));
        }
        return (T)enumValues.GetValue(Random.Next(enumValues.Length))!;
    }
}