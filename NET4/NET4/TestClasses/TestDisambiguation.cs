//extern alias ClassLibraryTest;
//extern alias ClassLibraryTest2;
//using System;
//using System.Reflection;
//using PDNUtils.Help;
//using PDNUtils.Runner.Attributes;

//namespace NET4.TestClasses
//{
//    [RunableClass]
//    public class TestDisambiguation
//    {
//        [Run(0)]
//        protected void Write()
//        {
//            var version1 = Assembly.GetAssembly(typeof(ClassLibraryTest::AmbNs.AmbiguosClass)).GetName().Version;
//            string realName = ClassLibraryTest::AmbNs.AmbiguosClass.REAL_NAME;
//            ConsolePrint.print("real name:{0} ver:{1}", realName, version1);

//            var version2 = Assembly.GetAssembly(typeof(ClassLibraryTest2::AmbNs.AmbiguosClass)).GetName().Version;
//            string realName2 = ClassLibraryTest2::AmbNs.AmbiguosClass.REAL_NAME;
//            ConsolePrint.print("real name2:{0} ver:{1}", realName2, version2);
//        }

//        [Run(0)]
//        protected void TestAmbiguous()
//        {
//            //Activator.CreateInstance()
//        }
//    }
//}