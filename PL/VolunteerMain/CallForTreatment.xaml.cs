//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Windows;
//using System.Windows.Controls;
//using System.ComponentModel;
//using BO;
//using BlApi;
//using System.Threading.Tasks;

//namespace VolunteerSystem
//{
//    public partial class CallForTreatment() : Window
//    {
//        private readonly ICall _callBl;
//        private readonly IVolunteer _volunteerBl;
//        private readonly int _volunteerId;
//        private IEnumerable<OpenCallInList> _allOpenCalls;
//        private OpenCallInList _selectedCall;

//        public CallForTreatment(int volunteerId) : this()
//        {
//            InitializeComponent();
//            _volunteerId = volunteerId;
//            _callBl = BlApi.Factory.Get().Call;
//            _volunteerBl = BlApi.Factory.Get().Volunteer;

//            InitializeFilters();
//            LoadOpenCalls();
//        }

//        /// <summary>
//        /// אתחול ComboBox של הסינון והמיון
//        /// </summary>
//        private void InitializeFilters()
//        {
//            // אתחול ComboBox של סוג קריאה
//            var callTypes = new List<object>
//            {
//                new { Text = "None", Value = (CallType?)null }
//            };

//            foreach (CallType callType in Enum.GetValues<CallType>())
//            {
//                if (callType != CallType.None)
//                {
//                    callTypes.Add(new { Text = GetCallTypeEnglishName(callType), Value = (CallType?)callType });
//                }
//            }

//            CallTypeFilterComboBox.ItemsSource = callTypes;
//            CallTypeFilterComboBox.DisplayMemberPath = "Text";
//            CallTypeFilterComboBox.SelectedValuePath = "Value";
//            CallTypeFilterComboBox.SelectedIndex = 0;

//            // אתחול ComboBox של מיון
//            var sortFields = new List<object>
//            {
//                new { Text = "Call Number", Value = OpenCallInListFields.Id },
//                new { Text = "Open Time", Value = OpenCallInListFields.Start_time },
//                new { Text = "Max Finish Time", Value = OpenCallInListFields.Max_finish_time },
//                new { Text = "Distance", Value = OpenCallInListFields.CallDistance },
//                new { Text = "Call Type", Value = OpenCallInListFields.CallType }
//            };

//            SortFieldComboBox.ItemsSource = sortFields;
//            SortFieldComboBox.DisplayMemberPath = "Text";
//            SortFieldComboBox.SelectedValuePath = "Value";
//            SortFieldComboBox.SelectedIndex = 0;
//        }

//        /// <summary>
//        /// טעינת רשימת הקריאות הפתוחות
//        /// </summary>
//        private void LoadOpenCalls()
//        {
//            try
//            {
//                var callTypeFilter = (CallType?)CallTypeFilterComboBox.SelectedValue;
//                var sortField = (OpenCallInListFields?)SortFieldComboBox.SelectedValue;

//                _allOpenCalls = _callBl.GetOpenCallsForVolunteer(_volunteerId, callTypeFilter, sortField);
//                OpenCallsDataGrid.ItemsSource = _allOpenCalls;

//                // עדכון הודעה אם אין קריאות
//                if (!_allOpenCalls.Any())
//                {
//                    MessageBox.Show("לא נמצאו קריאות פתוחות באזור שלך כרגע.", "אין קריאות",
//                        MessageBoxButton.OK, MessageBoxImage.Information);
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"שגיאה בטעינת הקריאות: {ex.Message}", "שגיאה",
//                    MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        /// <summary>
//        /// מחזיר שם בעברית לסוג הקריאה
//        /// </summary>
//        private string GetCallTypeEnglishName(CallType callType)
//        {
//            return callType switch
//            {
//                CallType.MusicPerformance => "Music Performance",
//                CallType.MusicTherapy => "Music Therapy",
//                CallType.SingingAndEmotionalSupport => "Singing and Emotional Support",
//                CallType.GroupActivities => "Group Activities",
//                CallType.PersonalizedMusicCare => "Personalized Music Care",
//                _ => callType.ToString()
//            };
//        }


//        /// <summary>
//        /// אירוע שינוי סינון לפי סוג קריאה
//        /// </summary>
//        private void CallTypeFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            if (IsLoaded)
//            {
//                LoadOpenCalls();
//            }
//        }

//        /// <summary>
//        /// אירוע שינוי מיון
//        /// </summary>
//        private void SortField_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            if (IsLoaded)
//            {
//                LoadOpenCalls();
//            }
//        }

//        /// <summary>
//        /// אירוע לחיצה על כפתור רענון
//        /// </summary>
//        private void RefreshButton_Click(object sender, RoutedEventArgs e)
//        {
//            LoadOpenCalls();
//        }

//        /// <summary>
//        /// אירוע לחיצה על כפתור עדכון כתובת
//        /// </summary>
//        private void UpdateAddressButton_Click(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                var addressWindow = new UpdateAddressWindow();
//                if (addressWindow.ShowDialog() == true)
//                {
//                    // עדכון כתובת המתנדב
//                    var volunteer = _volunteerBl.GetVolunteerDetails(_volunteerId);
//                    volunteer.Address = addressWindow.NewAddress;
//                    _volunteerBl.UpdateVolunteer(_volunteerId, volunteer);

//                    // רענון הרשימה
//                    LoadOpenCalls();
//                    MessageBox.Show("הכתובת עודכנה בהצלחה!", "עדכון הצליח",
//                        MessageBoxButton.OK, MessageBoxImage.Information);
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"שגיאה בעדכון הכתובת: {ex.Message}", "שגיאה",
//                    MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        /// <summary>
//        /// אירוע בחירת קריאה מהרשימה
//        /// </summary>
//        private void OpenCallsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            if (OpenCallsDataGrid.SelectedItem is OpenCallInList selectedCall)
//            {
//                _selectedCall = selectedCall;
//                DisplayCallDescription(selectedCall);
//                //LoadMapForCall(selectedCall);
//            }
//        }

//        /// <summary>
//        /// הצגת תיאור הקריאה
//        /// </summary>
//        private void DisplayCallDescription(OpenCallInList call)
//        {
//            var description = $"Call Number: {call.Id}\n" +
//                              $"Call Type: {GetCallTypeEnglishName(call.MyCallType)}\n" +
//                              $"Address: {call.Address}\n" +
//                              $"Open Time: {call.OpenTime:dd/MM/yyyy HH:mm}\n" +
//                              $"Max Finish Time: {call.MaxFinishTime?.ToString("dd/MM/yyyy HH:mm") ?? "Not Defined"}\n" +
//                              $"Distance from you: {call.distanceFromVolunteerToCall:F2} km\n\n";

//            if (!string.IsNullOrEmpty(call.VerbalDescription))
//            {
//                description += $"Description:\n{call.VerbalDescription}";
//            }
//            else
//            {
//                description += "No additional description.";
//            }

//            CallDescriptionTextBlock.Text = description;
//        }

//        /// <summary>
//        /// טעינת מפה עבור הקריאה הנבחרת
//        /// </summary>
//        //private void LoadMapForCall(OpenCallInList call)
//        //{
//        //    try
//        //    {
//        //        // כאן ניתן להוסיף קוד להצגת מפה
//        //        // לדוגמה, עם Google Maps או Bing Maps

