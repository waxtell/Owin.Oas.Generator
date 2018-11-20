using System;

namespace Owin.Oas.Generator.Exceptions
{
    public class UnableToLoadDependencyException : Exception
    {
        public UnableToLoadDependencyException(string dependencyName) 
            : base($"Unable to load dependency {dependencyName}")
        {
        }
    }
}
