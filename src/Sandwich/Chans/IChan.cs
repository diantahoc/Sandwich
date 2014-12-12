using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace Sandwich.Chans
{
    public interface IChan
    {
        string Name { get; }

        string Description { get; }

        string Key { get; }

        bool ValidateURL(string url);

        BoardInfo ValidateBoard(string board_letter);

        URLParserResults ParseURL(string url);

        string GetThreadURL(string board, int id);
        
        /// <summary>
        /// board letter, board description
        /// </summary>
        /// <returns></returns>
        KeyValuePair<string, BoardInfo>[] GetBoards();

        ThreadContainer GetThread(string board, int id);

        ThreadContainer[] GetPage(string board, int number);

        CatalogItem[] GetCatalog(string board);

        bool HasArchives { get; }

        IChanArchiveDataProvider ArchiveDataProvider { get; }

        Core.ReportStatus ReportPost(string board, int post_id, Core.ReportReason reason, SolvedCaptcha captcha);

        Core.DeleteStatus DeletePost(string board, int thread_id, int post_id, string password, bool file_only);

        bool RequireCaptcha { get; }

        IPostSenderResponse SendPost(RequestPosterData data);

        BitmapImage GetBoardSpoilerImage(BoardInfo board);
    }

    public struct URLParserResults 
    {
        public enum ActionType 
        {
            Unkown,
            HomePage,
            Catalog,
            Page,
            Thread
        }

        public ActionType Action { get; set; }

        public BoardInfo Board { get; set; }

        public int ThreadID { get; set; }

        public int PageNumber { get; set; }

    }
}
