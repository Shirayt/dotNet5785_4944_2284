using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using BlApi;

namespace PL
{
    public partial class LoginWindow : Window, INotifyPropertyChanged
    {
        static readonly IBl s_bl = Factory.Get();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private string _userId = "";
        public string UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                OnPropertyChanged();
            }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }

        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {

            ErrorMessage = "";

            if (string.IsNullOrWhiteSpace(UserId))
            {
                ErrorMessage = "User ID is required.";
                return;
            }

            if (!int.TryParse(UserId, out int parsedId))
            {
                ErrorMessage = "Invalid ID format.";
                return;
            }

            try
            {
                // App.CurrentUserId = parsedId;
                var role = s_bl.Volunteer.LoginVolunteerToSystem(parsedId, Password);

                switch (role)
                {
                    case BO.Role.Volunteer:
                        new Volunteer.VolunteerMainWindow(parsedId).Show();
                        break;
                    case BO.Role.Manager:
                        var result = MessageBox.Show(
                            "Login as admin or volunteer?\nYes - Admin\nNo - Volunteer",
                            "Choose Role", MessageBoxButton.YesNoCancel);

                        if (result == MessageBoxResult.Yes)
                        {
                            new ManagerMainWindow().Show();
                        }
                        else if (result == MessageBoxResult.No)
                        {
                            new Volunteer.VolunteerMainWindow(parsedId).Show();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
