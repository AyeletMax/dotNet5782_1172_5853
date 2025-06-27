//using BlApi;
//using BO;
//using System;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Windows;
//using System.Windows.Controls;

//namespace PL.Call
//{
//    public partial class OpenCallsWindow : Window, INotifyPropertyChanged
//    {
//        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
//        public BO.Volunteer CurrentVolunteer { get; set; }
//        public IEnumerable<CallType> CallTypes { get; set; }
//public IEnumerable<OpenCallInListFields> SortFields { get; set; } 


//        public ObservableCollection<OpenCallInList> OpenCalls { get; set; } = new();

//        private BO.OpenCallInListFields _sortField = BO.OpenCallInListFields.Id;
//        public BO.OpenCallInListFields SortField
//        {
//            get => _sortField;
//            set
//            {
//                _sortField = value;
//                OnPropertyChanged(nameof(SortField));
//                LoadOpenCalls();
//            }
//        }

//        private BO.CallType _filterStatus = BO.CallType.None;
//        public BO.CallType FilterStatus
//        {
//            get => _filterStatus;
//            set
//            {
//                _filterStatus = value;
//                OnPropertyChanged(nameof(FilterStatus));
//                LoadOpenCalls();
//            }
//        }

//        private OpenCallInList? selectedCall;
//        public OpenCallInList? SelectedCall
//        {
//            get => selectedCall;
//            set
//            {
//                selectedCall = value;
//                SelectedDescription = selectedCall?.VerbalDescription ?? string.Empty;
//                OnPropertyChanged(nameof(SelectedCall));
//                OnPropertyChanged(nameof(SelectedDescription));
//            }
//        }

//        private string selectedDescription = string.Empty;
//        public string SelectedDescription
//        {
//            get => selectedDescription;
//            set
//            {
//                selectedDescription = value;
//                OnPropertyChanged(nameof(SelectedDescription));
//            }
//        }

//        private string _currentAddress;
//        public string CurrentAddress
//        {
//            get => _currentAddress;
//            set
//            {
//                _currentAddress = value;
//                OnPropertyChanged(nameof(CurrentAddress));
//            }
//        }

//        public OpenCallsWindow(int volunteerId)
//        {
//            InitializeComponent();
//            CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);

//            CurrentAddress = CurrentVolunteer.Address??string.Empty;
//            CallTypes= Enum.GetValues(typeof(CallType)).Cast<CallType>();
//            SortFields = Enum.GetValues(typeof(OpenCallInListFields)).Cast<OpenCallInListFields>();
//            DataContext = this;
//            LoadOpenCalls();
//        }

//        private void LoadOpenCalls()
//        {
//            try
//            {
//                var calls = s_bl.Call.GetOpenCallsForVolunteer(
//                    CurrentVolunteer.Id,
//                    FilterStatus == BO.CallType.None ? null : FilterStatus,
//                    SortField);

//                OpenCalls.Clear();
//                foreach (var call in calls)
//                    OpenCalls.Add(call);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"שגיאה בטעינת קריאות: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        private void SelectCall_Click(object sender, RoutedEventArgs e)
//        {
//            if (sender is Button btn && btn.DataContext is OpenCallInList call)
//            {
//                try
//                {
//                    s_bl.Call.SelectCallForTreatment(CurrentVolunteer.Id, call.Id);
//                    OpenCalls.Remove(call);
//                    MessageBox.Show("הקריאה נבחרה לטיפול!", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"שגיאה בבחירת קריאה: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
//                }
//            }
//        }

//        private void FilterOrSort_Changed(object sender, SelectionChangedEventArgs e)
//        {
//            LoadOpenCalls();
//        }

//        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            if (SelectedCall != null)
//                SelectedDescription = SelectedCall.VerbalDescription ?? string.Empty;
//        }