//        //        // דוגמה פשוטה עם HTML + JavaScript
//        //        string mapHtml = GenerateMapHtml(call);
//        //        MapWebBrowser.NavigateToString(mapHtml);
//        //        MapWebBrowser.Visibility = Visibility.Visible;
//        //        MapPlaceholder.Visibility = Visibility.Collapsed;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        MapWebBrowser.Visibility = Visibility.Collapsed;
//        //        MapPlaceholder.Visibility = Visibility.Visible;
//        //        MapPlaceholder.Text = $"שגיאה בטעינת המפה: {ex.Message}";
//        //    }
//        //}

//        /// <summary>
//        /// יצירת HTML עבור המפה
//        /// </summary>
//        //private string GenerateMapHtml(OpenCallInList call)
//        //{
//        //    // דוגמה פשוטה - בפועל תצטרך להשתמש ב-API של Google Maps או Bing Maps
//        //    return $@"
//        //        <!DOCTYPE html>
//        //        <html>
//        //        <head>
//        //            <meta charset='utf-8'>
//        //            <title>מפה</title>
//        //            <style>
//        //                body {{ font-family: Arial, sans-serif; text-align: center; padding: 20px; }}
//        //                .info {{ background: #f0f0f0; padding: 10px; border-radius: 5px; margin: 10px; }}
//        //            </style>
//        //        </head>
//        //        <body>
//        //            <h3>מיקום הקריאה</h3>
//        //            <div class='info'>
//        //                <strong>כתובת הקריאה:</strong><br>
//        //                {call.Address}
//        //            </div>
//        //            <div class='info'>
//        //                <strong>מרחק ממיקומך:</strong><br>
//        //                {call.distanceFromVolunteerToCall:F2} ק״מ
//        //            </div>
//        //            <p style='color: #666; font-size: 12px;'>
//        //                להצגת מפה מלאה, יש להוסיף שילוב עם Google Maps API או Bing Maps
//        //            </p>
//        //        </body>
//        //        </html>";
//        //}

//        /// <summary>
//        /// אירוע לחיצה על כפתור בחירת קריאה לטיפול
//        /// </summary>
//        private void SelectCallButton_Click(object sender, RoutedEventArgs e)
//        {
//            if (sender is Button button && button.Tag is int callId)
//            {
//                try
//                {
//                    var result = MessageBox.Show(
//                        $"האם אתה בטוח שברצונך לבחור את הקריאה מספר {callId} לטיפול?",
//                        "אישור בחירת קריאה",
//                        MessageBoxButton.YesNo,
//                        MessageBoxImage.Question);

//                    if (result == MessageBoxResult.Yes)
//                    {
//                        _callBl.SelectCallForTreatment(_volunteerId, callId);

//                        MessageBox.Show(
//                            "הקריאה נבחרה בהצלחה לטיפול!\nהמערכת תעביר אותך למסך פרטי המתנדב.",
//                            "בחירה הצליחה",
//                            MessageBoxButton.OK,
//                            MessageBoxImage.Information);

//                        // סגירת החלון וחזרה למסך הראשי של המתנדב
//                        this.DialogResult = true;
//                        this.Close();
//                    }
//                }
//                catch (BlInvalidOperationException ex)
//                {
//                    MessageBox.Show($"לא ניתן לבחור את הקריאה: {ex.Message}", "שגיאת פעולה",
//                        MessageBoxButton.OK, MessageBoxImage.Warning);
//                    // רענון הרשימה במקרה של שינוי סטטוס
//                    LoadOpenCalls();
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"שגיאה בבחירת הקריאה: {ex.Message}", "שגיאה",
//                        MessageBoxButton.OK, MessageBoxImage.Error);
//                }
//            }
//        }

//        /// <summary>
//        /// אירוע סגירת החלון
//        /// </summary>
//        protected override void OnClosing(CancelEventArgs e)
//        {
//            // יכול להוסיף כאן לוגיקה נוספת לפני סגירה
//            base.OnClosing(e);
//        }
//    }

//    /// <summary>
//    /// חלון עדכון כתובת פשוט
//    /// </summary>
//    public partial class UpdateAddressWindow : Window
//    {
//        public string NewAddress { get; private set; }

//        public UpdateAddressWindow()
//        {
//            InitializeComponent();
//        }

//        private void InitializeComponent()
//        {
//            Title = "עדכון כתובת";
//            Width = 400;
//            Height = 200;
//            WindowStartupLocation = WindowStartupLocation.CenterScreen;

//            ResizeMode = ResizeMode.NoResize;

//            var grid = new Grid();
//            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
//            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
//            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

//            var label = new Label
//            {
//                Content = "הכנס כתובת חדשה:",
//                Margin = new Thickness(10),
//                FontSize = 14
//            };
//            Grid.SetRow(label, 0);
//            grid.Children.Add(label);

//            var addressTextBox = new TextBox
//            {
//                Name = "AddressTextBox",
//                Margin = new Thickness(10),
//                Padding = new Thickness(5),
//                FontSize = 12
//            };
//            Grid.SetRow(addressTextBox, 1);
//            grid.Children.Add(addressTextBox);

//            var buttonPanel = new StackPanel
//            {
//                Orientation = Orientation.Horizontal,
//                HorizontalAlignment = HorizontalAlignment.Center,
//                Margin = new Thickness(10)
//            };

//            var okButton = new Button
//            {
//                Content = "אישור",
//                Width = 80,
//                Height = 30,
//                Margin = new Thickness(5),
//                IsDefault = true
//            };
//            okButton.Click += (s, e) =>
//            {
//                if (string.IsNullOrWhiteSpace(addressTextBox.Text))
//                {
//                    MessageBox.Show("יש להכניס כתובת תקינה", "שגיאת קלט",
//                        MessageBoxButton.OK, MessageBoxImage.Warning);
//                    return;
//                }
//                NewAddress = addressTextBox.Text.Trim();
//                DialogResult = true;
//                Close();
//            };

//            var cancelButton = new Button
//            {
//                Content = "ביטול",
//                Width = 80,
//                Height = 30,
//                Margin = new Thickness(5),
//                IsCancel = true
//            };
//            cancelButton.Click += (s, e) =>
//            {
//                DialogResult = false;
//                Close();
//            };

//            buttonPanel.Children.Add(okButton);
//            buttonPanel.Children.Add(cancelButton);
//            Grid.SetRow(buttonPanel, 2);
//            grid.Children.Add(buttonPanel);

//            Content = grid;
//        }
//    }
//}
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Linq;
//using System.Windows.Data; // For ICollectionView
//using System.Windows.Input; // For ICommand

//using BO; // Assume you have BO namespace
//using BlApi; // Assume you have BlApi namespace
////using Microsoft.Maps.MapControl.WPF; // For Map related properties

//namespace VolunteerSystem.ViewModels // Changed namespace to ViewModels for clarity
//{
//    public class CallForTreatmentViewModel : INotifyPropertyChanged
//    {
//        private readonly ICall _callBl;
//        private readonly IVolunteer _volunteerBl;
//        private readonly int _volunteerId;

//        // Collection to hold all open calls fetched from BL
//        private ObservableCollection<OpenCallInList> _allOpenCalls;
//        // ICollectionView for filtering and sorting
//        public ICollectionView OpenCallsView { get; private set; }

