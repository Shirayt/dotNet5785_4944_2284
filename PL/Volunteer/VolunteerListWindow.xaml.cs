using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for VolunteerListWindow.xaml
    /// </summary>
    public partial class VolunteerListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public BO.VolunteerInList? SelectedVolunteer { get; set; }

        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }

        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

        public BO.VolunteerSortOption VolunteerSortOption { get; set; } = BO.VolunteerSortOption.None;

        public VolunteerListWindow()
        {
            InitializeComponent();
        }

        private void QueryVolunteerList()
        {
            VolunteerList = s_bl.Volunteer.GetVolunteersList(null, VolunteerSortOption);
        }

        private void volunteerListObserver()
        {
            QueryVolunteerList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.AddObserver(volunteerListObserver);
            QueryVolunteerList();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(volunteerListObserver);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
         VolunteerList = (VolunteerSortOption == BO.VolunteerSortOption.None)
                               ? s_bl.Volunteer.GetVolunteersList(null, null)!
                               : s_bl.Volunteer.GetVolunteersList(null, VolunteerSortOption)!;
        }

        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e) { }

        private void DeleteVolunteer_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedVolunteer != null)
            {
                var result = MessageBox.Show(
                   $"Are you sure you want to delete volunteer '{SelectedVolunteer.FullName}' (ID: {SelectedVolunteer.Id})?",
                   "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Volunteer.DeleteVolunteer(SelectedVolunteer.Id);
                        MessageBox.Show("Volunteer deleted successfully!", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to delete volunteer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void btnAddVolunteer_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerWindow().Show();
        }


        private void dgVolunteerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
            {
                new VolunteerWindow(SelectedVolunteer.Id).Show();
            }
        }

    }
}
