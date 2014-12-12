using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandwich.Chans
{
    public class RequestPosterData
    {
        public bool IsReply { get; set; }

        public BoardInfo Board { get; set; }
        public int ThreadID { get; set; }

        public string Name { get; set; }
        public string Subject { get; set; }
        public string Email { get; set; }
        public string Comment { get; set; }
        public string PostPassword { get; set; }

        public bool CaptchaRequired { get; set; }
        public SolvedCaptcha Captcha = null;

        public UploadFile[] GetFiles()
        {
            return files.ToArray();
        }

        private List<UploadFile> files = new List<UploadFile>();

        public void AddFile(UploadFile file)
        {
            this.files.Add(file);
        }

        public void RemoveFile(UploadFile file)
        {
            if (this.files.Contains(file))
            {
                this.files.Remove(file);
            }
        }

    }
}
