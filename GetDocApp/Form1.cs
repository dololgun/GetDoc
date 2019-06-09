using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetDocApp {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void BtnCreateFile_Click(object sender, EventArgs e) {
            for (int i=0; i<lbFileList.Items.Count; i++) {
                makeFile((string)lbFileList.Items[i]);
            }

            MessageBox.Show("완료하였습니다.");
        }

        private void makeFile(string fileFullpath) {

            List<Byte> listAllByte = new List<Byte>();

            Byte[] allByte;

            String[] allLine = File.ReadAllLines(fileFullpath);
            for (int i=0; i<allLine.Length; i++) {
                string[] indexBody = allLine[i].Split(new char[] {'-'});
                Byte[] bytes = Convert.FromBase64String(indexBody[1]);
                for(int j = 0; j < bytes.Length; j++) {
                    listAllByte.Add(bytes[j]);
                }
            }

            allByte = listAllByte.ToArray();
            // b64는 제거하고 저장
            String newDirctoryName = Path.GetDirectoryName(fileFullpath);
            String newFileName = Path.GetFileNameWithoutExtension(fileFullpath);
            File.WriteAllBytes(Path.Combine(newDirctoryName, newFileName), allByte);
        }

        private void BtnFolder_Click(object sender, EventArgs e) {
            DialogResult dr = folderBrowserDialog1.ShowDialog();
            if(dr == DialogResult.OK) {
                tbFolerPath.Text = folderBrowserDialog1.SelectedPath;
                getFileList();
            }
        }

        private void TbFolerPath_KeyPress(object sender, KeyPressEventArgs e) {
            if(e.KeyChar == 13) {
                getFileList();
            }
        }

        private void getFileList() {
            if (tbFolerPath.Text.Trim() == String.Empty) {                
                return;
            }

            lbFileList.Items.Clear();
            DirectoryInfo di = new DirectoryInfo(tbFolerPath.Text.Trim());
            getFileList(di);
        }

        private void getFileList(DirectoryInfo di) {

            foreach(FileInfo fileInfo in di.GetFiles()) {
                if(Path.GetExtension(fileInfo.Name) == ".b64") {
                    lbFileList.Items.Add(fileInfo.FullName);
                }
            }

            foreach(DirectoryInfo directoryInfo in di.GetDirectories()) {
                if (di.Attributes == FileAttributes.Directory) {
                    getFileList(directoryInfo);
                }
            }
        }
    }
}
