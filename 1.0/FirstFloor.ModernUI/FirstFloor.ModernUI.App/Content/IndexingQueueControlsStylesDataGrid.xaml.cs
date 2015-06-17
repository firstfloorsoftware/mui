using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
using Technicise.ShabdoKhoj.DbInteract;

namespace FirstFloor.ModernUI.App.Content
{
    // taken from MSDN (http://msdn.microsoft.com/en-us/library/system.windows.controls.datagrid.aspx)
    public enum IndexingStatus { Queued, OpticalCharacterRecognized, SpeechToTexted, MetadataExtracted, FaceRecognized, ObjectExtracted, Indexed };
    public class FilesForIndexing
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileId { get; set; }
        public DateTime TimeAdded { get; set; }
        public IndexingStatus Status { get; set; }
    }


    /// <summary>
    /// Interaction logic for ControlsStylesDataGrid.xaml
    /// </summary>
    public partial class IndexingQueueControlsStylesDataGrid : UserControl
    {
        public IndexingQueueControlsStylesDataGrid()
        {
            InitializeComponent();

            ObservableCollection<FilesForIndexing> custdata = GetData();

            //Bind the DataGrid to the FilesForIndexing data
            DG1.DataContext = custdata;
        }

        private ObservableCollection<FilesForIndexing> GetData()
        {
            var filesForIndexings = new ObservableCollection<FilesForIndexing>();

            List<T_Indexing_Queue> files = DbHelper.GetFilesInIndexQueue();

            foreach (T_Indexing_Queue file in files)
            {
                FilesForIndexing fileFromDb = new FilesForIndexing();
                fileFromDb.FileId = file.file_id.ToString();
                fileFromDb.FileName = file.file_name;
                fileFromDb.FilePath = file.file_path;
                fileFromDb.TimeAdded = file.time_added ?? DateTime.Now;
                fileFromDb.Status = (IndexingStatus)file.index_status;
                filesForIndexings.Add(fileFromDb);
            }

            return filesForIndexings;
        }

        private void DG1_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var contextMenu = new ContextMenu();

            MenuItem ocrMenu = new MenuItem { Header = "Perform OCR Indexing" };
            ocrMenu.Tag = (sender as DataGrid).SelectedItem;
            ocrMenu.Click += new RoutedEventHandler(ocrMenu_Click);
            contextMenu.Items.Add(ocrMenu);

            contextMenu.Items.Add(new Separator());

            MenuItem sttMenu = new MenuItem { Header = "Perform Speech to Text Indexing" };
            sttMenu.Click += new RoutedEventHandler(sttMenu_Click);
            contextMenu.Items.Add(sttMenu);

            contextMenu.Items.Add(new Separator());

            MenuItem vmMenu = new MenuItem { Header = "Perform Video MetaData Indexing" };
            vmMenu.Click += vmMenu_Click;
            contextMenu.Items.Add(vmMenu);

            contextMenu.Items.Add(new Separator());

            MenuItem opencvObjMenu = new MenuItem { Header = "Perform Objects recognition Indexing" };
            opencvObjMenu.Click += opencvObjMenu_Click;
            contextMenu.Items.Add(opencvObjMenu);

            contextMenu.Items.Add(new Separator());

            MenuItem generateTwtMenu = new MenuItem { Header = "Generate TagsWithTimestamp File" };
            generateTwtMenu.Click += generateTwtMenu_Click;
            contextMenu.Items.Add(generateTwtMenu);

            contextMenu.Items.Add(new Separator());

            MenuItem playTwtMenu = new MenuItem { Header = "Play Video with TagsWithTimestamp File" };
            playTwtMenu.Click += playTwtMenu_Click;
            contextMenu.Items.Add(playTwtMenu);
            contextMenu.IsOpen = true;
        }

        void displayAlert()
        {
            MessageBox.Show(DateTime.UtcNow.ToLocalTime().ToLongTimeString());
        }
        void displayAlert(string text)
        {
            MessageBox.Show(DateTime.UtcNow.ToLocalTime().ToLongTimeString() + " " + text);
        }
        void playTwtMenu_Click(object sender, RoutedEventArgs e)
        {
            //displayAlert();
            NavigationService navService = NavigationService .GetNavigationService(this);
            navService.Navigate(new System.Uri("/Pages/VideoPlayerLayoutWireframe.xaml")); //", UriKind.RelativeOrAbsolute)); 
        }

        void generateTwtMenu_Click(object sender, RoutedEventArgs e)
        {
            displayAlert();
        }

        void opencvObjMenu_Click(object sender, RoutedEventArgs e)
        {
            displayAlert();
        }

        void vmMenu_Click(object sender, RoutedEventArgs e)
        {
            displayAlert();
        }

        void sttMenu_Click(object sender, RoutedEventArgs e)
        {
            displayAlert();
        }

        void ocrMenu_Click(object sender, RoutedEventArgs e)
        {
            FilesForIndexing currentFile = (sender as MenuItem).Tag as FilesForIndexing;

            //displayAlert(currentFile.FileName);


            //create temp dir for the file id
            string tempDir = System.IO.Path.Combine("D://shabdokhoj//", currentFile.FileId);
            System.IO.Directory.CreateDirectory(tempDir);
            //create temp dir for storing images
            string tempDirFrame = System.IO.Path.Combine(tempDir, "frame");
            System.IO.Directory.CreateDirectory(tempDirFrame);
            //create temp dir for storing ocr
            string tempDirOcr = System.IO.Path.Combine(tempDir, "ocr");
            System.IO.Directory.CreateDirectory(tempDirOcr);

            GenerateFramesFromVideoFile(currentFile, tempDirFrame);

            ExtractOcrTexts(tempDirFrame, tempDirOcr);

            string outputTwtFileName = System.IO.Path.GetFileName(currentFile.FileName).Split('.')[0] + ".json";
            string outputTwtFilePath = System.IO.Path.Combine(tempDir, outputTwtFileName);
            GenerateTwtFromOcr(tempDirOcr, outputTwtFilePath);

        }

        private static void GenerateTwtFromOcr(string tempDirOcr, string outputTwtFileName)
        {
            //generate twt with json
            //perl  mergeOCR.pl "D:\shabdokhoj\bcaa2375-22ac-44ab-a859-da8c3c953bf4\ocr" ignor.txt "D:\ChandraPersonal\Mtech\shabdokhoj\ocr-text\mergeOCR\op.json"

            string perlExe = "perl mergeOCR.pl ";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

            startInfo.FileName = "cmd.exe";

            startInfo.Arguments = "/C " + perlExe + tempDirOcr + " ignor.txt " + outputTwtFileName;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        private static void ExtractOcrTexts(string tempDirFrame, string tempDirOcr)
        {
            foreach (string srcFile in System.IO.Directory.EnumerateFiles(tempDirFrame, "*.jpeg"))
            {
                //D:\ChandraPersonal\Mtech\shabdokhoj\ocr-text>"D:\Program Files (x86)\Tesseract-OCR\tesseract.exe" t.JPG out
                string tesseractExe = "tesseract.exe";
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

                startInfo.FileName = "cmd.exe";

                string outFileName = System.IO.Path.GetFileName(srcFile).Split('.')[0];
                string tempDirOcrFile = System.IO.Path.Combine(tempDirOcr, outFileName);

                startInfo.Arguments = "/C " + tesseractExe + " " + srcFile + " " + tempDirOcrFile;


                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
            }
        }

        private static void GenerateFramesFromVideoFile(FilesForIndexing currentFile, string tempDirFrame)
        {
            //generate frames using ffmpeg
            //D:\ChandraPersonal\Mtech\shabdokhoj\FFmpeg\ffmpeg -i lg.mp4 -r 1 -s 720x480 -f image2 frames/lg/foo-%03d.jpeg

            string ffmpegExe = "D:\\ChandraPersonal\\Mtech\\shabdokhoj\\FFmpeg\\ffmpeg";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

            startInfo.FileName = "cmd.exe";



            startInfo.Arguments = "/C " + ffmpegExe + " -i " + currentFile.FileName + " -r 1 -s 720x480 -f image2 " + tempDirFrame + "//foo-%03d.jpeg";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
    }
}
