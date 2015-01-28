using System;
using System.Windows.Forms;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.InterviewSnippets
{
    public class Test
    {
        ~Test()
        {
            var a = "a";
            return;
        }

        //    virtual void TestMeNow()
    //    {
    //        Console.WriteLine("It Works!!");
    //    }

    //    public void TestMe()
    //    {
    //        Console.WriteLine("Error");
    //    }
    }

    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        Test obj = new Test();
    //        obj.TestMe();
    //    }
    //}

    public class Form1 : Form
    {
        protected delegate void UpdateButtonsDelegate();
        protected delegate void UpdateTextDelegate();

        /// <summary>
        /// To be called on a special thread
        /// </summary>
        public void ProcessDataInAnotherThread()
        {
            //...
            //do some work
            //...

            Invoke(new UpdateTextDelegate(UpdateText));
            Invoke(new UpdateButtonsDelegate(UpdateButtons));
        }

        private void UpdateText()
        {
            //implementation
        }

        private void UpdateButtons()
        {
            //implementation
        }

    }

    [RunableClass]
    public class Reverse
    {
        [Run(0)]
        protected void reverse()
        {
            string inputString = "Solarwind interview coding";

            string reversedString = string.Empty;

            for (int i = 0; i < inputString.Length; i++)
            {
                reversedString += inputString[inputString.Length - i - 1]; ;
            }

            string allLow = reversedString.ToLower();



            char initCap = Char.ToUpper(allLow[0]);

            ConsolePrint.print(allLow.Replace(allLow[0], initCap));
        }
    }

}