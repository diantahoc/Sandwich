﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Sandwich
{
    public static class SessionManager
    {
        private static string session_dir;

        static SessionManager()
        {
            session_dir = Core.session_dir;
            File.Delete(Path.Combine(session_dir, "clean"));
        }

        public static void RegisterThread(Chans.IChan chan, BoardInfo board, int id)
        {
            string path = Path.Combine(session_dir, string.Format("thread-{0}-{1}-{2}", chan.Name, board.Letter, id));
            File.WriteAllText(path, "");
        }

        public static void RegisterCatalog(Chans.IChan chan, BoardInfo board)
        {
            string path = Path.Combine(session_dir, string.Format("catalog-{0}-{1}", chan.Name, board.Letter));

            File.WriteAllText(path, "");
        }

        public static void RegisterFile(PostFile pf)
        {
            // string md5 = Common.ByteArrayToString(Convert.FromBase64String(pf.hash));
            if (string.IsNullOrWhiteSpace(pf.hash)) { return; }
            string md5 = Common.MD5(pf.hash);
            string content = Newtonsoft.Json.JsonConvert.SerializeObject(
                new SessionFile()
                {
                    FileName = pf.filename,
                    Board = pf.board,
                    Ext = pf.ext,
                    Hash = pf.hash,
                    Height = pf.height,
                    Width = pf.width,
                    IsSpoiler = pf.IsSpoiler,
                    Owner = pf.owner,
                    Size = pf.size,
                    ThumbFileName = pf.thumbnail_tim,
                    ThumbH = pf.thumbH,
                    ThumbW = pf.thumbW
                }, Newtonsoft.Json.Formatting.Indented);
            string path = Path.Combine(session_dir, string.Format("file-{0}-{1}", pf.board, md5));

            File.WriteAllText(path, content);
        }

        public static void UnRegisterThread(Chans.IChan chan, BoardInfo board, int id)
        {
            File.Delete(Path.Combine(session_dir, string.Format("thread-{0}-{1}-{2}", chan.Name, board.Letter, id)));
        }

        public static void UnRegisterCatalog(Chans.IChan chan, BoardInfo board)
        {
            File.Delete(Path.Combine(session_dir, string.Format("catalog-{0}-{1}", chan.Name, board.Letter)));
        }

        public static void UnRegisterFile(PostFile pf)
        {
            if (string.IsNullOrWhiteSpace(pf.hash)) { return; }
            string md5 = Common.MD5(pf.hash);
            File.Delete(Path.Combine(session_dir, string.Format("file-{0}-{1}", pf.board, md5)));
        }
    }
}
