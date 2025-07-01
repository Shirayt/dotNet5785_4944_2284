using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using BlApi;
using BO;

namespace PL.Volunteer;

public partial class VolunteerMainWindow : Window, INotifyPropertyChanged
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public event PropertyChangedEventHandler? PropertyChanged;

    public BO.Volunteer Volunteer
    {
        get { return (BO.Volunteer)GetValue(VolunteerProperty); }
        set { SetValue(VolunteerProperty, value); }
    }

    public static readonly DependencyProperty VolunteerProperty =
        DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(VolunteerMainWindow), new PropertyMetadata(null));

    public CallInProgress? CallInProgress { get; set; }

    public bool HasCallInProgress => CallInProgress != null;
    public bool ShowChooseCallButton => CallInProgress == null;

    private readonly int _volunteerId;

    public VolunteerMainWindow(int volunteerId)
    {
        InitializeComponent();
        _volunteerId = volunteerId;
        LoadVolunteer(volunteerId);
    }

    private void LoadVolunteer(int id)
    {
        Volunteer = s_bl.Volunteer.GetVolunteerDetails(id);
        CallInProgress = Volunteer.callInProgress;
        NotifyAll();
    }

    private void NotifyAll()
    {
        OnPropertyChanged(nameof(Volunteer));
        OnPropertyChanged(nameof(CallInProgress));
        OnPropertyChanged(nameof(HasCallInProgress));
        OnPropertyChanged(nameof(ShowChooseCallButton));
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void UpdateVolunteer_Click(object _, RoutedEventArgs __)
    {
        try
        {
            s_bl.Volunteer.UpdateVolunteerDetails(Volunteer.Id, Volunteer);
            MessageBox.Show("Volunteer updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadVolunteer(Volunteer.Id);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void FinishTreatment_Click(object _, RoutedEventArgs __)
    {
        try
        {
            if (CallInProgress == null) return;
            s_bl.Call.MarkCallAsCompleted(Volunteer.Id, CallInProgress.AssignmentId);
            MessageBox.Show("Treatment finished", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadVolunteer(Volunteer.Id);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Finish Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CancelTreatment_Click(object _, RoutedEventArgs __)
    {
        try
        {
            if (CallInProgress != null)
            {
                s_bl.Call.CancelCallAssignment(Volunteer.Id, CallInProgress.CallId);
                MessageBox.Show("Treatment cancelled", "Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadVolunteer(Volunteer.Id);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Cancel Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ChooseCall_Click(object _, RoutedEventArgs __)
    {
        new ChooseCallForTreatmentWindow(Volunteer.Id).Show();
    }

    private void btnShowVolunteerCallsHistory_Click(object sender, RoutedEventArgs e)
    {
        new VolunteerCallHistoryWindow(Volunteer.Id).Show();
    }
 }
