using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EmploymentAgency.Classes;
using EmploymentAgency.Helpers;
using Microsoft.Win32;
using MySql.Data.MySqlClient;

#pragma warning disable CA2100

namespace EmploymentAgency.Views
{
    /// <summary>
    /// Логика взаимодействия для AdminMenu.xaml
    /// </summary>
    public partial class AdminMenu : Window
    {
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        Event @event;
        db db;
        System.Drawing.Image img;
        User user;
        List<Vacancy> vacancies = new List<Vacancy>();
        int currentVacanciesPage = 0;
        public AdminMenu(User user)
        {
            db = new db();
            this.user = user;
            InitializeComponent();
            for(int i = 0; i < 24; i++)
            {
                cbTimeH.Items.Add($"{i}");
            }
            for (int i = 0; i < 60; i++)
            {
                cbTimeM.Items.Add($"{i}");
                cbTimeS.Items.Add($"{i}");
            }
            cbTimeH.SelectedIndex = 0;
            cbTimeM.SelectedIndex = 0;
            cbTimeS.SelectedIndex = 0;
            MessageBox.Show($"{helper.WelcomeTime(DateTime.Now)}, {helper.WelcomeText(user.getsex())} {user.getname()}!");
            lWelcome.Content = $"Привет, {user.getname()}";
            MySqlConnection conn = db.newConnection();
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM Event LIMIT 1", conn);
            try
            {
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        @event = new Event(reader.GetValue(0).ToString(), reader.GetValue(1).ToString(), reader.GetDateTime(2));
                    }
                }
                lAdHeader.Content = @event.getname();
                lAdBody.Text = "Мероприятие: " + @event.getdescription();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private void timerTick(object sender, EventArgs e)
        {
            TimeSpan dtTo = @event.getdate().Subtract(DateTime.Now);
            lTime.Content = $"{dtTo.Days}д, {dtTo.Hours}ч, {dtTo.Minutes}мин, {dtTo.Seconds}сек";
            lTime1.Content = $"До мероприятия: {dtTo.Days}д, {dtTo.Hours}ч, {dtTo.Minutes}мин, {dtTo.Seconds}сек";
        }

        private void closeMenuBar()
        {
            ThicknessAnimation tAnim = new ThicknessAnimation();
            tAnim.From = new Thickness(0, 0, 0, 0);
            tAnim.To = new Thickness(-270, 0, 0, 0);
            tAnim.Duration = TimeSpan.FromSeconds(0.25);
            MenuCanvas.BeginAnimation(Canvas.MarginProperty, tAnim);

            DoubleAnimation dAnim = new DoubleAnimation();
            dAnim.From = 557;
            dAnim.To = 530;
            dAnim.Duration = TimeSpan.FromSeconds(0.25);
            timeCanvas.BeginAnimation(Canvas.TopProperty, dAnim);
        }

        private void bMain_Click(object sender, RoutedEventArgs e)
        {
            closeMenuBar();
            AdCanvas.Visibility = Visibility.Visible;
            EditProfileCanvas.Visibility = Visibility.Hidden;
            EditEventCanvas.Visibility = Visibility.Hidden;
            AddBuyedAdCanvas.Visibility = Visibility.Hidden;
            VacanciesCanvas.Visibility = Visibility.Hidden;
            EditDataCanvas.Visibility = Visibility.Hidden;
            //All canvases hidden
        }

        private void openMenuBar()
        {
            ThicknessAnimation tAnim = new ThicknessAnimation();
            tAnim.From = new Thickness(-270, 0, 0, 0);
            tAnim.To = new Thickness(0, 0, 0, 0);
            tAnim.Duration = TimeSpan.FromSeconds(0.25);
            MenuCanvas.BeginAnimation(Canvas.MarginProperty, tAnim);

            DoubleAnimation dAnim = new DoubleAnimation();
            dAnim.From = 530;
            dAnim.To = 557;
            dAnim.Duration = TimeSpan.FromSeconds(0.25);
            timeCanvas.BeginAnimation(Canvas.TopProperty, dAnim);
        }

        private void backIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            closeMenuBar();
        }

