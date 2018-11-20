using System;

namespace Owin.Oas.Generator.Exceptions
{
    public class UnableToLoadStartupTypeException : Exception
    {
        public UnableToLoadStartupTypeException(string startupTypeName, Exception ex)
            : base($"Unable to load startup type {startupTypeName}", ex)
        {
        }
    }
}
