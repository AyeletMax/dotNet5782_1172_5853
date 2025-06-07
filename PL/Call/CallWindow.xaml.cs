using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using BO;

namespace PL.Call
{
    /// <summary>
    /// Window for adding a new call to the system
    /// </summary>
    public partial class CallWindow : Window, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// The new call object
        /// </summary>
        public BO.Call NewCall { get; set; }

        private DateTime? _maxFinishDate;
        /// <summary>
        /// Maximum finish date for the call
        /// </summary>
        public DateTime? MaxFinishDate
        {
            get => _maxFinishDate;
            set
            {
                _maxFinishDate = value;
                OnPropertyChanged();
                UpdateMaxFinishTime();
            }
        }

        private string _maxFinishTime = "23:59";
        /// <summary>
        /// Maximum finish time for the call (HH:MM format)
        /// </summary>
        public string MaxFinishTime
        {
            get => _maxFinishTime;
            set
            {
                _maxFinishTime = value;
                OnPropertyChanged();
                UpdateMaxFinishTime();
            }
        }

        /// <summary>
        /// Whether the window is opened in edit mode
        /// </summary>
        public bool IsEditMode { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for adding a new call
        /// </summary>
        public CallWindow()
        {
            InitializeComponent();
            InitializeNewCall();
            IsEditMode = false;
            this.Title = "Add New Call";
        }

        /// <summary>
        /// Constructor for editing an existing call
        /// </summary>
        /// <param name="existingCall">The existing call to edit</param>
        public CallWindow(BO.Call existingCall)
        {
            InitializeComponent();
            NewCall = existingCall ?? throw new ArgumentNullException(nameof(existingCall));
            InitializeFromExistingCall();
            IsEditMode = true;
            this.Title = "Edit Call";
            AddButton.Content = "Update Call";
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize new call
        /// </summary>
        private void InitializeNewCall()
        {
            NewCall = new BO.Call
            {
                MyStatus = Status.Opened,
                OpenTime = DateTime.Now,
                MyCallType = CallType.None,
                Address = string.Empty,
                VerbalDescription = string.Empty,
                CallAssignments = new List<CallAssignInList>()
            };

            // Set default maximum time - 24 hours from today
            MaxFinishDate = DateTime.Today.AddDays(1);
            MaxFinishTime = "23:59";

            // Update maximum time in the call
            UpdateMaxFinishTime();
        }

        /// <summary>
        /// Initialize from existing call
        /// </summary>
        private void InitializeFromExistingCall()
        {
            if (NewCall.MaxFinishTime.HasValue)
            {
                MaxFinishDate = NewCall.MaxFinishTime.Value.Date;
                MaxFinishTime = NewCall.MaxFinishTime.Value.ToString("HH:mm");
            }
            else
            {
                MaxFinishDate = DateTime.Today.AddDays(1);
                MaxFinishTime = "23:59";
            }

            // Set the selected value in the combo box
            SetSelectedCallType();
        }

        /// <summary>
        /// Set the selected type in the call type combo box
        /// </summary>
        private void SetSelectedCallType()
        {
            foreach (ComboBoxItem item in CallTypeComboBox.Items)
            {
                if (item.Tag.ToString() == NewCall.MyCallType.ToString())
                {
                    CallTypeComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handle call type selection change
        /// </summary>
        private void CallTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CallTypeComboBox.SelectedItem is ComboBoxItem selectedItem &&
                selectedItem.Tag != null)
            {
                if (Enum.TryParse<CallType>(selectedItem.Tag.ToString(), out CallType callType))
                {
                    NewCall.MyCallType = callType;
                }
            }
        }

        /// <summary>
        /// Handle add/update button click
        /// </summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Format validation checks at the presentation layer
                if (!ValidateInputFormat())
                    return;

                // Update maximum finish time
                UpdateMaxFinishTime();

                // Send to business logic layer
                if (IsEditMode)
                {
                    BlApi.Factory.Get().Call.UpdateCallDetails(NewCall);
                    MessageBox.Show("Call updated successfully!", "Success",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    BlApi.Factory.Get().Call.AddCall(NewCall);
                    MessageBox.Show("Call added successfully!\nEmail sent to appropriate volunteers.",
                                  "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                this.DialogResult = true;
                this.Close();
            }
            catch (BlInvalidFormatException ex)
            {
                ShowErrorMessage($"Format error: {ex.Message}");
            }
            catch (BlInvalidOperationException ex)
            {
                ShowErrorMessage($"Invalid operation: {ex.Message}");
            }
            catch (BlDoesNotExistException ex)
            {
                ShowErrorMessage($"Not found: {ex.Message}");
            }
            catch (BlAlreadyExistsException ex)
            {
                ShowErrorMessage($"Already exists: {ex.Message}");
            }
            catch (BlGeneralDatabaseException ex)
            {
                ShowErrorMessage($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"General error: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle cancel button click
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validate input format
        /// </summary>
        /// <returns>true if input is valid</returns>
        private bool ValidateInputFormat()
        {
            // Check call type
            if (NewCall.MyCallType == CallType.None)
            {
                ShowErrorMessage("Please select a call type");
                CallTypeComboBox.Focus();
                return false;
            }

            // Check address
            if (string.IsNullOrWhiteSpace(NewCall.Address))
            {
                ShowErrorMessage("Please enter an address");
                AddressTextBox.Focus();
                return false;
            }

            if (NewCall.Address.Trim().Length < 5)
            {
                ShowErrorMessage("Address must contain at least 5 characters");
                AddressTextBox.Focus();
                return false;
            }

            // Check description length
            if (!string.IsNullOrEmpty(NewCall.VerbalDescription) &&
                NewCall.VerbalDescription.Length > 500)
            {
                ShowErrorMessage("Verbal description cannot exceed 500 characters");
                DescriptionTextBox.Focus();
                return false;
            }

            // Check time format
            if (!IsValidTimeFormat(MaxFinishTime))
            {
                ShowErrorMessage("Invalid time format. Please enter in HH:MM format (e.g., 14:30)");
                MaxFinishTimeTextBox.Focus();
                return false;
            }

            // Check maximum date and time
            if (!MaxFinishDate.HasValue)
            {
                ShowErrorMessage("Please select a maximum finish date for the call");
                MaxFinishDatePicker.Focus();
                return false;
            }

            if (MaxFinishDate.Value.Date < DateTime.Today)
            {
                ShowErrorMessage("Maximum finish date cannot be in the past");
                MaxFinishDatePicker.Focus();
                return false;
            }

            // Check that maximum time is greater than opening time
            UpdateMaxFinishTime();
            if (NewCall.MaxFinishTime.HasValue && NewCall.MaxFinishTime.Value <= NewCall.OpenTime)
            {
                ShowErrorMessage("Maximum finish time must be greater than the call opening time");
                MaxFinishTimeTextBox.Focus();
                return false;
            }

            HideErrorMessage();
            return true;
        }

        /// <summary>
        /// Validate time format
        /// </summary>
        /// <param name="timeString">Time string to validate</param>
        /// <returns>true if format is valid</returns>
        private bool IsValidTimeFormat(string timeString)
        {
            if (string.IsNullOrWhiteSpace(timeString))
                return false;

            // Check HH:MM format
            var timePattern = @"^([0-1]?[0-9]|2[0-3]):([0-5][0-9])$";
            if (!Regex.IsMatch(timeString, timePattern))
                return false;

            // Check if it can be parsed to valid time
            return TimeSpan.TryParseExact(timeString, @"h\:mm", CultureInfo.InvariantCulture, out _) ||
                   TimeSpan.TryParseExact(timeString, @"hh\:mm", CultureInfo.InvariantCulture, out _);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Update maximum finish time for the call
        /// </summary>
        private void UpdateMaxFinishTime()
        {
            if (MaxFinishDate.HasValue && !string.IsNullOrWhiteSpace(MaxFinishTime))
            {
                if (TimeSpan.TryParseExact(MaxFinishTime, @"h\:mm", CultureInfo.InvariantCulture, out TimeSpan time) ||
                    TimeSpan.TryParseExact(MaxFinishTime, @"hh\:mm", CultureInfo.InvariantCulture, out time))
                {
                    NewCall.MaxFinishTime = MaxFinishDate.Value.Date.Add(time);
                }
            }
        }

        /// <summary>
        /// Show error message
        /// </summary>
        /// <param name="message">Error message</param>
        private void ShowErrorMessage(string message)
        {
            ErrorMessageTextBlock.Text = message;
            ErrorMessageTextBlock.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hide error message
        /// </summary>
        private void HideErrorMessage()
        {
            ErrorMessageTextBlock.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}