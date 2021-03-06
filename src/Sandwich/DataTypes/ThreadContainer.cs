﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Web;
using Sandwich.Helpers;
using Sandwich.DataTypes;

namespace Sandwich
{
    public class ThreadContainer : IDisposable
    {
        private bool _disposed = false;

        private Dictionary<int, GenericPost> _childs;

        public ThreadContainer(Thread instance)
        {
            _childs = new Dictionary<int, GenericPost>();
            this.Instance = instance;

            this.Title = ThreadHelper.Guess_Post_Title(instance);
        }

        public Thread Instance { get; private set; }

        public void AddReply(GenericPost reply)
        {
            if (!_disposed)
            {
                if (_childs.ContainsKey(reply.PostNumber))
                {
                    //?? what do
                }
                else
                {
                    foreach (CommentToken token in reply.CommentTokens)
                    {
                        if (token.Type == CommentToken.TokenType.Quote)
                        {
                            int id = Convert.ToInt32(token.TokenData);
                            if (id == this.Instance.PostNumber) //if quoting op
                            {
                                this.Instance.MarkAsQuotedBy(reply.PostNumber);
                            }
                            else if (id == reply.PostNumber) //if quoting self
                            {
                                reply.MarkAsQuotedBy(id);
                            }
                            else
                            {
                                GenericPost qq = GetPost(Convert.ToInt32(token.TokenData));
                                if (qq != null) { qq.MarkAsQuotedBy(reply.PostNumber); }
                            }
                        }
                    }

                    this._childs.Add(reply.PostNumber, reply);
                }
            }
        }

        public GenericPost GetPost(int id)
        {
            if (!_disposed)
            {
                if (_childs.ContainsKey(id))
                {
                    return _childs[id];
                }
                else { return null; }
            }
            else { return null; }
        }

        public void RemovePost(int id)
        {
            if (!_disposed)
            {
                if (_childs.ContainsKey(id))
                {
                    _childs.Remove(id);
                }
            }
        }

        public GenericPost[] Replies { get { return _childs.Values.ToArray(); } }

        public string Title { get; private set; }

        public void Dispose()
        {
            if (this.Instance.file != null) { this.Instance.file.Dispose(); }
           
            foreach(GenericPost a in _childs.Values)
            {
                 if (a.file != null)
                   a.file.Dispose();
            }

            _childs.Clear();
            _disposed = true;
        }
    }
}