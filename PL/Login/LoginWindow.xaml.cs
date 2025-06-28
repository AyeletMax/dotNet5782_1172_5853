using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BlApi;
using BO;

namespace PL.Login;

public partial class LoginWindow : Window, INotifyPropertyChanged
{
    private readonly IBl _bl = BlApi.Factory.Get();

    public LoginWindow() => InitializeComponent();

    private bool _isManagerOptionsVisible = false;
    public bool IsManagerOptionsVisible
    {
        get => _isManagerOptionsVisible;
        set { _isManagerOptionsVisible = value; OnPropertyChanged(); }
    }

    private bool _isLoginPanelVisible = true;
    public bool IsLoginPanelVisible
    {
        get => _isLoginPanelVisible;
        set { _isLoginPanelVisible = value; OnPropertyChanged(); }
    }

    private string? idNumber;
    public string? IdNumber
    {
        get => idNumber;
        set { idNumber = value; OnPropertyChanged(); }
    }

    private string? password;
    public string? Password
    {
        get => password;
        set { password = value; OnPropertyChanged(); }
    }

    private string? errorMessage;
    public string? ErrorMessage
    {
        get => errorMessage;
        set { errorMessage = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox box)
        {
            Password = box.Password;

            BindingOperations.GetBindingExpression(box, PasswordBox.TagProperty)
                             ?.UpdateSource();
        }
    }

    private int _currentLoggedInManagerId = -1;
    private void Login_Click(object sender, RoutedEventArgs e)
    {
        ErrorMessage = "";

        if (!int.TryParse(IdNumber, out int id))
        {
            ErrorMessage = "Please Enter a real ID";
            return;
        }
        try
        {
            Role role = _bl.Volunteer.Login(id, Password ?? "");
            IdNumber = null;

            if (role == Role.Manager)
            {
                if (App.Current.Properties["IsManagerLoggedIn"] is true)
                {
                    ErrorMessage = "A Manager is already logged in to the system.";
                    return;
                }

                _currentLoggedInManagerId =id;  
                IsLoginPanelVisible = false;
                IsManagerOptionsVisible = true;

                return;
            }
            else
            {
                new VolunteerMainWindow(id).Show();
            }

            Close();
        }
        catch (BlDoesNotExistException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorMessage = "General Eroor: " + ex.Message;
        }
    }
    private void VolunteerPanel_Click(object sender, RoutedEventArgs e)
    {
        new VolunteerMainWindow(_currentLoggedInManagerId).Show();
        IsLoginPanelVisible = true;
        IsManagerOptionsVisible = false;

    }
    private void ManagerPanel_Click(object sender, RoutedEventArgs e)
    {
        App.Current.Properties["IsManagerLoggedIn"] = true;

        new MainWindow(_currentLoggedInManagerId).Show();
        IsLoginPanelVisible = true;
        IsManagerOptionsVisible = false;

    }
}
