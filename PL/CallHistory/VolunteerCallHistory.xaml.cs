using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BO;
using BlApi;

namespace PL.CallHistory;

public partial class VolunteerCallHistory : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    private readonly int _volunteerId;

    public VolunteerCallHistory(int volunteerId)
    {

        _volunteerId = volunteerId;
        //DataContext = this;

        CallTypes = Enum.GetValues(typeof(CallType)).Cast<CallType>().ToList();
        SortFields = Enum.GetValues(typeof(ClosedCallInListFields)).Cast<ClosedCallInListFields>().ToList();
        InitializeComponent();
        SelectedCallType = null;
        SelectedSortField = null;

        LoadClosedCalls();
    }

    public IEnumerable<CallType> CallTypes { get; set; }
    public IEnumerable<ClosedCallInListFields> SortFields { get; set; }

    public CallType? SelectedCallType
    {
        get => (CallType?)GetValue(SelectedCallTypeProperty);
        set => SetValue(SelectedCallTypeProperty, value);
    }

    public static readonly DependencyProperty SelectedCallTypeProperty =
        DependencyProperty.Register(nameof(SelectedCallType), typeof(CallType?), typeof(VolunteerCallHistory),
            new PropertyMetadata(null, OnFilterChanged));

    public ClosedCallInListFields? SelectedSortField
    {
        get => (ClosedCallInListFields?)GetValue(SelectedSortFieldProperty);
        set => SetValue(SelectedSortFieldProperty, value);
    }

    public static readonly DependencyProperty SelectedSortFieldProperty =
        DependencyProperty.Register(nameof(SelectedSortField), typeof(ClosedCallInListFields?), typeof(VolunteerCallHistory),
            new PropertyMetadata(null, OnFilterChanged));

    public IEnumerable<ClosedCallInList> ClosedCalls
    {
        get => (IEnumerable<ClosedCallInList>)GetValue(ClosedCallsProperty);
        set => SetValue(ClosedCallsProperty, value);
    }

    public static readonly DependencyProperty ClosedCallsProperty =
        DependencyProperty.Register(nameof(ClosedCalls), typeof(IEnumerable<ClosedCallInList>), typeof(VolunteerCallHistory), new PropertyMetadata(null));

    private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is VolunteerCallHistory window)
            window.LoadClosedCalls();
    }

    private void LoadClosedCalls()
    {
        try
        {
            ClosedCalls = s_bl.Call.GetClosedCallsByVolunteer(_volunteerId, SelectedCallType, SelectedSortField);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading call history: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
