using System.Text;

namespace PsdSharp.Tests;

public static class EncodingProvider
{
    #if !NET6_0_OR_GREATER
    private static Encoding? _latin1;

    public static Encoding Latin1
    {
        get
        {
            if (_latin1 is not null)
                return _latin1;
            
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            _latin1 = Encoding.GetEncoding("ISO-8859-1");
            return _latin1;
        }
    }
    
    #else
    
    public static Encoding Latin1 => Encoding.Latin1;
    #endif
}