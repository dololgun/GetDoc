using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Http;

namespace GetDocWeb.Controllers {
    public class ReceiverController : ApiController {
        public void Post([FromBody] SliceInfo sliceInfo) {

            if (sliceInfo.Value != null) {

                string savePath = String.Format(@"{0}.b64", sliceInfo.FileFullName);

                if (int.Parse(sliceInfo.Index) == 0 && File.Exists(savePath)) {
                    File.Delete(savePath);
                }

                using(StreamWriter sw = File.AppendText(savePath)) {
                    sw.WriteLine(String.Format("{0}-{1}", sliceInfo.Index, sliceInfo.Value));
                }
                DateTime cDt = File.GetCreationTime(sliceInfo.FileFullName);
                DateTime mDt = File.GetLastWriteTime(sliceInfo.FileFullName);

                File.SetCreationTime(savePath, cDt);
                File.SetLastWriteTime(savePath, mDt);

            }
        }
    }

    public class SliceInfo {
        public string Value { get; set; }
        public string FileFullName { get; set; }
        public string Index { get; set; }
    }
}
