using PL.Call;
using PL.CallHistory;
using PL.Helpers;
using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PL
{
    public partial class VolunteerMainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private readonly int _volunteerId;
        public VolunteerMainWindow(int volunteerId)
        {
            InitializeComponent();
            _volunteerId = volunteerId;
            Loaded += LoadPage;
            Closed += (s, e) =>
            {
                s_bl.Volunteer.RemoveObserver(() => LoadVolunteer(_volunteerId));
            };
        }
        public bool IsEditMode
        {
            get => (bool)GetValue(IsEditModeProperty);
            set => SetValue(IsEditModeProperty, value);
        }

        public static readonly DependencyProperty IsEditModeProperty =
            DependencyProperty.Register("IsEditMode", typeof(bool), typeof(VolunteerMainWindow), new PropertyMetadata(false));

        public string Password
        {
            get => (string)GetValue(PasswordProperty);
            set => SetValue(PasswordProperty, value);
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                nameof(Password), typeof(string), typeof(VolunteerMainWindow), new PropertyMetadata(string.Empty));
        public BO.Volunteer Volunteer
        {
            get => (BO.Volunteer)GetValue(VolunteerProperty);
            set => SetValue(VolunteerProperty, value);
        }

        public static readonly DependencyProperty VolunteerProperty =
            DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(VolunteerMainWindow));


        public BO.CallInProgress? CurrentCall
        {
            get => (BO.CallInProgress?)GetValue(CurrentCallProperty);
            set => SetValue(CurrentCallProperty, value);
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.CallInProgress), typeof(VolunteerMainWindow));


        public IEnumerable<BO.DistanceType> DistanceTypes =>
            Enum.GetValues(typeof(BO.DistanceType)).Cast<BO.DistanceType>();


        private volatile DispatcherOperation? _loadVolunteerOperation = null;

        private void LoadVolunteer(int volunteerId)
        {
            if (_loadVolunteerOperation is null || _loadVolunteerOperation.Status == DispatcherOperationStatus.Completed)
                _loadVolunteerOperation = Dispatcher.BeginInvoke(() =>
                {
                    Volunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);

                    var call = Volunteer.CurrentCallInProgress;
                    if (call?.MyStatus != BO.Status.Expired && call?.MyStatus != BO.Status.Closed)
                        CurrentCall = call;
                    else
                        CurrentCall = null;
                });
        }
        private void LoadPage(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadVolunteer(_volunteerId);
                s_bl.Volunteer.AddObserver(() => LoadVolunteer(_volunteerId));

            }
            catch (Exception ex)
            {
                BlExceptionHelper.ShowBlException(ex);
            }
        }
      

        private void CallHistory_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerCallHistory(Volunteer.Id).Show();
        }

        private void ChooseCall_Click(object sender, RoutedEventArgs e)
        {
            var openCallsWindow = new OpenCallsWindow(Volunteer.Id);
            openCallsWindow.Show();

        }
        private void ToggleEditMode_Click(object sender, RoutedEventArgs e)
        {
            if (IsEditMode)
            {
                try
                {
                    Volunteer.Password = Password;
                    if (string.IsNullOrWhiteSpace(Volunteer.Phone) || !Volunteer.Phone.All(char.IsDigit))
                        throw new Exception("Invalid phone number.");
                    if (!Volunteer.Email.Contains("@"))
                        throw new Exception("Invalid email address.");
                    if (string.IsNullOrWhiteSpace(Volunteer.Address))
                        throw new Exception("Address cannot be empty.");


                    s_bl.Volunteer.UpdateVolunteer(Volunteer.Id, Volunteer);

                    MessageBox.Show("Volunteer details updated successfully.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    BlExceptionHelper.ShowBlException(ex);
                    return;
                }
            }

            IsEditMode = !IsEditMode;
        }

        private void CancelTreatment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentCall is null) return;

                s_bl.Call.UpdateCallCancellation(Volunteer.Id, Volunteer.CurrentCallInProgress!.Id);
                MessageBox.Show("Treatment has been canceled.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadVolunteer(Volunteer.Id);
            }
            catch (Exception ex)
            {
                BlExceptionHelper.ShowBlException(ex);
            }
        }

        private void FinishTreatment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentCall is null) return;

                s_bl.Call.UpdateCallCompletion(Volunteer.Id, Volunteer.CurrentCallInProgress!.Id);
                MessageBox.Show("Call marked as finished.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadVolunteer(Volunteer.Id);
            }
            catch (Exception ex)
            {
                BlExceptionHelper.ShowBlException(ex);
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            if (Volunteer.MyRole == BO.Role.Manager)
            {
                base.OnClosed(e);
                App.Current.Properties["IsManagerLoggedIn"] = false;
            }
        }
        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        private void VolunteerObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    if (Volunteer == null)
                        return;

                    int id = Volunteer.Id;
                    Volunteer = null;
                    Volunteer = s_bl.Volunteer.GetVolunteerDetails(id);
                });
        }
        private volatile DispatcherOperation? _observerOperation2 = null; //stage 7

        private void OpenCallListObserver()
        {
            if (_observerOperation2 is null || _observerOperation2.Status == DispatcherOperationStatus.Completed)
                _observerOperation2 = Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        if (Volunteer?.Id != 0)
                        {
                            var updatedVolunteer = s_bl.Volunteer.GetVolunteerDetails(Volunteer!.Id);
                            CurrentCall = updatedVolunteer?.CurrentCallInProgress;
                        }
                    }
                    catch (Exception ex)
                    {
                        BlExceptionHelper.ShowBlException(ex);
                    }

                });
        }

    }
}