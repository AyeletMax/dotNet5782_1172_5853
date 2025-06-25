using System;
using System.Windows;
using System.Windows.Threading;
using Helpers;
using PL.Volunteer;
using PL.Call;

namespace PL
{
    public partial class MainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }

        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

        public TimeSpan RiskRange
        {
            get { return (TimeSpan)GetValue(RiskRangeProperty); }
            set { SetValue(RiskRangeProperty, value); }
        }
        public static readonly DependencyProperty RiskRangeProperty =
            DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(MainWindow), new PropertyMetadata(TimeSpan.Zero));

        private void AdvanceMinute_Click(object sender, RoutedEventArgs e) =>
            s_bl.Admin.AdvanceClock(BO.TimeUnit.MINUTE);

        private void AdvanceYear_Click(object sender, RoutedEventArgs e) =>
            s_bl.Admin.AdvanceClock(BO.TimeUnit.YEAR);

        private void AdvanceDay_Click(object sender, RoutedEventArgs e) =>
            s_bl.Admin.AdvanceClock(BO.TimeUnit.DAY);

        private void AdvanceMonth_Click(object sender, RoutedEventArgs e) =>
            s_bl.Admin.AdvanceClock(BO.TimeUnit.MONTH);

        private void AdvanceHour_Click(object sender, RoutedEventArgs e) =>
            s_bl.Admin.AdvanceClock(BO.TimeUnit.HOUR);

        private void UpdateConfig_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"RiskRange was updated to: {RiskRange.TotalMinutes} minutes.");
        }

        private void UpdateRiskRange_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(RiskRangeTextBox.Text, out int minutes))
            {
                try
                {
                    RiskRange = TimeSpan.FromMinutes(minutes);
                    s_bl.Admin.SetMaxRange(RiskRange);

                    MessageBox.Show($"Risk range updated to: {minutes} minutes.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while updating the risk range: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid numeric value for the risk range.");
            }
        }

        private void ClockObserver() => CurrentTime = s_bl.Admin.GetClock();

        private void ConfigObserver() => RiskRange = s_bl.Admin.GetMaxRange();

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetClock();
            RiskRange = s_bl.Admin.GetMaxRange();
            s_bl.Admin.AddClockObserver(ClockObserver);
            s_bl.Admin.AddConfigObserver(ConfigObserver);
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(ClockObserver);
            s_bl.Admin.RemoveConfigObserver(ConfigObserver);
        }
        private void InitializeDB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.InitializeDB();
                MessageBox.Show("The database was initialized successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while initializing the database: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetDB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.ResetDB();
                MessageBox.Show("The database was reset successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while resetting the database: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
     
        private void HandleVolunteers_Click(object sender, RoutedEventArgs e) => new VolunteerListWindow().Show();
        private void HandleCalls_Click(object sender, RoutedEventArgs e) => new CallListWindow().Show();
        //protected override void OnClosed(EventArgs e)
        //{
        //    base.OnClosed(e);
        //    App.Current.Properties["IsManagerLoggedIn"] = false;
        //}
    }
}
