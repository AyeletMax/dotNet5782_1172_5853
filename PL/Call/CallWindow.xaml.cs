using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using BO;
using System.Linq;
using System.Collections.Generic;
using PL.Helpers;

namespace PL.Call
{
    public partial class CallWindow : Window, INotifyPropertyChanged
    {
        private readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile DispatcherOperation? _observerOperation = null;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public IEnumerable<CallType> AvailableCallTypes { get; set; } =
            Enum.GetValues(typeof(CallType)).Cast<CallType>().Where(ct => ct != CallType.None);

        public CallWindow(int id = 0)
        {
            InitializeComponent();
            SetCurrentValue(ButtonTextProperty, id == 0 ? "Add" : "Update");

            if (id == 0)
            {
                CurrentCall = new BO.Call
                {
                    MyStatus = BO.Status.Opened,
                    OpenTime = s_bl.Admin.GetClock(),
                    CallAssignments = new List<BO.CallAssignInList>()
                };
            }
            else
            {
                try
                {
                    CurrentCall = s_bl.Call.GetCallDetails(id);
                    s_bl.Call.AddObserver(id, RefreshCallObserver);
                }
                catch (Exception ex)
                {
                    BlExceptionHelper.ShowBlException(ex);
                    Close();
                }
            }

            this.Closed += (s, e) =>
            {
                if (CurrentCall != null && CurrentCall.Id != 0)
                    s_bl.Call.RemoveObserver(CurrentCall.Id, RefreshCallObserver);
            };
        }

        public BO.Call? CurrentCall
        {
            get => (BO.Call?)GetValue(CurrentCallProperty);
            set
            {
                SetValue(CurrentCallProperty, value);
                OnPropertyChanged(nameof(CanEditDetails));
                OnPropertyChanged(nameof(CannotEditDetails));
                OnPropertyChanged(nameof(CanEditMaxFinishTime));
                OnPropertyChanged(nameof(IsEditMode));
                OnPropertyChanged(nameof(HasAssignments));
                OnPropertyChanged(nameof(CanEditOnlyMaxTime));
                OnPropertyChanged(nameof(CannotEdit));
                OnPropertyChanged(nameof(CanEdit));
                OnPropertyChanged(nameof(CannotEditMaxFinishTime));
                OnPropertyChanged(nameof(MaxFinishDate));
                OnPropertyChanged(nameof(MaxFinishTime));
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));

        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(CallWindow), new PropertyMetadata("Update"));

        public bool CanEditDetails =>
            CurrentCall is { MyStatus: BO.Status.Opened or BO.Status.AtRisk };

        public bool CannotEditDetails => !CanEditDetails;

        public bool CanEditMaxFinishTime =>
            CurrentCall is { MyStatus: BO.Status.Opened or BO.Status.AtRisk or BO.Status.InProgress or BO.Status.InProgressAtRisk };

        public bool IsEditMode => CurrentCall?.Id != 0;

        public bool HasAssignments => CurrentCall?.CallAssignments?.Any() == true;

        public bool CanEditOnlyMaxTime =>
            CurrentCall is { MyStatus: BO.Status.InProgress or BO.Status.InProgressAtRisk };

        public bool CannotEdit =>
            CurrentCall is { MyStatus: BO.Status.Closed or BO.Status.Expired };

        public bool CanEdit => CanEditDetails || CanEditMaxFinishTime;

        public bool CannotEditMaxFinishTime => !CanEditMaxFinishTime;

        public DateTime? MaxFinishDate
        {
            get => CurrentCall?.MaxFinishTime?.Date;
            set
            {
                if (CurrentCall != null && value.HasValue)
                {
                    var timeOfDay = CurrentCall.MaxFinishTime?.TimeOfDay ?? TimeSpan.Zero;
                    CurrentCall.MaxFinishTime = value.Value.Date.Add(timeOfDay);
                    OnPropertyChanged(nameof(MaxFinishDate));
                }
            }
        }

        public string MaxFinishTime
        {
            get => CurrentCall?.MaxFinishTime?.ToString("HH:mm") ?? "";
            set
            {
                if (CurrentCall != null && TimeSpan.TryParse(value, out var time))
                {
                    var date = CurrentCall.MaxFinishTime?.Date ?? DateTime.Today;
                    CurrentCall.MaxFinishTime = date.Add(time);
                    OnPropertyChanged(nameof(MaxFinishTime));
                }
            }
        }

        private void RefreshCallObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    if (CurrentCall == null) return;
                    int id = CurrentCall.Id;
                    CurrentCall = null;
                    CurrentCall = s_bl.Call.GetCallDetails(id);
                });
            }
        }

    
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentCall?.MaxFinishTime == null)
                    throw new BlInvalidFormatException("Please select a maximum finish time.");

                if (CurrentCall.MaxFinishTime <= CurrentCall.OpenTime)
                    throw new BlInvalidLogicException("End time must be after opening time.");

                if (CurrentCall.MyStatus is BO.Status.Closed or BO.Status.Expired)
                    throw new BlInvalidOperationException("Cannot update a call that is closed or expired.");

                if (CurrentCall.MyStatus is BO.Status.InProgress or BO.Status.InProgressAtRisk)
                {
                    s_bl.Call.UpdateCallDetails(CurrentCall!);
                    MessageBox.Show("Maximum time updated successfully!");
                    Close();
                    return;
                }

                if (string.IsNullOrWhiteSpace(CurrentCall?.Address))
                    throw new BlInvalidFormatException("Please enter an address.");

                if (CurrentCall.MyCallType == CallType.None)
                    throw new BlInvalidFormatException("Please select a call type.");

                if (ButtonText == "Add")
                {
                    s_bl.Call.AddCall(CurrentCall!);
                    MessageBox.Show("Call added successfully!");
                }
                else
                {
                    s_bl.Call.UpdateCallDetails(CurrentCall!);
                    MessageBox.Show("Call updated successfully!");
                }

                Close();
            }
            catch (Exception ex)
            {
                BlExceptionHelper.ShowBlException(ex);
            }
        }

    }
}