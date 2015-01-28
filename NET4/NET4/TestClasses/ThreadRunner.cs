using System;
using System.Threading;

namespace NET4.TestClasses
{
    
    class ThreadRunner
    {

        int max_threads = 21;
        
        Thread[] t_arr;

        public void Start()
        {
            t_arr = new Thread[max_threads];
            //Methods met = new Methods();
            int i = 0;
            for (i = 0; i < max_threads; i++)
            {
                t_arr[i] = new Thread(new Methods().TestMethod);
                t_arr[i].Start();
                //t_arr[i].Join();
            }
            //for (int i = 0; i < max_threads; i++)
            //{
            //    t_arr[i].Join();
            //    Console.WriteLine("["+i+"] thread joined");
            //}

        }

        class Methods
        {
            int cur_num;

            static int iTestCounter = 0;

            public void TestMethod()
            {
                cur_num = iTestCounter++;
                //Thread.Sleep(1000);
                int f_param = cur_num * 5;
                long f = Calculate(f_param);
                Console.WriteLine("counter [" + cur_num + "] fact(" + f_param + ")=" + f);
            }

            private long fact(int n)
            {
                if (n > 1)
                {
                    return n * fact(n - 1);
                }
                return 1;
            }

            // Recursive method that calculates the Nth Fibonacci number.
            public int Calculate(int n)
            {
                //lock (this)
                //{

                //Console.WriteLine("thread ["+cur_num+"] param["+n+"]");

                if (n <= 1)
                {
                    return n;
                }

                return Calculate(n - 1) + Calculate(n - 2);
                //}
            }

        }

    }
}
