using System;
using Microsoft.Practices.Unity;

namespace UnitySample
{
    public class SampleLogger
    {
        public void Log(string s)
        {
            UnityInstance.Container.Resolve<Action<string>>("output")(s);
        }
    }
}