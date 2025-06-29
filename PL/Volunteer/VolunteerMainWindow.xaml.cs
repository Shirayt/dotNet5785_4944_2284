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

    public BO.Volunteer Volunteer { get; set; } = new();
    public CallInProgress? CallInProgress { get; set; }

    public bool HasCallInProgress => CallInProgress != null;
    public bool ShowChooseCallButton => CallInProgress == null;
        //&& Volunteer.IsActive;

    public VolunteerMainWindow(int volunteerId)
    {
        InitializeComponent();
        DataContext = this;
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
            if (CallInProgress == null) return;
            //s_bl.Call.CancelCallAssignment(Volunteer.Id, CallInProgress.Id);
            MessageBox.Show("Treatment cancelled", "Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadVolunteer(Volunteer.Id);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Cancel Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ChooseCall_Click(object _, RoutedEventArgs __)
    {
        //new ChooseCallWindow(Volunteer.Id).ShowDialog();
        LoadVolunteer(Volunteer.Id);
    }

    private void Window_Loaded(object _, RoutedEventArgs __)
    {
        // nothing to do, already loaded
    }

    private void Window_Closing(object? _, CancelEventArgs __)
    {
        // optional: cleanup
    }
}