//        private void UpdateAddress_Click(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                CurrentVolunteer.Address = CurrentAddress;
//                s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer.Id, CurrentVolunteer);
//                MessageBox.Show("הכתובת עודכנה בהצלחה.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
//                LoadOpenCalls(); // לרענן מרחקים
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("שגיאה בעדכון כתובת: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        public event PropertyChangedEventHandler? PropertyChanged;
//        protected void OnPropertyChanged(string propertyName) =>
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//    }
//}
//using BlApi;
//using BO;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Windows;
//using System.Windows.Controls;

//namespace PL.Call
//{
//    public partial class OpenCallsWindow : Window, INotifyPropertyChanged
//    {
//        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
//        public BO.Volunteer CurrentVolunteer { get; set; }
//        public IEnumerable<BO.CallType> CallTypes { get; set; }
//        public IEnumerable<BO.OpenCallInListFields> SortFields { get; set; }

//        public IEnumerable<BO.OpenCallInList> OpenCalls
//        {
//            get => (IEnumerable<BO.OpenCallInList>)GetValue(OpenCallsProperty);
//            set => SetValue(OpenCallsProperty, value);
//        }

//        public static readonly DependencyProperty OpenCallsProperty =
//            DependencyProperty.Register("OpenCalls", typeof(IEnumerable<BO.OpenCallInList>), typeof(OpenCallsWindow), new PropertyMetadata(null));

//        private BO.OpenCallInListFields _sortField = BO.OpenCallInListFields.Id;
//        public BO.OpenCallInListFields SortField
//        {
//            get => _sortField;
//            set
//            {
//                _sortField = value;
//                OnPropertyChanged(nameof(SortField));
//                LoadOpenCalls();
//            }
//        }

//        private BO.CallType _filterStatus = BO.CallType.None;
//        public BO.CallType FilterStatus
//        {
//            get => _filterStatus;
//            set
//            {
//                _filterStatus = value;
//                OnPropertyChanged(nameof(FilterStatus));
//                LoadOpenCalls();
//            }
//        }

//        private BO.OpenCallInList? selectedCall;
//        public BO.OpenCallInList? SelectedCall
//        {
//            get => selectedCall;
//            set
//            {
//                selectedCall = value;
//                SelectedDescription = selectedCall?.VerbalDescription ?? string.Empty;
//                OnPropertyChanged(nameof(SelectedCall));
//                OnPropertyChanged(nameof(SelectedDescription));
//            }
//        }

//        private string selectedDescription = string.Empty;
//        public string SelectedDescription
//        {
//            get => selectedDescription;
//            set
//            {
//                selectedDescription = value;
//                OnPropertyChanged(nameof(SelectedDescription));
//            }
//        }

//        private string _currentAddress;
//        public string CurrentAddress
//        {
//            get => _currentAddress;
//            set
//            {
//                _currentAddress = value;
//                OnPropertyChanged(nameof(CurrentAddress));
//            }
//        }

//        public OpenCallsWindow(int volunteerId)
//        {
//            InitializeComponent();
//            CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);
//            CurrentAddress = CurrentVolunteer.Address ?? string.Empty;
//            CallTypes = Enum.GetValues(typeof(BO.CallType)).Cast<BO.CallType>();
//            SortFields = Enum.GetValues(typeof(BO.OpenCallInListFields)).Cast<BO.OpenCallInListFields>();
//            DataContext = this;
//            LoadOpenCalls();
//        }

//        private void LoadOpenCalls()
//        {
//            try
//            {
//                OpenCalls = s_bl.Call.GetOpenCallsForVolunteer(
//                    CurrentVolunteer.Id,
//                    FilterStatus == BO.CallType.None ? null : FilterStatus,
//                    SortField);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error loading calls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        private void SelectCall_Click(object sender, RoutedEventArgs e)
//        {
//            if (sender is Button btn && btn.DataContext is BO.OpenCallInList call)
//            {
//                try
//                {
//                    s_bl.Call.SelectCallForTreatment(CurrentVolunteer.Id, call.Id);
//                    LoadOpenCalls(); 
//                    MessageBox.Show("Call selected for treatment!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"Error selecting call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//                }
//            }
//        }

//        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            if (SelectedCall != null)
//                SelectedDescription = SelectedCall.VerbalDescription ?? string.Empty;
//        }

//        private void UpdateAddress_Click(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                CurrentVolunteer.Address = CurrentAddress;
//                s_bl.Volunteer.UpdateVolunteer(CurrentVolunteer.Id, CurrentVolunteer);
//                MessageBox.Show("Address updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
//                LoadOpenCalls(); 
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Error updating address: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        public event PropertyChangedEventHandler? PropertyChanged;
//        protected void OnPropertyChanged(string propertyName) =>
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//    }
//}
// OpenCallsWindow.xaml.cs
// OpenCallsWindow.xaml.cs
using BlApi;
using BO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PL.Call
{
    public partial class OpenCallsWindow : Window, INotifyPropertyChanged
    {
        private static readonly IBl bl = Factory.Get();

        public BO.Volunteer CurrentVolunteer { get; set; }
        public IEnumerable<CallType> CallTypes { get; set; }
        public IEnumerable<OpenCallInListFields> SortFields { get; set; }

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
            LoadOpenCalls();
        }

        private void LoadOpenCalls()
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
                MessageBox.Show($"Failed to load open calls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to select call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show($"Failed to update address: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

