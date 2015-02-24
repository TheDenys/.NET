using Microsoft.Practices.Unity;

namespace UnitySample
{
    public class UnityInstance
    {
        private static readonly IUnityContainer container = null;
        
        static UnityInstance()
        {
            container = new UnityContainer();
        }

        public static IUnityContainer Container
        {
            get { return container; }
        }
    }
}