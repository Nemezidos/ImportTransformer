using ImportTransformer.Controller;
using ImportTransformer.Model;
using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media;

namespace ImportTransformer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),// Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                DefaultExt = ".xml",
                Filter = "XML (*.xml)|*.xml|JSON (*.json)|*.json|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                PathToHeaderFile.Text = (fileInfo.FullName);
            }
        }

        private void GetSantensFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),// Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                DefaultExt = ".xlsx",
                Filter = "Worksheets (*.xlsx)|*.xlsx|Tables (*.csv)|*.csv|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                PathToSantensFile.Text = (fileInfo.FullName);
            }
        }

        private void GetTracelinkFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),// Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                DefaultExt = ".csv",
                Filter = "Tables (*.csv)|*.csv|Worksheets (*.xlsx)|*.xlsx|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                PathToTracelinkFile.Text = (fileInfo.FullName);
            }
        }

        private void GetSupportFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory(),// Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                DefaultExt = ".xlsx",
                Filter = "Worksheets (*.xlsx)|*.xlsx|Tables (*.csv)|*.csv|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(openFileDialog.FileName);
                PathToSupportFile.Text = (fileInfo.FullName);
            }
        }

        private void CreateHeaderFile(object sender, RoutedEventArgs e)
        {
            Core.CreateHeader(true, out string _);
        }

        #endregion

        private void ShowInstruction(object sender, RoutedEventArgs e)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "ImportTransformer.Instruction.txt";

            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using StreamReader reader = new StreamReader(stream);
            string result = reader.ReadToEnd();
            MessageBox.Show(result, "Instruction", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool InputCheck(out FilePaths outPaths)
        {
            bool result = true;
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
                FirstPart = FirstPart.IsChecked.Value, 
                SecondPart = SecondPart.IsChecked.Value 
            };

            try
            {
                if (InputCheck(out FilePaths pathes))
                    Core.DoMessages(pathes, needs);
                else
                    MessageBox.Show("Заполнено не всё!\nДальнейшая работа невозможна.");
            }
            catch (Exception ex)
            {
                Output.ErrorLog(ex);
                MessageBox.Show(ex.ToString(),
                                "Exception",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

        }
    }

    

}
