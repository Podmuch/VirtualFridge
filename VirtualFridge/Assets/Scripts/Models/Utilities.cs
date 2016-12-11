using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Utilities
{
    private const string UNREACHABLE_HOST = "Usługa sieciowa niedostępna, spróbuj jeszcze raz";
    private const string UNKNOWN_ERROR = "Nieznany bład, spróbuj jeszcze raz";

    public static string GetErrorMessageFromString(string webError)
    {
        if (webError.Contains("Could not resolve host"))
        {
            return UNREACHABLE_HOST;
        }
        return webError; //UNKNOWN_ERROR;
    }
}