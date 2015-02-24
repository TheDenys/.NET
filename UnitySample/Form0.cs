using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using Microsoft.Practices.Unity;
using System.Windows.Forms;
using Microsoft.Practices.Unity.Configuration;
using NET4.TestUnity;

namespace UnitySample
{
    public partial class Form0 : Form
    {
        public Form0()
        {
            InitializeComponent();

            // register method for output messages
            // RegisterInstance has singleton behavior
            UnityInstance.Container.RegisterInstance<Action<string>>("output",
                (s) =>
                {
                    textBox1.AppendText(DateTime.Now.ToString("hh:mm:ss.fff    ", CultureInfo.InvariantCulture));
                    textBox1.AppendText(s);
                    textBox1.AppendText(Environment.NewLine);
                });

        }

        private void btnEvilAcme_Click(object sender, EventArgs e)
        {
            IUnityContainer container = new UnityContainer();

            // register
            container.RegisterType<IAcmeLab, EvilAcmeLab>("evil");

            // resolve
            IAcmeLab lab = container.Resolve<IAcmeLab>("evil");

            // use
            IAcmeMonster monster = lab.CreateMonster();
            monster.DoAction();
        }

        private void btnChineseAcme_Click(object sender, EventArgs e)
        {
            IUnityContainer container = new UnityContainer();

            // register
            container.RegisterType<IAcmeLab, ChineeseAcmeLab>("chinese");

            // resolve
            IAcmeLab lab = container.Resolve<IAcmeLab>("chinese");

            // use
            IAcmeMonster monster = lab.CreateMonster();
            monster.DoAction();
        }

        private void btnOldAcme_Click(object sender, EventArgs e)
        {
            IUnityContainer container = new UnityContainer();

            // register
            container.RegisterType<IAcmeLab, AgeAcmeLab>();

            // resolve
            int age = (int)numericUpDownAge.Value;
            IAcmeLab lab = container.Resolve<IAcmeLab>(new ParameterOverride("age", age));

            // use
            IAcmeMonster monster = lab.CreateMonster();
            monster.DoAction();
        }

        private void btnTransient_Click(object sender, EventArgs e)
        {
            IUnityContainer container = new UnityContainer();

            // register transient
            container.RegisterType<object, object>("object");

            //resolve
            object o1 = UnityInstance.Container.Resolve<object>("object");
            object o2 = UnityInstance.Container.Resolve<object>("object");

            // use
            UnityInstance.Container.Resolve<Action<string>>("output")(string.Format("o1 {0} o2",
                                                                      o1 == o2 ? "equals to" : "is not equal to"));
        }

        private void btnSingleton_Click(object sender, EventArgs e)
        {
            IUnityContainer container = new UnityContainer();

            // register as singleton
            // Note: it's a singleton within container instance.
            container.RegisterType<object, object>("single", new ContainerControlledLifetimeManager());

            // resolve
            object s1 = container.Resolve<object>("single");
            object s2 = container.Resolve<object>("single");

            // use
            UnityInstance.Container.Resolve<Action<string>>("output")(string.Format("s1 {0} s2",
                                                                      s1 == s2 ? "equals to" : "is not equal to"));
        }

        private void btnNonregistered_Click(object sender, EventArgs e)
        {
            IUnityContainer container = new UnityContainer();

            // NonRegisteredType was not registered in container!
            container.Resolve<NonRegisteredType>();
        }

        private void btnResolveAll_Click(object sender, EventArgs e)
        {
            IUnityContainer container = new UnityContainer();
            
            // register
            container
                .RegisterType<IAcmeLab, EvilAcmeLab>("evil")
                .RegisterType<IAcmeLab, ChineeseAcmeLab>("chinese");

            // resolve all
            // This can be used in plugin system
            IEnumerable<IAcmeLab> list = container.ResolveAll<IAcmeLab>();


            // use
            foreach (IAcmeLab lab in list)
            {
                lab.CreateMonster().DoAction();
            }
        }

        private void btnInjectDependencies_Click(object sender, EventArgs e)
        {
            IUnityContainer container = new UnityContainer();
            
            ClassWithDependencies cwd = new ClassWithDependencies();

            // resolve dependencies for us
            container.BuildUp(cwd);
        }

        private void btnCtorInjection_Click(object sender, EventArgs e)
        {
            IUnityContainer container = new UnityContainer();

            // configure injection
            container.Configure<InjectedMembers>().
                ConfigureInjectionFor<ClassWithEvenMoreDependecies>(
                    new InjectionConstructor("Form0"),
                    new InjectionProperty("Logger")
                );

            // resolve
            ClassWithEvenMoreDependecies cwemd = container.Resolve<ClassWithEvenMoreDependecies>();
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            IUnityContainer container = new UnityContainer().LoadConfiguration();
            ITest iTest = container.Resolve<ITest>("Test");
        }
    }
}
