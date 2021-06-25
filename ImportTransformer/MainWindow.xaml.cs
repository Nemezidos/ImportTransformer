using ImportTransformer.Controller;
using ImportTransformer.Model;
using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using NLog;

namespace ImportTransformer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        public MainWindow()
        {
            InitializeComponent();

            PathToHeaderFile.Text = Core.ExistHeaders();
        }

        #region GetFiles

        private void GetHeaderFile(object sender, RoutedEventArgs e)
        {
            if (false)
            {

            }
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),// Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                DefaultExt = ".xml",
                Filter = "XML (*.xml)|*.xml|JSON (*.json)|*.json|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var fileInfo = new FileInfo(openFileDialog.FileName);
                PathToHeaderFile.Text = (fileInfo.FullName);
            }
        }

        private void GetSantensFile(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),// Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                DefaultExt = ".xlsx",
                Filter = "Worksheets (*.xlsx)|*.xlsx|Tables (*.csv)|*.csv|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var fileInfo = new FileInfo(openFileDialog.FileName);
                PathToSantensFile.Text = (fileInfo.FullName);
            }
        }

        private void GetTracelinkFile(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),// Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                DefaultExt = ".csv",
                Filter = "Tables (*.csv)|*.csv|Worksheets (*.xlsx)|*.xlsx|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var fileInfo = new FileInfo(openFileDialog.FileName);
                PathToTracelinkFile.Text = (fileInfo.FullName);
            }
        }

        private void GetSupportFile(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),// Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                DefaultExt = ".xlsx",
                Filter = "Worksheets (*.xlsx)|*.xlsx|Tables (*.csv)|*.csv|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var fileInfo = new FileInfo(openFileDialog.FileName);
                PathToSupportFile.Text = (fileInfo.FullName);
            }
        }

        private void CreateHeaderFile(object sender, RoutedEventArgs e)
        {
            Core.CreateHeader(true, out var _);
        }

        #endregion

        private void ShowInstruction(object sender, RoutedEventArgs e)
        {
            var assembly = Assembly.GetExecutingAssembly();
            const string resourceName = "ImportTransformer.Instruction.txt";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            var result = reader.ReadToEnd();
            MessageBox.Show(result, "Instruction", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool InputCheck(out FilePaths outPaths)
        {
            var result = true;
            outPaths = new FilePaths();

            var temp = PathToHeaderFile.Text.Trim(' ', '"');
            if (!string.IsNullOrEmpty(temp) && File.Exists(temp))
                outPaths.Headers = temp;
            else
            {
                PathToHeaderFile.Background = new SolidColorBrush(Colors.LightPink);
                result = false;
            }

            temp = PathToSantensFile.Text.Trim(' ', '"');
            if (!string.IsNullOrEmpty(temp) && File.Exists(temp))
                outPaths.Santens = temp;
            else
            {
                PathToSantensFile.Background = new SolidColorBrush(Colors.LightPink);
                result = false;
            }

            temp = PathToTracelinkFile.Text.Trim(' ', '"');
            if (!string.IsNullOrEmpty(temp) && File.Exists(temp))
                outPaths.Tracelink = temp;
            else
            {
                PathToTracelinkFile.Background = new SolidColorBrush(Colors.LightPink);
                result = false;
            }

            temp = PathToSupportFile.Text.Trim(' ', '"');
            if (!string.IsNullOrEmpty(temp) && File.Exists(temp))
                outPaths.Support = temp;
            else
            {
                PathToSupportFile.Background = new SolidColorBrush(Colors.LightPink);
                result = false;
            }

            return result;
        }

        private void MakeMessages(object sender, RoutedEventArgs e)
        {
            var needs = new PartsNeeded { 
                FirstPart = FirstPart.IsChecked != null && FirstPart.IsChecked.Value, 
                SecondPart = SecondPart.IsChecked != null && SecondPart.IsChecked.Value 
            };

            try
            {
                if (InputCheck(out var paths))
                    Core.DoMessages(paths, needs);
                else
                    MessageBox.Show("Заполнено не всё!\nДальнейшая работа невозможна.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Возникла ошибка: {ex.Message} \n{ex.StackTrace}");
                MessageBox.Show(ex.Message,
                                "Exception",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

        }
    }

    

}
