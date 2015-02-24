namespace UnitySample
{
    public class ClassWithEvenMoreDependecies
    {

        private string initiator;

        public ClassWithEvenMoreDependecies(string initiator)
        {
            this.initiator = initiator;
        }

        private SampleLogger _logger;
        public SampleLogger Logger
        {
            get { return _logger; }
            set
            {
                _logger = value;
                _logger.Log(string.Format("initiator={0}", initiator));
            }
        }
    }
}