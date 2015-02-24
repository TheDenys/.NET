using Microsoft.Practices.Unity;

namespace UnitySample
{
    public class ClassWithDependencies
    {
        private SampleLogger _logger;

        [Dependency]
        public SampleLogger Logger
        {
            get { return _logger; }
            set
            {
                _logger = value;
                _logger.Log("Logger was injected by Unity");
            }
        }
    }
}