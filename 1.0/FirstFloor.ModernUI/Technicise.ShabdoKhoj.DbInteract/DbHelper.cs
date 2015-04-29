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
                   T_Indexing_Queue xgqReponse = new T_Indexing_Queue();
                   if (string.IsNullOrEmpty(filename) == false)
                   {
                       xgqReponse.file_name = filename;
                       xgqReponse.file_path = filepath;
                   }
                   else
                       return "No File Selected.";

                   xgqReponse.time_added = DateTime.UtcNow;
                   xgqReponse.file_id = new Guid();
                   entities.T_Indexing_Queue.Add(xgqReponse);
                   entities.SaveChanges();
               }
           }
           catch (Exception ex)
           {
               return ex.Message;
           }
           return null;
       }
    }
}
