using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Remoting;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.AppDomainStuff
{
    [RunableClass]
    public class CreateProxies
    {

        [Run(0)]
        protected void StartDomain()
        {
            AppDomainSetup appDomainSetup = new AppDomainSetup();

            AppDomain domain = AppDomain.CreateDomain("TestDomain", AppDomain.CurrentDomain.Evidence, appDomainSetup);
            ObjectHandle o = domain.CreateInstance(Assembly.GetExecutingAssembly().GetName().Name, typeof(TestDomainClassSer).FullName);
            TestDomainClassSer proxy = o.Unwrap() as TestDomainClassSer;

            if (proxy != null)
            {
                proxy.TestMethod();
            }
            //---

            proxy = domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().GetName().FullName, typeof(TestDomainClassSer).FullName, true, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, CultureInfo.InvariantCulture, null) as TestDomainClassSer;

            if (proxy != null)
            {
                proxy.TestMethod();
            }

            TestDomainClassMBR proxy2 = domain.CreateInstance(Assembly.GetExecutingAssembly().GetName().Name, typeof(TestDomainClassMBR).FullName, true, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, CultureInfo.InvariantCulture, null).Unwrap() as TestDomainClassMBR;

            if (proxy2 != null)
            {
                proxy2.TestMethod();
            }

            TestDomainClassMBRInfiniteRemote proxy3 = domain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().GetName().Name, typeof(TestDomainClassMBRInfiniteRemote).FullName, true, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, CultureInfo.InvariantCulture, null) as TestDomainClassMBRInfiniteRemote;

            if (proxy3 != null)
            {
                proxy3.TestMethod();
            }


            AppDomain.Unload(domain);

            proxy3.TestMethod();
        }

    }

    class TestDomainClassMBR : MarshalByRefObject
    {
        public TestDomainClassMBR()
        {
            ConsolePrint.print("test ctor in the domain {0}", AppDomain.CurrentDomain.FriendlyName);
        }

        public void TestMethod()
        {
            ConsolePrint.print("test method in the domain {0}", AppDomain.CurrentDomain.FriendlyName);
        }
    }

    class TestDomainClassMBRInfiniteRemote : MarshalByRefObject
    {
        public TestDomainClassMBRInfiniteRemote()
        {
            ConsolePrint.print("test ctor in the domain {0}", AppDomain.CurrentDomain.FriendlyName);
        }

        public void TestMethod()
        {
            ConsolePrint.print("test method in the domain {0}", AppDomain.CurrentDomain.FriendlyName);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }

    [Serializable]
    class TestDomainClassSer
    {
        public TestDomainClassSer()
        {
            ConsolePrint.print("test ctor in the domain {0}", AppDomain.CurrentDomain.FriendlyName);
        }

        public void TestMethod()
        {
            ConsolePrint.print("test method in the domain {0}", AppDomain.CurrentDomain.FriendlyName);
        }
    }
}