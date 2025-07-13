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
    public partial class VolunteerWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public BO.Volunteer CurrentVolunteer
        {
            get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register(nameof(CurrentVolunteer), typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

        private int VolunteerId { get; set; }

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
            ButtonText = volunteerId == 0 ? "Add" : "Update";

            if (volunteerId == 0)
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
            else
            {
                try
                {
                    var volunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);
                    if (volunteer == null)
                    {
                        MessageBox.Show("Volunteer not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        Close();
                        return;
                    }
                    CurrentVolunteer = volunteer;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading volunteer details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
        }

        private async void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonText == "Add")
                {
                    await s_bl.Volunteer.AddVolunteer(CurrentVolunteer);
                    MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    await s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer.Id, CurrentVolunteer);
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
