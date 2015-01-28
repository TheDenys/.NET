using System;
using System.Security.Principal;

namespace PDNUtils.Help
{
    public class ImpersonationWrapper : IDisposable
    {
        private WindowsImpersonationContext ctx;
        
        public ImpersonationWrapper(WindowsIdentity identity)
        {
            if (identity != null)
            {
                ctx = identity.Impersonate();
            }
        }

        public void Do(Action action)
        {
            action();
        }

        public T Do<T>(Func<T> func)
        {
            return func();
        }

        public void Dispose()
        {
            if (ctx != null)
            {
                ctx.Dispose();
            }
        }
    }
}