using System.Windows;
using BO;
using BlApi;
using System.Collections.Generic;
using System.Linq;
using System;

namespace PL.Volunteer;

public partial class VolunteerWindow : Window
{
    private readonly IBl _volunteerBl = Factory.Get();

    public string ButtonText
    {
        get => (string)GetValue(ButtonTextProperty);
        set => SetValue(ButtonTextProperty, value);
    }

    public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register(
            nameof(ButtonText),
            typeof(string),
            typeof(VolunteerWindow),
            new PropertyMetadata("Add"));

    public IEnumerable<BO.Role> RoleCollection { get; set; }
    public IEnumerable<BO.DistanceType> DistanceTypeCollection { get; set; }

    public BO.Volunteer? CurrentVolunteer
    {
        get => (BO.Volunteer?)GetValue(CurrentVolunteerProperty);
        set => SetValue(CurrentVolunteerProperty, value);
    }

    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register(
            nameof(CurrentVolunteer),
            typeof(BO.Volunteer),
            typeof(VolunteerWindow),
            new PropertyMetadata(null));

    public string Password
    {
        get => (string)GetValue(PasswordProperty);
        set => SetValue(PasswordProperty, value);
    }

    public static readonly DependencyProperty PasswordProperty =
        DependencyProperty.Register(
            nameof(Password),
            typeof(string),
            typeof(VolunteerWindow),
            new PropertyMetadata(string.Empty));

    public VolunteerWindow(int id = 0)
    {
        InitializeComponent();

        RoleCollection = Enum.GetValues(typeof(BO.Role)).Cast<BO.Role>();
        DistanceTypeCollection = Enum.GetValues(typeof(BO.DistanceType)).Cast<BO.DistanceType>();

        if (id != 0)
        {
            var volunteer = _volunteerBl.Volunteer.GetVolunteerDetails(id);
            if (volunteer != null)
            {
                CurrentVolunteer = volunteer;
                ButtonText = "Update";
            }
            else
            {
                MessageBox.Show("Volunteer not found.");
                Close();
            }
        }
        else
        {
            CurrentVolunteer = new BO.Volunteer
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
        }

        DataContext = this;
    }

    private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
    {
        //try
        //{
        //    if (CurrentVolunteer == null)
        //        return;

        //    CurrentVolunteer.Password = Password;

        //    if (CurrentVolunteer.Id == 0)
        //    {
        //        _volunteerBl.Volunteer.AddVolunteer(CurrentVolunteer);
        //        MessageBox.Show("Volunteer added successfully.");
        //    }
        //    else
        //    {
        //        _volunteerBl.Volunteer.UpdateVolunteer(CurrentVolunteer.Id, CurrentVolunteer);
        //        MessageBox.Show("Volunteer updated successfully.");
        //    }

        //    Password = "";
        //    PasswordBox.Clear();
        //    Close();
        //}
        //catch (Exception ex)
        //{
        //    MessageBox.Show($"Error: {ex.Message}");
        //}
        try
        {
            if (ButtonText == "Add")
            {
                //bl.Volunteer.Create(CurrentVolunteer!);
                //MessageBox.Show("המתנדב נוסף בהצלחה!", "הוספה", MessageBoxButton.OK, MessageBoxImage.Information);
                _volunteerBl.Volunteer.AddVolunteer(CurrentVolunteer!);
                MessageBox.Show("Volunteer added successfully.");
            }
            else if (ButtonText == "Update")
            {
                //s_bl.Volunteer.Update(CurrentVolunteer!);
                //MessageBox.Show("המתנדב עודכן בהצלחה!", "עדכון", MessageBoxButton.OK, MessageBoxImage.Information);
                _volunteerBl.Volunteer.UpdateVolunteer(CurrentVolunteer!.Id, CurrentVolunteer);
                 MessageBox.Show("Volunteer updated successfully.");
            }

            // סגירת החלון הנוכחי
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        var passwordBox = sender as System.Windows.Controls.PasswordBox;
        if (passwordBox != null)
            Password = passwordBox.Password;
    }
}
