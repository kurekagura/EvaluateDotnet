using Microsoft.Web.WebView2.Core;
using System;
using System.Windows;

namespace DashJS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
            await webView.EnsureCoreWebView2Async(null);
            //https://learn.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2.setvirtualhostnametofoldermapping?view=webview2-dotnet-1.0.1938.49&WT.mc_id=WD-MVP-5001077
            webView.CoreWebView2.SetVirtualHostNameToFolderMapping("appassets", "assets", CoreWebView2HostResourceAccessKind.DenyCors);
            webView.Source = new Uri("https://appassets/index.html");
        }

        private void WebView_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.IsSuccess);

            // リクエストのフィルタを作成し、HTTP-GETリクエストをフックする
            webView.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            webView.CoreWebView2.WebResourceRequested += CoreWebView2_WebResourceRequested;

            //webView.CoreWebView2.OpenDevToolsWindow();
        }

        private void CoreWebView2_WebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Request.Uri);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