//        private OpenCallInList? _selectedCall;
//        public OpenCallInList? SelectedCall
//        {
//            get => _selectedCall;
//            set
//            {
//                if (_selectedCall != value)
//                {
//                    _selectedCall = value;
//                    OnPropertyChanged(nameof(SelectedCall));
//                    OnPropertyChanged(nameof(CallDetailsText)); // Update details text
//                    OnPropertyChanged(nameof(VolunteerLocation)); // Update map location
//                    OnPropertyChanged(nameof(CallLocation)); // Update map location
//                }
//            }
//        }

//        private IEnumerable<object> _callTypes;
//        public IEnumerable<object> CallTypes
//        {
//            get => _callTypes;
//            set
//            {
//                _callTypes = value;
//                OnPropertyChanged(nameof(CallTypes));
//            }
//        }

//        private CallType? _selectedCallTypeFilter;
//        public CallType? SelectedCallTypeFilter
//        {
//            get => _selectedCallTypeFilter;
//            set
//            {
//                if (_selectedCallTypeFilter != value)
//                {
//                    _selectedCallTypeFilter = value;
//                    OnPropertyChanged(nameof(SelectedCallTypeFilter));
//                    ApplyFiltersAndSort(); // Re-apply filter when changed
//                }
//            }
//        }

//        private IEnumerable<object> _sortOptions;
//        public IEnumerable<object> SortOptions
//        {
//            get => _sortOptions;
//            set
//            {
//                _sortOptions = value;
//                OnPropertyChanged(nameof(SortOptions));
//            }
//        }

//        private OpenCallInListFields? _selectedSortOption;
//        public OpenCallInListFields? SelectedSortOption
//        {
//            get => _selectedSortOption;
//            set
//            {
//                if (_selectedSortOption != value)
//                {
//                    _selectedSortOption = value;
//                    OnPropertyChanged(nameof(SelectedSortOption));
//                    ApplyFiltersAndSort(); // Re-apply sort when changed
//                }
//            }
//        }

//        private string _statusMessage = "";
//        public string StatusMessage
//        {
//            get => _statusMessage;
//            set
//            {
//                _statusMessage = value;
//                OnPropertyChanged(nameof(StatusMessage));
//            }
//        }

//        // Properties for Map integration
//        private Location _volunteerLocation;
//        public Location VolunteerLocation
//        {
//            get => _volunteerLocation;
//            set
//            {
//                _volunteerLocation = value;
//                OnPropertyChanged(nameof(VolunteerLocation));
//            }
//        }

//        private Location? _callLocation;
//        public Location? CallLocation
//        {
//            get => _callLocation;
//            set
//            {
//                _callLocation = value;
//                OnPropertyChanged(nameof(CallLocation));
//            }
//        }

//        // This property combines all details for the TextBlock
//        public string CallDetailsText
//        {
//            get
//            {
//                if (SelectedCall == null)
//                {
//                    return "Select a call from the list to see the details.";
//                }

//                var description = $"Call Number: {SelectedCall.Id}\n" +
//                                  $"Call Type: {GetCallTypeEnglishName(SelectedCall.MyCallType)}\n" +
//                                  $"Address: {SelectedCall.Address}\n" +
//                                  $"Open Time: {SelectedCall.OpenTime:dd/MM/yyyy HH:mm}\n" +
//                                  $"Max Finish Time: {SelectedCall.MaxFinishTime?.ToString("dd/MM/yyyy HH:mm") ?? "Not Defined"}\n" +
//                                  $"Distance from you: {SelectedCall.DistanceFromVolunteerToCall:F2} km\n\n";

//                if (!string.IsNullOrEmpty(SelectedCall.VerbalDescription))
//                {
//                    description += $"Description:\n{SelectedCall.VerbalDescription}";
//                }
//                else
//                {
//                    description += "No additional description.";
//                }
//                return description;
//            }
//        }

//        // Commands for buttons
//        public ICommand RefreshCommand { get; private set; }
//        public ICommand UpdateVolunteerLocationCommand { get; private set; }
//        public ICommand SelectCallCommand { get; private set; } // Command for selecting a call for treatment

//        public CallForTreatmentViewModel(int volunteerId)
//        {
//            _volunteerId = volunteerId;
//            _callBl = BlApi.Factory.Get().Call;
//            _volunteerBl = BlApi.Factory.Get().Volunteer;

//            _allOpenCalls = new ObservableCollection<OpenCallInList>();
//            OpenCallsView = CollectionViewSource.GetDefaultView(_allOpenCalls);

//            InitializeFilters();
//            LoadOpenCalls();
//            LoadVolunteerLocation(); // Load volunteer's current location for map

//            // Initialize Commands
//            RefreshCommand = new RelayCommand(LoadOpenCalls);
//            UpdateVolunteerLocationCommand = new RelayCommand(UpdateVolunteerLocation);
//            SelectCallCommand = new RelayCommand<int>(SelectCallForTreatment);
//        }

//        private void InitializeFilters()
//        {
//            var callTypes = new List<object>
//            {
//                new { Text = "All Call Types", Value = (CallType?)null }
//            };

//            foreach (CallType callType in Enum.GetValues<CallType>())
//            {
//                if (callType != CallType.None)
//                {
//                    callTypes.Add(new { Text = GetCallTypeEnglishName(callType), Value = (CallType?)callType });
//                }
//            }
//            CallTypes = callTypes;

//            var sortFields = new List<object>
//            {
//                new { Text = "Call Number", Value = OpenCallInListFields.Id },
//                new { Text = "Open Time", Value = OpenCallInListFields.Start_time },
//                new { Text = "Max Finish Time", Value = OpenCallInListFields.Max_finish_time },
//                new { Text = "Distance", Value = OpenCallInListFields.CallDistance },
//                new { Text = "Call Type", Value = OpenCallInListFields.CallType }
//            };
//            SortOptions = sortFields;

//            // Set initial selected values for ComboBoxes
//            SelectedCallTypeFilter = null; // "All Call Types"
//            SelectedSortOption = OpenCallInListFields.Id; // Default sort by Call Number
//        }

//        private void LoadOpenCalls()
//        {
//            try
//            {
//                _allOpenCalls.Clear();
//                var callsFromBl = _callBl.GetOpenCallsForVolunteer(_volunteerId, SelectedCallTypeFilter, SelectedSortOption);
//                foreach (var call in callsFromBl)
//                {
//                    _allOpenCalls.Add(call);
//                }

//                ApplyFiltersAndSort(); // Apply filters and sort after loading new data

//                if (!_allOpenCalls.Any())
//                {
//                    StatusMessage = "No open calls found in your area at the moment.";
//                }
//                else
//                {
//                    StatusMessage = "";
//                }
//            }
//            catch (Exception ex)
//            {
//                StatusMessage = $"Error loading calls: {ex.Message}";
//                MessageBox.Show(StatusMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        // Apply filters and sorting to the ICollectionView
//        private void ApplyFiltersAndSort()
//        {
//            OpenCallsView.Filter = null; // Clear existing filter
//            if (SelectedCallTypeFilter.HasValue)
//            {
//                OpenCallsView.Filter = item =>
//                {
//                    if (item is OpenCallInList call)
//                    {
//                        return call.MyCallType == SelectedCallTypeFilter.Value;
//                    }
//                    return false;
//                };
//            }

//            OpenCallsView.SortDescriptions.Clear(); // Clear existing sort
//            if (SelectedSortOption.HasValue)
//            {
//                var sortDirection = ListSortDirection.Ascending; // Default ascending

