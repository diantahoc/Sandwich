//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Sandwich
//{
//    static class ThreadMonitor
//    {
        

//        public static void MonitorThread(string board, int tid) 
//        {
          
//        }

//    }

//    public class MonitoredThread 
//    {
//        public int ID { get; set; }
//        public string Board { get; set; }
//        public ThreadStatus Status { get; set; }
//        public DateTime LastChecked { get; set; }
//        public enum ThreadStatus { Died, Alive }

//        private ThreadContainer tc;

//        private ThreadContainer new_tc;

//        private System.ComponentModel.BackgroundWorker worker;

//        public MonitoredThread() 
//        {
//            worker = new System.ComponentModel.BackgroundWorker();

//            worker.DoWork += new System.ComponentModel.DoWorkEventHandler(worker_DoWork);
//            worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
//        }

//        private List<string> saved_files = new List<string>();
//        private Dictionary<string, string[]> queued_files = new Dictionary<string, string[]>();    

//        void worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
//        {
//            try
//            {
//                //new_tc = Core.GetThreadData(this.Board, this.ID);
//            }
//            catch (Exception ex)
//            {
//                if (ex.Message == "404")
//                {
//                    die();
//                    e.Cancel = true;
//                }
//                else
//                {
//                    throw;
//                }
//            }
//        }

//        void worker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
//        {
//            if (!e.Cancelled) { HandleData(); }
//        }



//        public void Check() 
//        {
//            if (this.Status == ThreadStatus.Alive)
//            {
//                if (!worker.IsBusy) { worker.RunWorkerAsync(); }
//            }
//        }

//        private void HandleData() 
//        {
//            if (new_tc != null) 
//            {
//                int old_replies_count = tc.Replies.Count();
//                int new_replies_count = new_tc.Replies.Count();

//                if (new_replies_count > old_replies_count) 
//                {
//                    int last_index_before_new_replies = (old_replies_count - 1);

//                    for (; last_index_before_new_replies < new_replies_count; last_index_before_new_replies++) 
//                    {
//                        GenericPost f = new_tc.Replies[last_index_before_new_replies];
                        
//                        if (f.file != null) { QueueFile(f.file); }

//                    }

//                    //new replies

//                    if (NewReplies != null) { NewReplies(this); }
//                }
//                else if (new_replies_count == old_replies_count)
//                {
//                    //no new replies
//                }
//                else if(new_replies_count < old_replies_count)
//                {
//                    //some post are deleted.
//                }

//                tc.Dispose();
//                tc = null;
//                tc = new_tc;
//            }
//        }

//        private void QueueFile(PostFile file) 
//        {
//            if (queued_files.ContainsKey(file.hash))
//            {
//                return;
//            }
//            else 
//            {
//                //URL, FILENAME.PNG
//                queued_files.Add(file.hash, new string[] { file.FullImageLink, file.filename + "." + file.ext });
//            }
//        }

//        private void die() 
//        {
//            this.Status = MonitoredThread.ThreadStatus.Died;
//            if (StatusChanged != null) { StatusChanged(this); }
//        }

//        public delegate void NewRepliesEvent(MonitoredThread t);
//        public delegate void StatusChangedEvent(MonitoredThread t);


//        public event NewRepliesEvent NewReplies;
//        public event StatusChangedEvent StatusChanged;

//    }
//}