using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace Bai01
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public CauHinh newInfo { get; set; }
        private CauHinh _ch;

        public SettingsWindow(CauHinh chr)
        {
            _ch = chr;
            InitializeComponent();
        }


        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            newInfo = new CauHinh()
            {
                sv = server_TextBox.Text,
                dtb = db_TextBox.Text
            };
            
            DialogResult = true;
            MessageBoxResult result = MessageBox.Show("Thay doi thanh cong", "thong bao");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = _ch;
        }


        private void Cancle_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
