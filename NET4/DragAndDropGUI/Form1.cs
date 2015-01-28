using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DragAndDropGUI
{
    public partial class Form1 : Form
    {
        private Point mouseDownPosition;

        private DataGridView dgvMouseDown;

        public Form1()
        {
            InitializeComponent();
            FillDgv();
        }

        private void FillDgv()
        {
            dataGridView1.Rows.Add("dgv1-r1");
            dataGridView1.Rows.Add("dgv1-r2");
            dataGridView1.Rows.Add("dgv1-r3");

            dataGridView2.Rows.Add("dgv2-r1");
            dataGridView2.Rows.Add("dgv2-r2");
            dataGridView2.Rows.Add("dgv2-r3");
        }

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                var dgv = sender as DataGridView;
                MessageBox.Show(dgv[0, e.RowIndex].Value as string);
            }
        }

        private void dgv_MouseMove(object sender, MouseEventArgs e)
        {
            var dgv = sender as DataGridView;
            var p = dgv.PointToClient(Control.MousePosition);
            var htInfo = dgv.HitTest(p.X, p.Y);
            Trace.WriteLine(string.Format("e.X:{0} e.Y:{1} e.loc:{2}\ncol:{3} row:{4}", e.X, e.Y, e.Location, htInfo.ColumnIndex, htInfo.RowIndex));

            if (e.Button != MouseButtons.Left || dgv.CurrentCell == null) return;
            object data = dgv.CurrentCell.Value;
            dgv.DoDragDrop(data, DragDropEffects.Copy);
        }

        private void dgv_DragDrop(object sender, DragEventArgs e)
        {
            var dgv = sender as DataGridView;
            object data = e.Data.GetData(DataFormats.Text);
            dgv.Rows.Add(data);
        }

        private void dgv_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = ReferenceEquals(dgvMouseDown, sender) == false ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void dgv_DragLeave(object sender, EventArgs e)
        {

        }

        private void dgv_DragOver(object sender, DragEventArgs e)
        {

        }

        private void dgv_MouseDown(object sender, MouseEventArgs e)
        {
            dgvMouseDown = sender as DataGridView;
        }

        private void dgv_MouseUp(object sender, MouseEventArgs e)
        {
            dgvMouseDown = null;
        }


    }
}
