using BlApi;
using BO;
using PL.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PL.Call
{
    public partial class OpenCallsWindow : Window, INotifyPropertyChanged
    {
        private static readonly IBl bl = Factory.Get();

        public BO.Volunteer CurrentVolunteer { get; set; }
        public IEnumerable<CallType> CallTypes { get; set; }
        public IEnumerable<OpenCallInListFields> SortFields { get; set; }

        private volatile DispatcherOperation? _loadOpenCallsOperation = null;

        public IEnumerable<OpenCallInList> OpenCalls
        {
            get => (IEnumerable<OpenCallInList>)GetValue(OpenCallsProperty);
            set => SetValue(OpenCallsProperty, value);
        }

        public static readonly DependencyProperty OpenCallsProperty =
            DependencyProperty.Register("OpenCalls", typeof(IEnumerable<OpenCallInList>), typeof(OpenCallsWindow), new PropertyMetadata(null));

        private OpenCallInListFields _sortField = OpenCallInListFields.Id;
        public OpenCallInListFields SortField
        {
            get => _sortField;
            set
            {
                _sortField = value;
                OnPropertyChanged(nameof(SortField));
                LoadOpenCalls();
            }
        }

        private CallType _filterStatus = CallType.None;
        public CallType FilterStatus
        {
            get => _filterStatus;
            set
            {
                _filterStatus = value;
                OnPropertyChanged(nameof(FilterStatus));
                LoadOpenCalls();
            }
        }



        private OpenCallInList? _selectedCall;
        public OpenCallInList? SelectedCall
        {
            get => _selectedCall;
            set
            {
                _selectedCall = value;
                OnPropertyChanged(nameof(SelectedCall));
                OnPropertyChanged(nameof(IsCallSelected));
                OnPropertyChanged(nameof(SelectedDescription));
            }
        }

        public bool IsCallSelected => SelectedCall != null;

        public string SelectedDescription => SelectedCall?.VerbalDescription ?? string.Empty;

        private string _currentAddress;
        public string CurrentAddress
        {
            get => _currentAddress;
            set
            {
                _currentAddress = value;
                OnPropertyChanged(nameof(CurrentAddress));
            }
        }

        public OpenCallsWindow(int volunteerId)
        {
            InitializeComponent();
            CurrentVolunteer = bl.Volunteer.GetVolunteerDetails(volunteerId);

            if (CurrentVolunteer.CurrentCallInProgress != null)
            {
                MessageBox.Show("You already have an active call.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                Close();
                return;
            }

            CurrentAddress = CurrentVolunteer.Address ?? string.Empty;
            CallTypes = Enum.GetValues(typeof(CallType)).Cast<CallType>();
            SortFields = Enum.GetValues(typeof(OpenCallInListFields)).Cast<OpenCallInListFields>();

            DataContext = this;
            Loaded += LoadPage;
            Closed += (s, e) =>
            {
                bl.Call.RemoveObserver(LoadOpenCalls);
            };
        }
        private void LoadPage(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadOpenCalls();
                bl.Call.AddObserver(LoadOpenCalls);

            }
            catch (Exception ex)
            {
                BlExceptionHelper.ShowBlException(ex);
            }
        }
        private void LoadOpenCalls()
        {
            if (_loadOpenCallsOperation == null || _loadOpenCallsOperation.Status == DispatcherOperationStatus.Completed)
            {
                _loadOpenCallsOperation = Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        OpenCalls = bl.Call.GetOpenCallsForVolunteer(
                            CurrentVolunteer.Id,
                            FilterStatus == CallType.None ? null : FilterStatus,
                            SortField);
                        OpenCalls = new ObservableCollection<OpenCallInList>(OpenCalls.Where(c => CurrentVolunteer.LargestDistance is null ||
                        c.distanceFromVolunteerToCall <= CurrentVolunteer.LargestDistance));
                    }
                    catch (Exception ex)
                    {
                        BlExceptionHelper.ShowBlException(ex);
                    }
                });
            }
        }

        private void SelectCall_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is OpenCallInList call)
            {
                try
                {
                    bl.Call.SelectCallForTreatment(CurrentVolunteer.Id, call.Id);
                    LoadOpenCalls();
                    MessageBox.Show("Call successfully selected for treatment.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                catch (Exception ex)
                {
                    BlExceptionHelper.ShowBlException(ex);
                }
            }
        }

        private void UpdateAddress_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentVolunteer.Address = CurrentAddress;
                CurrentVolunteer.Password = "";
                bl.Volunteer.UpdateVolunteer(CurrentVolunteer.Id, CurrentVolunteer);
                LoadOpenCalls();
                MessageBox.Show("Address updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                BlExceptionHelper.ShowBlException(ex);
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SelectedDescription));
            OnPropertyChanged(nameof(IsCallSelected));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
