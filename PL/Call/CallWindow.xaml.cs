using System;
using System.Collections.Generic;
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
    /// Window for adding a new call or viewing/editing an existing call
    /// </summary>
    public partial class CallWindow : Window, INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// The call object (new or existing)
        /// </summary>
        public BO.Call Call { get; set; }

        private DateTime? _maxFinishDate;
        /// <summary>
        /// Maximum finish date for the call
        /// </summary>
        public DateTime? MaxFinishDate
        {
            get => _maxFinishDate;
            set
            {
                if (_maxFinishDate != value)
                {
                    _maxFinishDate = value;
                    OnPropertyChanged();
                    UpdateMaxFinishTime();
                }
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
                if (_maxFinishTime != value)
                {
                    _maxFinishTime = value;
                    OnPropertyChanged();
                    UpdateMaxFinishTime();
                }
            }
        }

        /// <summary>
        /// Available call types for the ComboBox
        /// </summary>
        public IEnumerable<CallType> AvailableCallTypes { get; set; } =
            Enum.GetValues(typeof(CallType)).Cast<CallType>().Where(ct => ct != CallType.None);

        /// <summary>
        /// Selected call type for binding
        /// </summary>
        public CallType SelectedCallType
        {
            get => Call?.MyCallType ?? CallType.None;
            set
            {
                if (Call != null && Call.MyCallType != value)
                {
                    Call.MyCallType = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Call));
                }
            }
        }

        private int? observedCallId;

        /// <summary>
        /// Whether the window is opened in edit mode
        /// </summary>
        public bool IsEditMode { get; private set; }

        /// <summary>
        /// Whether all fields can be edited based on call status
        /// </summary>
        public bool CanEditAllFields { get; private set; }

        /// <summary>
        /// Whether only max finish time can be edited
        /// </summary>
        public bool CanEditOnlyMaxTime { get; private set; }

        /// <summary>
        /// Whether max finish time can be edited (combination of all cases where it's allowed)
        /// </summary>
        public bool CanEditMaxTime => CanEditAllFields || CanEditOnlyMaxTime;

        /// <summary>
        /// Whether all fields cannot be edited (opposite of CanEditAllFields)
        /// </summary>
        public bool CannotEditAllFields => !CanEditAllFields;

        /// <summary>
        /// Whether max time cannot be edited (opposite of CanEditMaxTime)
        /// </summary>
        public bool CannotEditMaxTime => !CanEditMaxTime;

        /// <summary>
        /// Whether no fields can be edited
        /// </summary>
        public bool CannotEdit { get; private set; }

        /// <summary>
        /// Whether the call can be saved/updated
        /// </summary>
        public bool CanEdit => !CannotEdit;

        /// <summary>
        /// Whether assignments section should be visible
        /// </summary>
        public bool HasAssignments => Call?.CallAssignments?.Any() == true;

        /// <summary>
        /// Button text based on mode
        /// </summary>
        public string ButtonText => IsEditMode ? "Update" : "Add";

        /// <summary>
        /// Window title based on mode and call ID
        /// </summary>
        public string WindowTitle => IsEditMode ? $"Call Details - ID: {Call?.Id}" : "Add New Call";

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for adding a new call
        /// </summary>
        public CallWindow()
        {
            InitializeComponent();
            DataContext = this;
            InitializeNewCall();
            IsEditMode = false;
            SetEditPermissions();
            OnPropertyChanged(nameof(WindowTitle));
            OnPropertyChanged(nameof(ButtonText));
        }

        /// <summary>
        /// Constructor for viewing/editing an existing call
        /// </summary>
        /// <param name="existingCall">The existing call to view/edit</param>
        public CallWindow(BO.Call existingCall)
        {
            InitializeComponent();
            DataContext = this;
            Call = existingCall ?? throw new ArgumentNullException(nameof(existingCall));
            InitializeFromExistingCall();
            IsEditMode = true;
            SetEditPermissions();
            OnPropertyChanged(nameof(WindowTitle));
            OnPropertyChanged(nameof(ButtonText));

            observedCallId = Call.Id;
            BlApi.Factory.Get().Call.AddObserver(OnCallChanged);
            this.Closed += (s, e) => BlApi.Factory.Get().Call.RemoveObserver(OnCallChanged);
        }

        private void OnCallChanged()
        {
            if (observedCallId.HasValue)
            {
                var updated = BlApi.Factory.Get().Call.GetCallDetails(observedCallId.Value);
                Call = updated;
                OnPropertyChanged(nameof(Call));
                InitializeFromExistingCall();
                SetEditPermissions();
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize new call
        /// </summary>
        private void InitializeNewCall()
        {
            Call = new BO.Call
            {
                MyStatus = Status.Opened,
                OpenTime = DateTime.Now,
                MyCallType = CallType.None,
                Address = string.Empty,
                VerbalDescription = string.Empty,
                CallAssignments = new List<CallAssignInList>()
            };

            // Set default maximum time - 24 hours from now
            MaxFinishDate = DateTime.Today.AddDays(1);
            MaxFinishTime = "23:59";

            // Update maximum time in the call
            UpdateMaxFinishTime();

            // Notify property changes
            OnPropertyChanged(nameof(Call));
            OnPropertyChanged(nameof(SelectedCallType));
        }

        /// <summary>
        /// Initialize from existing call
        /// </summary>
        private void InitializeFromExistingCall()
        {
            if (Call.MaxFinishTime.HasValue)
            {
                MaxFinishDate = Call.MaxFinishTime.Value.Date;
                MaxFinishTime = Call.MaxFinishTime.Value.ToString("HH:mm");
            }
            else
            {
                MaxFinishDate = DateTime.Today.AddDays(1);
                MaxFinishTime = "23:59";
            }

            // Don't change status when viewing - preserve original status
            // UpdateCallStatus();

            // Notify property changes
            OnPropertyChanged(nameof(Call));
            OnPropertyChanged(nameof(SelectedCallType));
            OnPropertyChanged(nameof(WindowTitle));
        }

        /// <summary>
        /// Update call status based on current conditions - only for new calls
        /// </summary>
        private void UpdateCallStatus()
        {
            // Only update status for new calls or when explicitly saving
            if (IsEditMode)
                return;

            if (Call.MyStatus == Status.Closed)
                return;

            if (Call.MaxFinishTime.HasValue && DateTime.Now > Call.MaxFinishTime.Value)
            {
                Call.MyStatus = Status.Expired;
            }
            else if (Call.CallAssignments != null && Call.CallAssignments.Any(a => a.ExitTime == null))
            {
                Call.MyStatus = Status.InProgress;
            }
            else
            {
                Call.MyStatus = Status.Opened;
            }
            OnPropertyChanged(nameof(Call));
        }

        /// <summary>
        /// Set edit permissions based on call status
        /// </summary>
        private void SetEditPermissions()
        {
            if (!IsEditMode)
            {
                // New call - all fields editable
                CanEditAllFields = true;
                CanEditOnlyMaxTime = false;
                CannotEdit = false;
            }
            else
            {
                // Existing call - permissions based on status
                switch (Call.MyStatus)
                {
                    case Status.Opened:
                        CanEditAllFields = true;
                        CanEditOnlyMaxTime = false;
                        CannotEdit = false;
                        break;

                    case Status.AtRisk:
                        CanEditAllFields = true;
                        CanEditOnlyMaxTime = false;
                        CannotEdit = false;
                        break;

                    case Status.InProgress:
                        CanEditAllFields = false;
                        CanEditOnlyMaxTime = true;
                        CannotEdit = false;
                        break;

                    case Status.InProgressAtRisk:
                        CanEditAllFields = false;
                        CanEditOnlyMaxTime = true;
                        CannotEdit = false;
                        break;

                    case Status.Closed:
                        CanEditAllFields = false;
                        CanEditOnlyMaxTime = false;
                        CannotEdit = true;
                        break;

                    case Status.Expired:
                        CanEditAllFields = false;
                        CanEditOnlyMaxTime = false;
                        CannotEdit = true;
                        break;

                    default:
                        CanEditAllFields = false;
                        CanEditOnlyMaxTime = false;
                        CannotEdit = true;
                        break;
                }
            }

            OnPropertyChanged(nameof(CanEditAllFields));
            OnPropertyChanged(nameof(CanEditOnlyMaxTime));
            OnPropertyChanged(nameof(CanEditMaxTime));
            OnPropertyChanged(nameof(CannotEditAllFields));
            OnPropertyChanged(nameof(CannotEditMaxTime));
            OnPropertyChanged(nameof(CannotEdit));
            OnPropertyChanged(nameof(CanEdit));
            OnPropertyChanged(nameof(HasAssignments));
        }

        #endregion

        #region Event Handlers

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
                    BlApi.Factory.Get().Call.UpdateCallDetails(Call);
                    MessageBox.Show("Call updated successfully!", "Success",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    BlApi.Factory.Get().Call.AddCall(Call);
                    MessageBox.Show("Call added successfully!\nEmail sent to appropriate volunteers.",
                                  "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                this.Close();
            }
            catch (BlInvalidFormatException ex)
            {
                MessageBox.Show($"Format error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BlInvalidOperationException ex)
            {
                MessageBox.Show($"Invalid operation: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BlDoesNotExistException ex)
            {
                MessageBox.Show($"Not found: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BlAlreadyExistsException ex)
            {
                MessageBox.Show($"Already exists: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BlGeneralDatabaseException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"General error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handle cancel button click
        /// </summary>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
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
            // Skip validation for fields that cannot be edited
            if (CanEditAllFields)
            {
                // Check call type
                if (Call.MyCallType == CallType.None)
                {
                    MessageBox.Show("Please select a call type", "Validation Error",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                // Check address
                if (string.IsNullOrWhiteSpace(Call.Address))
                {
                    MessageBox.Show("Please enter an address", "Validation Error",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (Call.Address.Trim().Length < 5)
                {
                    MessageBox.Show("Address must contain at least 5 characters", "Validation Error",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                // Check description length
                if (!string.IsNullOrEmpty(Call.VerbalDescription) &&
                    Call.VerbalDescription.Length > 500)
                {
                    MessageBox.Show("Verbal description cannot exceed 500 characters", "Validation Error",
                                  MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            // Max finish time validation (if editable)
            if (CanEditMaxTime)
            {
                // Check time format
                if (!IsValidTimeFormat(MaxFinishTime))
                {
                    MessageBox.Show("Invalid time format. Please enter in HH:MM format (e.g., 14:30)",
                                  "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                // Check maximum date and time
                if (!MaxFinishDate.HasValue)
                {
                    MessageBox.Show("Please select a maximum finish date for the call",
                                  "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (MaxFinishDate.Value.Date < DateTime.Today)
                {
                    MessageBox.Show("Maximum finish date cannot be in the past",
                                  "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                // Check that maximum time is greater than opening time
                UpdateMaxFinishTime();
                if (Call.MaxFinishTime.HasValue && Call.MaxFinishTime.Value <= Call.OpenTime)
                {
                    MessageBox.Show("Maximum finish time must be greater than the call opening time",
                                  "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

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
            if (MaxFinishDate.HasValue && IsValidTimeFormat(MaxFinishTime))
            {
                if (TimeSpan.TryParseExact(MaxFinishTime, @"h\:mm", CultureInfo.InvariantCulture, out TimeSpan time) ||
                    TimeSpan.TryParseExact(MaxFinishTime, @"hh\:mm", CultureInfo.InvariantCulture, out time))
                {
                    Call.MaxFinishTime = MaxFinishDate.Value.Date.Add(time);
                }
            }
            // Only update status for new calls
            if (!IsEditMode)
            {
                UpdateCallStatus();
            }
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