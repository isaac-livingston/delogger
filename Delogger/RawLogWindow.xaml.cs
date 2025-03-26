using ICSharpCode.AvalonEdit.Highlighting;
using Newtonsoft.Json.Linq;
using System.Windows;

namespace Delogger;

public partial class RawLogWindow : Window
{
    private readonly string _rawText;
    private bool _isFormatted = false;

    public RawLogWindow(string rawLog)
    {
        InitializeComponent();
        _rawText = rawLog;
        JsonEditor.Text = _rawText;
    }

    private void FormatJsonMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (!_isFormatted)
        {
            try
            {
                // Parse and format JSON with indentation.
                JToken parsedJson = JToken.Parse(_rawText);
                string formatted = parsedJson.ToString(Formatting.Indented);
                JsonEditor.Text = formatted;
                JsonEditor.SyntaxHighlighting = HighlightingManager.Instance.HighlightingDefinitions.FirstOrDefault(h => h.Name == "JavaScript");
                _isFormatted = true;
                FormatJsonMenuItem.Header = "Show Raw";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Content is not valid JSON:\n" + ex.Message,
                                "Formatting Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            // Revert back to the raw text.
            JsonEditor.Text = _rawText;
            JsonEditor.SyntaxHighlighting = null;
            _isFormatted = false;
            FormatJsonMenuItem.Header = "Format JSON";
        }
    }
}
