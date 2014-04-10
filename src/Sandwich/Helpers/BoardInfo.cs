using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace Sandwich
{
    static class BoardInfo
    {
        private static string[] sfw_board = { "g","a","gd","k","3","adv","an","asp","c","cgl","co","diy","fa","fit",
                                             "int" ,"mu","n","o","out","po","p","sci","sp","tg","trv","tv","wsg","v","vg","vr","vp","x","w","biz"};

        private static string[] common_files = { "gif", "jpg", "jpeg", "png" };

        private static Dictionary<string, BoardInfoContainer> data = new Dictionary<string, BoardInfoContainer>();

        static BoardInfo()
        {

            data.Add("g", new BoardInfoContainer
            {
                Letter = "g",
                Description = "Technology",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files
            });

            data.Add("3", new BoardInfoContainer
            {
                Letter = "3",
                Description = "3DCG",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files
            });

            data.Add("a", new BoardInfoContainer
            {
                Letter = "a",
                Description = "Anime & Manga",
                Bumplimit = 500,
                ImageLimit = 250,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files
            });

            data.Add("adv", new BoardInfoContainer
            {
                Letter = "adv",
                Description = "Advice",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files
            });

            data.Add("an", new BoardInfoContainer
            {
                Letter = "an",
                Description = "Animals & Nature",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files
            });

            data.Add("asp", new BoardInfoContainer
            {
                Letter = "asp",
                Description = "Alternative Sports",

                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("b", new BoardInfoContainer
            {
                Letter = "b",
                Description = "Random",

                Bumplimit = 500,
                ImageLimit = 250,
                PagesCount = 16,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("c", new BoardInfoContainer
            {
                Letter = "c",
                Description = "Anime/Cute",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("cgl", new BoardInfoContainer
            {
                Letter = "cgl",
                Description = "Cosplay & EGL",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("ck", new BoardInfoContainer
            {
                Letter = "ck",
                Description = "Food & Cooking",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("cm", new BoardInfoContainer
            {
                Letter = "cm",
                Description = "Cute/Male",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("co", new BoardInfoContainer
            {
                Letter = "co",
                Description = "Comics & Cartoons",
                Bumplimit = 500,
                ImageLimit = 250,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("d", new BoardInfoContainer
            {
                Letter = "d",
                Description = "Hentai/Alternative",
                Bumplimit = 300,
                ImageLimit = 250,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("diy", new BoardInfoContainer
            {
                Letter = "diy",
                Description = "Do it Yourself",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("e", new BoardInfoContainer
            {
                Letter = "e",
                Description = "Ecchi",

                Bumplimit = 300,
                ImageLimit = 250,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("fa", new BoardInfoContainer
            {
                Letter = "fa",
                Description = "Fashion",

                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("fit", new BoardInfoContainer
            {
                Letter = "fit",
                Description = "Health & Fitness",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files
            });

            data.Add("gd", new BoardInfoContainer
            {
                Letter = "gd",
                Description = "Graphic Design",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = new string[] { "pdf" },
                AllowedFiles = common_files

            });

            data.Add("gif", new BoardInfoContainer
            {
                Letter = "gif",
                Description = "Adult GIF",
                Bumplimit = 300,
                ImageLimit = 250,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = new string[] { "gif" },

            });

            data.Add("h", new BoardInfoContainer
            {
                Letter = "h",
                Description = "Hentai",

                Bumplimit = 300,
                ImageLimit = 250,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("hc", new BoardInfoContainer
            {
                Letter = "hc",
                Description = "Hardcore",
                Bumplimit = 300,
                ImageLimit = 250,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("hm", new BoardInfoContainer
            {
                Letter = "hm",
                Description = "Handsome Men",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files
            });

            data.Add("hr", new BoardInfoContainer
            {
                Letter = "hr",
                Description = "High Resolution",
                Bumplimit = 300,
                ImageLimit = 250,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("i", new BoardInfoContainer
            {
                Letter = "i",
                Description = "Oekaki",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("ic", new BoardInfoContainer
            {
                Letter = "ic",
                Description = "Artwork/Critique",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("int", new BoardInfoContainer
            {
                Letter = "int",
                Description = "International",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("jp", new BoardInfoContainer
            {
                Letter = "jp",
                Description = "Otaku Culture",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("k", new BoardInfoContainer
            {
                Letter = "k",
                Description = "Weapons",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            //data.Add("lgbt", new BoardInfoContainer
            //{
            //    Letter = "lgbt",
            //    Description = "Lesbian, Gay, Bisexual & Transgender",
            //    
            //    Bumplimit = 300,
            //    ImageLimit = 150,
            //    PagesCount = 11,
            //    ExtraFiles = null,
            //    AllowedFiles = common_files

            //});

            data.Add("lit", new BoardInfoContainer
            {
                Letter = "lit",
                Description = "Literature",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("m", new BoardInfoContainer
            {
                Letter = "m",
                Description = "Mecha",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("mlp", new BoardInfoContainer
            {
                Letter = "mlp",
                Description = "Pony",
                Bumplimit = 500,
                ImageLimit = 250,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("mu", new BoardInfoContainer
            {
                Letter = "mu",
                Description = "Music",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("n", new BoardInfoContainer
            {
                Letter = "n",
                Description = "Transportation",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("o", new BoardInfoContainer
            {
                Letter = "o",
                Description = "Auto",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("out", new BoardInfoContainer
            {
                Letter = "out",
                Description = "Outdoors",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("p", new BoardInfoContainer
            {
                Letter = "p",
                Description = "Photo",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files
            });

            data.Add("po", new BoardInfoContainer
            {
                Letter = "po",
                Description = "Papercraft & Origami",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = new string[] { "pdf" },
                AllowedFiles = common_files
            });

            //data.Add("pol", new BoardInfoContainer
            //{
            //    Letter = "pol",
            //    Description = "Politically Incorrect",
            //    Bumplimit = 300,
            //    ImageLimit = 150,
            //    PagesCount = 11,
            //    ExtraFiles = null,
            //    AllowedFiles = common_files
            //});

            //data.Add("q", new BoardInfoContainer
            //{
            //    Letter = "q",
            //    Description = "4chan Discussion",
            //    Bumplimit = 1000,
            //    ImageLimit = 200,
            //    PagesCount = 11,
            //    ExtraFiles = null,
            //    AllowedFiles = common_files
            //});

            data.Add("r", new BoardInfoContainer
            {
                Letter = "r",
                Description = "Request",
                Bumplimit = 250,
                ImageLimit = 150,
                PagesCount = 16,
                ExtraFiles = null,
                AllowedFiles = common_files
            });

            data.Add("r9k", new BoardInfoContainer
            {
                Letter = "r9k",
                Description = "ROBOT9001",
                Bumplimit = 500,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files
            });

            data.Add("s", new BoardInfoContainer
            {
                Letter = "s",
                Description = "Sexy Beautiful Women",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files
            });

            data.Add("s4s", new BoardInfoContainer
            {
                Letter = "s4s",
                Description = "Shit 4chan Says",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files
            });

            data.Add("sci", new BoardInfoContainer
            {
                Letter = "sci",
                Description = "Science & Math",
                Bumplimit = 250,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files
            });

            data.Add("soc", new BoardInfoContainer
            {
                Letter = "soc",
                Description = "Social",
                Bumplimit = 500,
                ImageLimit = 250,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files
            });

            data.Add("sp", new BoardInfoContainer
            {
                Letter = "sp",
                Description = "Sports",
                Bumplimit = 500,
                ImageLimit = 250,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("t", new BoardInfoContainer
            {
                Letter = "t",
                Description = "Torrents",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("tg", new BoardInfoContainer
            {
                Letter = "tg",
                Description = "Traditional Games",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = new string[] { "pdf" },
                AllowedFiles = common_files

            });

            data.Add("toy", new BoardInfoContainer
            {
                Letter = "toy",
                Description = "Toys",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("trv", new BoardInfoContainer
            {
                Letter = "trv",
                Description = "Travel",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("tv", new BoardInfoContainer
            {
                Letter = "tv",
                Description = "Television & Film",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("u", new BoardInfoContainer
            {
                Letter = "u",
                Description = "Yuri",
                Bumplimit = 300,
                ImageLimit = 250,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("v", new BoardInfoContainer
            {
                Letter = "v",
                Description = "Video Games",
                Bumplimit = 500,
                ImageLimit = 250,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("vg", new BoardInfoContainer
            {
                Letter = "vg",
                Description = "Video Game Generals",
                Bumplimit = 750,
                ImageLimit = 375,
                PagesCount = 6,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("vp", new BoardInfoContainer
            {
                Letter = "vp",
                Description = "Pokémon",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("vr", new BoardInfoContainer
            {
                Letter = "vr",
                Description = "Retro Games",
                Bumplimit = 300,
                ImageLimit = 250,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("w", new BoardInfoContainer
            {
                Letter = "w",
                Description = "Anime/Wallpapers",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files
            });

            data.Add("wg", new BoardInfoContainer
            {
                Letter = "wg",
                Description = "Wallpapers/General",
                Bumplimit = 300,
                ImageLimit = 250,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files
            });

            data.Add("wsg", new BoardInfoContainer
            {
                Letter = "wsg",
                Description = "Worksafe GIF",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("x", new BoardInfoContainer
            {
                Letter = "x",
                Description = "Paranormal",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("biz", new BoardInfoContainer
            {
                Letter = "biz",
                Description = "Business & Finance",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = null,
                AllowedFiles = common_files

            });

            data.Add("f", new BoardInfoContainer
            {
                Letter = "f",
                Description = "Flash",
                Bumplimit = 300,
                ImageLimit = 150,
                PagesCount = 11,
                ExtraFiles = new string[]{"swf"},
                AllowedFiles = null

            });

            //data.Add("y", new BoardInfoContainer
            //{
            //    Letter = "y",
            //    Description = "Yaoi",
            //    Bumplimit = 300,
            //    ImageLimit = 250,
            //    PagesCount = 11,
            //    ExtraFiles = null,
            //    AllowedFiles = common_files

            //});


            Dictionary<string, BoardInfoContainer> sorted = new Dictionary<string, BoardInfoContainer>();

            foreach (KeyValuePair<string, BoardInfoContainer> a in data.OrderBy(x => x.Key))
            {
                sorted.Add(a.Key, a.Value);
            }

            data = sorted;

            sfw_normal = new BitmapImage(new Uri("pack://application:,,,/Sandwich;component/Resources/icons/sfw-normal.png", UriKind.Absolute));
            sfw_normal.Freeze();

            nsfw_normal = new BitmapImage(new Uri("pack://application:,,,/Sandwich;component/Resources/icons/nsfw-normal.png", UriKind.Absolute));
            nsfw_normal.Freeze();
        }

        public static string[] GetBoardFiles(string g)
        {
            if (data.ContainsKey(g))
            {
                BoardInfoContainer d = data[g];
                if (d.ExtraFiles == null)
                {
                    return d.AllowedFiles;
                }
                else
                {
                    return d.AllowedFiles.Union(d.ExtraFiles).ToArray();
                }
            }
            else
            {
                return new string[] { };
            }
        }

        public static string GetBoardTitle(string letter)
        {
            if (data.ContainsKey(letter.ToLower()))
            {
                return data[letter.ToLower()].Description;
            }
            else
            {
                return letter;
            }
        }

        public static int GetBoardMaximumFileSize(string board)
        {
            if (board == "b" || board == "s4s" || board == "r9k") { return 2 * 1048576; }

            if (board == "gd" || board == "hm" || board == "hr" || board == "po" || board == "r" || board == "s" || board == "trv" || board == "tg") { return 8 * 1048576; }

            if (board == "out" || board == "p" || board == "w" || board == "wg") { return 5 * 1048576; }

            if (board == "gif" || board == "soc" || board == "sp" || board == "wsg") { return 4 * 1048576; }

            return 3 * 1048576;
        }


        public static System.Windows.Media.Color GetPostColor(string letter)
        {
            //214, 218, 240 for sfw
            //240, 224, 214 for nsfw
            if (Array.IndexOf(sfw_board, letter.ToLower()) >= 0)
            {
                return System.Windows.Media.Color.FromRgb(214, 218, 240);
            }
            else
            {
                return System.Windows.Media.Color.FromRgb(240, 224, 214);
            }
        }

        public static System.Windows.Media.Color GetBoardBackColor(string letter)
        {
            //214, 218, 240 for sfw
            //240, 224, 214 for nsfw
            if (Array.IndexOf(sfw_board, letter.ToLower()) >= 0)
            {
                return System.Windows.Media.Color.FromRgb(238, 242, 255);
            }
            else
            {
                return System.Windows.Media.Color.FromRgb(255, 255, 238);
            }
        }

        private static BitmapImage sfw_normal;
        //private static BitmapImage sfw_unread = new BitmapImage(new Uri("pack://application:,,,/Sandwich;component/Resources/icons/sfw-unread.png", UriKind.Absolute));

        private static BitmapImage nsfw_normal;
        //private static BitmapImage nsfw_unread;

        public static BitmapImage GetBoardIcon(string letter)
        {
            if (Array.IndexOf(sfw_board, letter) == -1)
            {
                return nsfw_normal;
            }
            else
            {
                return sfw_normal;
            }
        }

        public static string[] BoardsLetters
        {
            get { return data.Keys.ToArray(); }
        }

        private static Dictionary<string, BitmapImage> _spoilerImages = new Dictionary<string, BitmapImage>();

        public static BitmapImage GetSpoilerImage(string board)
        {
            if (_spoilerImages.ContainsKey(board))
            {
                return _spoilerImages[board];
            }
            else
            {
                BitmapImage i = new BitmapImage(new Uri("pack://application:,,,/Sandwich;component/Resources/spoiler_images/spoiler-" + board + "1.png", UriKind.Absolute));
                i.Freeze();
                _spoilerImages.Add(board, i);
                return i;
            }
        }

    }

    public class BoardInfoContainer
    {
        public string Letter { get; set; }

        public string Description { get; set; }

        public int PagesCount { get; set; }
        public int Bumplimit { get; set; }
        public int ImageLimit { get; set; }

        public string[] ExtraFiles { get; set; }
        public string[] AllowedFiles { get; set; }
    }
}
