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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Data;
using System.ComponentModel;
using System.Windows.Interop;
using System.Threading;
using System.Windows.Media.Animation;
using EmploymentAgency.Classes;
using EmploymentAgency.Helpers;
using MySql.Data.MySqlClient;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.IO;
using Image = System.Drawing.Image;
using Microsoft.Win32;
using System.Reflection;
using System.Diagnostics.SymbolStore;
using System.Diagnostics;
using System.Windows.Ink;
using PdfiumViewer;
using System.Drawing.Printing;

#pragma warning disable CA2100

namespace EmploymentAgency
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        Event @event;
        db db;
        User user;
        List<Vacancy> vacancies = new List<Vacancy>();
        int currentVacanciesPage = 0;

        public MainWindow(User user)
        {
            
            db = new db();
            this.user = user;
            InitializeComponent();
            cbFilters.SelectedIndex = 0;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new Auth().Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            closeMenuBar();
            BueydAdsCanvas.Visibility = Visibility.Hidden;
            AdCanvas.Visibility = Visibility.Hidden;
            VacanciesCanvas.Visibility = Visibility.Hidden;
            SummaryCanvas.Visibility = Visibility.Hidden;
            MapCanvas.Visibility = Visibility.Hidden;
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

        private void closeMenuBar()
        {
            ThicknessAnimation tAnim = new ThicknessAnimation();
            tAnim.From = new Thickness(0, 0, 0, 0);
            tAnim.To = new Thickness(-270, 0, 0, 0);
            tAnim.Duration = TimeSpan.FromSeconds(0.25);
            MenuCanvas.BeginAnimation(Canvas.MarginProperty, tAnim);

            DoubleAnimation dAnim = new DoubleAnimation();
            dAnim.From = 462;
            dAnim.To = 435;
            dAnim.Duration = TimeSpan.FromSeconds(0.25);
            timeCanvas.BeginAnimation(Canvas.TopProperty, dAnim);
        }

        private void openMenuBar()
        {
            ThicknessAnimation tAnim = new ThicknessAnimation();
            tAnim.From = new Thickness(-270, 0, 0, 0);
            tAnim.To = new Thickness(0, 0, 0, 0);
            tAnim.Duration = TimeSpan.FromSeconds(0.25);
            MenuCanvas.BeginAnimation(Canvas.MarginProperty, tAnim);

            DoubleAnimation dAnim = new DoubleAnimation();
            dAnim.From = 435;
            dAnim.To = 462;
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

        private void bMain_Click(object sender, RoutedEventArgs e)
        {
            closeMenuBar();
            AdCanvas.Visibility = Visibility.Visible;
            EditProfileCanvas.Visibility = Visibility.Hidden;
            BueydAdsCanvas.Visibility = Visibility.Hidden;
            VacanciesCanvas.Visibility = Visibility.Hidden;
            SummaryCanvas.Visibility = Visibility.Hidden;
            MapCanvas.Visibility = Visibility.Hidden;
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
            if(suc)
            {
                MessageBox.Show("Данные обновлены!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void bBuyedAds_Click(object sender, RoutedEventArgs e)
        {
            closeMenuBar();
            sp.Children.Clear();
            AdCanvas.Visibility = Visibility.Hidden;
            EditProfileCanvas.Visibility = Visibility.Hidden;
            VacanciesCanvas.Visibility = Visibility.Hidden;
            SummaryCanvas.Visibility = Visibility.Hidden;
            MapCanvas.Visibility = Visibility.Hidden;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Visible));
                MySqlConnection conn = db.newConnection();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Ads", conn);
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Dispatcher.Invoke((Action)(() => sp.Children.Add(newAdCanvas(reader.GetValue(2).ToString(), reader.GetValue(3).ToString(), reader.GetValue(4).ToString(), helper.byteArrayToImage((byte[])reader[1])))));
                        }
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
            
            BueydAdsCanvas.Visibility = Visibility.Visible;
        }

        public static Canvas newAdCanvas(string Title, string Body, string Footer, ImageSource image)
        {
            Canvas canvas = new Canvas();
            Grid grid = new Grid();
            canvas.MinHeight = 50;
            canvas.MinWidth = 315;
            grid.MinHeight = 50;
            grid.MinWidth = 315;
            TextBlock tbTitle = new TextBlock();
            TextBlock tbBody = new TextBlock();
            TextBlock tbFooter = new TextBlock();
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            img.MinWidth = 50;
            img.MinHeight = 50;
            img.Width = 50;
            img.Height = 50;
            img.Source = image;
            img.Stretch = Stretch.UniformToFill;
            img.HorizontalAlignment = HorizontalAlignment.Left;
            tbTitle.HorizontalAlignment = HorizontalAlignment.Left;
            tbTitle.Margin = new Thickness(60, 0, 0, 0);
            tbBody.HorizontalAlignment = HorizontalAlignment.Left;
            tbBody.Margin = new Thickness(60, 15, 0, 0);
            tbFooter.HorizontalAlignment = HorizontalAlignment.Left;
            tbFooter.Margin = new Thickness(60, 30, 0, 0);
            canvas.MouseDown += Canvas_MouseDown;
            tbTitle.Text = Title;
            tbBody.Text = Body;
            tbFooter.Text = Footer;
            grid.Children.Add(img);
            grid.Children.Add(tbTitle);
            grid.Children.Add(tbBody);
            grid.Children.Add(tbFooter);
            Grid.SetColumn(img, 0);
            Grid.SetColumn(tbTitle, 0);
            Grid.SetColumn(tbBody, 0);
            Grid.SetColumn(tbFooter, 0);
            Grid.SetRow(img, 0);
            Grid.SetRow(tbTitle, 0);
            Grid.SetRow(tbBody, 0);
            Grid.SetRow(tbFooter, 0);
            canvas.Children.Add(grid);
            return canvas;
        }

        private static void Canvas_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Canvas canvas = (Canvas)sender;
            Grid grid = (Grid)canvas.Children[0];
            System.Windows.Controls.TextBlock label = grid.Children[1] as System.Windows.Controls.TextBlock;
            System.Windows.Controls.TextBlock label1 = grid.Children[2] as System.Windows.Controls.TextBlock;
            System.Windows.Controls.TextBlock label2 = grid.Children[3] as System.Windows.Controls.TextBlock;
            System.Windows.Controls.Image image = (System.Windows.Controls.Image)grid.Children[0];
            
            (((((canvas.Parent as StackPanel).Parent as ScrollViewer).Parent as Grid).Parent as Canvas).Children[1] as System.Windows.Controls.Image).Source = image.Source;
            (((((canvas.Parent as StackPanel).Parent as ScrollViewer).Parent as Grid).Parent as Canvas).Children[2] as System.Windows.Controls.Label).Content = label.Text;
            (((((canvas.Parent as StackPanel).Parent as ScrollViewer).Parent as Grid).Parent as Canvas).Children[3] as System.Windows.Controls.Label).Content = label1.Text;
            (((((canvas.Parent as StackPanel).Parent as ScrollViewer).Parent as Grid).Parent as Canvas).Children[4] as System.Windows.Controls.Label).Content = label2.Text;
        }

        private void bVacancies_Click(object sender, RoutedEventArgs e)
        {
            closeMenuBar();
            cbSearchFilter.SelectedIndex = 0;
            AdCanvas.Visibility = Visibility.Hidden;
            EditProfileCanvas.Visibility = Visibility.Hidden;
            BueydAdsCanvas.Visibility = Visibility.Hidden;
            SummaryCanvas.Visibility = Visibility.Hidden;
            MapCanvas.Visibility = Visibility.Hidden;

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
                            lVacancyID.Text = $"Вакансия #{vacancies[0].getid().ToString()}";
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            closeMenuBar();
            AdCanvas.Visibility = Visibility.Hidden;
            EditProfileCanvas.Visibility = Visibility.Hidden;
            BueydAdsCanvas.Visibility = Visibility.Hidden;
            VacanciesCanvas.Visibility = Visibility.Hidden;
            SummaryCanvas.Visibility = Visibility.Hidden;
            MapCanvas.Visibility = Visibility.Visible;
            gMapControl1.Bearing = 0;
            //Настройки для компонента GMap.
            gMapControl1.Bearing = 0;

            //CanDragMap - Если параметр установлен в True,
            //пользователь может перетаскивать карту 
            ///с помощью правой кнопки мыши. 
            gMapControl1.CanDragMap = true;

            //Указываем, что перетаскивание карты осуществляется 
            //с использованием левой клавишей мыши.
            //По умолчанию - правая.
            gMapControl1.DragButton = MouseButton.Left;

            gMapControl1.ScaleMode = GMap.NET.WindowsPresentation.ScaleModes.Dynamic;
            gMapControl1.MouseWheelZoomEnabled = true;
            //MarkersEnabled - Если параметр установлен в True,
            //любые маркеры, заданные вручную будет показаны.
            //Если нет, они не появятся.

            //Указываем значение максимального приближения.
            gMapControl1.MaxZoom = 20;

            //Указываем значение минимального приближения.
            gMapControl1.MinZoom = 2;

            //Устанавливаем центр приближения/удаления
            //курсор мыши.
            gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;

            //gMapControl1.SetPositionByKeywords("Russia, Krasnoyarsk Krai, Krasnoyarsk");
            gMapControl1.Position = new GMap.NET.PointLatLng(56.015514, 92.974354);

            GMap.NET.WindowsPresentation.GMapMarker marker = new GMap.NET.WindowsPresentation.GMapMarker(new GMap.NET.PointLatLng(56.015514, 92.974354));
            marker.Tag = "marker";
            Ellipse shape = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = System.Windows.Media.Brushes.Green,
                Stroke = System.Windows.Media.Brushes.Green,
                StrokeThickness = 1.5,
                ToolTip = "Office 1"

            };
            shape.MouseDown += Shape_MouseDown;
            marker.Shape = shape;
            
            gMapControl1.Markers.Add(marker);
            gMapControl1.ShowTileGridLines = false;
            gMapControl1.Zoom = 12.17;
            gMapControl1.MapProvider = GMap.NET.MapProviders.GMapProviders.GoogleMap;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerOnly;
        }

        private void Shape_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //TODO TOOLTIP
        }

        private void backButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(currentVacanciesPage > 0)
            {
                currentVacanciesPage--;
                title.Text = vacancies[currentVacanciesPage].gettitle();
                body.Text = vacancies[currentVacanciesPage].getbody() + $"\nОклад: {vacancies[currentVacanciesPage].getsalary()}\nРежим работы: {vacancies[currentVacanciesPage].getmode()}\nОбразование: {vacancies[currentVacanciesPage].geteducation()}";
                address.Text = vacancies[currentVacanciesPage].getaddress();
                vacancyImage.Source = vacancies[currentVacanciesPage].getimage();
                lVacancyID.Text = $"Вакансия #{vacancies[currentVacanciesPage].getid().ToString()}";
                vacanciesPages.Text = $"страница {currentVacanciesPage+1} из {vacancies.Count}";
            }
        }

        private void nextButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (currentVacanciesPage < vacancies.Count-1)
            {
                currentVacanciesPage++;
                title.Text = vacancies[currentVacanciesPage].gettitle();
                body.Text = vacancies[currentVacanciesPage].getbody() + $"\nОклад: {vacancies[currentVacanciesPage].getsalary()}\nРежим работы: {vacancies[currentVacanciesPage].getmode()}\nОбразование: {vacancies[currentVacanciesPage].geteducation()}";
                address.Text = vacancies[currentVacanciesPage].getaddress();
                vacancyImage.Source = vacancies[currentVacanciesPage].getimage();
                lVacancyID.Text = $"Вакансия #{vacancies[currentVacanciesPage].getid().ToString()}";
                vacanciesPages.Text = $"страница {currentVacanciesPage + 1} из {vacancies.Count}";
            }
        }

        private void bOpenFilters_Click(object sender, RoutedEventArgs e)
        {
            ThicknessAnimation tAnim = new ThicknessAnimation();
            tAnim.From = new Thickness(285, 0, 0, 0);
            tAnim.To = new Thickness(0, 0, 0, 0);
            tAnim.Duration = TimeSpan.FromSeconds(0.25);
            filtersCanvas.BeginAnimation(Canvas.MarginProperty, tAnim);
        }

        private void closeFilters_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ThicknessAnimation tAnim = new ThicknessAnimation();
            tAnim.From = new Thickness(0, 0, 0, 0);
            tAnim.To = new Thickness(285, 0, 0, 0);
            tAnim.Duration = TimeSpan.FromSeconds(0.25);
            filtersCanvas.BeginAnimation(Canvas.MarginProperty, tAnim);
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
                                lVacancyID.Text = $"Вакансия #{vacancies[0].getid().ToString()}";
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
                    MySqlCommand cmd = new MySqlCommand("SELECT Vacancies.id, Vacancies.image, Vacancies.name, Vacancies.description, Vacancies.address, Vacancies.salary, JobModes.`mode`, Educations.education FROM Vacancies, JobModes, Educations WHERE JobModes.id = Vacancies.`mode` AND Educations.id = Vacancies.education", db.newConnection());
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
                                lVacancyID.Text = $"Вакансия #{vacancies[0].getid().ToString()}";
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
            switch(cbFilters.SelectedIndex)
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
                        db.closeConnection();
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

        private void bClearFilter_Click(object sender, RoutedEventArgs e)
        {
            lbFiltersType.SelectedIndex = -1;
        }

        private void bNewSummary_Click(object sender, RoutedEventArgs e)
        {
            closeMenuBar();
            AdCanvas.Visibility = Visibility.Hidden;
            EditProfileCanvas.Visibility = Visibility.Hidden;
            BueydAdsCanvas.Visibility = Visibility.Hidden;
            VacanciesCanvas.Visibility = Visibility.Hidden;
            MapCanvas.Visibility = Visibility.Hidden;
            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += (o, ea) =>
            {
                Dispatcher.Invoke(() => {
                    cbEdu.Items.Clear();
                    cbJob.Items.Clear();
                    BusyIndicator.Visibility = Visibility.Visible;
                });
                MySqlCommand cmd = new MySqlCommand("SELECT education FROM Educations", db.newConnection());
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Dispatcher.Invoke((Action)(() => cbEdu.Items.Add(reader.GetString(0))));
                    }
                    Dispatcher.Invoke(() => cbEdu.SelectedIndex = 0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                cmd = new MySqlCommand("SELECT mode FROM JobModes", db.newConnection());
                try
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Dispatcher.Invoke((Action)(() => cbJob.Items.Add(reader.GetString(0))));
                    }
                    Dispatcher.Invoke(() => cbJob.SelectedIndex = 0); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                db.closeConnection();
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Hidden));
            };
            worker.RunWorkerAsync();
            SummaryCanvas.Visibility = Visibility.Visible;
        }

        private void bOpenPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "PNG|*.png|JPG|*.jpg|JPEG|*.jpeg";
            if (ofd.ShowDialog().Value)
                tPhoto.Text = ofd.FileName;
        }

        private void bCreateSummary_Click(object sender, RoutedEventArgs e)
        {
            if (tAddress.Text.Length > 0 && tPhone.Text.Length > 0 && tPhoto.Text.Length > 0 && tPosition.Text.Length > 0 && tSpecial.Text.Length > 0)
            {
                if (File.Exists(tPhoto.Text))
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "PDF files(*.pdf)|*.pdf";
                    if (sfd.ShowDialog().Value)
                    {
                        helper.summaryPdf($"{user.getlastname()} {user.getname()} {user.getpatronymic()}", tPhone.Text, tAddress.Text, tSpecial.Text, tPosition.Text, cbEdu.Text, user.getdateOfBirth().ToString("MM.dd.yyyy"), cbJob.Text, tPhoto.Text, sfd.FileName);
                        if (cbPrint.IsChecked.Value)
                        {
                            PdfDocument doc = PdfDocument.Load(sfd.FileName);
                            var printDocument = doc.CreatePrintDocument();
                            printDocument.PrintController = new StandardPrintController();
                            printDocument.Print();
                            
                        }
                    }
                }
                else
                    MessageBox.Show("Неверно указано фото!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
                MessageBox.Show("Неверно заполнены данные!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void bCreateBlank_Click(object sender, RoutedEventArgs e)
        {
            lVacancyHeaderID.Content = $"Вакансия #{vacancies[currentVacanciesPage].getid()}";
            createVacancyBlank.Visibility = Visibility.Visible;

        }

        private void bCloseVacancyBlank_MouseDown(object sender, MouseButtonEventArgs e)
        {
            createVacancyBlank.Visibility = Visibility.Hidden;
        }

        private void bCreateBlankAppr_Click(object sender, RoutedEventArgs e)
        {
            if(tBlankPhone.Text.Length > 5)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PDF files(*.pdf)|*.pdf";
                if (sfd.ShowDialog().Value)
                {
                    helper.blankPdf($"{user.getlastname()} {user.getname()} {user.getpatronymic()}", tBlankPhone.Text, vacancies[currentVacanciesPage].getaddress(), vacancies[currentVacanciesPage].gettitle(), vacancies[currentVacanciesPage].gettitle(), vacancies[currentVacanciesPage].geteducation(), vacancies[currentVacanciesPage].getsalary(), vacancies[currentVacanciesPage].getmode(), vacancies[currentVacanciesPage].getid().ToString(), sfd.FileName);
                    BackgroundWorker worker = new BackgroundWorker();
                    worker.DoWork += (o, ea) =>
                    {
                        Dispatcher.Invoke((Action)(() => BusyIndicator.Visibility = Visibility.Visible));
                        MySqlCommand cmd = new MySqlCommand("INSERT INTO Blanks(fio, phone, vacancy, date) VALUES(@fio, @phone, @vacancy, @date)", db.newConnection());
                        cmd.Parameters.AddWithValue("@fio", Dispatcher.Invoke(() => $"{user.getlastname()} {user.getname()} {user.getpatronymic()}"));
                        cmd.Parameters.AddWithValue("@phone", Dispatcher.Invoke(() => tBlankPhone.Text));
                        cmd.Parameters.AddWithValue("@vacancy", Dispatcher.Invoke(() => vacancies[currentVacanciesPage].getid()));
                        cmd.Parameters.AddWithValue("date", DateTime.Now.ToString("yyyy-MM-dd"));
                        try
                        {
                            cmd.ExecuteNonQuery();
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
                    if (cbPrintBlank.IsChecked.Value)
                    {
                        PdfDocument doc = PdfDocument.Load(sfd.FileName);
                        var printDocument = doc.CreatePrintDocument();
                        printDocument.PrintController = new StandardPrintController();
                        printDocument.Print();

                    }
                }
                
            }
            else
                MessageBox.Show("Неверно заполнены данные!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
