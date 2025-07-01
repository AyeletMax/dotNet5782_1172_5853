using BO;
using DO;
using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallListWindow.xaml
    /// </summary>
    public partial class CallListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        private volatile DispatcherOperation? _callListObserverOperation = null;
        public CallListWindow(int Id)
        {
            InitializeComponent();
            Volunteer = s_bl.Volunteer.GetVolunteerDetails(Id);
            queryVolunteerList();
        }
        public BO.Volunteer Volunteer
        {
            get => (BO.Volunteer)GetValue(VolunteerProperty);
            set => SetValue(VolunteerProperty, value);
        }

        public static readonly DependencyProperty VolunteerProperty =
            DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(CallListWindow));

        public BO.CallInList? SelectedCall { get; set; }


        public ObservableCollection<BO.CallInList> CallList
        {
            get { return (ObservableCollection<BO.CallInList>)GetValue(CallListProperty); }
            set { SetValue(CallListProperty, value); }
        }

        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallList", typeof(ObservableCollection<BO.CallInList>), typeof(PL.Call.CallListWindow), new PropertyMetadata(null));

        private BO.CallType callType = BO.CallType.None;
        public BO.CallType CallType
        {
            get => callType;
            set
            {
                if (callType != value)
                {
                    callType = value;
                    queryVolunteerList();
                }
            }
        }


        private Status callStatus = Status.None;
        public Status CallStatus
        {
            get => callStatus;
            set
            {
                if (callStatus != value)
                {
                    callStatus = value;
                    queryVolunteerList();
                }
            }
        }

        private void btnDeleteCall_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCall is BO.CallInList call)
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {call.CallId}?", "Delete Call", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                try
                {
                    if (result == MessageBoxResult.Yes)
                        s_bl.Call.DeleteCall(call.CallId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnUnassignCall_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCall is BO.CallInList call)
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to unassign  {call.CallId}?", "Unassiagn Call", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                    try
                    {
                        if (SelectedCall is null) return;

                        s_bl.Call.UpdateCallCancellation(Volunteer.Id, call.CallId);
                        MessageBox.Show("Call has been unassigned.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                        queryVolunteerList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to cancel treatment:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (sender is DataGrid callsDataGrid && callsDataGrid.SelectedItem is BO.CallInList selectedCall && selectedCall.Id.HasValue)
            {
                var callDetails = BlApi.Factory.Get().Call.GetCallDetails(selectedCall.Id.Value);
                var editWindow = new CallWindow(callDetails);
                editWindow.Show();
            }
        }



        private ObservableCollection<BO.CallInList> FilterCallList()
        {
            IEnumerable<BO.CallInList> result;

            if (CallType != BO.CallType.None && CallStatus != Status.None)
                result = s_bl.Call.GetCallList(CallInListFields.CallType, CallType)
                                 .Where(c => c.MyStatus == CallStatus);
            else if (CallType != BO.CallType.None)
                result = s_bl.Call.GetCallList(CallInListFields.CallType, CallType);
            else if (CallStatus != Status.None)
                result = s_bl.Call.GetCallList(CallInListFields.MyStatus, CallStatus);
            else
                result = s_bl.Call.GetCallList();

            return new ObservableCollection<BO.CallInList>(result);
        }

        private void queryVolunteerList()
        {
            CallList = FilterCallList();
        }
        private void callListObserver()
        {
            if (_callListObserverOperation == null || _callListObserverOperation.Status == DispatcherOperationStatus.Completed)
            {
                _callListObserverOperation = Dispatcher.BeginInvoke(new Action(() =>
                {
                    queryVolunteerList();
                }));
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
            => new CallWindow().Show();

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(callListObserver);

        private void Window_Closed(object sender, EventArgs e)
                   => s_bl.Call.RemoveObserver(callListObserver);

    }
}