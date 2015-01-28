using System;
using System.Linq;
using System.Threading;
using System.Windows;

namespace MultiGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnDraw_Click(object sender, RoutedEventArgs e)
        {
            rectangle1.Width += 10;

            Thread t = new Thread(() =>
                                      {
                                          Enumerable.Range(0, 5).ToList().ForEach(i =>
                                            {
                                                Progress(5);
                                                Thread.Sleep(300);
                                            }
                                            );
                                      });
            t.Start();
        }

        private void Progress(int progress)
        {
            this.Dispatcher.Invoke((Action)(() => { rectangle1.Width += progress; }));
        }
    }
}
