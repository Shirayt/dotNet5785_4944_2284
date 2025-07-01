using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PL.Call
{
    public partial class CallWindow : Window, INotifyPropertyChanged
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public BO.Call CurrentCall
        {
            get { return (BO.Call)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register(nameof(CurrentCall), typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));

        private int CallId { get; set; }


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

        private IEnumerable<BO.CallAssignInList> _assignments;
        public IEnumerable<BO.CallAssignInList> Assignments
        {
            get => _assignments;
            set
            {
                _assignments = value;
                OnPropertyChanged();
            }
        }


        public CallWindow(int callId = 0)
        {
            InitializeComponent();
            CallId = callId;
            ButtonText = callId == 0 ? "Add" : "Update";

            if (callId == 0)
            {
                CurrentCall = new BO.Call
                {
                    //Id = 0,
                    CallType = BO.CallType.Emergency,
                    Description = string.Empty,
                    FullAddress = string.Empty,
                    Latitude = null,
                    Longitude = null,
                    OpenTime = s_bl.Admin.GetClock(),
                    MaxEndTime = null,
                    Status = BO.CallStatus.Open,
                    CallAssignInList = new List<BO.CallAssignInList>()
                };
            }
            else
            {
                try
                {
                    var call = s_bl.Call.GetCallDetails(callId);
                    if (call == null)
                    {
                        MessageBox.Show("Call not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        Close();
                        return;
                    }
                    CurrentCall = call;
                    Assignments = s_bl.Call.GetAssignmentsByCallId(callId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading call details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
        }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ButtonText == "Add")
                {
                    s_bl.Call.AddCall(CurrentCall);
                    MessageBox.Show("Call added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    s_bl.Call.UpdateCallDetails(CurrentCall);
                    MessageBox.Show("Call updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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