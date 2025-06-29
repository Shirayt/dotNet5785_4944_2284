using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL.Call
{
    public partial class CallListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.CallInList? SelectedCall { get; set; }

        public IEnumerable<BO.CallInList> CallList
        {
            get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
            set { SetValue(CallListProperty, value); }
        }

        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallListWindow), new PropertyMetadata(null));

        public BO.CallInListFields CallSortOption { get; set; } = BO.CallInListFields.None;

        public CallListWindow()
        {
            InitializeComponent();
        }

        private void QueryCallList()
        {
            CallList = s_bl.Call.GetCallsList(null, null, CallSortOption);
        }

        private void callListObserver()
        {
            QueryCallList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(callListObserver);
            QueryCallList();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(callListObserver);
        }

        //////////////////////
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is BO.CallInListFields selectedOption)
            {
                CallSortOption = selectedOption;
                QueryCallList();
            }
        }

        private void DeleteCall_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BO.CallInList call)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete call (ID: {call.Id})?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Call.DeleteCall(call.Id);
                        MessageBox.Show("Call deleted successfully!", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to delete call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void btnAddCall_Click(object sender, RoutedEventArgs e)
        {
            var window = new CallWindow();
            window.Show();
        }

        private void dgCallList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall is null)
                return;

            var window = new CallWindow(SelectedCall.Id);
            window.Show();
        }
    }
}
