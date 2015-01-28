using System;
using System.Windows.Forms;
using LightIndexer.Indexing;
using LightIndexerGUI.Classes;
using LightIndexerGUI.Classes.Presenters;
using LightIndexerGUI.Classes.Views;

namespace LightIndexerGUI.Forms
{
    public partial class SplashScreen : Form, ISplashScreenView
    {

        SplashScreenPresenter presenter;

        private LightIndexerApplicationContext applicationContext;

        private int count;

        internal SplashScreen(LightIndexerApplicationContext applicationContext)
            : this()
        { this.applicationContext = applicationContext; }

        public SplashScreen()
        {
            InitializeComponent();
            presenter = new SplashScreenPresenter(this);

            initBackgroundWorker.DoWork += (o, e) => count = IndexingFacade.Count;

            initBackgroundWorker.RunWorkerCompleted += (o, e) =>
                                                           {
                                                               bool isIndexEmpty = count == 0;
                                                               applicationContext.MainForm = isIndexEmpty
                                                                   ? new Tree(applicationContext) as Form
                                                                   : new LightIndexerForm() as Form;
                                                               applicationContext.MainForm.Show();
                                                               Close();
                                                           };
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            initBackgroundWorker.RunWorkerAsync();
        }

    }
}
