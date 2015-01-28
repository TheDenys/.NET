using Microsoft.Practices.Unity;

namespace PDNUtils.Unity
{
    public class UnityInstance
    {
        private static IUnityContainer container = new UnityContainer();

        public static IUnityContainer Container()
        {
            return container;
        }

    }
}