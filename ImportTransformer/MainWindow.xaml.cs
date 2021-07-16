using ImportTransformer.Controller;
using ImportTransformer.Model;
using Microsoft.Win32;
using System;
using System.IO;
using System.Media;
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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static string OpenDirectory { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            PathToHeaderFile.Text = Core.ExistHeaders();
        }

        #region GetFiles

        private void GetHeaderFile(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),// Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                DefaultExt = ".xml",
                Filter = "XML (*.xml)|*.xml|JSON (*.json)|*.json|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var fileInfo = new FileInfo(openFileDialog.FileName);
                PathToHeaderFile.Text = fileInfo.FullName;
            }
        }

        private void GetSantensFile(object sender, RoutedEventArgs e)
        {
            OpenDirectory = string.IsNullOrEmpty(OpenDirectory) ? Directory.GetCurrentDirectory() : string.Empty;
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = OpenDirectory,// Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                DefaultExt = ".xlsx",
                Filter = "Worksheets (*.xlsx)|*.xlsx|Tables (*.csv)|*.csv|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var fileInfo = new FileInfo(openFileDialog.FileName);
                OpenDirectory = fileInfo.Directory?.FullName;
                PathToSantensFile.Text = fileInfo.FullName;
            }
        }

        private void GetTracelinkFile(object sender, RoutedEventArgs e)
        {
            OpenDirectory = string.IsNullOrEmpty(OpenDirectory) ? Directory.GetCurrentDirectory() : string.Empty;
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = OpenDirectory,// Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                DefaultExt = ".csv",
                Filter = "Tables (*.csv)|*.csv|Worksheets (*.xlsx)|*.xlsx|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var fileInfo = new FileInfo(openFileDialog.FileName);
                OpenDirectory = fileInfo.Directory?.FullName;
                PathToTracelinkFile.Text = fileInfo.FullName;
            }
        }

        private void GetSupportFile(object sender, RoutedEventArgs e)
        {
            OpenDirectory = string.IsNullOrEmpty(OpenDirectory) ? Directory.GetCurrentDirectory() : string.Empty;
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = OpenDirectory,// Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                DefaultExt = ".xlsx",
                Filter = "Worksheets (*.xlsx)|*.xlsx|Tables (*.csv)|*.csv|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var fileInfo = new FileInfo(openFileDialog.FileName);
                OpenDirectory = fileInfo.Directory?.FullName;
                PathToSupportFile.Text = fileInfo.FullName;
            }
        }

        private void CreateHeaderFile(object sender, RoutedEventArgs e)
        {
            Core.CreateHeader(true, out var path);
            MessageBox.Show($"Создан новый файл заголовков. \nПуть: {path}");
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
                {
                    Core.DoMessages(paths, needs);
                    SystemSounds.Hand.Play();
                    MessageBox.Show("Все отчеты сформированы", "It is done!", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else
                    MessageBox.Show("Заполнено не всё!\nДальнейшая работа невозможна.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, $"Возникла ошибка: {ex.Message} \n{ex.StackTrace}");
                MessageBox.Show(ex.Message,
                                "Exception",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

        }
    }

    

}
