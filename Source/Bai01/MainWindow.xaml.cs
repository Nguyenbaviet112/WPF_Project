using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
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

namespace Bai01
{
    public static class StringExtension
    {
        public static bool IsEmpty(this string data)
        {
            bool result = data.Length == 0;
            return result;

            //return data.Length == 0; nên gán kết quả cho 1 biến trước khi trả về. Vì khi debug chúng ta sẽ thấy được kết
            //quả đúng hay sai, nếu return về trực tiếp khi debug sẽ không nhìn thấy được giá trị trả về.
        }
    }

    public class CauHinh : INotifyPropertyChanged
    {
        public string sv { get; set; } = "";

        public string dtb { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public string username_lb { get; set; } = "";
        public string password_lb { get; set; } = "";
        public CauHinh chr { get; set; } = null;
        public string Username { get; set; } = "";

        public string Password_var { get; set; } = "";



        public event PropertyChangedEventHandler PropertyChanged;

        private void usernameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            username_Label.Visibility = Visibility.Hidden;
        }

        private void usernameTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (usernameTextBox.Text.Length == 0)
            {
                username_Label.Visibility = Visibility.Visible;
            }
            else
            {
                username_Label.Visibility = Visibility.Hidden;
            }

            
        }

        private void passwordTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            password_Label.Visibility = Visibility.Hidden;
        }

        private void passwordTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (passwordTextBox.Password.Length == 0)
            {
                password_Label.Visibility = Visibility.Visible;
            }
            else
            {
                password_Label.Visibility = Visibility.Hidden;
            }    

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            versionLabel.Content = $"v{version}";

            username_lb = "username";
            password_lb = "password";


            chr = new CauHinh()
            {
                sv = ConfigurationManager.AppSettings["server"],
                dtb = ConfigurationManager.AppSettings["database"]
            };

            this.DataContext = chr;
            this.DataContext = this;

        }


        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {


            var screen = new SettingsWindow(chr);

            if (screen.ShowDialog() == true)
            {
                var newinfo = screen.newInfo;
                chr.sv = newinfo.sv;
                chr.dtb = newinfo.dtb;
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["server"].Value = chr.sv;
                config.AppSettings.Settings["database"].Value = chr.dtb;
                config.Save(ConfigurationSaveMode.Minimal);
            }

        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            Password_var = passwordTextBox.Password;

            //Method Extension
            if (Username.IsEmpty() && Password_var.IsEmpty())
            {
                MessageBox.Show("Username và Password không được để trống!");
            }

            else if (Username.IsEmpty())  //IsEmpty(Username).
            {
                MessageBox.Show("Username không được để trống!");
            }
            else if(Password_var.IsEmpty())
            {
                MessageBox.Show("Password không được để trống!");
            }

            else if (Username == "nv" && Password_var == "123")
            {
                var screen = new DashboardWindow();

                screen.Dying += XuLiTruocLucChet;

                screen.Show();


                this.Hide();

            }


            else if (Username == "admin" && Password_var == "123")
            {
                var screen = new AdminWindow();

                screen.Dying += XuLiTruocLucChet;

                screen.Show();


                this.Hide();

            }

            else
            {
                MessageBox.Show("Tài khoản không tồn tại");
            }                

        }

        private void ReSet()
        {
            Username = "";
            passwordTextBox.Password = "";

            username_lb = "username";
            password_lb = "password";

            if (usernameTextBox.Text.Length == 0)
            {
                username_Label.Visibility = Visibility.Visible;
            }
            else
            {
                username_Label.Visibility = Visibility.Hidden;
            }

            if (passwordTextBox.Password.Length == 0)
            {
                password_Label.Visibility = Visibility.Visible;
            }
            else
            {
                password_Label.Visibility = Visibility.Hidden;
            }

        }

        private void XuLiTruocLucChet()
        {
            ReSet();
            this.Show();
        }
    }
}