//                switch (SelectedSortOption.Value)
//                {
//                    case OpenCallInListFields.Id:
//                        OpenCallsView.SortDescriptions.Add(new SortDescription("Id", sortDirection));
//                        break;
//                    case OpenCallInListFields.Start_time:
//                        OpenCallsView.SortDescriptions.Add(new SortDescription("OpenTime", sortDirection));
//                        break;
//                    case OpenCallInListFields.Max_finish_time:
//                        OpenCallsView.SortDescriptions.Add(new SortDescription("MaxFinishTime", sortDirection));
//                        break;
//                    case OpenCallInListFields.CallDistance:
//                        OpenCallsView.SortDescriptions.Add(new SortDescription("DistanceFromVolunteerToCall", sortDirection));
//                        break;
//                    case OpenCallInListFields.CallType:
//                        OpenCallsView.SortDescriptions.Add(new SortDescription("MyCallType", sortDirection));
//                        break;
//                    default:
//                        break;
//                }
//            }
//            else
//            {
//                // Default sort if no option is selected
//                OpenCallsView.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Ascending));
//            }

//            OpenCallsView.Refresh(); // Apply changes
//        }

//        private string GetCallTypeEnglishName(CallType callType)
//        {
//            return callType switch
//            {
//                CallType.MusicPerformance => "Music Performance",
//                CallType.MusicTherapy => "Music Therapy",
//                CallType.SingingAndEmotionalSupport => "Singing and Emotional Support",
//                CallType.GroupActivities => "Group Activities",
//                CallType.PersonalizedMusicCare => "Personalized Music Care",
//                _ => callType.ToString()
//            };
//        }

//        private void UpdateVolunteerLocation()
//        {
//            try
//            {
//                var addressWindow = new UpdateAddressWindow();
//                if (addressWindow.ShowDialog() == true)
//                {
//                    var volunteer = _volunteerBl.GetVolunteerDetails(_volunteerId);
//                    volunteer.Address = addressWindow.NewAddress;
//                    _volunteerBl.UpdateVolunteer(_volunteerId, volunteer);

//                    LoadOpenCalls(); // Reload calls as volunteer location changed
//                    LoadVolunteerLocation(); // Update map with new volunteer location
//                    StatusMessage = "Address updated successfully!";
//                }
//            }
//            catch (Exception ex)
//            {
//                StatusMessage = $"Error updating address: {ex.Message}";
//                MessageBox.Show(StatusMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        private void LoadVolunteerLocation()
//        {
//            try
//            {
//                var volunteer = _volunteerBl.GetVolunteerDetails(_volunteerId);
//                if (volunteer?.Address?.Location != null)
//                {
//                    VolunteerLocation = new Location(volunteer.Address.Location.Latitude, volunteer.Address.Location.Longitude);
//                }
//            }
//            catch (Exception ex)
//            {
//                StatusMessage = $"Error loading volunteer location: {ex.Message}";
//            }
//        }

//        private void SelectCallForTreatment(int callId)
//        {
//            try
//            {
//                var result = MessageBox.Show(
//                    $"Are you sure you want to select call number {callId} for treatment?",
//                    "Confirm Call Selection",
//                    MessageBoxButton.YesNo,
//                    MessageBoxImage.Question);

//                if (result == MessageBoxResult.Yes)
//                {
//                    _callBl.SelectCallForTreatment(_volunteerId, callId);

//                    MessageBox.Show(
//                        "The call has been successfully selected for treatment!\nThe system will return you to the volunteer details screen.",
//                        "Selection Successful",
//                        MessageBoxButton.OK,
//                        MessageBoxImage.Information);

//                    // You might want to signal the main window to close this one
//                    // or handle navigation. For simplicity, we'll indicate success.
//                    // This is usually handled by an event or a service.
//                    // For now, we'll just reload the calls to reflect the change.
//                    LoadOpenCalls();
//                    SelectedCall = null; // Clear selected call after selection
//                }
//            }
//            catch (BO.BlInvalidOperationException ex) // Assuming this is your custom exception
//            {
//                StatusMessage = $"Could not select call: {ex.Message}";
//                MessageBox.Show(StatusMessage, "Operation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
//                LoadOpenCalls(); // Refresh list in case status changed
//            }
//            catch (Exception ex)
//            {
//                StatusMessage = $"Error selecting call: {ex.Message}";
//                MessageBox.Show(StatusMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        // Implementation of INotifyPropertyChanged
//        public event PropertyChangedEventHandler? PropertyChanged;
//        protected void OnPropertyChanged(string propertyName)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }
//    }

//    // Helper class for ICommand implementation (RelayCommand)
//    public class RelayCommand : ICommand
//    {
//        private readonly Action _execute;
//        private readonly Func<bool>? _canExecute;

//        public event EventHandler? CanExecuteChanged
//        {
//            add { CommandManager.RequerySuggested += value; }
//            remove { CommandManager.RequerySuggested -= value; }
//        }

//        public RelayCommand(Action execute, Func<bool>? canExecute = null)
//        {
//            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
//            _canExecute = canExecute;
//        }

//        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
//        public void Execute(object? parameter) => _execute();
//    }

//    public class RelayCommand<T> : ICommand
//    {
//        private readonly Action<T> _execute;
//        private readonly Func<T, bool>? _canExecute;

//        public event EventHandler? CanExecuteChanged
//        {
//            add { CommandManager.RequerySuggested += value; }
//            remove { CommandManager.RequerySuggested -= value; }
//        }

//        public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
//        {
//            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
//            _canExecute = canExecute;
//        }

//        public bool CanExecute(object? parameter) => _canExecute?.Invoke((T)parameter!) ?? true;
//        public void Execute(object? parameter) => _execute((T)parameter!);
//    }
//}
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Windows;
//using System.Windows.Controls;
//using System.ComponentModel;
//using System.Collections.ObjectModel;
//using System.Windows.Data;

//using BO;
//using BlApi;

//namespace VolunteerSystem
//{
//    public partial class CallForTreatment : Window
//    {
//        private readonly IBl _bl = Factory.Get();
//        private readonly int _volunteerId;

//        public IEnumerable<OpenCallInList> OpenCallList
//        {
//            get { return (IEnumerable<OpenCallInList>)GetValue(OpenCallListProperty); }
//            set { SetValue(OpenCallListProperty, value); }
//        }

//        public static readonly DependencyProperty OpenCallListProperty =
//            DependencyProperty.Register("OpenCallList", typeof(IEnumerable<OpenCallInList>), typeof(CallForTreatment), new PropertyMetadata(null));

//        public CallType TypeOfCall { get; set; } = CallType.None;
//        public OpenCallInList? SelectedCall { get; set; }

//        public CallForTreatment(int volunteerId)
//        {
//            InitializeComponent();
//            _volunteerId = volunteerId;
//            DataContext = this;

//            InitializeFilters();
//            Loaded += Window_Loaded;
//            Closed += Window_Closed;
//        }

//        /// <summary>
//        /// אתחול ComboBox של הסינון והמיון
//        /// </summary>
//        private void InitializeFilters()
//        {
//            // אתחול ComboBox של סוג קריאה
//            var callTypes = new List<object>
//            {
//                new { Text = "None", Value = (CallType?)null }
//            };

//            foreach (CallType callType in Enum.GetValues<CallType>())
//            {
//                if (callType != CallType.None)
//                {
//                    callTypes.Add(new { Text = GetCallTypeEnglishName(callType), Value = (CallType?)callType });
//                }
//            }

