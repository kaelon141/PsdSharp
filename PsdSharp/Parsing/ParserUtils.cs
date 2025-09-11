using System.Data;
using System.Text;

namespace PsdSharp.Parsing;

internal static class ParserUtils
{
    public static DataException DataCorrupt()
    {
        return new DataException("PSD file is corrupt.");
    }
}