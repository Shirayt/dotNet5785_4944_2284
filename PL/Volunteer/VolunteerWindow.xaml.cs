using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerWindow.xaml
    /// </summary>
    public partial class VolunteerWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public BO.Volunteer CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

        private int VolunteerId { get; set; } = 0;

        private string _buttonText = "Add";
        public string ButtonText
        {
            get => _buttonText;
            set
            {
                if (_buttonText != value)
                {
                    _buttonText = value;
                    OnPropertyChanged();
                }
            }
        }
        public VolunteerWindow(int volunteerId = 0)
        {
            InitializeComponent();

            VolunteerId = volunteerId;
            ButtonText = VolunteerId == 0 ? "Add" : "Update";

            try
            {
                if (VolunteerId != 0)
                {
                    var volunteer = s_bl.Volunteer.GetVolunteerDetails(VolunteerId);
                    if (volunteer == null)
                    {
                        MessageBox.Show("Volunteer not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        Close();
                        return;
                    }

                    CurrentVolunteer = volunteer;
                }
                else
                {
                    CurrentVolunteer = new BO.Volunteer()
                    {
                        Id = 0,
                        FullName = string.Empty,
                        PhoneNumber = string.Empty,
                        Email = string.Empty,
                        Password = null,
                        CurrentFullAddress = null,
                        Latitude = null,
                        Longitude = null,
                        Role = BO.Role.Volunteer,
                        IsActive = false,
                        MaxDistanceForCall = null,
                        DistanceType = BO.DistanceType.Air,
                        AmountOfCompletedCalls = 0,
                        AmountOfSelfCancelledCalls = 0,
                        AmountOfExpiredCalls = 0,
                        callInProgress = null
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }
   
        private void volunteerObserver()
        {
            int id = CurrentVolunteer!.Id;
            CurrentVolunteer = null;
            CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
        }
        private void VolunteerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer != null && CurrentVolunteer.Id != 0)
            {
                s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, volunteerObserver);
                volunteerObserver();
            }
        }
        private void VolunteerWindow_Closed(object? sender, EventArgs e)
        {
            if (CurrentVolunteer != null && CurrentVolunteer.Id != 0)
            {
                s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, volunteerObserver);
            }
        }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonText == "Add")
                {
                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer);
                    MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer.Id, CurrentVolunteer);
                    MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