//            CallTypeFilterComboBox.ItemsSource = callTypes;
//            CallTypeFilterComboBox.DisplayMemberPath = "Text";
//            CallTypeFilterComboBox.SelectedValuePath = "Value";
//            CallTypeFilterComboBox.SelectedIndex = 0;

//            // אתחול ComboBox של מיון
//            var sortFields = new List<object>
//            {
//                new { Text = "Call Number", Value = OpenCallInListFields.Id },
//                new { Text = "Open Time", Value = OpenCallInListFields.Start_time },
//                new { Text = "Max Finish Time", Value = OpenCallInListFields.Max_finish_time },
//                new { Text = "Distance", Value = OpenCallInListFields.CallDistance },
//                new { Text = "Call Type", Value = OpenCallInListFields.CallType }
//            };

//            SortFieldComboBox.ItemsSource = sortFields;
//            SortFieldComboBox.DisplayMemberPath = "Text";
//            SortFieldComboBox.SelectedValuePath = "Value";
//            SortFieldComboBox.SelectedIndex = 0;
//        }

//        /// <summary>
//        /// מחזיר שם בעברית לסוג הקריאה
//        /// </summary>
//        private string GetCallTypeEnglishName(CallType callType)
//        {
//            return callType switch
//            {
//                CallType.MusicPerformance => "Music Performance",
//                CallType.MusicTherapy => "Music Therapy",
//                CallType.SingingAndEmotionalSupport => "Singing and Emotional Support",
//                CallType.GroupActivities => "Group Activities",
//                CallType.PersonalizedMusicCare => "Personalized Music Care",
//                _ => callType.ToString()
//            };
//        }

//        /// <summary>
//        /// טעינת רשימת הקריאות הפתוחות
//        /// </summary>
//        private void QueryOpenCallList()
//        {
//            try
//            {
//                var callTypeFilter = (CallType?)CallTypeFilterComboBox.SelectedValue;
//                var sortField = (OpenCallInListFields?)SortFieldComboBox.SelectedValue;

//                OpenCallList = _bl.Call.GetOpenCallsForVolunteer(_volunteerId, callTypeFilter, sortField);

//                // עדכון הודעה אם אין קריאות
//                if (!OpenCallList.Any())
//                {
//                    MessageBox.Show("לא נמצאו קריאות פתוחות באזור שלך כרגע.", "אין קריאות",
//                        MessageBoxButton.OK, MessageBoxImage.Information);
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"שגיאה בטעינת הקריאות: {ex.Message}", "שגיאה",
//                    MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        /// <summary>
//        /// הצגת תיאור הקריאה
//        /// </summary>
//        private void DisplayCallDescription(OpenCallInList call)
//        {
//            var description = $"Call Number: {call.Id}\n" +
//                              $"Call Type: {GetCallTypeEnglishName(call.MyCallType)}\n" +
//                              $"Address: {call.Address}\n" +
//                              $"Open Time: {call.OpenTime:dd/MM/yyyy HH:mm}\n" +
//                              $"Max Finish Time: {call.MaxFinishTime?.ToString("dd/MM/yyyy HH:mm") ?? "Not Defined"}\n" +
//                              $"Distance from you: {call.distanceFromVolunteerToCall:F2} km\n\n";

//            if (!string.IsNullOrEmpty(call.VerbalDescription))
//            {
//                description += $"Description:\n{call.VerbalDescription}";
//            }
//            else
//            {
//                description += "No additional description.";
//            }

//            CallDescriptionTextBlock.Text = description;
//        }

//        // Event Handlers
//        private void CallTypeFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            if (IsLoaded)
//            {
//                QueryOpenCallList();
//            }
//        }

//        private void SortField_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            if (IsLoaded)
//            {
//                QueryOpenCallList();
//            }
//        }

//        private void RefreshButton_Click(object sender, RoutedEventArgs e)
//        {
//            QueryOpenCallList();
//        }

//        private void UpdateAddressButton_Click(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                var addressWindow = new UpdateAddressWindow();
//                if (addressWindow.ShowDialog() == true)
//                {
//                    // עדכון כתובת המתנדב
//                    var volunteer = _bl.Volunteer.GetVolunteerDetails(_volunteerId);
//                    volunteer.Address = addressWindow.NewAddress;
//                    _bl.Volunteer.UpdateVolunteer(_volunteerId, volunteer);

//                    // רענון הרשימה
//                    QueryOpenCallList();
//                    MessageBox.Show("הכתובת עודכנה בהצלחה!", "עדכון הצליח",
//                        MessageBoxButton.OK, MessageBoxImage.Information);
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"שגיאה בעדכון הכתובת: {ex.Message}", "שגיאה",
//                    MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        private void OpenCallsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            if (OpenCallsDataGrid.SelectedItem is OpenCallInList selectedCall)
//            {
//                SelectedCall = selectedCall;
//                DisplayCallDescription(selectedCall);
//            }
//        }

//        private void SelectCallButton_Click(object sender, RoutedEventArgs e)
//        {
//            if (sender is Button button && button.Tag is int callId)
//            {
//                try
//                {
//                    var result = MessageBox.Show(
//                        $"האם אתה בטוח שברצונך לבחור את הקריאה מספר {callId} לטיפול?",
//                        "אישור בחירת קריאה",
//                        MessageBoxButton.YesNo,
//                        MessageBoxImage.Question);

//                    if (result == MessageBoxResult.Yes)
//                    {
//                        _bl.Call.SelectCallForTreatment(_volunteerId, callId);

//                        MessageBox.Show(
//                            "הקריאה נבחרה בהצלחה לטיפול!\nהמערכת תעביר אותך למסך פרטי המתנדב.",
//                            "בחירה הצליחה",
//                            MessageBoxButton.OK,
//                            MessageBoxImage.Information);

//                        this.DialogResult = true;
//                        this.Close();
//                    }
//                }
//                catch (BlInvalidOperationException ex)
//                {
//                    MessageBox.Show($"לא ניתן לבחור את הקריאה: {ex.Message}", "שגיאת פעולה",
//                        MessageBoxButton.OK, MessageBoxImage.Warning);
//                    QueryOpenCallList();
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"שגיאה בבחירת הקריאה: {ex.Message}", "שגיאה",
//                        MessageBoxButton.OK, MessageBoxImage.Error);
//                }
//            }
//        }

//        // Observer Methods
//        private void OpenCallListObserver() => QueryOpenCallList();

//        private void Window_Loaded(object sender, RoutedEventArgs e)
//        {
//            _bl.Call.AddObserver(OpenCallListObserver);
//            QueryOpenCallList();
//        }

//        private void Window_Closed(object sender, EventArgs e)
//        {
//            _bl.Call.RemoveObserver(OpenCallListObserver);
//        }
//    }

//    /// <summary>
//    /// חלון עדכון כתובת פשוט
//    /// </summary>
//    public partial class UpdateAddressWindow : Window
//    {
//        public string NewAddress { get; private set; }

//        public UpdateAddressWindow()
//        {
//            InitializeComponent();
//        }

//        private void InitializeComponent()
//        {
//            Title = "עדכון כתובת";
//            Width = 400;
//            Height = 200;
//            WindowStartupLocation = WindowStartupLocation.CenterScreen;
//            ResizeMode = ResizeMode.NoResize;

//            var grid = new Grid();
//            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
//            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
//            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

