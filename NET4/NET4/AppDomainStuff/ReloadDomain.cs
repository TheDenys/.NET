using System;
using System.Globalization;
using System.Reflection;
using PDNUtils.Runner.Attributes;
using log4net;

namespace NET4.AppDomainStuff
{
    [RunableClass]
    public class ReloadDomain
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Run(0)]
        protected void Run()
        {
            Facade.Instance.MethodA();
            Facade.Reset();
            Facade.Reset();

            Facade.Instance.MethodA();
            Facade.Reset();
        }
    }

    class Facade : IFoo
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static readonly object sync = new object();
        static volatile Facade instance;

        public static Facade Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (sync)
                    {
                        if (instance == null)
                        {
                            instance = new Facade();
                        }
                    }
                }

                return instance;
            }
        }

        public static void Reset()
        {
            lock (sync)
            {
                try
                {
                }
                finally
                {
                    if (instance != null)
                    {
                        AppDomain.Unload(instance.domain);
                        instance.domain = null;
                        instance.fooProxy = null;
                        instance = null;
                    }
                }
            }
        }

        AppDomain domain;
        FooProxy fooProxy;

        private Facade()
        {
            domain = GetNewDomain();
            fooProxy = GetNewFooProxy(domain);
        }

        public void MethodA()
        {
            fooProxy.MethodA();
        }

        static AppDomain GetNewDomain()
        {
            AppDomainSetup appDomainSetup = new AppDomainSetup();
            AppDomain domain = AppDomain.CreateDomain("TestDomain", AppDomain.CurrentDomain.Evidence, appDomainSetup);
            return domain;
        }

        static FooProxy GetNewFooProxy(AppDomain domain)
        {
            FooProxy proxy = domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().GetName().Name, typeof(FooProxy).FullName, true, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, CultureInfo.InvariantCulture, null) as FooProxy;
            return proxy;
        }
    }

    interface IFoo
    {
        void MethodA();
    }

    class FooProxy : MarshalByRefObject, IFoo
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Foo foo;

        FooProxy()
        {
            foo = new Foo();
            log.Debug("ctor");
        }

        public void MethodA()
        {
            log.Debug("FooProxy: method a");
            foo.MethodA();
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }

    class Foo : IFoo
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Foo()
        {
            log.Debug("ctor");
        }

        public void MethodA()
        {
            log.Debug("Foo: real method a");
        }
    }

}