using System;
using System.Collections.Generic;
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

namespace FirstFloor.ModernUI.App.Pages
{
    /// <summary>
    /// Interaction logic for LayoutWireframe.xaml
    /// </summary>
    public partial class VideoPlayerLayoutWireframe : UserControl
    {
        public VideoPlayerLayoutWireframe()
        {
            InitializeComponent();
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            //videoPlayerWebView.Navigate("http://localhost:8080/Shabdo-Khoj-Extension-for-VideoJs/demo.html");
            //videoPlayerWebView.Navigate("http://localhost:8080/shodhkhojPlayer/demo.html");
        }

        private void selectVideoFile_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = "*.*";
            dlg.Filter = "All Files (*.*)|*.*";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                videoFilePathTB.Text = filename;
                //MessageBox.Show(filename);
                //string returnMsg = DbHelper.AddFileInIndexQueue(filename, filename);
                //if (returnMsg != null)
                //    MessageBox.Show(returnMsg);
            }
        }

        private void selectTwtFile_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = "*.*";
            dlg.Filter = "All Files (*.*)|*.*";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                twtFilePathTB.Text = filename;
                //MessageBox.Show(filename);
                //string returnMsg = DbHelper.AddFileInIndexQueue(filename, filename);
                //if (returnMsg != null)
                //    MessageBox.Show(returnMsg);
            }
        }



        private void playSelectedVideo_Click(object sender, RoutedEventArgs e)
        {

            string perlExe = "perl generateFilePlayer.pl ";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

            startInfo.FileName = "cmd.exe";

            //string perlCommandForFileNameChange = " -p -e \"s/shobdokhojVideoFileName/video File/g\" demo.html > " ;
            string inputVideoFilePath = System.IO.Path.GetFileName(videoFilePathTB.Text);
            string inputVideoFileName = System.IO.Path.GetFileNameWithoutExtension(inputVideoFilePath);
            string inputTwtFileName = System.IO.Path.GetFileName(twtFilePathTB.Text);
            string videoPlayerServerpath = videoPlayerServerPathTB.Text;
            string inputVideoFileType = "video/mp4";

            startInfo.Arguments = "/C " + perlExe + " " + videoPlayerServerpath + "\\ " + inputVideoFilePath + " " + inputVideoFileName + " " + inputVideoFileType + " " + inputTwtFileName;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            string outputVideoFile = System.IO.Path.Combine(videoPlayerServerpath, inputVideoFilePath);
            string outputTwtFile = System.IO.Path.Combine(videoPlayerServerpath, inputTwtFileName);

            if (!System.IO.File.Exists(outputVideoFile))
                System.IO.File.Copy(videoFilePathTB.Text, outputVideoFile);

            if (!System.IO.File.Exists(outputTwtFile))
                System.IO.File.Copy(twtFilePathTB.Text, outputTwtFile);

            string playerUrl = "http://localhost:8080/shabdokhojPlayer/" + inputVideoFileName + ".html";
            MessageBox.Show(playerUrl);
            browser.Address = playerUrl;
            browser.Load(playerUrl);
        }

        private void selectVideoPlayerServerPath_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = "*.*";
            dlg.Filter = "All Files (*.*)|*.*";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                videoPlayerServerPathTB.Text = System.IO.Path.GetDirectoryName(filename);
                //MessageBox.Show(filename);
                //string returnMsg = DbHelper.AddFileInIndexQueue(filename, filename);
                //if (returnMsg != null)
                //    MessageBox.Show(returnMsg);
            }
        }
    }
}
