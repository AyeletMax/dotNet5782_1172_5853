using System;
using System.Windows;
using System.Windows.Threading;
using Helpers;
namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private readonly ObserverManager _observerManager = new();

        public MainWindow()
        {
            InitializeComponent();

            _observerManager.AddListObserver(ClockObserver);
            _observerManager.AddListObserver(ConfigObserver);
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
                    TimeSpan maxRange = TimeSpan.FromMinutes(minutes);
                    s_bl.Admin.SetMaxRange(maxRange);

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
    }
}
