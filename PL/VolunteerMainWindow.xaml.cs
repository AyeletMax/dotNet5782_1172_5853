using PL.CallHistory;
using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PL
{
    public partial class VolunteerMainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public VolunteerMainWindow(int volunteerId)
        {
            InitializeComponent();
            LoadVolunteer(volunteerId);
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
                nameof(Password),typeof(string),typeof(VolunteerMainWindow),new PropertyMetadata(string.Empty));
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


        private void LoadVolunteer(int volunteerId)
        {
           Volunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);

           var call = Volunteer.CurrentCallInProgress;
            //var call = s_bl.Call.GetCurrentCallOfVolunteer(volunteerId);
            if (call?.MyStatus != BO.Status.Expired && call?.MyStatus != BO.Status.Closed)
                CurrentCall = call;
            else
                CurrentCall = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadVolunteer(Volunteer.Id);
        }

        private void CallHistory_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerCallHistory(Volunteer.Id).Show();
        }

        private void ChooseCall_Click(object sender, RoutedEventArgs e)
        {
            new CallForTreatment(Volunteer.Id).Show(); 
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
                    MessageBox.Show($"Update failed:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            IsEditMode = !IsEditMode;
        }
        //private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        //{
        //    var passwordBox = sender as System.Windows.Controls.PasswordBox;
        //    if (passwordBox != null)
        //        Password = passwordBox.Password;
        //}
        private void CancelTreatment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentCall is null) return;

                s_bl.Call.UpdateCallCancellation(Volunteer.Id,Volunteer.CurrentCallInProgress!.Id);
                MessageBox.Show("Treatment has been canceled.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadVolunteer(Volunteer.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to cancel treatment:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FinishTreatment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentCall is null) return;

                s_bl.Call.UpdateCallCompletion(Volunteer.Id,Volunteer.CurrentCallInProgress!.Id);
                MessageBox.Show("Call marked as finished.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadVolunteer(Volunteer.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to finish treatment:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}