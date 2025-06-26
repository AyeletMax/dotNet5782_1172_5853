using BlApi;
using BO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PL.Call
{
    public partial class OpenCallsWindow : Window
    {
        private readonly IBl bl = Factory.Get();
        public BO.Volunteer CurrentVolunteer { get; }

        public ObservableCollection<OpenCallInList> OpenCalls { get; set; } = new();

        private OpenCallInListFields _sortField = OpenCallInListFields.Id;
        public IEnumerable<CallType> VolunteerFields => Enum.GetValues(typeof(CallType)).Cast<CallType>();
        public IEnumerable<OpenCallInListFields> OpenCallFields => Enum.GetValues(typeof(OpenCallInListFields)).Cast<OpenCallInListFields>();

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

        private OpenCallInList? selectedCall;
        public OpenCallInList? SelectedCall
        {
            get => selectedCall;
            set
            {
                selectedCall = value;
                SelectedDescription = selectedCall?.VerbalDescription ?? string.Empty;
                OnPropertyChanged(nameof(SelectedCall));
                OnPropertyChanged(nameof(SelectedDescription));
            }
        }

        private string selectedDescription = string.Empty;
        public string SelectedDescription
        {
            get => selectedDescription;
            set
            {
                selectedDescription = value;
                OnPropertyChanged(nameof(SelectedDescription));
            }
        }

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

            CurrentVolunteer = bl.Volunteer.GetVolunteerDetails(volunteerId) ?? throw new ArgumentException("Volunteer not found", nameof(volunteerId));
            CurrentAddress = CurrentVolunteer.Address ?? string.Empty;

            DataContext = this;
            LoadOpenCalls();
        }

        private void LoadOpenCalls()
        {
            try
            {
                var calls = bl.Call.GetOpenCallsForVolunteer(
                    CurrentVolunteer.Id,
                    FilterStatus == CallType.None ? null : FilterStatus,
                    SortField);

                OpenCalls.Clear();
                foreach (var call in calls)
                    OpenCalls.Add(call);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading calls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectCall_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is OpenCallInList call)
            {
                try
                {
                    bl.Call.SelectCallForTreatment(CurrentVolunteer.Id, call.Id);
                    OpenCalls.Remove(call);
                    MessageBox.Show("The call has been selected for treatment!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error selecting call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void FilterOrSort_Changed(object sender, SelectionChangedEventArgs e)
        {
            LoadOpenCalls();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedCall != null)
                SelectedDescription = SelectedCall.VerbalDescription ?? string.Empty;
        }

        private void UpdateAddress_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentVolunteer.Address = CurrentAddress;
                bl.Volunteer.UpdateVolunteer(CurrentVolunteer.Id, CurrentVolunteer);
                MessageBox.Show("Address updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadOpenCalls(); // refresh distances
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating address: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
