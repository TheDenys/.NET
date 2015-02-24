namespace UnitySample
{
    public class NonRegisteredType
    {
        // Unity will resolve the ctor parameter itself.
        // SampleLogger is not registered either so Unity will instantiate it.
        public NonRegisteredType(SampleLogger logger)
        {
            logger.Log("NonRegisteredType ctor called.");
        }
    }
}