using System;
using System.Windows;
using System.Windows.Threading;
using Helpers;
using PL.Volunteer;
using PL.Call;
using PL.Helpers;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BO;
using System.Security.Cryptography;

namespace PL
{
    public partial class MainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile DispatcherOperation? _clockObserverOperation = null;
        private volatile DispatcherOperation? _configObserverOperation = null;
        public class StatusCount
        {
            public BO.Status Status { get; set; }
            public int Count { get; set; }
        }

        public ObservableCollection<StatusCount> CallStatusCounts { get; set; } = new ObservableCollection<StatusCount>();

        public int Interval
        {
            get { return (int)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(int), typeof(MainWindow), new PropertyMetadata(1));

        public bool IsSimulatorRunning
        {
            get { return (bool)GetValue(IsSimulatorRunningProperty); }
            set { SetValue(IsSimulatorRunningProperty, value); }
        }
        public static readonly DependencyProperty IsSimulatorRunningProperty =
            DependencyProperty.Register("IsSimulatorRunning", typeof(bool), typeof(MainWindow), new PropertyMetadata(false));
        public MainWindow(int Id)
        {
            InitializeComponent();
            Volunteer = s_bl.Volunteer.GetVolunteerDetails(Id);
            Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
            ShowStatusCommand = new RelayCommand(ShowStatusExecute);
            LoadCallStatusCounts();
        }
        private void ShowStatusExecute(object parameter)
        {

            if (parameter is BO.Status status)
            {
                new Call.CallListWindow(Volunteer.Id, status.ToString()).Show();

            }
        }
        private void LoadCallStatusCounts()
        {
            CallStatusCounts.Clear();
            var counts = s_bl.Call.GetCallQuantitiesByStatus().ToList();
            var statuses = Enum.GetValues(typeof(BO.Status)).Cast<BO.Status>().ToList();
            for (int i = 0; i < statuses.Count; i++)
            {
                CallStatusCounts.Add(new StatusCount { Status = statuses[i], Count = i < counts.Count ? counts[i] : 0 });
            }
        }

        public BO.Volunteer Volunteer
        {
            get => (BO.Volunteer)GetValue(VolunteerProperty);
            set => SetValue(VolunteerProperty, value);
        }

        public static readonly DependencyProperty VolunteerProperty =
            DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(MainWindow));
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
        public ICommand ShowStatusCommand { get; }

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

       
        private void btnUpdateRiskRange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.SetMaxRange(RiskRange);

                MessageBox.Show($"Risk range updated");
            }
            catch (Exception ex)
            {
                BlExceptionHelper.ShowBlException(ex);
            }
        }

       
        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        private void ClockObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        var currentTime = s_bl.Admin.GetClock();
                        CurrentTime = currentTime;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to update clock: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            }
        }



        private volatile DispatcherOperation? _observerOperation2 = null; //stage 7
        private void ConfigObserver()
        {
            if (_observerOperation2 is null || _observerOperation2.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation2 = Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        RiskRange = s_bl.Admin.GetMaxRange();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to update configuration variables: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            }
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                CurrentTime = s_bl.Admin.GetClock();
                RiskRange = s_bl.Admin.GetMaxRange();
                LoadCallStatusCounts();
                s_bl.Admin.AddClockObserver(ClockObserver);
                s_bl.Admin.AddConfigObserver(ConfigObserver);
                s_bl.Call.AddObserver(CallsObserver);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading admin data: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private volatile DispatcherOperation? _observerOperation3 = null; //stage 7

        private void CallsObserver()
        {
            if (_observerOperation3 is null || _observerOperation3.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation3 = Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        LoadCallStatusCounts();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to update clock: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            }
        }
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                // Stop the simulator if it's running
                if (IsSimulatorRunning)
                {
                    s_bl.Admin.StopSimulator();
                    IsSimulatorRunning = false;
                }

                // Remove observers
                s_bl.Admin.RemoveClockObserver(ClockObserver);
                s_bl.Admin.RemoveConfigObserver(ConfigObserver);
                App.Current.Properties["IsManagerLoggedIn"] = false;
                s_bl.Call.RemoveObserver(CallsObserver);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during window closure: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            
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
                BlExceptionHelper.ShowBlException(ex);
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
                BlExceptionHelper.ShowBlException(ex);
            }
        }

        private void HandleVolunteers_Click(object sender, RoutedEventArgs e) => new VolunteerListWindow().Show();
        private void HandleCalls_Click(object sender, RoutedEventArgs e) => new CallListWindow(Volunteer.Id).Show();
        private void SimulatorToggleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsSimulatorRunning)
                {
                    s_bl.Admin.StartSimulator(Interval);
                    IsSimulatorRunning = true;
                }
                else
                {
                    s_bl.Admin.StopSimulator();
                    IsSimulatorRunning = false;
                }
            }
            catch (Exception ex)
            {
                BlExceptionHelper.ShowBlException(ex);
            }
        }
    }
}