//            var label = new Label
//            {
//                Content = "הכנס כתובת חדשה:",
//                Margin = new Thickness(10),
//                FontSize = 14
//            };
//            Grid.SetRow(label, 0);
//            grid.Children.Add(label);

//            var addressTextBox = new TextBox
//            {
//                Name = "AddressTextBox",
//                Margin = new Thickness(10),
//                Padding = new Thickness(5),
//                FontSize = 12
//            };
//            Grid.SetRow(addressTextBox, 1);
//            grid.Children.Add(addressTextBox);

//            var buttonPanel = new StackPanel
//            {
//                Orientation = Orientation.Horizontal,
//                HorizontalAlignment = HorizontalAlignment.Center,
//                Margin = new Thickness(10)
//            };

//            var okButton = new Button
//            {
//                Content = "אישור",
//                Width = 80,
//                Height = 30,
//                Margin = new Thickness(5),
//                IsDefault = true
//            };
//            okButton.Click += (s, e) =>
//            {
//                if (string.IsNullOrWhiteSpace(addressTextBox.Text))
//                {
//                    MessageBox.Show("יש להכניס כתובת תקינה", "שגיאת קלט",
//                        MessageBoxButton.OK, MessageBoxImage.Warning);
//                    return;
//                }
//                NewAddress = addressTextBox.Text.Trim();
//                DialogResult = true;
//                Close();
//            };

//            var cancelButton = new Button
//            {
//                Content = "ביטול",
//                Width = 80,
//                Height = 30,
//                Margin = new Thickness(5),
//                IsCancel = true
//            };
//            cancelButton.Click += (s, e) =>
//            {
//                DialogResult = false;
//                Close();
//            };

//            buttonPanel.Children.Add(okButton);
//            buttonPanel.Children.Add(cancelButton);
//            Grid.SetRow(buttonPanel, 2);
//            grid.Children.Add(buttonPanel);

//            Content = grid;
//        }
//    }
//}
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Windows;
//using System.Windows.Controls;
//using System.ComponentModel; // Required for INotifyPropertyChanged, though DependencyProperty handles it for UI
//using BO;
//using BlApi;

//namespace VolunteerSystem
//{
//    public partial class CallForTreatment : Window, INotifyPropertyChanged
//    {
//        private readonly IBl _bl = Factory.Get();
//        private readonly int _volunteerId;

//        public event PropertyChangedEventHandler? PropertyChanged;

//        protected void OnPropertyChanged(string propertyName)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }

//        // Dependency Property for OpenCallList
//        public IEnumerable<OpenCallInList> OpenCallList
//        {
//            get { return (IEnumerable<OpenCallInList>)GetValue(OpenCallListProperty); }
//            set { SetValue(OpenCallListProperty, value); }
//        }

//        public static readonly DependencyProperty OpenCallListProperty =
//            DependencyProperty.Register("OpenCallList", typeof(IEnumerable<OpenCallInList>), typeof(CallForTreatment), new PropertyMetadata(null));

//        // Dependency Property for SelectedCall
//        public OpenCallInList? SelectedCall
//        {
//            get { return (OpenCallInList?)GetValue(SelectedCallProperty); }
//            set { SetValue(SelectedCallProperty, value); }
//        }

//        public static readonly DependencyProperty SelectedCallProperty =
//            DependencyProperty.Register("SelectedCall", typeof(OpenCallInList), typeof(CallForTreatment), new PropertyMetadata(null, OnSelectedCallChanged));

//        private static void OnSelectedCallChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            if (d is CallForTreatment treatmentWindow && e.NewValue is OpenCallInList selectedCall)
//            {
//                treatmentWindow.DisplayCallDescription(selectedCall);
//            }
//            else if (d is CallForTreatment treatmentWindowNull && e.NewValue == null)
//            {
//                treatmentWindowNull.CallDescriptionTextBlock.Text = "Select a call from the list to see the details.";
//            }
//        }

//        // Dependency Property for SelectedCallDetails (for displaying in TextBlock)
//        public string SelectedCallDetails
//        {
//            get { return (string)GetValue(SelectedCallDetailsProperty); }
//            set { SetValue(SelectedCallDetailsProperty, value); }
//        }

//        public static readonly DependencyProperty SelectedCallDetailsProperty =
//            DependencyProperty.Register("SelectedCallDetails", typeof(string), typeof(CallForTreatment), new PropertyMetadata("Select a call from the list to see the details."));


//        // Properties for ComboBoxes (using regular properties with OnPropertyChanged for simplicity or Dependency Properties if they need to be bound elsewhere)
//        private CallType? _selectedCallType;
//        public CallType? SelectedCallType
//        {
//            get => _selectedCallType;
//            set
//            {
//                if (_selectedCallType != value)
//                {
//                    _selectedCallType = value;
//                    OnPropertyChanged(nameof(SelectedCallType));
//                    QueryOpenCallList();
//                }
//            }
//        }

//        private OpenCallInListFields? _selectedSortField;
//        public OpenCallInListFields? SelectedSortField
//        {
//            get => _selectedSortField;
//            set
//            {
//                if (_selectedSortField != value)
//                {
//                    _selectedSortField = value;
//                    OnPropertyChanged(nameof(SelectedSortField));
//                    QueryOpenCallList();
//                }
//            }
//        }

//        // Collections for ComboBox ItemsSource
//        public List<object> CallTypes { get; set; }
//        public List<object> SortFields { get; set; }

//        public CallForTreatment(int volunteerId)
//        {
//            InitializeComponent();
//            _volunteerId = volunteerId;
//            DataContext = this; // Set DataContext to this window

//            InitializeComboBoxSources(); // Initialize the source collections for ComboBoxes

//            Loaded += Window_Loaded;
//            Closed += Window_Closed;
//        }

//        /// <summary>
//        /// אתחול מקורות הנתונים עבור ה-ComboBoxes של הסינון והמיון.
//        /// </summary>
//        private void InitializeComboBoxSources()
//        {
//            CallTypes = new List<object>
//            {
//                new { Text = "None", Value = (CallType?)null }
//            };
//            foreach (CallType callType in Enum.GetValues<CallType>())
//            {
//                if (callType != CallType.None)
//                {
//                    CallTypes.Add(new { Text = GetCallTypeEnglishName(callType), Value = (CallType?)callType });
//                }
//            }

//            SortFields = new List<object>
//            {
//                new { Text = "Call Number", Value = OpenCallInListFields.Id },
//                new { Text = "Open Time", Value = OpenCallInListFields.Start_time },
//                new { Text = "Max Finish Time", Value = OpenCallInListFields.Max_finish_time },
//                new { Text = "Distance", Value = OpenCallInListFields.CallDistance },
//                new { Text = "Call Type", Value = OpenCallInListFields.CallType }
//            };

//            // Set initial selections for ComboBoxes
//            SelectedCallType = null; // Default to 'None' or no filter
//            SelectedSortField = OpenCallInListFields.Id; // Default sort by Call Number
//        }

//        /// <summary>
//        /// מחזיר שם באנגלית לסוג הקריאה.
//        /// </summary>
//        private string GetCallTypeEnglishName(CallType callType)
//        {
//            return callType switch
//            {
//                CallType.MusicPerformance => "Music Performance",
//                CallType.MusicTherapy => "Music Therapy",
//                CallType.SingingAndEmotionalSupport => "Singing and Emotional Support",
//                CallType.GroupActivities => "Group Activities",
//                CallType.PersonalizedMusicCare => "Personalized Music Care",
//                _ => callType.ToString()
//            };
//        }

