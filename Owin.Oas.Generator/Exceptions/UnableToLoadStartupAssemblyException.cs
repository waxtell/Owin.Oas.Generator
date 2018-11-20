using System;

namespace Owin.Oas.Generator.Exceptions
{
    public class UnableToLoadStartupAssemblyException : Exception
    {
        public UnableToLoadStartupAssemblyException(string startupAssemblyName, Exception ex) 
            : base($"Unable to load startup assembly {startupAssemblyName}", ex)
        {
        }
    }
}
