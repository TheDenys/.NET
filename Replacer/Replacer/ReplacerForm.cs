using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Replacer.Instruments;

namespace Replacer
{
    public partial class ReplacerForm : Form
    {
        private Replacer r;

        private Instrument selectedInstrument;

        public ReplacerForm()
        {
            InitializeComponent();
            SetYNEnable(false);
            FillInstrumentsBox();
            UpdateSelectedInstrument();
        }

        private void FillInstrumentsBox()
        {
            cbInstrument.DisplayMember = "Item1";
            cbInstrument.ValueMember = "Item2";
            cbInstrument.DataSource = new List<Tuple<string, Instrument>>
                                          {
                                              new Tuple<string, Instrument>("Garble", new GarbleInstrument()),
                                              new Tuple<string, Instrument>("Guid", new GuidInstrument()),
                                          };
        }

        private void tbFileName_DragEnter(object sender, DragEventArgs e)
        {
            // Check if the Dataformat of the data can be accepted
            // (we only accept file drops from Explorer, etc.)
            bool acceptDragData = e.Data.GetDataPresent(DataFormats.FileDrop);
            e.Effect = acceptDragData ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void tbFileName_DragDrop(object sender, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];
            (sender as TextBox).Text = files[0];
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            string filename = tbFileName.Text;

            if (string.IsNullOrWhiteSpace(filename))
            {
                return;
            }

            DoInteractiveReplace(filename);
        }

        private void DoInteractiveReplace(string filename)
        {
            progressBar1.Value = 0;
            btnReplace.Enabled = false;
            r = new Replacer(filename, tbPrefix.Text, tbSuffix.Text, selectedInstrument);
            r.OnConfirm += ReplaceConfirm;
            r.OnFinish += FinishedReplace;
            r.Start();
        }

        private void FinishedReplace()
        {
            r.Dispose();
            r = null;
            progressBar1.Value = 100;
            btnReplace.Enabled = true;
            SetYNEnable(false);
        }

        private void ReplaceConfirm(Tuple<string, string> tuple)
        {
            progressBar1.Value = r.PercentDone;

            string item1 = tuple.Item1;
            textBox1.Text = item1;
            textBox2.Text = tuple.Item2;

            if (item1 != null && item1.IndexOf(Environment.NewLine, StringComparison.InvariantCulture) != -1)
            {
                MessageBox.Show("Multiline detected!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            SetYNEnable(true);
        }

        private void SetYNEnable(bool enable)
        {
            btnNo.Enabled = enable;
            btnYes.Enabled = enable;
            btnCancel.Enabled = enable;
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            SetYNEnable(false);
            r.SendConfirm(true);
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            SetYNEnable(false);
            r.SendConfirm(false);
        }

        private void UpdateSelectedInstrument()
        {
            selectedInstrument = cbInstrument.SelectedValue as Instrument;
        }

        private void cbInstrument_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateSelectedInstrument();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string fn = tbFileName.Text;

            if (!string.IsNullOrWhiteSpace(fn))
            {
                openFileDialog.FileName = fn;
            }
            else
            {
                openFileDialog.InitialDirectory = Environment.CurrentDirectory;
            }

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                tbFileName.Text = openFileDialog.FileName;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (r != null)
            {
                r.Close();
            }
        }
    }
}
