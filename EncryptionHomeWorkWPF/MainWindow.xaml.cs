using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EncryptionHomeWorkWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public CancellationTokenSource MyToken { get; set; }
        public string FilePathOne
        {
            get { return (string)GetValue(FilePathOneProperty); }
            set { SetValue(FilePathOneProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilePathOne.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilePathOneProperty =
            DependencyProperty.Register("FilePathOne", typeof(string), typeof(MainWindow));



        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(MainWindow));


        public MyThreadClass MyTC { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void BtnFileOne_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                FilePathOne = openFileDialog.FileName;
                TBOne.Text = File.ReadAllText(FilePathOne);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MyToken = new CancellationTokenSource();

            MyTC = new MyThreadClass(FilePathOne, Password);
            MyTC.MyEvent += MyTC_MyEvent;
            TBOne.Text = File.ReadAllText(FilePathOne);
            TBTwo.Text = "";
            ThreadPool.QueueUserWorkItem(MyTC.Do, MyToken.Token);
        }

        private void MyTC_MyEvent(long Max, int Progress, char ch)
        {
            if (Max == Progress)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    PB.Maximum = 100;
                    PB.Value = 0;

                    TBTwo.Text += ch;

                    TBOne.Text = TBTwo.Text;
                }));
                MessageBox.Show("Progress Succesfull");
                MyTC.MyEvent -= MyTC_MyEvent;
                MyTC = null;
            }
            else
                this.Dispatcher.Invoke(new Action(() =>
                {
                    PB.Maximum = Max;
                    PB.Value = Progress;
                    if (!MyToken.IsCancellationRequested)
                        TBTwo.Text += ch;
                }));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MyTC != null)
                MyTC.MyEvent -= MyTC_MyEvent;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MyToken.Cancel();
            //TBOne.Text = File.ReadAllText(FilePathOne);
            TBOne.Text = MyTC.OldValue;
            TBTwo.Text = "";
            {

            }
        }
    }
}
