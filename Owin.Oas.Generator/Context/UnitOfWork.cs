using System;

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
                SetContext(newContext);
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

        private void SetContext(string context)
        {
            AppDomain
                .CurrentDomain
                .SetData("APPBASE", context);
        }
    }
}