        private void menuIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            openMenuBar();
        }

        private void bEdit_Click(object sender, RoutedEventArgs e)
        {
            bool changed = false;
            bool suc = false;
            if (TName.Text.Length > 0)
            {
                user.setname(TName.Text);
                changed = true;
            }
            if (TSurname.Text.Length > 0)
            {
                user.setlastname(TSurname.Text);
                changed = true;
            }
            if (TPatronymic.Text.Length > 0)
            {
                user.setpatronymic(TPatronymic.Text);
                changed = true;
            }
            if (user.getsex() != cbSex.SelectedIndex + 1)
            {
                user.setsex(cbSex.SelectedIndex + 1);
                changed = true;
            }
            if (user.getcountry() != cbCountry.SelectedItem.ToString())
            {
                user.setcountry(cbCountry.SelectedItem.ToString());
                changed = true;
            }
            if (user.getdateOfBirth().ToString("yyyy-MM-dd") != dtBirth.SelectedDate.Value.ToString("yyyy-MM-dd"))
            {
                user.setdateOfBirth(dtBirth.SelectedDate.Value);
                changed = true;
            }
            if (changed)
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (o, ea) =>
                {
                    Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Visible));
                    try
                    {
                        MySqlConnection conn = db.newConnection();
                        MySqlCommand cmd = new MySqlCommand("UPDATE Users SET name=@name, lastname=@lastname, patronymic=@patronymic, sex=@sex, dateOfBirth=@birth, country=(SELECT id FROM Countries WHERE country = @country) WHERE id = @id", conn);
                        cmd.Parameters.AddWithValue("@name", Dispatcher.Invoke(() => helper.ToNameFormat(user.getname())));
                        cmd.Parameters.AddWithValue("@id", Dispatcher.Invoke(() => user.getid()));
                        cmd.Parameters.AddWithValue("@lastname", Dispatcher.Invoke(() => helper.ToNameFormat(user.getlastname())));
                        cmd.Parameters.AddWithValue("@patronymic", Dispatcher.Invoke(() => helper.ToNameFormat(user.getpatronymic())));
                        cmd.Parameters.AddWithValue("@sex", Dispatcher.Invoke(() => cbSex.SelectedItem.ToString() == "Мужской" ? 1 : 2));
                        cmd.Parameters.AddWithValue("@birth", Dispatcher.Invoke(() => dtBirth.SelectedDate.Value.ToString("yyyy-MM-dd")));
                        cmd.Parameters.AddWithValue("@country", Dispatcher.Invoke(() => cbCountry.SelectedItem.ToString()));
                        cmd.ExecuteNonQuery();
                        db.closeConnection();
                        
                        Dispatcher.Invoke((Action)(() =>
                        {
                            lWelcome.Content = $"Привет, {user.getname()}";
                            suc = true;
                        }));


                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke((Action)(() => suc = false));
                        MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                };
                worker.RunWorkerCompleted += (o, ea) =>
                {
                    Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Hidden));
                };
                worker.RunWorkerAsync();

            }
            if (TPassword.Text.Length > 0 && TPassword.Text.Length > 0)
            {
                if (TPassword.Text == TPassword1.Text)
                {
                    if (Regex.Match(TPassword.Text, "[a-z]").Success && Regex.Match(TPassword.Text, "[0-9]").Success && Regex.Match(TPassword.Text, "[!@#$%^]").Success)
                    {
                        BackgroundWorker worker = new BackgroundWorker();
                        worker.DoWork += (o, ea) =>
                        {
                            Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Visible));
                            try
                            {
                                MySqlConnection conn = db.newConnection();
                                MySqlCommand cmd = new MySqlCommand("UPDATE Users SET password=@pass WHERE id = @id", conn);
                                cmd.Parameters.AddWithValue("@pass", Dispatcher.Invoke(() => hash.hashString(TPassword.Text)));
                                cmd.Parameters.AddWithValue("@id", Dispatcher.Invoke(() => user.getid()));
                                cmd.ExecuteNonQuery();
                                db.closeConnection();
                                Dispatcher.Invoke((Action)(() => suc = true));
                            }
                            catch (Exception ex)
                            {
                                Dispatcher.Invoke((Action)(() => suc = false));
                                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        };
                        worker.RunWorkerCompleted += (o, ea) =>
                        {
                            Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Hidden));
                        };
                        worker.RunWorkerAsync();

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
            if (suc)
            {
                MessageBox.Show("Данные обновлены!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void bEditProfile_Click(object sender, RoutedEventArgs e)
        {
            closeMenuBar();
            AddBuyedAdCanvas.Visibility = Visibility.Hidden;
            EditEventCanvas.Visibility = Visibility.Hidden;
            AdCanvas.Visibility = Visibility.Hidden;
            VacanciesCanvas.Visibility = Visibility.Hidden;
            EditDataCanvas.Visibility = Visibility.Hidden;
            TPassword.Text = "";
            TPassword1.Text = "";
            TName.Text = "";
            TSurname.Text = "";
            TPatronymic.Text = "";
            EditProfileCanvas.Visibility = Visibility.Visible;
            cbSex.Items.Clear();
            cbCountry.Items.Clear();

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Visible));
                MySqlConnection conn = db.newConnection();
                MySqlCommand cmd = new MySqlCommand("SELECT Sex FROM Sexes", conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Dispatcher.Invoke((Action)(() => cbSex.Items.Add(reader.GetValue(0))));
                }
                conn.Close();
                conn = db.newConnection();
                cmd = new MySqlCommand("SELECT Country FROM Countries", conn);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Dispatcher.Invoke((Action)(() => cbCountry.Items.Add(reader.GetValue(0))));
                }
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Hidden));
            };
            worker.RunWorkerAsync();


            cbSex.SelectedIndex = user.getsex() - 1;
            cbCountry.SelectedItem = user.getcountry();
            lEmail.Content = user.getunhashedEmail();
            dtBirth.SelectedDate = user.getdateOfBirth();
        }

        private void bExit_Click(object sender, RoutedEventArgs e)
        {
            new Auth().Show();
            this.Close();
        }

        private void bEditEvent_Click(object sender, RoutedEventArgs e)
        {
            if(tEventName.Text.Length > 0 && tEventBody.Text.Length > 0 && dtEventDate.SelectedDate.Value > DateTime.Now)
            {
                DateTime dt = new DateTime();
                dt = dt.AddHours(Int32.Parse(cbTimeH.SelectedIndex.ToString()));
                dt = dt.AddMinutes(Int32.Parse(cbTimeM.SelectedIndex.ToString()));
                dt = dt.AddSeconds(Int32.Parse(cbTimeS.SelectedIndex.ToString()));
                string date = dtEventDate.SelectedDate.Value.ToString("yyyy-MM-dd") + $" {dt.ToString("HH:mm:ss")}";

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (o, ea) =>
                {
                    Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Visible));
                    MySqlConnection conn = db.newConnection();
                    MySqlCommand cmd = new MySqlCommand("UPDATE Event SET name=@name, description=@desc, date=@date", conn);
                    cmd.Parameters.AddWithValue("@name", Dispatcher.Invoke(() => tEventName.Text));
                    cmd.Parameters.AddWithValue("@desc", Dispatcher.Invoke(() => tEventBody.Text));
                    cmd.Parameters.AddWithValue("@date", Dispatcher.Invoke(() => date));
                    cmd.ExecuteNonQuery();
                };
                worker.RunWorkerCompleted += (o, ea) =>
                {
                    Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Hidden));
                    @event.setdate(DateTime.Parse(Dispatcher.Invoke(() => date)));
                    MessageBox.Show("Данные успешно обновлены!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                };
                worker.RunWorkerAsync();

            }
            else
            {
                MessageBox.Show("Данные заполнены неверно!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void bEditEvent1_Click(object sender, RoutedEventArgs e)
        {
            closeMenuBar();
            AddBuyedAdCanvas.Visibility = Visibility.Hidden;
            EditProfileCanvas.Visibility = Visibility.Hidden;
            VacanciesCanvas.Visibility = Visibility.Hidden;
            EditDataCanvas.Visibility = Visibility.Hidden;
            AdCanvas.Visibility = Visibility.Hidden;
            EditEventCanvas.Visibility = Visibility.Visible;
        }

        private void bAddAdImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            img = System.Drawing.Image.FromFile(ofd.FileName);
            ImageSourceConverter imgs = new ImageSourceConverter();
            iAdImage.Source = imgs.ConvertFromString(ofd.FileName) as ImageSource;
        }

        private void bAddAd_Click(object sender, RoutedEventArgs e)
        {
            if(iAdImage.Source != null && tAdBody.Text.Length > 0 && tAdTitle.Text.Length > 0 && tAdFooter.Text.Length > 0)
            {
                try
                {
                    byte[] b = helper.imageToByteArray(img);
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += (o, ea) =>
                    {
                        Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Visible));
                        MySqlConnection conn = db.newConnection();
                        MySqlCommand cmd = new MySqlCommand("INSERT INTO Ads(image, title, conditions, address) VALUES(@image, @title, @conditions, @address)", conn);
                        ImageSourceConverter imgs = new ImageSourceConverter();
                        cmd.Parameters.AddWithValue("@image", Dispatcher.Invoke(() => b));
                        cmd.Parameters.AddWithValue("@title", Dispatcher.Invoke(() => tAdTitle.Text));
                        cmd.Parameters.AddWithValue("@conditions", Dispatcher.Invoke(() => tAdBody.Text));
                        cmd.Parameters.AddWithValue("@address", Dispatcher.Invoke(() => tAdFooter.Text));
                        cmd.ExecuteNonQuery();
                        db.closeConnection();
                    };
                    worker.RunWorkerCompleted += (o, ea) =>
                    {
                        Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Hidden));
                        MessageBox.Show("Добавлено!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    };
                    worker.RunWorkerAsync();
                    img = null;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Зполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void bAddAdCanvas_Click(object sender, RoutedEventArgs e)
        {
            closeMenuBar();
            AdCanvas.Visibility = Visibility.Hidden;
            EditProfileCanvas.Visibility = Visibility.Hidden;
            EditEventCanvas.Visibility = Visibility.Hidden;
            VacanciesCanvas.Visibility = Visibility.Hidden;
            EditDataCanvas.Visibility = Visibility.Hidden;
            iAdImage.Source = null;
            tAdBody.Text = "";
            tAdFooter.Text = "";
            tAdTitle.Text = "";
            AddBuyedAdCanvas.Visibility = Visibility.Visible;
        }

        private void bSearchVacancy_Click(object sender, RoutedEventArgs e)
        {
            if (TSearch.Text.Length > 0 || lbFiltersType.SelectedIndex > -1)
            {
                vacancies.Clear();
                string column = cbSearchFilter.SelectedIndex == 0 ? "name" : cbSearchFilter.SelectedIndex == 1 ? "description" : "address";
                string cmdText = helper.generateSerchQuery(ref cbFilters, ref TSearch, ref lbFiltersType, column);
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (o, ea) =>
                {
                    Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Visible));
                    MySqlCommand cmd = new MySqlCommand(cmdText, db.newConnection());
                    if (cmdText.Contains("LIKE"))
                        Dispatcher.Invoke((Action)(() => cmd.Parameters.AddWithValue("@Search", $"%{TSearch.Text}%")));
                    try
                    {
                        MySqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Dispatcher.Invoke((Action)(() => vacancies.Add(new Vacancy(reader.GetInt32(0), helper.byteArrayToImage((byte[])reader[1]), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetValue(5).ToString(), reader.GetString(6), reader.GetString(7)))));
                            }
                            Dispatcher.Invoke((Action)(() =>
                            {
                                currentVacanciesPage = 0;
                                title.Text = vacancies[0].gettitle();
                                body.Text = vacancies[0].getbody() + $"\nОклад: {vacancies[0].getsalary()}\nРежим работы: {vacancies[0].getmode()}\nОбразование: {vacancies[0].geteducation()}";
                                address.Text = vacancies[0].getaddress();
                                vacancyImage.Source = vacancies[0].getimage();
                                vacanciesPages.Text = $"страница 1 из {vacancies.Count}";
                            }));
                        }
                        else
                        {
                            MessageBox.Show("По вашему запросу ничего не найдено", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                };
                worker.RunWorkerCompleted += (o, ea) =>
                {
                    Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Hidden));
                };
                worker.RunWorkerAsync();
            }
            else
            {
                cbSearchFilter.SelectedIndex = 0;
                vacancies.Clear();
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (o, ea) =>
                {
                    Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Visible));
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM Vacancies", db.newConnection());
                    try
                    {
                        MySqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                Dispatcher.Invoke((Action)(() => vacancies.Add(new Vacancy(reader.GetInt32(0), helper.byteArrayToImage((byte[])reader[1]), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetValue(5).ToString(), reader.GetString(6), reader.GetString(7)))));
                            }
                            Dispatcher.Invoke((Action)(() =>
                            {
                                currentVacanciesPage = 0;
                                title.Text = vacancies[0].gettitle();
                                body.Text = vacancies[0].getbody() + $"\nОклад: {vacancies[0].getsalary()}\nРежим работы: {vacancies[0].getmode()}\nОбразование: {vacancies[0].geteducation()}";
                                address.Text = vacancies[0].getaddress();
                                vacancyImage.Source = vacancies[0].getimage();
                                vacanciesPages.Text = $"страница 1 из {vacancies.Count}";
                            }));
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                };
                worker.RunWorkerCompleted += (o, ea) =>
                {
                    Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Hidden));
                };
                worker.RunWorkerAsync();
            }
        }
        private void cbFilters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cbFilters.SelectedIndex)
            {
                case -1:
                {
                    lbFiltersType.Items.Clear();
                    break;
                }
                case 0:
                {
                    lbFiltersType.Items.Clear();
                    lbFiltersType.Items.Add("до 30 000 Р");
                    lbFiltersType.Items.Add("30 000 - 50 000 Р");
                    lbFiltersType.Items.Add("50 000 - 75 000 Р");
                    lbFiltersType.Items.Add("75 000 - 100 000 Р");
                    lbFiltersType.Items.Add("свыше 100 000 Р");
                    break;
                }
                case 1:
                {
                    lbFiltersType.Items.Clear();
                    BackgroundWorker worker = new BackgroundWorker();

                    worker.DoWork += (o, ea) =>
                    {
                        Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Visible));
                        MySqlCommand cmd = new MySqlCommand("SELECT mode FROM JobModes", db.newConnection());
                        try
                        {
                            MySqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                Dispatcher.Invoke((Action)(() => lbFiltersType.Items.Add(reader.GetString(0))));
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    };
                    worker.RunWorkerCompleted += (o, ea) =>
                    {
                        Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Hidden));
                    };
                    worker.RunWorkerAsync();
                    break;
                }
                case 2:
                {
                    lbFiltersType.Items.Clear();
                    BackgroundWorker worker = new BackgroundWorker();

                    worker.DoWork += (o, ea) =>
                    {
                        Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Visible));
                        MySqlCommand cmd = new MySqlCommand("SELECT education FROM Educations", db.newConnection());
                        try
                        {
                            MySqlDataReader reader = cmd.ExecuteReader();
                            while (reader.Read())
                            {
                                Dispatcher.Invoke((Action)(() => lbFiltersType.Items.Add(reader.GetString(0))));
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    };
                    worker.RunWorkerCompleted += (o, ea) =>
                    {
                        Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Hidden));
                    };
                    worker.RunWorkerAsync();

                    break;
                }
            }
        }

        private void backButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (currentVacanciesPage > 0)
            {
                currentVacanciesPage--;
                title.Text = vacancies[currentVacanciesPage].gettitle();
                body.Text = vacancies[currentVacanciesPage].getbody() + $"\nОклад: {vacancies[currentVacanciesPage].getsalary()}\nРежим работы: {vacancies[currentVacanciesPage].getmode()}\nОбразование: {vacancies[currentVacanciesPage].geteducation()}";
                address.Text = vacancies[currentVacanciesPage].getaddress();
                vacancyImage.Source = vacancies[currentVacanciesPage].getimage();
                vacanciesPages.Text = $"страница {currentVacanciesPage + 1} из {vacancies.Count}";
            }
        }

        private void nextButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (currentVacanciesPage < vacancies.Count - 1)
            {
                currentVacanciesPage++;
                title.Text = vacancies[currentVacanciesPage].gettitle();
                body.Text = vacancies[currentVacanciesPage].getbody() + $"\nОклад: {vacancies[currentVacanciesPage].getsalary()}\nРежим работы: {vacancies[currentVacanciesPage].getmode()}\nОбразование: {vacancies[currentVacanciesPage].geteducation()}";
                address.Text = vacancies[currentVacanciesPage].getaddress();
                vacancyImage.Source = vacancies[currentVacanciesPage].getimage();
                vacanciesPages.Text = $"страница {currentVacanciesPage + 1} из {vacancies.Count}";
            }
        }

        private void bOpenFilters_Click(object sender, RoutedEventArgs e)
        {
            ThicknessAnimation tAnim = new ThicknessAnimation();
            tAnim.From = new Thickness(0, 0, 0, 0);
            tAnim.To = new Thickness(-285, 0, 0, 0);
            tAnim.Duration = TimeSpan.FromSeconds(0.25);
            filtersCanvas.BeginAnimation(Canvas.MarginProperty, tAnim);
        }

        private void closeFilters_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ThicknessAnimation tAnim = new ThicknessAnimation();
            tAnim.From = new Thickness(-285, 0, 0, 0);
            tAnim.To = new Thickness(0, 0, 0, 0);
            tAnim.Duration = TimeSpan.FromSeconds(0.25);
            filtersCanvas.BeginAnimation(Canvas.MarginProperty, tAnim);
        }
        private void bClearFilter_Click(object sender, RoutedEventArgs e)
        {
            lbFiltersType.SelectedIndex = -1;
        }

        private void bSearchData_Click(object sender, RoutedEventArgs e)
        {
            closeMenuBar();
            cbSearchFilter.SelectedIndex = 0;
            AdCanvas.Visibility = Visibility.Hidden;
            EditProfileCanvas.Visibility = Visibility.Hidden;
            EditEventCanvas.Visibility = Visibility.Hidden;
            AddBuyedAdCanvas.Visibility = Visibility.Hidden;
            EditDataCanvas.Visibility = Visibility.Hidden;

            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (o, ea) =>
            {
                Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Visible));
                MySqlConnection conn = db.newConnection();
                MySqlCommand cmd = new MySqlCommand("SELECT Vacancies.id, Vacancies.image, Vacancies.name, Vacancies.description, Vacancies.address, Vacancies.salary, JobModes.`mode`, Educations.education FROM Vacancies, JobModes, Educations WHERE JobModes.id = Vacancies.`mode` AND Educations.id = Vacancies.education", conn);
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        vacancies.Clear();
                        while (reader.Read())
                        {
                            Dispatcher.Invoke((Action)(() => vacancies.Add(new Vacancy(reader.GetInt32(0), helper.byteArrayToImage((byte[])reader[1]), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetValue(5).ToString(), reader.GetString(6), reader.GetString(7)))));
                        }
                        Dispatcher.Invoke((Action)(() =>
                        {
                            currentVacanciesPage = 0;
                            title.Text = vacancies[0].gettitle();
                            body.Text = vacancies[0].getbody() + $"\nОклад: {vacancies[0].getsalary()}\nРежим работы: {vacancies[0].getmode()}\nОбразование: {vacancies[0].geteducation()}";
                            address.Text = vacancies[0].getaddress();
                            vacancyImage.Source = vacancies[0].getimage();
                            vacanciesPages.Text = $"страница 1 из {vacancies.Count}";
                        }));

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Hidden));
            };
            worker.RunWorkerAsync();

            VacanciesCanvas.Visibility = Visibility.Visible;
        }

        private void bEditData_Click(object sender, RoutedEventArgs e)
        {
            AdCanvas.Visibility = Visibility.Hidden;
            EditProfileCanvas.Visibility = Visibility.Hidden;
            EditEventCanvas.Visibility = Visibility.Hidden;
            AddBuyedAdCanvas.Visibility = Visibility.Hidden;
            VacanciesCanvas.Visibility = Visibility.Hidden;
            EditDataCanvas.Visibility = Visibility.Visible;
            BackgroundWorker worker = new BackgroundWorker();
            cbSearchDataFilter.SelectedIndex = 0;
            worker.DoWork += (o, ea) =>
            {
                Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Visible));
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM Users WHERE admin=0", db.newConnection()))
                    {
                        DataTable dt = new DataTable();
                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                        da.Fill(dt);
                        Dispatcher.Invoke(() => dataGrid.DataContext = dt);
                        Dispatcher.Invoke(() => dataGrid.Columns[0].IsReadOnly = true);
                    }
                    db.closeConnection();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Hidden));
            };
            worker.RunWorkerAsync();
        }

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if(e.EditAction == DataGridEditAction.Commit)
            {
                //MessageBox.Show(((sender as DataGrid).SelectedCells[0].Item as DataRowView).Row[0].ToString());
                BackgroundWorker worker = new BackgroundWorker();

                worker.DoWork += (o, ea) =>
                {
                    
                    Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Visible));
                    Dispatcher.Invoke((Action)(() => (sender as DataGrid).CommitEdit(DataGridEditingUnit.Cell, true))); 
                    try
                    {
                        using (MySqlCommand cmd = new MySqlCommand($"UPDATE Users SET {Dispatcher.Invoke(()=> Enum.GetValues(typeof(helper.UserColumns)).GetValue((sender as DataGrid).SelectedCells[0].Column.DisplayIndex+1)) }=" +
                            $"{Dispatcher.Invoke(() => ((TextBox)e.EditingElement).Text)}" +
                            $" WHERE id='{((sender as DataGrid).SelectedCells[0].Item as DataRowView).Row[0]}'", db.newConnection()))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        db.closeConnection();
                        Dispatcher.Invoke((Action)(() => e.Cancel = true));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                };
                worker.RunWorkerCompleted += (o, ea) =>
                {
                    Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Hidden));
                };
                worker.RunWorkerAsync();
                
            }
            else
            {
                e.Cancel = false;
            }
        }

        private void cbSearchDataFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
