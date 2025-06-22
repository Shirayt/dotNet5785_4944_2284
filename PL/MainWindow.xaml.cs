using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BlApi;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        /// <summary>
        /// Clock update view method
        /// </summary>
        private void clockObserver()
        {
            CurrentTime = s_bl.Admin.GetClock();
        }

        /// <summary>
        /// Config variables update view method
        /// </summary>
        private void configObserver()
        {
            RiskRange = s_bl.Admin.GetRiskRange();
        }

        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        public static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow), new PropertyMetadata(default(DateTime)));

        //public DateTime Clock
        //{
        //    get { return (DateTime)GetValue(ClockProperty); }
        //    set { SetValue(ClockProperty, value); }
        //}

        //public static readonly DependencyProperty ClockProperty =
        //    DependencyProperty.Register("Clock", typeof(DateTime), typeof(MainWindow), new PropertyMetadata(string.Empty));

        public TimeSpan RiskRange
        {
            get { return (TimeSpan)GetValue(RiskRangeProperty); }
            set { SetValue(RiskRangeProperty, value); }
        }

        public static readonly DependencyProperty RiskRangeProperty =
            DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(MainWindow), new PropertyMetadata(TimeSpan.Zero));



        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //initialize with real values
            CurrentTime = s_bl.Admin.GetClock();
            RiskRange = s_bl.Admin.GetRiskRange();

            //register to be observable
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddClockObserver(configObserver);
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
        }

        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Minute);
        }
        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Hour);
        }
        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Day);
        }
        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Month);
        }
        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Year);
        }

        //private void UpdateClock_Click(object sender, RoutedEventArgs e)
        //{
        //        s_bl.Admin.SetClock(Clock);
        //}

        private void UpdateRiskRange_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetRiskRange(RiskRange);
        }

        private void btnCalls_Click(object sender, RoutedEventArgs e)
        {
            //new CallsListWindow().Show();
        }


        /// <summary>
        /// Treament of initialize DB btn
        /// </summary>
        private void btnInitializeDB_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to initialize the database?", "Initialize DB",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    Mouse.OverrideCursor = Cursors.Wait; // change cursor to sand clock

                    // close all windows despite the main window
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window != this)
                            window.Close();
                    }

                    s_bl.Admin.InitializeDB();
                }
                finally
                {
                    Mouse.OverrideCursor = null; // return to regular cursor
                }
            }
        }

        /// <summary>
        /// Treament of reset DB btn
        /// </summary>
        private void btnResetDB_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to reset the database?", "Reset DB",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    Mouse.OverrideCursor = Cursors.Wait; // change cursor to sand clock

                    // close all windows despite the main window
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window != this)
                            window.Close();
                    }

                    s_bl.Admin.ResetDB();
                }
                finally
                {
                    Mouse.OverrideCursor = null; // return to regular cursor
                }
            }
        }


    }
}