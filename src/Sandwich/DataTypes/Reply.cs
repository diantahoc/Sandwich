using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandwich.DataTypes;
using Sandwich.Helpers;

namespace Sandwich
{
   public class Reply : GenericPost
    {
        public Thread Owner { get; private set; }

        public Reply(Thread owner, GenericPost i) 
        {
            this.Owner = owner;
            base.board = owner.board;

            base.comment = i.comment;
            base.email = i.email;
            base.file = i.file;
            base.name = i.name;
            base.PostNumber = i.PostNumber;
            base.subject = i.subject;
            base.time = i.time;
            base.trip = i.trip;
            base.country_name = i.country_name;
            base.country_flag = i.country_flag;
            base.capcode = i.capcode;
            base.type = i.type;

            this.CommentTokens = ThreadHelper.TokenizeComment(this);
        }

        public CommentToken[] CommentTokens { get; private set; }
    }
}
