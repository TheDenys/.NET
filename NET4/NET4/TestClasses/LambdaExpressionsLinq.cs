using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using PDNUtils.Runner;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{
    [RunableClass]
    public class LambdaExpressionsLinq : RunableBase
    {
        [Run(0)]
        public static void Test()
        {
            Expression<Func<int, int, int>> expression = (int x, int y) => (int)Math.PI * x + y + 1;
            Console.Out.WriteLine("expr:" + expression.Body.ToString());

            Func<int, int, int> func = expression.Compile();

            int param = 9;
            Console.Out.WriteLine(string.Format("x({0}) = {1}", param, func(param, param)));

            int[] ints = { 4, 8 };
            Console.Out.WriteLine("arr:" + CollToString(ints, x => "_" + x + "_", ","));
            Console.Out.WriteLine("arr2:" + CollToString(ints, delegate(int a) { return "x" + a; }, ","));
        }

        [Run(0)]
        public static void MakeExpr()
        {
            var x = Expression.Parameter(typeof(int), "x");
            var y = Expression.Parameter(typeof(int), "y");
            var expr = Expression<Func<int, int, int>>.Lambda<Func<int, int, int>>(Expression.Subtract(x, y), x, y);
            Func<int, int, int> f = expr.Compile();
            Console.Out.WriteLine("f(12,4)=" + f(12, 4));
        }

        [Run(0)]
        // http://msdn.microsoft.com/en-us/library/vstudio/bb397951.aspx
        internal void FactorialFromMsdn()
        {
            ParameterExpression value = Expression.Parameter(typeof(int), "value");
            ParameterExpression result = Expression.Parameter(typeof(int), "result");
            LabelTarget label = Expression.Label(typeof(int));

            BlockExpression block = Expression.Block(
                new[] { result },
                Expression.Assign(result, Expression.Constant(1)), // result = 1
                    Expression.Loop(
                        Expression.IfThenElse(
                // condition: value > 1
                            Expression.GreaterThan(value, Expression.Constant(1)),
                // true: result *= value--
                            Expression.MultiplyAssign(result, Expression.PostDecrementAssign(value)),
                //false: exit loop, goto label
                            Expression.Break(label, result)
                        ),
                // label jump to
                        label
                    )
                );

            Func<int, int> factorial = Expression.Lambda<Func<int, int>>(block, value).Compile();

            int n = 6;
            int f = factorial(n);

            DebugFormat("fact({0})={1}", n, f);
        }

        public static string CollToString<T>(IEnumerable<T> coll, Func<T, string> tostr, string separator)
        {
            string s = string.Join(separator, coll.Select(tostr).ToArray());
            return s;
        }
    }
}