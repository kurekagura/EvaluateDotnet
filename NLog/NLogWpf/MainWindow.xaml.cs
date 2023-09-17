using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Windows;

namespace NLogWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ILogger<MainWindow> _log;
        private readonly IConfiguration _config;

        public MainWindow(IConfiguration configuration, ILogger<MainWindow> log)
        {
            _config = configuration;
            _log = log;
            InitializeComponent();
            _log.LogTrace($"Exiting:{nameof(MainWindow)}");
        }

        protected override void OnContentRendered(EventArgs e)
        {
            _log.LogInformation($"Enterd:{nameof(OnContentRendered)}");
            base.OnContentRendered(e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _log.LogInformation($"Enterd:{nameof(Window_Loaded)}");
            try
            {
                var url = _config.GetSection("CustomKey1").Value ?? throw new Exception();
                _log.LogDebug("CustomKey1={}", url);
            }
            catch (Exception ex)
            {
                _log.LogCritical(ex, "In {}", nameof(OnContentRendered));
            }
            _log.LogTrace($"Exiting:{nameof(Window_Loaded)}");
        }
    }
}
