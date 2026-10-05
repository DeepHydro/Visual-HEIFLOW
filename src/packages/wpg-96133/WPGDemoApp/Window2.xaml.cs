using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPGDemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Window2 : Window
    {

        public class tmpclass
        {
            //[TypeConverter(typeof(ExpandableObjectConverter))]
            public List<string> StringList { get; set;}
            public List<int> IntList { get; set; }
            public List<object> ObjectList { get; set; }
            public List<tmpclass> TMPlst{get;set;}

        }

        public Window2()
        {            
            InitializeComponent();            
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            myGrid.Instance = myButton;
        }

        private void myCheckbox_Click(object sender, RoutedEventArgs e)
        {
            myGrid.Instance = myCheckbox;
        }

        private void myProgressBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            myGrid.Instance = myProgressBar;
        }

       System.Windows.Forms.Button mybutt;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.Integration.WindowsFormsHost myhost = new System.Windows.Forms.Integration.WindowsFormsHost();
            mybutt = new System.Windows.Forms.Button();
            mybutt.Width = 180;
            mybutt.Height = 20;
            mybutt.Text = "Winforms Button";
            mybutt.Name = "mybutt";
            myhost.Child = mybutt;
            mybutt.Click += new EventHandler(mybutt_Click);
            myCanvasHost.Children.Add(myhost);
        }

        void mybutt_Click(object sender, EventArgs e)
        {
            myGrid.Instance = mybutt;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            myGrid.Instance = myButton2;
        }

        public tmpclass tmpClassInstace;
        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            tmpClassInstace = new tmpclass();

            tmpClassInstace.StringList = new List<string>();
            tmpClassInstace.StringList.Add("Test1");
            tmpClassInstace.StringList.Add("Test2");
            tmpClassInstace.StringList.Add("Test3");
            tmpClassInstace.StringList.Add("Test4");
            tmpClassInstace.StringList.Add("Test5");

            tmpClassInstace.IntList = new List<int>();
            tmpClassInstace.IntList.Add(1);
            tmpClassInstace.IntList.Add(2);
            tmpClassInstace.IntList.Add(3);
            tmpClassInstace.IntList.Add(4);
            tmpClassInstace.IntList.Add(5);

            tmpClassInstace.ObjectList = new List<Object>();
            tmpClassInstace.ObjectList.Add("Test1");
            tmpClassInstace.ObjectList.Add(2);
            tmpClassInstace.ObjectList.Add("Test3");
            tmpClassInstace.ObjectList.Add(4);
            tmpClassInstace.ObjectList.Add("Test5");

            myGrid.Instance = tmpClassInstace;
        }
    }
}
