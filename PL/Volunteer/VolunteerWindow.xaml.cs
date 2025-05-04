using System;
using System.Net;
using System.Windows;


namespace PL.Volunteer
{
    public partial class VolunteerWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.Volunteer Volunteer { get; set; }
        public string ButtonText { get; set; }

        private BlApi.IVolunteer _volunteerBl;

        public VolunteerWindow(BO.Volunteer volunteer, BlApi.IVolunteer volunteerBl, int Id = 0)
        {
            InitializeComponent();


            CurrentVolunteer = (Id != 0) ? s_bl.Volunteer.Read(Id)! : new BO.Volunteer
            {
                Id = 0,
                Name = "",
                Phone = "",
                Email = "",
                Address = "",
                Active = false,
                Latitude = 0,
                Longitude = 0,
                LargestDistance = 0,
                MyDistanceType = BO.DistanceType.None,
                MyRole = BO.Role.None
            };


            this.Volunteer = volunteer;
            this.DataContext = this;
            this._volunteerBl = volunteerBl;

            ButtonText = volunteer.Id == 0 ? "Add" : "Update";
        }
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            //if (Volunteer.Id == 0)
            //{
            //    // לוגיקה להוספה של וולונטר חדש
            //    BO.Volunteer newVolunteer = new BO.Volunteer
            //    {
            //        Name = txtName.Text,
            //        Phone = txtPhone.Text,
            //        Email = txtEmail.Text,
            //        Address = txtAddress.Text,
            //        Password = txtPassword.Password,
            //        Active = chkActive.IsChecked ?? false,
            //        MyRole = (BO.Role)cmbRole.SelectedItem
            //    };

            //    try
            //    {
            //        // קריאה לפונקציה להוספת וולונטר חדש
            //        _volunteerBl.AddVolunteer(newVolunteer);
            //        MessageBox.Show("Volunteer added successfully!");
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show($"Error adding volunteer: {ex.Message}");
            //    }
            //}
            //else
            //{
            //    // לוגיקה לעדכון וולונטר קיים
            //    BO.Volunteer updatedVolunteer = new BO.Volunteer
            //    {
            //        Id = Volunteer.Id,  // ID נשאר אותו דבר
            //        Name = txtName.Text,
            //        Phone = txtPhone.Text,
            //        Email = txtEmail.Text,
            //        Address = txtAddress.Text,
            //        Password = txtPassword.Password,
            //        Active = chkActive.IsChecked ?? false,
            //        MyRole = (BO.Role)cmbRole.SelectedItem
            //    };

            //    try
            //    {
            //        // קריאה לפונקציה לעדכון וולונטר קיים
            //        _volunteerBl.UpdateVolunteer(Volunteer.Id, updatedVolunteer);
            //        MessageBox.Show("Volunteer updated successfully!");
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show($"Error updating volunteer: {ex.Message}");
            //    }
            //}
        }
        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentCourse", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));
    }
}
