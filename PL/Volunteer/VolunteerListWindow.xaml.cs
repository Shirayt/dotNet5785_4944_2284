﻿using System;
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
            if (sender is ComboBox comboBox && comboBox.SelectedItem is BO.VolunteerSortOption selectedOption)
            {
                VolunteerSortOption = selectedOption;
                VolunteerList = s_bl.Volunteer.GetVolunteersList(null, VolunteerSortOption);
            };

        }
        private void btnAddVolunteer_Click(object sender, RoutedEventArgs e)
        {
            var window = new VolunteerWindow();
            bool? result = window.ShowDialog();
        }


        private void dgVolunteerList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer is null)
                return;

            var window = new VolunteerWindow(SelectedVolunteer.Id);
            window.Show(); 
        }

    }
}
