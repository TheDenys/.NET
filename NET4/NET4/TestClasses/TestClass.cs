using System;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{

    public class ParamClass
    {
        private String strParam = "initial";

        public void setParamDefault()
        {
            strParam = "changed";
        }
        
        public void setParam(String str)
        {
            strParam = str;
        }

        public String getParam()
        {
            return strParam;
        }
    }
    
    [RunableClass]
    public class TestDeclaring
    {
        public static void arraysDeclaring()
        {
            int [,] threeD = new int [2,2];
        }
        
        public static void TestPassingMethod(ParamClass paramIn, out ParamClass paramOut, ref ParamClass paramRef)
        {
            paramIn.setParamDefault();
            paramOut = new ParamClass();
            paramOut.setParamDefault();
            paramRef = new ParamClass();
            paramRef.setParamDefault();
        }

        [Run(0)]
        public static void Invoke()
        {
            ParamClass pcIn = new ParamClass();
            ParamClass pcOut;
            ParamClass pcRef = null;
            TestPassingMethod(pcIn, out pcOut, ref pcRef);
            Console.WriteLine("pcIn="+pcIn+" ["+pcIn.getParam()+"]");
            Console.WriteLine("pcOut="+pcOut+" ["+pcOut.getParam()+"]");
            Console.WriteLine("pcRef="+pcRef+" ["+pcRef.getParam()+"]");
        }
    }
}