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
    public enum IndexingStatus { Queued, OpticalCharacterRecognized, SpeechToTexted, MetadataExtracted, FaceRecognized, ObjectExtracted,Indexed };
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

           List<T_Indexing_Queue> files =  DbHelper.GetFilesInIndexQueue();

            foreach(T_Indexing_Queue file in files)
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
    }
}
