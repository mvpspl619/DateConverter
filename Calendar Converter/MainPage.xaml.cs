using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Calendar_Converter.Resources;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;

namespace Calendar_Converter
{
    public partial class MainPage : PhoneApplicationPage
    {
        int hr = 0;
        int mn = 0;
        int sc = 0;

        // Constructor
        public MainPage()
        {
            Loaded += MainPage_Loaded;
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            IsolatedStorageSettings iso = IsolatedStorageSettings.ApplicationSettings;
            if (iso.Contains("loadNumber"))
            {
                double temp = Convert.ToDouble(iso["loadNumber"]);
                iso["loadNumber"] = ++temp;
                iso.Save();
            }
            else
            {
                iso["loadNumber"] = 1;
            }
            if (!iso.Contains("hasReviewed"))
            {
                iso["hasReviewed"] = false;
            }
            iso.Save();
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void calculateButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(julianDate.Text))
                {
                    if (julianDate.Text == null || julianDate.Text == "")
                    {
                        MessageBox.Show("Please enter a valid julian date.");
                    }
                    else
                    {
                        double j = Convert.ToDouble(julianDate.Text);
                        string era;
                        double j1 = 0, j2 = 0, j3 = 0, j4 = 0, j5 = 0;
                        var intgr = Math.Floor(j);
                        var frac = j - intgr;
                        var gregjd = 2299160.5;

                        if (j >= gregjd)
                        {
                            var tmp = Math.Floor(((intgr - 1867216.0) - 0.25) / 36524.25);
                            j1 = intgr + 1 + tmp - Math.Floor(0.25 * tmp);
                        }
                        else
                            j1 = intgr;
                        var df = frac + 0.5;
                        if (df >= 1.0)
                        {
                            df -= 1.0;
                            ++j1;
                        }

                        j2 = j1 + 1524.0;
                        j3 = Math.Floor(6680.0 + ((j2 - 2439870.0) - 122.1) / 365.25);
                        j4 = Math.Floor(j3 * 365.25);
                        j5 = Math.Floor((j2 - j4) / 30.6001);

                        var d = Math.Floor(j2 - j4 - Math.Floor(j5 * 30.6001));
                        var m = Math.Floor(j5 - 1.0);
                        if (m > 12)
                            m -= 12;
                        var y = Math.Floor(j3 - 4715.0);
                        if (m > 2)
                            --y;
                        if (y <= 0)
                            --y;

                        var hr = Math.Floor(df * 24.0);
                        var mn = Math.Floor((df * 24.0 - hr) * 60.0);
                        var f = ((df * 24.0 - hr) * 60.0 - mn) * 60.0;
                        var sc = Math.Floor(f);
                        f -= sc;
                        if (f > 0.5)
                            ++sc;
                        if (sc == 60)
                        {
                            sc = 0;
                            ++mn;
                        }
                        if (mn == 60)
                        {
                            mn = 0;
                            ++hr;
                        }
                        if (hr == 24)
                        {
                            hr = 0;
                            ++d;
                        }
                        if (y < 0)
                        {
                            y = -y;
                            era = "BC";
                        }
                        else
                            era = "AD";


                        GregDateIs.Visibility = Visibility.Visible;
                        ShowGregDate.Visibility = Visibility.Visible;
                        ShowGregTime.Visibility = Visibility.Visible;
                        string month = "none";
                        string mon = Convert.ToString(m);
                        switch (mon)
                        {
                            case "1":
                                month = "Jan";
                                break;
                            case "2":
                                month = "Feb";
                                break;
                            case "3":
                                month = "Mar";
                                break;
                            case "4":
                                month = "Apr";
                                break;
                            case "5":
                                month = "May";
                                break;
                            case "6":
                                month = "Jun";
                                break;
                            case "7":
                                month = "Jul";
                                break;
                            case "8":
                                month = "Aug";
                                break;
                            case "9":
                                month = "Sep";
                                break;
                            case "10":
                                month = "Oct";
                                break;
                            case "11":
                                month = "Nov";
                                break;
                            case "12":
                                month = "Dec";
                                break;
                            default:
                                month = "none";
                                break;

                        }
                        ShowGregDate.Text = string.Format("{3} {0}/{1}/{2} ", d, month, y, era);
                        ShowGregTime.Text = string.Format("{0}:{1}:{2}", hr, mn, sc);
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid julian date.");
                }
            }
            catch
            {
                MessageBox.Show("Oops, something happened. Why don't you try again with correct data ?");
            }
        }

