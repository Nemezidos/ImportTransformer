using ImportTransformer.Controller;
using ImportTransformer.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;

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

            string dir = Directory.GetCurrentDirectory();

            if (!File.Exists(dir + @"\Headers.xml"))
            {
                var tempHeaders = new MsgHeaderData("seller", "subject", "packing", "control", "receiver", "customreceiver", "hub");

                XmlSerializer serializer = new XmlSerializer(typeof(MsgHeaderData));

                XmlWriterSettings settings = new XmlWriterSettings() { OmitXmlDeclaration = false, Indent = true, Encoding = new UTF8Encoding(false) };

                using (Stream writer = new FileStream(dir + @"\Headers.xml", FileMode.OpenOrCreate))
                using (var wr = XmlWriter.Create(writer, settings))
                {
                    var a = XmlWriter.Create(writer, settings);
                    serializer.Serialize(a, tempHeaders);
                }
            }

            if (File.Exists(dir + @"\Headers.xml"))
            {
                PathToHeaderFile.Text = $@"{dir}\Headers.xml";
            }
        }

        #region GetFiles

        private void GetHeaderFile(object sender, RoutedEventArgs e)
        {
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
                Filter = "Worksheets (*.xlsx)|*.xlsx|Tables (*.csv)|*.csv|All Files (*.*)|*.*"
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
                PathToTracelinkFile.Text = (fileInfo.FullName);
            }
        }

        private void CreateHeaderFile(object sender, RoutedEventArgs e)
        {
            string dir = Directory.GetCurrentDirectory();

            if (!File.Exists(dir + @"\NewHeaders.xml"))
            {
                var tempHeaders = new MsgHeaderData("seller", "subject", "packing", "control", "receiver", "customreceiver", "hub");

                XmlSerializer serializer = new XmlSerializer(typeof(MsgHeaderData));

                XmlWriterSettings settings = new XmlWriterSettings() { OmitXmlDeclaration = false, Indent = true, Encoding = new UTF8Encoding(false) };

                using (Stream writer = new FileStream(dir + @"\NewHeaders.xml", FileMode.OpenOrCreate))
                using (var wr = XmlWriter.Create(writer, settings))
                {
                    var a = XmlWriter.Create(writer, settings);
                    serializer.Serialize(a, tempHeaders);
                }
            }
        }

        #endregion

        private void ShowInstruction(object sender, RoutedEventArgs e)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "ImportTransformer.Instruction.txt";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                MessageBox.Show(result, "Instruction", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private (bool, FilePathes) InputCheck()
        {
            bool result = true;
            var temp = new FilePathes();

            if (string.IsNullOrEmpty(PathToHeaderFile.Text.Trim(' ', '"')))
            {
                if (!File.Exists(PathToHeaderFile.Text.Trim(' ', '"')))
                {
                    PathToHeaderFile.Background = new SolidColorBrush(Colors.LightPink);
                    result = false;
                }
                else
                {
                    temp.Headers = PathToHeaderFile.Text.Trim(' ', '"');
                }
            }

            if (string.IsNullOrEmpty(PathToSantensFile.Text.Trim(' ', '"')))
            {
                if (!File.Exists(PathToSantensFile.Text.Trim(' ', '"')))
                {
                    PathToSantensFile.Background = new SolidColorBrush(Colors.LightPink);
                    result = false;
                }
                else
                {
                    temp.Santens = PathToSantensFile.Text.Trim(' ', '"');
                }
            }

            if (string.IsNullOrEmpty(PathToTracelinkFile.Text.Trim(' ', '"')))
            {
                if (!File.Exists(PathToTracelinkFile.Text.Trim(' ', '"')))
                {
                    PathToTracelinkFile.Background = new SolidColorBrush(Colors.LightPink);
                    result = false;
                }
                else
                {
                    temp.Tracelink = PathToTracelinkFile.Text.Trim(' ', '"');
                }
            }

            if (string.IsNullOrEmpty(PathToSupportFile.Text.Trim(' ', '"')))
            {
                if (!File.Exists(PathToSupportFile.Text.Trim(' ', '"')))
                {
                    PathToSupportFile.Background = new SolidColorBrush(Colors.LightPink);
                    result = false;
                }
                else
                {
                    temp.Support = PathToSupportFile.Text.Trim(' ', '"');
                }
            }

            return (result, temp);
        }

        private void MakeMessages(object sender, RoutedEventArgs e)
        {
            try
            {
                var tempCheck = InputCheck();
                if (tempCheck.Item1)
                {
                    Stopwatch SW = new Stopwatch();
                    SW.Start();

                    var pathes = tempCheck.Item2;
                    pathes.Results = Directory.GetCurrentDirectory();

                    DateTime timestamp = DateTime.Now;

                    List<SantensReport> ssccReport = Input.InputReport(pathes.Santens, 0);
                    List<SantensReport> sgtinReport = Input.InputReport(pathes.Santens, 1);

                    List<CryptoCode> filteredCodes;
                    {
                        List<CryptoCode> codes = Input.InputCsv(pathes.Tracelink);
                        filteredCodes = Transformator.Filter(codes, sgtinReport);
                    }

                    MsgHeaderData header;
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(MsgHeaderData));

                        using (Stream reader = new FileStream(pathes.Headers, FileMode.Open))
                        {
                            header = (MsgHeaderData)serializer.Deserialize(reader);
                        }
                    }

                    var supportData = Input.InputSupport(pathes.Support);

                    if (FirstPart.IsChecked.Value)
                    {
                        Output.CreateUtilisationReport(filteredCodes, pathes.Results, timestamp);
                        Output.Create300Message(filteredCodes, header, pathes.Results, timestamp);
                        Output.Create321Message(filteredCodes, header, supportData, pathes.Results, timestamp);
                    }

                    if (SecondPart.IsChecked.Value)
                    {
                        Output.Create915Message(sgtinReport, supportData.Gtin, header, pathes.Results, timestamp.AddMinutes(10));
                        Output.Create915Message(ssccReport, supportData.Gtin, header, pathes.Results, timestamp.AddMinutes(20));
                        
                        Output.Create331Message(ssccReport, header, supportData, pathes.Results, timestamp.AddHours(1));
                        Output.Create336Message(ssccReport, header, supportData, pathes.Results, timestamp.AddHours(2));
                        
                        if (new DirectoryInfo(@$"{pathes.Results}\forUpload").Exists)
                            Transformator.Split(@$"{pathes.Results}\forUpload");
                    }

                    SW.Stop();

                    Logging.LogTimer($"Скрипт исполнен за {Convert.ToString(SW.Elapsed.TotalMilliseconds)} миллисекунд(ы)", pathes.Results);
                }
                else
                {
                    MessageBox.Show("Заполнено не всё!\nДальнейшая работа невозможна.");
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorLog(ex);
                MessageBox.Show(ex.ToString(),
                                "Exception",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

        }
    }

    

}
