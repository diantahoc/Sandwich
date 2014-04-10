using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sandwich.ArchiveExtensions
{
    public static class ArchiveDataProvider
    {
        private static Archive[] Archives;

        private static string archive_data_file = Path.Combine(Core.misc_dir, "archives_data.json");

        public static void UpdateData()
        {
            string update_url = @"https://raw2.github.com/MayhemYDG/4chan-x/v3/json/archives.json";

            string data = "";

            using (WebClient nc = new WebClient())
            {
                try
                {
                    data = nc.DownloadString(update_url);
                    File.WriteAllText(archive_data_file, data);
                }
                catch (Exception) { }
            }

            if (string.IsNullOrEmpty(data))
            {
                // throw new Exception("Cannot update archive data");
                Archives = new Archive[] { }; // load an empty archives array, means there is no archives.
            }
            else
            {
                Archives = load_archive_data(data);
            }
        }

        public static void init()
        {
            System.Threading.Tasks.Task.Factory.StartNew((Action)delegate
            {
                if (File.Exists(archive_data_file))
                {
                    Archives = load_archive_data(File.ReadAllText(archive_data_file));
                }
                else
                {
                    UpdateData();
                }
            });
        }

        private static Archive[] load_archive_data(string data)
        {
            List<Dictionary<string, object>> list = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(data);

            List<Archive> il = new List<Archive>();

            for (int i = 0; i < list.Count; i++)
            {

                Dictionary<string, object> archive_data = (Dictionary<string, object>)list[i];

                if (archive_data.ContainsKey("withCredentials"))
                {
                    continue;
                }

                Archive t = new Archive();

                t.Name = archive_data["name"].ToString();

                switch (archive_data["software"].ToString())
                {
                    case "fuuka":
                        t.Type = Archive.ArchiveType.Fuuka;
                        break;
                    case "foolfuuka":
                        t.Type = Archive.ArchiveType.FoolFuuka;
                        break;
                    default:
                        //DROP THIS TYPE OF ARCHIVE
                        continue;
                }

                string preffix = "";

                if (Convert.ToBoolean(archive_data["http"]))
                {
                    preffix = "http://";
                }
                else
                {
                    if (Convert.ToBoolean(archive_data["https"]))
                    {
                        preffix = "https://";
                    }
                    else
                    {
                        //does not support http nor https
                        continue;
                    }
                }

                switch (t.Type)
                {
                    case Archive.ArchiveType.FoolFuuka:
                        t.ThreadUrl = preffix + archive_data["domain"] + "/{0}/thread/{1}"; // 0 is board letter, 1 is thread id
                        t.FileURL = ""; //FoolFuuka
                        break;
                    case Archive.ArchiveType.Fuuka:
                        t.ThreadUrl = preffix + archive_data["domain"] + "/{0}/thread/{1}";
                        t.FileURL = preffix + archive_data["domain"];
                        break;
                    default:
                        continue;
                }

                t.Domain = preffix + archive_data["domain"];

                //----------------------------------

                List<string> supported_boards = new List<string>();

                foreach (JValue board in (JContainer)archive_data["boards"])
                {
                    supported_boards.Add(board.Value.ToString());
                }

                t.SupportedBoards = supported_boards.ToArray();

                List<string> supported_files = new List<string>();

                foreach (JValue board in (JContainer)archive_data["files"])
                {
                    supported_files.Add(board.Value.ToString());
                }

                t.SupportedFullImages = supported_files.ToArray();

                il.Add(t);

            }

            return il.ToArray(); ;
        }

        public static ThreadContainer GetThreadData(string board, int tid)
        {
            Archive selected_arch = null;

            Archive arch_with_files = null;
            Archive arch_nofile = null;

            foreach (Archive arch in Archives)
            {
                if (arch.IsBoardSupported(board) && arch.IsFileSupported(board))
                {
                    arch_with_files = arch;
                }
                else if (arch.IsBoardSupported(board))
                {
                    arch_nofile = arch;
                }
            }

            //Prioritize archives with full file support.
            if (arch_with_files != null)
            {
                selected_arch = arch_with_files;
            }
            else
            {
                selected_arch = arch_nofile;
            }

            if (selected_arch != null)
            {
                switch (selected_arch.Type)
                {
                    case Archive.ArchiveType.Fuuka:
                        return FuukaParser.Parse(selected_arch, board, tid);
                    case Archive.ArchiveType.FoolFuuka:
                        return FoolFuukaParser.Parse(selected_arch, board, tid);
                    default:
                        return null;
                }
            }

            return null;
        }

        public static bool BoardHasArchive(string board)
        {
            foreach (Archive a in Archives)
            {
                if (a.IsBoardSupported(board)) { return true; }
            }

            return false;
        }

    }
}