//        /// <summary>
//        /// טעינת רשימת הקריאות הפתוחות בהתאם לסינון ולמיון.
//        /// </summary>
//        private void QueryOpenCallList()
//        {
//            try
//            {
//                OpenCallList = _bl.Call.GetOpenCallsForVolunteer(_volunteerId, SelectedCallType, SelectedSortField);

//                if (!OpenCallList.Any())
//                {
//                    MessageBox.Show("לא נמצאו קריאות פתוחות באזור שלך כרגע.", "אין קריאות",
//                                    MessageBoxButton.OK, MessageBoxImage.Information);
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"שגיאה בטעינת הקריאות: {ex.Message}", "שגיאה",
//                                MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }
//        private void SortField_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            // Add logic to handle the selection change event for the SortFieldComboBox
//            // Example: Update the sorting logic based on the selected value
//            var selectedValue = (sender as ComboBox)?.SelectedValue;
//            if (selectedValue != null)
//            {
//                // Implement sorting logic here
//                MessageBox.Show($"Sort field changed to: {selectedValue}");
//            }
//        }
//        private void CallTypeFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            if (IsLoaded)
//            {
//                QueryOpenCallList();
//            }
//        }
//        /// <summary>
//        /// הצגת תיאור הקריאה הנבחרת ב-TextBlock.
//        /// </summary>
//        private void DisplayCallDescription(OpenCallInList call)
//        {
//            SelectedCallDetails = $"Call Number: {call.Id}\n" +
//                                  $"Call Type: {GetCallTypeEnglishName(call.MyCallType)}\n" +
//                                  $"Address: {call.Address}\n" +
//                                  $"Open Time: {call.OpenTime:dd/MM/yyyy HH:mm}\n" +
//                                  $"Max Finish Time: {call.MaxFinishTime?.ToString("dd/MM/yyyy HH:mm") ?? "Not Defined"}\n" +
//                                  $"Distance from you: {call.distanceFromVolunteerToCall:F2} km\n\n";

//            if (!string.IsNullOrEmpty(call.VerbalDescription))
//            {
//                SelectedCallDetails += $"Description:\n{call.VerbalDescription}";
//            }
//            else
//            {
//                SelectedCallDetails += "No additional description.";
//            }
//        }

//        // Event Handlers - No longer needed for SelectionChanged on ComboBoxes due to TwoWay Binding
//        //private void CallTypeFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) { /* Handled by binding to SelectedCallType */ }
//        //private void SortField_SelectionChanged(object sender, SelectionChangedEventArgs e) { /* Handled by binding to SelectedSortField */ }

//        private void RefreshButton_Click(object sender, RoutedEventArgs e)
//        {
//            QueryOpenCallList();
//        }

//        private void UpdateAddressButton_Click(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                var addressWindow = new UpdateAddressWindow();
//                if (addressWindow.ShowDialog() == true)
//                {
//                    var volunteer = _bl.Volunteer.GetVolunteerDetails(_volunteerId);
//                    volunteer.Address = addressWindow.NewAddress;
//                    _bl.Volunteer.UpdateVolunteer(_volunteerId, volunteer);

//                    MessageBox.Show("הכתובת עודכנה בהצלחה!", "עדכון הצליח",
//                                    MessageBoxButton.OK, MessageBoxImage.Information);

//                    QueryOpenCallList(); // Refresh list after address update as distances might change
//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"שגיאה בעדכון הכתובת: {ex.Message}", "שגיאה",
//                                MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }

//        // This event handler is now largely redundant for displaying details, as it's handled by SelectedCall DependencyProperty
//        // It remains here to show that direct handling is still possible if needed for other logic specific to selection.
//        private void OpenCallsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
//        {
//            // The SelectedCall DependencyProperty and its callback OnSelectedCallChanged now handle displaying the details.
//            // You might keep this if you have other logic that needs to run specifically on selection change in the DataGrid.
//        }

//        private void SelectCallButton_Click(object sender, RoutedEventArgs e)
//        {
//            if (sender is Button button && button.Tag is int callId)
//            {
//                try
//                {
//                    var result = MessageBox.Show(
//                        $"האם אתה בטוח שברצונך לבחור את הקריאה מספר {callId} לטיפול?",
//                        "אישור בחירת קריאה",
//                        MessageBoxButton.YesNo,
//                        MessageBoxImage.Question);

//                    if (result == MessageBoxResult.Yes)
//                    {
//                        _bl.Call.SelectCallForTreatment(_volunteerId, callId);

//                        MessageBox.Show(
//                            "הקריאה נבחרה בהצלחה לטיפול!\nהמערכת תעביר אותך למסך פרטי המתנדב.",
//                            "בחירה הצליחה",
//                            MessageBoxButton.OK,
//                            MessageBoxImage.Information);

