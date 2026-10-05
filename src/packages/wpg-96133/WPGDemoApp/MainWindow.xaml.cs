using System.Windows;

namespace WPGDemoApp
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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var mywin = new Window1();
            mywin.Show();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            var mywin = new Window2();
            mywin.Show();
        }
    }
}