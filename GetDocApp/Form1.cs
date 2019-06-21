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

            // 생성일자, 수정일자 원본과 똑같도록 변경

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
                MessageBox.Show("경로를 입력해야 합니다");
                return;
            }

            lbFileList.Items.Clear();
            try {
                DirectoryInfo di = new DirectoryInfo(tbFolerPath.Text.Trim());
                getFileList(di);
            }
            catch (ArgumentException) {
                MessageBox.Show("잘못된 경로 입니다");
                return;
            }
            catch (PathTooLongException) {
                MessageBox.Show("잘못된 경로 입니다");
                return;
            }
            catch (DirectoryNotFoundException) {
                MessageBox.Show("잘못된 경로 입니다");
                return;
            }
        }

        private void getFileList(DirectoryInfo di) {
            try {
                foreach (FileInfo fileInfo in di.GetFiles()) {
                    if (Path.GetExtension(fileInfo.Name) == ".b64") {
                        lbFileList.Items.Add(fileInfo.FullName);
                    }
                }
            }
            catch (DirectoryNotFoundException) {
                throw;
            }


            foreach(DirectoryInfo directoryInfo in di.GetDirectories()) {
                if (di.Attributes == FileAttributes.Directory) {
                    getFileList(directoryInfo);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e) {
            this.tbFolerPath.Text = Directory.GetCurrentDirectory();
        }
    }
}
