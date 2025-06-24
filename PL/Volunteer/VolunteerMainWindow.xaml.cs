//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Windows;
//using System.Windows.Controls;
//using System.ComponentModel;
//using BO;
//using BlApi;

//namespace VolunteerSystem
//{
//    public partial class VolunteerMainWindow : Window
//    {
//        private readonly ICall _callBl;
//        private readonly IVolunteer _volunteerBl;
//        private readonly int _volunteerId;
//        private IEnumerable<OpenCallInList> _allOpenCalls;
//        private OpenCallInList _selectedCall;

//        public VolunteerMainWindow(int volunteerId)
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
//                new { Text = "הכל", Value = (CallType?)null }
//            };

//            foreach (CallType callType in Enum.GetValues<CallType>())
//            {
//                if (callType != CallType.None)
//                {
//                    callTypes.Add(new { Text = GetCallTypeHebrewName(callType), Value = (CallType?)callType });
//                }
//            }

//            CallTypeFilterComboBox.ItemsSource = callTypes;
//            CallTypeFilterComboBox.DisplayMemberPath = "Text";
//            CallTypeFilterComboBox.SelectedValuePath = "Value";
//            CallTypeFilterComboBox.SelectedIndex = 0;

//            // אתחול ComboBox של מיון
//            var sortFields = new List<object>
//            {
//                new { Text = "מספר קריאה", Value = OpenCallInListFields.Id },
//                new { Text = "זמן פתיחה", Value = OpenCallInListFields.OpenTime },
//                new { Text = "זמן סיום מקסימלי", Value = OpenCallInListFields.MaxFinishTime },
//                new { Text = "מרחק", Value = OpenCallInListFields.distanceFromVolunteerToCall },
//                new { Text = "סוג קריאה", Value = OpenCallInListFields.MyCallType }
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
//        private string GetCallTypeHebrewName(CallType callType)
//        {
//            return callType switch
//            {
//                CallType.Emergency => "חירום",
//                CallType.Medical => "רפואי",
//                CallType.Technical => "טכני",
//                CallType.Social => "חברתי",
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
//                LoadMapForCall(selectedCall);
//            }
//        }

//        /// <summary>
//        /// הצגת תיאור הקריאה
//        /// </summary>
//        private void DisplayCallDescription(OpenCallInList call)
//        {
//            var description = $"מספר קריאה: {call.Id}\n" +
//                             $"סוג קריאה: {GetCallTypeHebrewName(call.MyCallType)}\n" +
//                             $"כתובת: {call.Address}\n" +
//                             $"זמן פתיחה: {call.OpenTime:dd/MM/yyyy HH:mm}\n" +
//                             $"זמן סיום מקסימלי: {call.MaxFinishTime?.ToString("dd/MM/yyyy HH:mm") ?? "לא מוגדר"}\n" +
//                             $"מרחק ממך: {call.distanceFromVolunteerToCall:F2} ק״מ\n\n";

//            if (!string.IsNullOrEmpty(call.VerbalDescription))
//            {
//                description += $"תיאור מילולי:\n{call.VerbalDescription}";
//            }
//            else
//            {
//                description += "אין תיאור מילולי נוסף.";
//            }

//            CallDescriptionTextBlock.Text = description;
//        }

//        /// <summary>
//        /// טעינת מפה עבור הקריאה הנבחרת
//        /// </summary>
//        private void LoadMapForCall(OpenCallInList call)
//        {
//            try
//            {
//                // כאן ניתן להוסיף קוד להצגת מפה
//                // לדוגמה, עם Google Maps או Bing Maps

//                // דוגמה פשוטה עם HTML + JavaScript
//                string mapHtml = GenerateMapHtml(call);
//                MapWebBrowser.NavigateToString(mapHtml);
//                MapWebBrowser.Visibility = Visibility.Visible;
//                MapPlaceholder.Visibility = Visibility.Collapsed;
//            }
//            catch (Exception ex)
//            {
//                MapWebBrowser.Visibility = Visibility.Collapsed;
//                MapPlaceholder.Visibility = Visibility.Visible;
//                MapPlaceholder.Text = $"שגיאה בטעינת המפה: {ex.Message}";
//            }
//        }

//        /// <summary>
//        /// יצירת HTML עבור המפה
//        /// </summary>
//        private string GenerateMapHtml(OpenCallInList call)
//        {
//            // דוגמה פשוטה - בפועל תצטרך להשתמש ב-API של Google Maps או Bing Maps
//            return $@"
//                <!DOCTYPE html>
//                <html>
//                <head>
//                    <meta charset='utf-8'>
//                    <title>מפה</title>
//                    <style>
//                        body {{ font-family: Arial, sans-serif; text-align: center; padding: 20px; }}
//                        .info {{ background: #f0f0f0; padding: 10px; border-radius: 5px; margin: 10px; }}
//                    </style>
//                </head>
//                <body>
//                    <h3>מיקום הקריאה</h3>
//                    <div class='info'>
//                        <strong>כתובת הקריאה:</strong><br>
//                        {call.Address}
//                    </div>
//                    <div class='info'>
//                        <strong>מרחק ממיקומך:</strong><br>
//                        {call.distanceFromVolunteerToCall:F2} ק״מ
//                    </div>
//                    <p style='color: #666; font-size: 12px;'>
//                        להצגת מפה מלאה, יש להוסיף שילוב עם Google Maps API או Bing Maps
//                    </p>
//                </body>
//                </html>";
//        }

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
//            WindowStartupLocation = WindowStartupLocation.CenterParent;
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