        private void calculateJulianButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(dateBox.Text) || !String.IsNullOrEmpty(monthBox.Text) || !String.IsNullOrEmpty(yearBox.Text))
                {
                    if (dateBox.Text != "" && monthBox.Text != "" && yearBox.Text != "")
                    {
                        double date = Convert.ToDouble(dateBox.Text);
                        double month = Convert.ToDouble(monthBox.Text);
                        double year = Convert.ToDouble(yearBox.Text);
                        if (0 < date && date <= 31)
                        {
                            if (0 < month && month <= 12)
                            {
                                int d = Convert.ToInt32(date);
                                int m = Convert.ToInt32(month);
                                int y = Convert.ToInt32(year);

                                int index = BCADPicker.SelectedIndex;
                                string era = "AD";
                                switch (index)
                                {
                                    case 0:
                                        era = "AD";
                                        break;
                                    case 1:
                                        era = "BC";
                                        break;
                                }

                                double jy = 0, ja = 0, jm = 0;
                                if (y == 0)
                                {
                                    MessageBox.Show("There is no year 0 in Julian system!");
                                    return;
                                }
                                if (y == 1582 && m == 10 && d > 4 && d < 15 && era == "AD")
                                {
                                    MessageBox.Show("The dates 5 through 14 October, 1582, do not exist in the Gregorian system!");
                                }
                                if (y < 0)
                                    ++y;
                                if (era == "BC")
                                    y = -y + 1;
                                if (m > 2)
                                {
                                    jy = y;
                                    jm = m + 1;
                                }
                                else
                                {
                                    jy = y - 1;
                                    jm = m + 13;
                                }

                                var intgr = Math.Floor(Math.Floor(365.25 * jy) + Math.Floor(30.6001 * jm) + d + 1720995);
                                var gregcal = 15 + 31 * (10 + 12 * 1582);
                                if (d + 31 * (m + 12 * y) >= gregcal)
                                {
                                    ja = Math.Floor(0.01 * jy);
                                    intgr += 2 - ja + Math.Floor(0.25 * ja);
                                }
                                var dayfrac = hr / 24.0 - 0.5;
                                if (dayfrac < 0.0)
                                {
                                    dayfrac += 1.0;
                                    --intgr;
                                }

                                var frac = dayfrac + (mn + sc / 60.0) / 60.0 / 24.0;
                                var jd0 = (intgr + frac) * 100000;
                                var jd = Math.Floor(jd0);
                                if (jd0 - jd > 0.5)
                                    ++jd;
                                jd = jd / 100000;
                                //DateTime dat = new DateTime(y, m, d);
                                //double number = dat.ToOADate() + 2415018.5;
                                //int j = d - 32075 + 1461 * Math.Floor(y + 4800 + (m - 14) / 12) / 4 + 367 * Math.Floor(m - 2 - Math.Floor((m - 14) / 12) * 12 / 12) - 3 * Math.Floor(Math.Floor(y + 4900 + (m - 14) / 12) / 100) / 4;
                                JulianDateIs.Visibility = Visibility.Visible;
                                ShowJulianDate.Visibility = Visibility.Visible;
                                ShowJulianDate.Text = String.Format("{0}", jd);
                            }
                            else
                            {
                                MessageBox.Show("Please enter a month's value between 1 and 12");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Please enter a date's value between 1 and 31");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please enter a valid gregorian date.");
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid gregorian date.");
                }
            }
            catch
            {
                MessageBox.Show("Oops, something happened. Why don't you try again with correct data ?");
            }
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.RelativeOrAbsolute));
        }

        private void timePicker_ValueChanged(object sender, DateTimeValueChangedEventArgs e)
        {
            DateTime time = Convert.ToDateTime(timePicker.Value);
            hr = time.Hour;
            mn = time.Minute;
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            IsolatedStorageSettings iso = IsolatedStorageSettings.ApplicationSettings;
            bool review = Convert.ToBoolean(iso["hasReviewed"]);
            if (review == false)
                {
                    double temp = Convert.ToDouble(iso["loadNumber"]);
                    if (temp == 1 || temp == 6 || temp == 15)
                    {
                        MessageBoxResult rslt = MessageBox.Show("If you like our app, would you like to rate it ? Reviews inspire us to develop more useful applications.", "Rate and Review", MessageBoxButton.OKCancel);
                        if (rslt == MessageBoxResult.OK)
                        {
                            MarketplaceReviewTask mktp = new MarketplaceReviewTask();
                            mktp.Show();
                            iso["hasReviewed"] = true;
                            iso.Save();
                        }
                    }
                }
            
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}