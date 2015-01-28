using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.EffectiveCSharp
{

    internal interface InternalInterface
    {
        int GetStatus();
    }

    internal struct ValueType : InternalInterface
    {
        private int _status;

        public ValueType(int status)
        {
            _status = status;
        }

        public int GetStatus()
        {
            return _status;
        }
    }

    internal sealed class ReferenceType : InternalInterface
    {
        private int _status;

        public ReferenceType(int status)
        {
            _status = status;
        }

        public int GetStatus()
        {
            return _status;
        }
    }

    [RunableClass]
    internal sealed class TestvalRef
    {
        [Run(0)]
        internal void Go()
        {
            InternalInterface[] ii = new InternalInterface[2];

            ii[0] = new ValueType(1);
            ii[1] = new ReferenceType(2);

            for (int i = 0; i < ii.Length; i++)
            {
                var internalInterface = ii[i];
                int status = internalInterface.GetStatus();
                bool equal = object.ReferenceEquals(internalInterface, ii[i]);
                ConsolePrint.print("status:{0}, equals:{1}", status, equal);
            }
        }

        [Run(0)]
        internal void BoxingUnboxing()
        {
            ValueType vt = new ValueType();

            InternalInterface i1 = vt;
            InternalInterface i2 = vt;

            ConsolePrint.print("Equals(vt,i1) : {0}", object.Equals(vt, i1));// true
            ConsolePrint.print("Equals(i1,i2) : {0}", object.Equals(i1, i2));// true
            ConsolePrint.print("ReferenceEquals(vt,i1) : {0}", object.ReferenceEquals(vt, i1));// false
            ConsolePrint.print("ReferenceEquals(i1,i2) : {0}", object.ReferenceEquals(i1, i2));// false
        }
    }
}