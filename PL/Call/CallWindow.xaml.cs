using System;
using System.Collections.Generic;
using System.Windows;

namespace PL.Call
{
    public partial class CallWindow : Window
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
        public string ButtonText { get; set; }

        public CallWindow()
        {
            CallId = 0;
            ButtonText = "Add";
            InitializeComponent();
            CurrentCall = new BO.Call
            {
                Id = 0,
                CallType = BO.CallType.Emergency,
                Description = string.Empty,
                FullAddress = string.Empty,
                Latitude = null,
                Longitude = null,
                OpenTime = DateTime.Now,
                MaxEndTime = null,
                Status = BO.CallStatus.Open,
                CallAssignInList = new List<BO.CallAssignInList>()
            };
        }

        public CallWindow(int callId)
        {
            InitializeComponent();
            CallId = callId;
            ButtonText = "Update";

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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading call details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
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
    }
}
