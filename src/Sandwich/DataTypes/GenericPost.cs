using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandwich.DataTypes;
using Sandwich.Helpers;

namespace Sandwich
{
   public class GenericPost
    {
       public GenericPost() 
       {
           this.type = PostType.FourChan;
           this.capcode = Capcode.None;
       }

       public int PostNumber { get; set; }
       
       public DateTime time { get; set; }

       public string comment { get; set; }

       public string subject { get; set; }

       public string trip { get; set; }

       public string name { get; set; }

       public string email { get; set; }

       public string board { get; set; }

       public PostFile file;

       public PostType type { get; set; }

       public Capcode capcode { get; set; }

       public string country_flag { get; set; }
       public string country_name { get; set; }

       public enum PostType { FourChan, Fuuka, FoolFuuka }

       public enum Capcode { Admin, Mod, Developer, None }

       private List<int> _my_quoters = new List<int>();

       public void MarkAsQuotedBy(int id)
       {
           if (!_my_quoters.Contains(id))
           {
               _my_quoters.Add(id);
           }
       }

       public int[] QuotedBy { get { return _my_quoters.ToArray(); } }

       public CommentToken[] CommentTokens { get { return ThreadHelper.TokenizeComment(this.comment, this.type); } }

       public enum ThreadTag
       {
           Other,
           Game,
           Loop,
           Japanese,
           Anime,
           Porn,
           Hentai,
           NoTag, 
           Unknown
       }

       public ThreadTag Tag { get; set; }
   }
}
