using Delogger.Services;
using Microsoft.Win32;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Delogger;

public partial class MainWindow : Window
{
    private readonly DataService _dataService = new();

    // Initialize the log entries list properly.
    private List<LogEntry> _logEntries = [];

    public MainWindow()
    {
        InitializeComponent();
    }

    private void OpenLogFile_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dlg = new()
        {
            DefaultExt = ".json",
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*"
        };

        bool? result = dlg.ShowDialog();
        if (result == true)
        {
            LoadLogFile(dlg.FileName);
            Title = $"Delogger - {dlg.FileName}";
        }
    }

    private void LoadLogFile(string filePath)
    {
        _logEntries = _dataService.ReadLogFile(filePath);

        LogListView.ItemsSource = _logEntries;

        // Set up filtering and sorting on the ListView's CollectionView.
        CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(LogListView.ItemsSource);
        view.Filter = LogFilter;
        view.SortDescriptions.Clear();
        view.SortDescriptions.Add(new SortDescription("Timestamp", ListSortDirection.Descending));
    }

    private void LogListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (LogListView.SelectedItem is LogEntry selectedLog)
        {
            // Convert the Properties dictionary into a list of key/value pairs for display.
            var details = selectedLog.Properties
                .Select(p => new { p.Key, Value = p.Value?.ToString() })
                .ToList();

            // Bind the details to the DataGrid.
            DetailsDataGrid.ItemsSource = details;

            // Update the Exception panel with the Exception text.
            ExceptionTextBox.Text = selectedLog.Exception ?? string.Empty;
        }
    }

    private bool LogFilter(object item)
    {
        if (item is not LogEntry log)
            return false;

        // Apply log level filter from the ComboBox.
        if (LogLevelComboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            var levelFilter = selectedItem.Content.ToString();
            if (!string.Equals(levelFilter, "All", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(log.Level, levelFilter, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        // Apply free text filter.
        if (!string.IsNullOrWhiteSpace(FilterTextBox.Text))
        {
            var textFilter = FilterTextBox.Text.ToLower();
            var matchesText = (log.Level?.ToLowerInvariant().Contains(textFilter, StringComparison.InvariantCultureIgnoreCase) ?? false) ||
                              (log.MessageTemplate?.ToLower().Contains(textFilter, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                               log.Timestamp.ToString("G").Contains(textFilter, StringComparison.CurrentCultureIgnoreCase);
            if (!matchesText)
                return false;
        }

        return true;
    }

    private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        CollectionViewSource.GetDefaultView(LogListView?.ItemsSource)?.Refresh();
    }

    private void LogLevelComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        CollectionViewSource.GetDefaultView(LogListView?.ItemsSource)?.Refresh();
    }

    private void CopyCellText_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem menuItem &&
            menuItem.Parent is ContextMenu contextMenu &&
            contextMenu.PlacementTarget is DataGridCell cell)
        {
            TextBlock tb = FindVisualChild<TextBlock>(cell)!;
            if (tb != null && !string.IsNullOrEmpty(tb.Text))
            {
                Clipboard.SetText(tb.Text);
            }
            else
            {
                Clipboard.SetText(cell.Content?.ToString() ?? string.Empty);
            }
        }
    }

    private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
    {
        if (parent == null) return null;
        int count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T correctlyTyped)
            {
                return correctlyTyped;
            }
            var childOfChild = FindVisualChild<T>(child);
            if (childOfChild != null)
            {
                return childOfChild;
            }
        }
        return null;
    }

    private void ViewRawLogButton_Click(object sender, RoutedEventArgs e)
    {
        if (LogListView.SelectedItem is LogEntry selectedLog)
        {
            // Create and show the modal window with the raw log text.
            RawLogWindow rawLogWindow = new RawLogWindow(selectedLog.RawLogEntry);
            rawLogWindow.Owner = this; // Set the owner window, if desired.
            rawLogWindow.ShowDialog();
        }
    }
}