//                        this.DialogResult = true;
//                        this.Close();
//                    }
//                }
//                catch (BlInvalidOperationException ex)
//                {
//                    MessageBox.Show($"לא ניתן לבחור את הקריאה: {ex.Message}", "שגיאת פעולה",
//                                    MessageBoxButton.OK, MessageBoxImage.Warning);
//                    QueryOpenCallList(); // Refresh the list if an operation failed
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show($"שגיאה בבחירת הקריאה: {ex.Message}", "שגיאה",
//                                    MessageBoxButton.OK, MessageBoxImage.Error);
//                }
//            }
//        }

//        // Observer Methods
//        private void OpenCallListObserver() => QueryOpenCallList();

//        private void Window_Loaded(object sender, RoutedEventArgs e)
//        {
//            _bl.Call.AddObserver(OpenCallListObserver);
//            QueryOpenCallList(); // Initial load of calls when window is loaded
//        }

//        private void Window_Closed(object sender, EventArgs e)
//        {
//            _bl.Call.RemoveObserver(OpenCallListObserver);
//        }
//    }

//    /// <summary>
//    /// חלון עדכון כתובת פשוט (ללא שינוי, הוא נשאר כמו שהוא)
//    /// </summary>
//    public partial class UpdateAddressWindow : Window
//    {
//        public string NewAddress { get; private set; }

//        public UpdateAddressWindow()
//        {
//            InitializeComponent();
//        }

//        private void InitializeComponent()
//        {
//            Title = "עדכון כתובת";
//            Width = 400;
//            Height = 200;
//            WindowStartupLocation = WindowStartupLocation.CenterScreen;
//            ResizeMode = ResizeMode.NoResize;

//            var grid = new Grid();
//            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
//            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
//            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

//            var label = new Label
//            {
//                Content = "הכנס כתובת חדשה:",
//                Margin = new Thickness(10),
//                FontSize = 14
//            };
//            Grid.SetRow(label, 0);
//            grid.Children.Add(label);

//            var addressTextBox = new TextBox
//            {
//                Name = "AddressTextBox",
//                Margin = new Thickness(10),
//                Padding = new Thickness(5),
//                FontSize = 12
//            };
//            Grid.SetRow(addressTextBox, 1);
//            grid.Children.Add(addressTextBox);

//            var buttonPanel = new StackPanel
//            {
//                Orientation = Orientation.Horizontal,
//                HorizontalAlignment = HorizontalAlignment.Center,
//                Margin = new Thickness(10)
//            };

//            var okButton = new Button
//            {
//                Content = "אישור",
//                Width = 80,
//                Height = 30,
//                Margin = new Thickness(5),
//                IsDefault = true
//            };
//            okButton.Click += (s, e) =>
//            {
//                if (string.IsNullOrWhiteSpace(addressTextBox.Text))
//                {
//                    MessageBox.Show("יש להכניס כתובת תקינה", "שגיאת קלט",
//                                    MessageBoxButton.OK, MessageBoxImage.Warning);
//                    return;
//                }
//                NewAddress = addressTextBox.Text.Trim();
//                DialogResult = true;
//                Close();
//            };

//            var cancelButton = new Button
//            {
//                Content = "ביטול",
//                Width = 80,
//                Height = 30,
//                Margin = new Thickness(5),
//                IsCancel = true
//            };
//            cancelButton.Click += (s, e) =>
//            {
//                DialogResult = false;
//                Close();
//            };

//            buttonPanel.Children.Add(okButton);
//            buttonPanel.Children.Add(cancelButton);
//            Grid.SetRow(buttonPanel, 2);
//            grid.Children.Add(buttonPanel);

//            Content = grid;
//        }
//    }
//}
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
using BlApi;
using BO;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for ChooseCallWindow.xaml
    /// </summary>
    public partial class CallForTreatment : Window
    {
        private readonly IBl s_bl = Factory.Get();
        private readonly int _volunteerId;

        public IEnumerable<BO.OpenCallInList> OpenCallList
        {
            get { return (IEnumerable<BO.OpenCallInList>)GetValue(OpenCallListProperty); }
            set { SetValue(OpenCallListProperty, value); }
        }

        public static readonly DependencyProperty OpenCallListProperty =
            DependencyProperty.Register("OpenCallList", typeof(IEnumerable<BO.OpenCallInList>), typeof(CallForTreatment), new PropertyMetadata(null));




        public BO.CallType TypeOfCall { get; set; } = BO.CallType.None;

        /// <summary>
        /// Gets or sets the currently selected volunteer in the list.
        /// </summary>
        public BO.OpenCallInList? SelectedCall { get; set; }

        /// <summary>
        /// Handles double-click event on the volunteer list to open the selected volunteer's details.
        /// </summary>
        private void openCallList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        public ObservableCollection<CallType> CallTypeList { get; } =
       new ObservableCollection<CallType>(Enum.GetValues(typeof(CallType)).Cast<CallType>());
        public CallType SelectedCallType { get; set; }

        public ObservableCollection<OpenCallInListFields> SortFields { get; } =
            new ObservableCollection<OpenCallInListFields>(Enum.GetValues(typeof(OpenCallInListFields)).Cast<OpenCallInListFields>());
        public OpenCallInListFields SelectedSortField { get; set; }

        private async void QueryOpenCallList()
        {
            try
            {
                // טען את הרשימה ברקע
                var calls = await Task.Run(() =>
                    (TypeOfCall == BO.CallType.None)
                        ? s_bl.Call.GetOpenCallsForVolunteer(_volunteerId, null, null)
                        : s_bl.Call.GetOpenCallsForVolunteer(_volunteerId, TypeOfCall, null)
                );

                OpenCallList = calls;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בטעינת הקריאות: {ex.Message}", "שגיאה",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CallTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QueryOpenCallList();
        }


        private void OpenCallListObserver()
            => QueryOpenCallList();
        private void Window_Loaded(object sender, RoutedEventArgs e)
           => s_bl.Call.AddObserver(OpenCallListObserver);


        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(OpenCallListObserver);
        public CallForTreatment(int volunteerId)
        {
            InitializeComponent();
            _volunteerId = volunteerId;
            Loaded += Window_Loaded;
            DataContext = this;
            QueryOpenCallList();
            //Closed += Window_Closed; // ← כאן היה הבעיה
        }
        // Add this method to the code-behind file (CallForTreatment.xaml.cs)
        private void UpdateAddressButton_Click(object sender, RoutedEventArgs e)
        {
            AddressTextBox.Text = ""; 
            UpdateAddressPopup.Visibility = Visibility.Visible;
        }

        private void UpdateAddressOk_Click(object sender, RoutedEventArgs e)
        {
            var newAddress = AddressTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(newAddress))
            {
                MessageBox.Show("Please enter a valid address.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var volunteer = s_bl.Volunteer.GetVolunteerDetails(_volunteerId);
                volunteer.Address = newAddress;
                s_bl.Volunteer.UpdateVolunteer(_volunteerId, volunteer);
                QueryOpenCallList();
                MessageBox.Show("Address updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating address: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            UpdateAddressPopup.Visibility = Visibility.Collapsed;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            QueryOpenCallList();
        }
        private void OpenCallsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OpenCallsDataGrid.SelectedItem is OpenCallInList selectedCall)
            {
                SelectedCall = selectedCall;
            }
        }

        // Add this method to the CallForTreatment class
        private void DisplayCallDescription(OpenCallInList call)
        {
            string description = $"Call Number: {call.Id}\n" +
                                 $"Call Type: {GetCallTypeEnglishName(call.MyCallType)}\n" +
                                 $"Address: {call.Address}\n" +
                                 $"Open Time: {call.OpenTime:dd/MM/yyyy HH:mm}\n" +
                                 $"Max Finish Time: {call.MaxFinishTime?.ToString("dd/MM/yyyy HH:mm") ?? "Not Defined"}\n" +
                                 $"Distance from you: {call.distanceFromVolunteerToCall:F2} km\n\n";

            if (!string.IsNullOrEmpty(call.VerbalDescription))
                 description += $"Description:\n{call.VerbalDescription}";
            
            else
                description += "No additional description.";
           
            // Assumes you have a TextBlock named CallDescriptionTextBlock in your XAML
            CallDescriptionTextBlock.Text = description;
        }

        // Also add this helper if not present
        private string GetCallTypeEnglishName(CallType callType)
        {
            return callType switch
            {
                CallType.MusicPerformance => "Music Performance",
                CallType.MusicTherapy => "Music Therapy",
                CallType.SingingAndEmotionalSupport => "Singing and Emotional Support",
                CallType.GroupActivities => "Group Activities",
                CallType.PersonalizedMusicCare => "Personalized Music Care",
                _ => callType.ToString()
            };
        }
        // Add this method to the code-behind file (CallForTreatment.xaml.cs)
        private void UpdateAddressCancel_Click(object sender, RoutedEventArgs e)
        {
            // Hide the UpdateAddressPopup when the Cancel button is clicked
            UpdateAddressPopup.Visibility = Visibility.Collapsed;
        }
        private void SortField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
                 QueryOpenCallList();
        }

        private void SelectCallButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int callId)
            {
                try
                {
                    var result = MessageBox.Show(
                        $"Are you sure you want to select call number {callId} for treatment?",
                        "Confirm Call Selection",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        s_bl.Call.SelectCallForTreatment(_volunteerId, callId);

                        MessageBox.Show(
                            "The call has been successfully selected for treatment!\nThe system will return you to the volunteer details screen.",
                            "Selection Successful",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        this.DialogResult = true;
                        this.Close();
                    }
                }
                catch (BlInvalidOperationException ex)
                {
                    MessageBox.Show($"Could not select the call: {ex.Message}", "Operation Error",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    QueryOpenCallList(); // Refresh the list if an operation failed
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error selecting the call: {ex.Message}", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}