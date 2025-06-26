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
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(VolunteerMainWindow));


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
    }
}