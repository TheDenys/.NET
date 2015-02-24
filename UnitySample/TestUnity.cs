using System;
using Microsoft.Practices.Unity;
using UnitySample;

namespace NET4.TestUnity
{

    public interface IAcmeLab : IDisposable
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
                // blatant example of DI anti-pattern
                // don't repeat this at home! use ctor or property injection instead!
                UnityInstance.Container.Resolve<Action<string>>("output")("I am an evil ACME Monster! I will make everyone's stock shares worth nothing!");
            }
        }

        public void Dispose()
        {
            UnityInstance.Container.Resolve<Action<string>>("output")("Disposing evil ACMELab.");
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
                UnityInstance.Container.Resolve<Action<string>>("output")("我是一个中国的ACME的怪物！我将一杯大米吨塑料玩具！！");
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
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
                UnityInstance.Container.Resolve<Action<string>>("output")(string.Format("I am {0} years old monster.", age));
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

}