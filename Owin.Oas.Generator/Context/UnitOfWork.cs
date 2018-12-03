using System;
using System.IO;

namespace Owin.Oas.Generator.Context
{
    internal class UnitOfWork
    {
        private readonly string _startingContext;

        public UnitOfWork(string startingContext)
        {
            _startingContext = startingContext;
        }

        public void DoWorkInContext(string newContext, Action work)
        {
            if (!string.IsNullOrEmpty(newContext))
            {
                SetContext
                (
                    Path.IsPathRooted(newContext)
                        ? newContext
                        : Path.GetFullPath(newContext)
                );
            }

            try
            {
                work.Invoke();
            }
            finally
            {
                if (!string.IsNullOrEmpty(newContext))
                {
                    SetContext(_startingContext);
                }
            }
        }

        private static void SetContext(string context)
        {
            AppDomain
                .CurrentDomain
                .SetData("APPBASE", context);
        }
    }
}
