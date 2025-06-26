using BO;
using DO;
using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallListWindow.xaml
    /// </summary>
    public partial class CallListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
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


        public IEnumerable<BO.CallInList> CallList
        {
            get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
            set { SetValue(CallListProperty, value); }
        }

        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(PL.Call.CallListWindow), new PropertyMetadata(null));

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
                try
                {
                    if (result == MessageBoxResult.Yes)
                        s_bl.Call.UpdateCallCancellation(Volunteer.Id,call.CallId);
                    queryVolunteerList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            //MessageBox.Show("לא עובד בגלל המורה");
        }

        private void DataGrid_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (sender is DataGrid callsDataGrid && callsDataGrid.SelectedItem is BO.CallInList selectedCall && selectedCall.Id.HasValue)
            {
                var callDetails = BlApi.Factory.Get().Call.GetCallDetails(selectedCall.Id.Value);
                var editWindow = new CallWindow(callDetails);
                editWindow.Show();
            }
            // new CallWindow(SelectedCall).Show();
        }

        //private IEnumerable<BO.CallInList> FilterCallList()
        //{
        //    return (CallStatus == Status.None) ?
        //      s_bl?.Call.GetCallList() ?? Enumerable.Empty<BO.CallInList>() :
        //      s_bl.Call.GetCallList(CallInListFields.MyStatus, CallStatus, null);
        //}
        private IEnumerable<BO.CallInList> FilterCallList()
        {
            return (CallType, CallStatus) switch
            {
                (BO.CallType.None, Status.None) => s_bl.Call.GetCallList(),

                (BO.CallType.None, var status) when status != Status.None => s_bl.Call.GetCallList(CallInListFields.MyStatus, status, null),

                (var type, Status.None) when type != BO.CallType.None => s_bl.Call.GetCallList(CallInListFields.CallType, type, null),
            };
        }
        private void queryVolunteerList()
        {
            CallList = FilterCallList();
        }
        private void callListObserver()
                => queryVolunteerList();

        private void btnAdd_Click(object sender, RoutedEventArgs e)
            =>new CallWindow().Show();

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Call.AddObserver(callListObserver);

        private void Window_Closed(object sender, EventArgs e)
                   => s_bl.Call.RemoveObserver(callListObserver);

    }
}