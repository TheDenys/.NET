using System;

namespace NET4.EffectiveCSharpTestClasses
{
    internal class DisposableClass : IDisposable
    {
        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (disposed)
            {
                return;
            }

            if (isDisposing)
            {
                // release unmanaged
            }

            disposed = true;
        }

        ~DisposableClass()
        {
            Dispose(false);
        }
    }

    internal class DerivedDisposable : DisposableClass
    {
        private bool disposed;

        public void Dispose()
        {
            try
            {
                Dispose(true); //true: safe to free managed resources
            }
            finally
            {
                base.Dispose();
            }
            //
            // GC.SuppressFinalize(this); will be called in base class
        }

        protected override void Dispose(bool isDisposing)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
        }

        ~DerivedDisposable()
        {
            Dispose(false);
        }
    }
}