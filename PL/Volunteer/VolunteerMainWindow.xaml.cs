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

    private int VolunteerId;

    public VolunteerMainWindow(int volunteerId)
    {
        InitializeComponent();
        VolunteerId = volunteerId;
        LoadVolunteer(VolunteerId);
    }

    private void LoadVolunteer(int id)
    {
        try
        {
            Volunteer = s_bl.Volunteer.GetVolunteerDetails(id);
            CallInProgress = Volunteer.callInProgress;
            NotifyAll();
        }
        catch (BO.BlDoesNotExistException ex)
        {
            MessageBox.Show($"Volunteer not found or missing.\n{ex.InnerException?.Message}", "Volunteer Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading volunteer data:\n{ex.Message}", "General Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void NotifyAll()
    {
        OnPropertyChanged(nameof(Volunteer));
        OnPropertyChanged(nameof(CallInProgress));
        OnPropertyChanged(nameof(HasCallInProgress));
        OnPropertyChanged(nameof(ShowChooseCallButton));
    }
    private void volunteerObserver() => LoadVolunteer(VolunteerId);

    private void VolunteerMainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Volunteer.AddObserver(VolunteerId, volunteerObserver);
        volunteerObserver();

    }
    private void VolunteerMainWindow_Closed(object? sender, EventArgs e)
    {
        if (Volunteer != null)
            s_bl.Volunteer.RemoveObserver(VolunteerId, volunteerObserver);
    }

    private void UpdateVolunteer_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Volunteer.UpdateVolunteerDetails(Volunteer.Id, Volunteer);
            MessageBox.Show("Volunteer updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void FinishTreatment_Click(object sender, RoutedEventArgs e)
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

    private void CancelTreatment_Click(object sender, RoutedEventArgs e)
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

    private void ChooseCall_Click(object sender, RoutedEventArgs e)
    {
        new ChooseCallForTreatmentWindow(Volunteer.Id).Show();
        this.Close();///לתקן אחרי טאג 7
    }

    private void btnShowVolunteerCallsHistory_Click(object sender, RoutedEventArgs e)
    {
        new VolunteerCallHistoryWindow(Volunteer.Id).Show();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
    }

}
