//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Shapes;

//namespace PL.Login;

///// <summary>
///// Interaction logic for LogingWindow.xaml
///// </summary>
//public partial class LogingWindow : Window
//{
//    public LogingWindow()
//    {
//        InitializeComponent();
//    }
//}
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BlApi;
using BO;
using VolunteerSystem;

namespace PL.Login;

public partial class LoginWindow : Window, INotifyPropertyChanged
{
    private readonly IBl _bl = BlApi.Factory.Get();

    public LoginWindow() => InitializeComponent();

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

            if (role == Role.Manager)
            {
                if (App.Current.Properties["IsManagerLoggedIn"] is true)
                {
                    ErrorMessage = "A Manager is already logged in to the system.";
                    return;
                }

                App.Current.Properties["IsManagerLoggedIn"] = true;
                new MainWindow();
            }
            else
            {
                new MainWindow();

                return;
            }

            Close();
        }
        catch (BlDoesNotExistException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorMessage = "שגיאה כללית: " + ex.Message;
        }
    }
}
