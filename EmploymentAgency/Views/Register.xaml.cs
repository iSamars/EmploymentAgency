using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EmploymentAgency.Helpers;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace EmploymentAgency.Forms
{
    /// <summary>
    /// Логика взаимодействия для Register.xaml
    /// </summary>
    /// 
    public partial class Register : Window
    {
        db db;
        string cCode;

        public Register()
        {
            InitializeComponent();
            db = new db();
            cbSex.Items.Clear();
            cbCountry.Items.Clear();
            try
            {
                MySqlConnection conn = db.newConnection();
                MySqlCommand cmd = new MySqlCommand("SELECT Sex FROM Sexes", conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cbSex.Items.Add(reader.GetValue(0));
                }
                conn.Close();
                conn = db.newConnection();
                cmd = new MySqlCommand("SELECT Country FROM Countries", conn);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cbCountry.Items.Add(reader.GetValue(0));
                }
                cbSex.SelectedIndex = 0;
                cbCountry.SelectedIndex = 0;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(TLogin.Text.Length > 0 && TPassword.Text.Length > 6 && TPassword1.Text.Length > 6 && TName.Text.Length > 0 && TSurname.Text.Length > 0 && TPatronymic.Text.Length > 0)
            {
                //check email
                if(Regex.Match(TLogin.Text, "[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}", RegexOptions.IgnoreCase).Success)
                {
                    //valid email
                    //check password
                    if(TPassword.Text == TPassword1.Text)
                    {
                        if(Regex.Match(TPassword.Text, "[a-z]").Success && Regex.Match(TPassword.Text, "[0-9]").Success && Regex.Match(TPassword.Text, "[!@#$%^]").Success)
                        {
                            //valid password
                            //check age
                            if(dtBirth.SelectedDate.HasValue && (helper.CalculateAge(dtBirth.SelectedDate.Value) > 17))
                            {
                                //valid age
                                cCode = helper.getRndCode();
                                helper.sendMainConfirm(TName.Text, cCode, TLogin.Text);
                                Data.Visibility = Visibility.Hidden;
                                Confirm.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                MessageBox.Show("Вы должны быть старше 18 лет!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                            
                        }
                        else
                        {
                            MessageBox.Show("Пароль слишком легкий!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Пароли не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    //invalid email
                    MessageBox.Show("Неверный Email адрес!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                } 
            }
            else
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            new Auth().Show();
            this.Close();
        }

        private void bBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Data.Visibility = Visibility.Visible;
            Confirm.Visibility = Visibility.Hidden;
        }

        private void bConfirm_Click(object sender, RoutedEventArgs e)
        {
            //confirm
            if(TCode.Text == cCode)
            {
                try
                {
                    MySqlConnection conn = db.newConnection();
                    MySqlCommand cmd = new MySqlCommand("INSERT INTO Users(login, password, name, lastname, patronymic, sex, dateOfBirth, country) VALUES(@login, @password, @name, @lastname, @patronymic, @sex, @birth, (SELECT id FROM Countries WHERE Country = @country))", conn);
                    cmd.Parameters.AddWithValue("@login", hash.hashString(TLogin.Text));
                    cmd.Parameters.AddWithValue("@password", hash.hashString(TPassword.Text));
                    cmd.Parameters.AddWithValue("@name", helper.ToNameFormat(TName.Text));
                    cmd.Parameters.AddWithValue("@lastname", helper.ToNameFormat(TSurname.Text));
                    cmd.Parameters.AddWithValue("@patronymic", helper.ToNameFormat(TPatronymic.Text));
                    cmd.Parameters.AddWithValue("@sex", cbSex.SelectedItem.ToString() == "Мужской" ? 1 : 2);
                    cmd.Parameters.AddWithValue("@birth", dtBirth.SelectedDate.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@country", cbCountry.SelectedItem.ToString());
                    cmd.ExecuteNonQuery();
                    db.closeConnection();
                    MessageBox.Show("Пользователь успешно зарегистрирован!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    new Auth().Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Duplicate entry"))
                        MessageBox.Show($"Пользователь {TLogin.Text} уже существует!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Код неверный!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
