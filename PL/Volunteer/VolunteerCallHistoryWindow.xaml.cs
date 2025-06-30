using BlApi;
using BO;
using System;
using System.Collections.Generic;
using System.Windows;

namespace PL.Volunteer;

public partial class VolunteerCallHistoryWindow : Window
{
    private readonly IBl _bl = Factory.Get();
    private readonly int _volunteerId;

    public List<ClosedCallInList> ClosedCalls
    {
        get => (List<ClosedCallInList>)GetValue(ClosedCallsProperty);
        set => SetValue(ClosedCallsProperty, value);
    }

    public static readonly DependencyProperty ClosedCallsProperty =
        DependencyProperty.Register("ClosedCalls", typeof(List<ClosedCallInList>), typeof(VolunteerCallHistoryWindow));

    public FilterAndSortByFields? FilterField
    {
        get => (FilterAndSortByFields?)GetValue(FilterFieldProperty);
        set => SetValue(FilterFieldProperty, value);
    }

    public static readonly DependencyProperty FilterFieldProperty =
        DependencyProperty.Register("FilterField", typeof(FilterAndSortByFields?), typeof(VolunteerCallHistoryWindow));

    public FilterAndSortByFields? SortField
    {
        get => (FilterAndSortByFields?)GetValue(SortFieldProperty);
        set => SetValue(SortFieldProperty, value);
    }

    public static readonly DependencyProperty SortFieldProperty =
        DependencyProperty.Register("SortField", typeof(FilterAndSortByFields?), typeof(VolunteerCallHistoryWindow));

    public VolunteerCallHistoryWindow(int volunteerId)
    {
        _volunteerId = volunteerId;
        InitializeComponent();
    }

    private void Window_Loaded(object _, RoutedEventArgs __) => LoadData();

    private void Apply_Click(object _, RoutedEventArgs __) => LoadData();


    private void LoadData()
    {
        ClosedCalls = new List<ClosedCallInList>(
            _bl.Call.GetClosedCallsByVolunteer(_volunteerId, FilterField, SortField));
    }
}