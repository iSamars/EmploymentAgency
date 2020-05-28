using System;
using System.Collections.Generic;
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
using System.Drawing;
using System.Windows.Interop;
using System.Security.Cryptography;
using EmploymentAgency.Helpers;
using MySql.Data;
using MySql.Data.MySqlClient;
using EmploymentAgency.Classes;
using EmploymentAgency.Views;

namespace EmploymentAgency
{
    /// <summary>
    /// Логика взаимодействия для Auth.xaml
    /// </summary>
    public partial class Auth : Window
    {
        int unsuccessLogins = 0;
        db db;
        private string code;
        public Auth()
        {
            InitializeComponent();
            db = new db();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            code = SetRndCode();
            SetCaptcha();
        }
        private void SetCaptcha()
        {
            string _code = code;
            Random rnd = new Random();
            Bitmap _captcha = new Bitmap(178, 39);
            System.Drawing.FontStyle style = System.Drawing.FontStyle.Italic | System.Drawing.FontStyle.Strikeout | System.Drawing.FontStyle.Regular | System.Drawing.FontStyle.Bold;
            Font font = new Font("Arial", 25f, style);

            Graphics GFX = Graphics.FromImage(_captcha);

            GFX.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            GFX.FillRectangle(System.Drawing.Brushes.LightBlue, 0, 0, _captcha.Width, _captcha.Height);

            for (int j = 0; j < 351; j++)
            {
                for (int i = 0; i < 70; i++)
                {
                    System.Drawing.Color color = System.Drawing.Color.FromArgb(rnd.Next());
                    SolidBrush brush = new SolidBrush(color);
                    GFX.FillRectangle(brush, j, i, 15, 15);
                }
            }

            int b = 10;
            int c = 3;
            Char[] _arrCode = _code.ToArray();

            for (int i = 0; i < 5; i++)
            {
                System.Drawing.Color color = System.Drawing.Color.FromArgb(rnd.Next(0, 256), rnd.Next(0, 256), rnd.Next(0, 256));
                SolidBrush brush = new SolidBrush(color);

                GFX.DrawString(_arrCode[i].ToString(), font, brush, new System.Drawing.Point(b, c));
                b += 30;
            }
            CaptchaBox.Source = Imaging.CreateBitmapSourceFromHBitmap(_captcha.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
        private string SetRndCode()
        {
            string code = String.Empty;
            Random rnd = new Random();
            for (int i = 0; i < 5; i++)
            {
                if (rnd.Next(0, 2) == 1)
                {
                    code += rnd.Next(0, 10).ToString();
                }
                else
                {
                    code += (char)(rnd.Next(1072, 1103));
                }
            }
            return code;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(captchaText.Text == code)
            {
                if(TPassword.Visibility == Visibility.Hidden)
                {
                    TPassword.Password = TLogin_Copy.Text;
                }
                if(TLogin.Text.Length > 0 && TPassword.Password.Length > 0)
                {
                    MySqlConnection conn = db.newConnection();
                    MySqlCommand cmd = new MySqlCommand("SELECT Users.id, Users.login, Users.password, Users.admin, Users.name, Users.lastname, Users.patronymic, Users.sex, Users.dateOfBirth, Countries.Country " +
                                                        "FROM Users, Countries " +
                                                        "WHERE Users.login = @login " +
                                                            "AND Countries.id = Users.country LIMIT 1", conn);
                    cmd.Parameters.AddWithValue("@login", hash.hashString(TLogin.Text));
                    try
                    {
                        MySqlDataReader reader = cmd.ExecuteReader();
                        if(reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                if (reader.GetValue(2) != null && reader.GetValue(2).ToString() == hash.hashString(TPassword.Password))
                                {
                                    bool admin = Convert.ToInt32(reader.GetValue(3)) == 1 ? true : false;
                                    User user = new User(Convert.ToInt32(reader.GetValue(0)), reader.GetValue(1).ToString(), TLogin.Text, reader.GetValue(2).ToString(), admin, reader.GetValue(4).ToString(), reader.GetValue(5).ToString(), reader.GetValue(6).ToString(), Convert.ToInt32(reader.GetValue(7)), reader.GetDateTime(8), reader.GetValue(9).ToString());
                                    if(!admin)
                                    {
                                        //user form
                                        new MainWindow(user).Show();
                                        this.Close();
                                    }
                                    else
                                    {
                                        //admin form
                                        new AdminMenu(user).Show();
                                        this.Close();
                                    }
                                    
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Пользователя {TLogin.Text} не существует!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                            code = SetRndCode();
                            SetCaptcha();
                            captchaText.Text = "";
                            unsuccessLogins++;
                            if (unsuccessLogins == 3)
                            {
                                MessageBox.Show("Слишком много неудачных попыток входа!");
                                Environment.Exit(0);
                            }
                                
                        }
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message, ex.Source);
                    }
                }
                else
                {
                    MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            else
            {
                MessageBox.Show("Капча введена неверно!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                code = SetRndCode();
                SetCaptcha();
            }
        }

        private void Register_MouseDown(object sender, MouseButtonEventArgs e)
        {
            new Forms.Register().Show();
            this.Close();
        }

        private void ImageAwesome_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(TPassword.Visibility == Visibility.Visible)
            {
                TPassword.Visibility = Visibility.Hidden;
                TLogin_Copy.Visibility = Visibility.Visible;
                TLogin_Copy.Text = TPassword.Password;
            }
            else
            {
                TPassword.Visibility = Visibility.Visible;
                TLogin_Copy.Visibility = Visibility.Hidden;
                TPassword.Password = TLogin_Copy.Text;
            }
        }

        private void TPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (TPassword.Password.Length > 0)
                passLabel.Visibility = Visibility.Hidden;
            else
                passLabel.Visibility = Visibility.Visible;
        }
    }
}
