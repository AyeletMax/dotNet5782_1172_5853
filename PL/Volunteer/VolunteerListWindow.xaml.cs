using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer
{
    public partial class VolunteerListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public VolunteerListWindow()
        {
            InitializeComponent();
            SortFields = Enum.GetValues(typeof(BO.VolunteerSortField)).Cast<BO.VolunteerSortField>().ToList();
            RefreshVolunteerList();
            s_bl?.Volunteer.AddObserver(RefreshVolunteerList);
        }

        private void RefreshVolunteerList()
        {
            VolunteerList = s_bl.Volunteer.GetVolunteersList(
                isActive: null,
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
                    RefreshVolunteerList();
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

        private void Window_Loaded(object sender, RoutedEventArgs e) =>
            s_bl?.Volunteer.AddObserver(RefreshVolunteerList);

        private void Window_Closed(object sender, EventArgs e) =>
            s_bl?.Volunteer.RemoveObserver(RefreshVolunteerList);
    }
}
