using System.Drawing;
using System.Windows.Forms;

namespace PDNUtils.Help
{
    /// <summary>
    /// Extension methods for forms
    /// </summary>
    public static class GUIMethods
    {
        /// http://msdn.microsoft.com/en-us/library/system.windows.forms.form.startposition(VS.80).aspx
        /// msdn article Form.StartPosition Property
        /// <summary>The Form '<paramref name="form"/>' will be displayed centered in the '<paramref name="containerForm"/>'</summary>
        /// <param name="form">child form which is being centered</param>
        /// <param name="containerForm">parent form</param>
        public static void CenterFormTo(this Form form, Form containerForm)
        {
            Point point = new Point(); Size formSize = form.Size;
            Rectangle workingArea = Screen.GetWorkingArea(containerForm);
            Rectangle rect = containerForm.Bounds;
            point.X = ((rect.Left + rect.Right) - formSize.Width) / 2;
            if (point.X < workingArea.X) point.X = workingArea.X;
            else if ((point.X + formSize.Width) > (workingArea.X + workingArea.Width))
                point.X = (workingArea.X + workingArea.Width) - formSize.Width;
            point.Y = ((rect.Top + rect.Bottom) - formSize.Height) / 2;
            if (point.Y < workingArea.Y) point.Y = workingArea.Y;
            else if ((point.Y + formSize.Height) > (workingArea.Y + workingArea.Height))
                point.Y = (workingArea.Y + workingArea.Height) - formSize.Height;
            form.Location = point;
        }
    }
}