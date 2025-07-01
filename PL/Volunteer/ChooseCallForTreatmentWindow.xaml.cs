using BlApi;
using BO;
using System;
using System.Collections.Generic;
using System.Windows;

namespace PL.Volunteer;

public partial class ChooseCallForTreatmentWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    private readonly int _volunteerId;
    public List<OpenCallInList> OpenCalls
    {
        get => (List<OpenCallInList>)GetValue(OpenCallsProperty);
        set => SetValue(OpenCallsProperty, value);
    }

    public static readonly DependencyProperty OpenCallsProperty =
        DependencyProperty.Register("OpenCalls", typeof(List<OpenCallInList>), typeof(ChooseCallForTreatmentWindow));

    public OpenCallInList? SelectedCall
    {
        get => (OpenCallInList?)GetValue(SelectedCallProperty);
        set => SetValue(SelectedCallProperty, value);
    }

    public static readonly DependencyProperty SelectedCallProperty =
        DependencyProperty.Register("SelectedCall", typeof(OpenCallInList), typeof(ChooseCallForTreatmentWindow));

    public FilterAndSortByFields? SortField
    {
        get => (FilterAndSortByFields?)GetValue(SortFieldProperty);
        set => SetValue(SortFieldProperty, value);
    }

    public static readonly DependencyProperty SortFieldProperty =
        DependencyProperty.Register("SortField", typeof(FilterAndSortByFields?), typeof(ChooseCallForTreatmentWindow));

    public ChooseCallForTreatmentWindow(int volunteerId)
    {
        _volunteerId = volunteerId;
        InitializeComponent();
    }

    private void Window_Loaded(object _, RoutedEventArgs __) => LoadCalls();

    private void Apply_Click(object _, RoutedEventArgs __) => LoadCalls();

    private void Select_Click(object _, RoutedEventArgs __)
    {
        if (SelectedCall == null)
        {
            MessageBox.Show("Please select a call.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            s_bl.Call.SelectCallForTreatment(_volunteerId, SelectedCall.Id);
            MessageBox.Show("Call selected successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LoadCalls()
    {
        OpenCalls = new List<OpenCallInList>(s_bl.Call.GetOpenCallsByVolunteer(_volunteerId, null, SortField));
    }
}
