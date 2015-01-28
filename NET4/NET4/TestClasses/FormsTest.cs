using System;
using System.Windows.Forms;
using PDNUtils.Runner.Attributes;

namespace NET4.TestClasses
{

    [RunableClass]
    public class FormsTest
    {
        [Run(0)]
        protected void Show()
        {
            var f = new StubForm();
            Application.Run(f);
        }
    }

    public class StubForm : Form
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            new TestDialog().ShowDialog();
            this.Controls.Add(new Button());
        }
    }
}