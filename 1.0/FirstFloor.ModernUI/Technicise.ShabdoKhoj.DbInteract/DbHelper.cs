using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Technicise.ShabdoKhoj.DbInteract
{
    public static class DbHelper
    {
        public static string AddFileInIndexQueue(string filename, string filepath)
        {
            try
            {
                using (TechniciseShabdoKhojEntities entities = new TechniciseShabdoKhojEntities())
                {
                    T_Indexing_Queue fileToBeIndexed = new T_Indexing_Queue();
                    if (string.IsNullOrEmpty(filename) == false)
                    {
                        fileToBeIndexed.file_name = filename;
                        fileToBeIndexed.file_path = filepath;
                    }
                    else
                        return "No File Selected.";

                    fileToBeIndexed.time_added = DateTime.UtcNow;
                    fileToBeIndexed.file_id = Guid.NewGuid();
                    fileToBeIndexed.index_status = 0;
                    entities.T_Indexing_Queue.Add(fileToBeIndexed);
                    entities.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }

        public static List<T_Indexing_Queue> GetFilesInIndexQueue()
        {
            try
            {
                using (TechniciseShabdoKhojEntities entities = new TechniciseShabdoKhojEntities())
                {
                    List<T_Indexing_Queue> filesFromIndexQueue = entities.T_Indexing_Queue.ToList();
                    return filesFromIndexQueue;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}
