using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BO;
using System.Windows.Controls;

/*
namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallListWindow.xaml
    /// </summary>
    public partial class CallListWindow: Window
    {
        public CallListWindow()
        {
            InitializeComponent();
        }
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public IEnumerable<BO.VolunteerInList> CallList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(CallListProperty); }
            set { SetValue(CallListProperty, value); }
        }

        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

    }
}
*/

/*using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;*/

namespace PL.Call
{
    public partial class CallListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.CallInList? SelectedCall { get; set; }

        public CallListWindow()
        {
            InitializeComponent();
            SortFields = Enum.GetValues(typeof(BO.CallInListFields)).Cast<BO.CallInListFields>().ToList();
            StatusTypes = Enum.GetValues(typeof(BO.Status)).Cast<BO.Status>().ToList();
            CallTypes = Enum.GetValues(typeof(BO.CallType)).Cast<BO.CallType>().ToList();
            RefreshCallList();
            s_bl?.Call.AddObserver(RefreshCallList);
        }

        private void RefreshCallList()
        {
            var callList = s_bl.Call.GetCallList(
              filterField: SelectedStatusFilter == null ? null : BO.CallInListFields.MyStatus,
              filterValue: SelectedStatusFilter
          );
        }

        public IEnumerable<BO.CallInList> CallList
        {
            get => (IEnumerable<BO.CallInList>)GetValue(CallListProperty);
            set => SetValue(CallListProperty, value);
        }

        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallListWindow), new PropertyMetadata(null));

        public List<BO.CallInListFields> SortFields { get; set; }
        public List<BO.Status> StatusTypes { get; set; }
        public List<BO.CallType> CallTypes { get; set; }

        private BO.CallInListFields _selectedSortField = BO.CallInListFields.CallId;
        public BO.CallInListFields SelectedSortField
        {
            get => _selectedSortField;
            set
            {
                if (_selectedSortField != value)
                {
                    _selectedSortField = value;
                    RefreshCallList();
                }
            }
        }

        private BO.Status? _selectedStatusFilter = null;
        public BO.Status? SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set
            {
                if (_selectedStatusFilter != value)
                {
                    _selectedStatusFilter = value;
                    RefreshCallList();
                }
            }
        }

        private BO.CallType _selectedCallTypeFilter = BO.CallType.None;
        public BO.CallType SelectedCallTypeFilter
        {
            get => _selectedCallTypeFilter;
            set
            {
                if (_selectedCallTypeFilter != value)
                {
                    _selectedCallTypeFilter = value;
                    RefreshCallList();
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is BO.CallInList call)
            {
                if (CanDeleteCall(call) && ConfirmDeletion(call))
                {
                    TryDeleteCall(call.CallId);
                }
            }
        }

        private bool CanDeleteCall(BO.CallInList call)
        {
            return call.MyStatus == BO.Status.Opened && call.TotalAllocations == 0;
        }

        private bool ConfirmDeletion(BO.CallInList call)
        {
            var result = MessageBox.Show(
                $"Are you sure you want to delete call ID: {call.CallId}?",
                "Confirm Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            return result == MessageBoxResult.Yes;
        }

        private void TryDeleteCall(int callId)
        {
            try
            {
                s_bl.Call.DeleteCall(callId);
            }
            catch (BO.BlDeletionException ex)
            {
                MessageBox.Show($"Cannot delete call:\n{ex.Message}",
                    "Delete Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error:\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelAssignmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is BO.CallInList call)
            {
                if (CanCancelAssignment(call) && ConfirmCancelAssignment(call))
                {
                    TryCancelAssignment(call.CallId);
                }
            }
        }

        private bool CanCancelAssignment(BO.CallInList call)
        {
            return call.MyStatus == BO.Status.InProgress || call.MyStatus == BO.Status.InProgressAtRisk;
        }

        private bool ConfirmCancelAssignment(BO.CallInList call)
        {
            var result = MessageBox.Show(
                $"Are you sure you want to cancel the current assignment for call ID: {call.CallId}?\nVolunteer: {call.LastVolunteer}",
                "Confirm Assignment Cancellation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            return result == MessageBoxResult.Yes;
        }

        private void TryCancelAssignment(int callId)
        {
            try
            {
                s_bl.Call.UpdateCallCancellation(callId, SelectedCall.Id.Value);
                MessageBox.Show("Assignment cancelled successfully. Email notification sent to volunteer.",
                    "Assignment Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to cancel assignment:\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) =>
            s_bl?.Call.AddObserver(RefreshCallList);

        private void Window_Closed(object sender, EventArgs e) =>
            s_bl?.Call.RemoveObserver(RefreshCallList);

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            new CallWindow().Show();
        }



        private void lsvCallList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (SelectedCall != null)
                    //new CallWindow(SelectedCall.CallId).Show();
                    return;
            }
            catch (BlDoesNotExistException ex)
            {
                MessageBox.Show($"Unexpected error:\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error:\n{ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}