using Microsoft.Practices.Unity;
using PDNUtils.Help;
using PDNUtils.Runner.Attributes;

namespace NET4.TestUnity
{
    [RunableClass]
    public class TestUnity
    {
        [Run(0)]
        public void TestUnityBasics()
        {
            IUnityContainer uc1 = new UnityContainer().RegisterType<IAcmeLab, EvilAcmeLab>("ACME");
            IAcmeLab lab1 = uc1.Resolve<IAcmeLab>("ACME");
            IAcmeMonster monster1 = lab1.CreateMonster();
            monster1.DoAction();

            IUnityContainer uc2 = new UnityContainer().RegisterType<IAcmeLab, ChineeseAcmeLab>();
            IAcmeLab lab2 = uc2.Resolve<IAcmeLab>();
            IAcmeMonster monster2 = lab2.CreateMonster();
            monster2.DoAction();


            IUnityContainer uc3 = new UnityContainer().RegisterType<IAcmeLab, AgeAcmeLab>();
            IAcmeLab lab3 = uc3.Resolve<IAcmeLab>(new ParameterOverride("age", 1000));
            IAcmeMonster monster3 = lab3.CreateMonster();
            monster3.DoAction();
        }

    }

    public interface IAcmeLab
    {
        IAcmeMonster CreateMonster();
    }

    public interface IAcmeMonster
    {
        void DoAction();
    }

    public class EvilAcmeLab : IAcmeLab
    {
        public IAcmeMonster CreateMonster()
        {
            return new EvilAcmeMonster();
        }

        private class EvilAcmeMonster : IAcmeMonster
        {
            public void DoAction()
            {
                ConsolePrint.print("I am an evil ACME Monster! I will make everyone's stok shares worth nothing!");
            }
        }
    }

    public class ChineeseAcmeLab : IAcmeLab
    {
        public IAcmeMonster CreateMonster()
        {
            return new ChineeseAcmeMonster();
        }

        private class ChineeseAcmeMonster : IAcmeMonster
        {
            public void DoAction()
            {
                ConsolePrint.print("我是一个中国的ACME的怪物！我将一杯大米吨塑料玩具！！");
            }
        }
    }

    public class AgeAcmeLab : IAcmeLab
    {
        private int age;

        public AgeAcmeLab(int age)
        {
            this.age = age;
        }

        public IAcmeMonster CreateMonster()
        {
            return new AgeMonster(age);
        }

        private class AgeMonster : IAcmeMonster
        {
            private int age;

            public AgeMonster(int age)
            {
                this.age = age;
            }

            public void DoAction()
            {
                ConsolePrint.print("I am {0} years old monster.", age);
            }
        }
    }

}