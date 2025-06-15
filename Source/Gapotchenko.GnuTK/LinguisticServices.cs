using System.Text;

namespace Gapotchenko.GnuTK;

static class LinguisticServices
{
    public static string SingleQuote(string value) => $"'{value}'";

    public static string CombineWithOr(params IEnumerable<string> values)
    {
        using var enumerator = values.GetEnumerator();

        if (!enumerator.MoveNext())
            return string.Empty;

        var builder = new StringBuilder();

        for (; ; )
        {
            string current = enumerator.Current;
            bool hasNext = enumerator.MoveNext();
            if (builder.Length != 0)
            {
                if (hasNext)
                    builder.Append(", ");
                else
                    builder.Append(" or ");
            }
            builder.Append(current);
            if (!hasNext)
                break;
        }

        return builder.ToString();
    }
}
