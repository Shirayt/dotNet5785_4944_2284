﻿using System.Text;
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
using PL.Volunteer;
using PL.Call;
using System.Collections.ObjectModel;
using Helpers;

namespace PL;

/// <summary>
/// Interaction logic for ManagerMainWindow.xaml
/// </summary>

public partial class ManagerMainWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    private volatile bool _configObserverWorking = false; // stage 7
    private volatile bool _clockObserverWorking = false; // stage 7

    /// <summary>
    /// Clock update view method
    /// </summary>
    private void clockObserver() // stage 7
    {
        if (_clockObserverWorking)
            return;

        _clockObserverWorking = true;
        _ = Dispatcher.BeginInvoke(() =>
        {
            try
            {
                CurrentTime = s_bl.Admin.GetClock();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Clock observer error:\n{ex.Message}", "Observer Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _clockObserverWorking = false;
            }
        });
    }


    /// <summary>
    /// Config variables update view method
    /// </summary>
    private void configObserver() 
    {
        if (_configObserverWorking)
            return;

        _configObserverWorking = true;
        _ = Dispatcher.BeginInvoke(() =>
        {
            try
            {
                RiskRange = s_bl.Admin.GetRiskRange();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Config observer error:\n{ex.Message}", "Observer Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _configObserverWorking = false;
            }
        });
    }


    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }

    public static readonly DependencyProperty CurrentTimeProperty =
    DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(ManagerMainWindow), new PropertyMetadata(default(DateTime)));

    public TimeSpan RiskRange
    {
        get { return (TimeSpan)GetValue(RiskRangeProperty); }
        set { SetValue(RiskRangeProperty, value); }
    }

    public static readonly DependencyProperty RiskRangeProperty =
        DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(ManagerMainWindow), new PropertyMetadata(TimeSpan.Zero));

    public class CallStatusCount
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public ObservableCollection<CallStatusCount> CallQuantities
    {
        get { return (ObservableCollection<CallStatusCount>)GetValue(CallQuantitiesProperty); }
        set { SetValue(CallQuantitiesProperty, value); }
    }

    public static readonly DependencyProperty CallQuantitiesProperty =
        DependencyProperty.Register(nameof(CallQuantities), typeof(ObservableCollection<CallStatusCount>), typeof(ManagerMainWindow), new PropertyMetadata(new ObservableCollection<CallStatusCount>()));


    public static readonly DependencyProperty IntervalProperty =
    DependencyProperty.Register("Interval", typeof(int), typeof(ManagerMainWindow), new PropertyMetadata(1));

    public int Interval
    {
        get => (int)GetValue(IntervalProperty);
        set => SetValue(IntervalProperty, value);
    }

    public static readonly DependencyProperty IsSimulatorRunningProperty =
    DependencyProperty.Register("IsSimulatorRunning", typeof(bool), typeof(ManagerMainWindow), new PropertyMetadata(false));

    public bool IsSimulatorRunning
    {
        get => (bool)GetValue(IsSimulatorRunningProperty);
        set => SetValue(IsSimulatorRunningProperty, value);
    }


    public ManagerMainWindow()
    {
        InitializeComponent();
    }

    private void LoadCallQuantities()
    {
        var list = new ObservableCollection<CallStatusCount>();

        var counts = s_bl.Call.GetCallQuantitiesByStatus().ToList();
        var statuses = Enum.GetValues(typeof(BO.CallStatus)).Cast<BO.CallStatus>().ToList();

        for (int i = 0; i < statuses.Count; i++)
        {
            list.Add(new CallStatusCount
            {
                Status = statuses[i].ToString(),
                Count = counts[i]
            });
        }

        CallQuantities = list;
    }


    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (IsSimulatorRunning)
        {
            s_bl.Admin.StopSimulator();
            IsSimulatorRunning = false;
        }

        //initialize with real values
        CurrentTime = s_bl.Admin.GetClock();
        RiskRange = s_bl.Admin.GetRiskRange();

        //register to be observable
        s_bl.Admin.AddClockObserver(clockObserver);
        s_bl.Admin.AddClockObserver(configObserver);

        LoadCallQuantities();
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        if (IsSimulatorRunning)
        {
            s_bl.Admin.StopSimulator();
            IsSimulatorRunning = false;
        }

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

    private void UpdateRiskRange_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.SetRiskRange(RiskRange);
    }

    private void btnVolunteers_Click(object sender, RoutedEventArgs e)
    {
        new VolunteerListWindow().Show();
    }

    private void btnCalls_Click(object sender, RoutedEventArgs e)
    {
        new CallListWindow().Show();
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
                Mouse.OverrideCursor = Cursors.Wait;

                foreach (Window window in Application.Current.Windows)
                {
                    if (window != this)
                        window.Close();
                }

                s_bl.Admin.InitializeDB();
                MessageBox.Show("Database initialized successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Initialization failed:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("Database reset was successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            finally
            {
                Mouse.OverrideCursor = null; // return to regular cursor
            }
        }
    }

    private void ToggleSimulator_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!IsSimulatorRunning)
            {
                s_bl.Admin.StartSimulator(Interval);
                IsSimulatorRunning = true;
            }
            else
            {
                s_bl.Admin.StopSimulator();
                IsSimulatorRunning = false;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

}

