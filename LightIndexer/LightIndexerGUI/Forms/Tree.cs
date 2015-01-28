using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LightIndexerGUI.Classes;
using LightIndexerGUI.Classes.Models;
using LightIndexerGUI.Classes.Presenters;
using LightIndexerGUI.Classes.Views;
using log4net;

namespace LightIndexerGUI.Forms
{
    public partial class Tree : Form, ITreeView
    {

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly TreePresenter treePresenter;

        private LightIndexerApplicationContext applicationContext;

        public Tree()
        {
            InitializeComponent();
            treePresenter = new TreePresenter(this, new TreeModel());
        }

        internal Tree(LightIndexerApplicationContext applicationContext)
            : this()
        {
            this.applicationContext = applicationContext;
        }

        public TreeNodeCollection Nodes
        {
            get { return tvFolders.Nodes; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            treePresenter.OnLoad();
        }

        void tvFolders_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            treePresenter.BeforeNodeExpand(e.Node);
        }

        private void btnIndex_Click(object sender, EventArgs e)
        {
            treePresenter.StartIndex();
        }

        private void tvFolders_AfterCheck(object sender, TreeViewEventArgs e)
        {
            treePresenter.AfterCheck(e);
        }

        public Font FoldersFont
        {
            get { return tvFolders.Font; }
        }

        public void ShowIndexProgress(IEnumerable<string> paths)
        {
            //open form with progressbar for indexing
            new IndexProgress(paths).ShowDialog();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            treePresenter.DeleteIndex();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (applicationContext != null)
            {
                applicationContext.MainForm = new LightIndexerForm();
                applicationContext.MainForm.Show();
            }
        }
    }
}
