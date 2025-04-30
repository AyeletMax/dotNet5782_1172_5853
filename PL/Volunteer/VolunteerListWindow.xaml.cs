using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerListWindow.xaml
    /// </summary>
    public partial class VolunteerListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public VolunteerListWindow()
        {
            InitializeComponent();
            SortFields = Enum.GetValues(typeof(BO.VolunteerSortField)).Cast<BO.VolunteerSortField>().ToList();
            ObserveVolunteerList(); // במקום לקרוא ישירות ל־GetVolunteersList
            s_bl.Volunteer.AddObserver(ObserveVolunteerList); // זו אותה מתודה שמשמשת לרענון
        }

        private void ObserveVolunteerList()
        {
            VolunteerList = s_bl.Volunteer.GetVolunteersList(
                sortBy: SelectedSortField == BO.VolunteerSortField.None ? null : SelectedSortField);
        }

        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get => (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty);
            set => SetValue(VolunteerListProperty, value);
        }

        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

        public List<BO.VolunteerSortField> SortFields { get; set; }

        private BO.VolunteerSortField _selectedSortField = BO.VolunteerSortField.None;
        public BO.VolunteerSortField SelectedSortField
        {
            get => _selectedSortField;
            set
            {
                if (_selectedSortField != value)
                {
                    _selectedSortField = value;
                    ObserveVolunteerList(); // במקום לכתוב שוב את GetVolunteersList
                }
            }
        }

        private void SortByComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (ComboBoxItem)((ComboBox)sender).SelectedItem;
            var content = selectedItem.Content.ToString();
            SelectedSortField = content switch
            {
                "Name" => BO.VolunteerSortField.Name,
                "Total Responses Handled" => BO.VolunteerSortField.TotalResponsesHandled,
                "Total Responses Cancelled" => BO.VolunteerSortField.TotalResponsesCancelled,
                "Total Expired Responses" => BO.VolunteerSortField.TotalExpiredResponses,
                "Sum of Calls" => BO.VolunteerSortField.SumOfCalls,
                "Sum of Cancellation" => BO.VolunteerSortField.SumOfCancellation,
                "Sum of Expired Calls" => BO.VolunteerSortField.SumOfExpiredCalls,
                _ => BO.VolunteerSortField.None,
            };
        }
        private void queryVolunteerList()=> VolunteerList = (SelectedSortField == BO.VolunteerSortField.None) ?
         s_bl?.Volunteer.GetVolunteersList() :
         s_bl?.Volunteer.GetVolunteersList(SelectedSortField);


        private void volunteerListObserver()
            => queryVolunteerList();

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Volunteer.AddObserver(volunteerListObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(volunteerListObserver);
    }
}
