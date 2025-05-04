using System;
using System.Windows;


namespace PL.Volunteer
{
    public partial class VolunteerWindow : Window
    {
        public BO.Volunteer Volunteer { get; set; }
        public string ButtonText { get; set; }

        private BlApi.IVolunteer _volunteerBl;

        public VolunteerWindow(BO.Volunteer volunteer, BlApi.IVolunteer volunteerBl)
        {
            InitializeComponent();
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
    }
}
