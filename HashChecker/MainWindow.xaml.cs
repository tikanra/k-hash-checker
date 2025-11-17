using System;
using System.Windows;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Win32;
using System.Threading;
using System.Text;
using System.Linq;
using System.Windows.Input;

namespace HashChecker
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string DEFAULT_CONTENT = "请拖拽文件到此处进行校验";

        private static uint[] CRC_TABLE =
         {
         0x0, 0x77073096, 0xee0e612c, 0x990951ba, 0x76dc419, 0x706af48f, 0xe963a535, 0x9e6495a3,
         0xedb8832, 0x79dcb8a4, 0xe0d5e91e, 0x97d2d988, 0x9b64c2b, 0x7eb17cbd, 0xe7b82d07, 0x90bf1d91,
         0x1db71064, 0x6ab020f2, 0xf3b97148, 0x84be41de, 0x1adad47d, 0x6ddde4eb, 0xf4d4b551, 0x83d385c7,
         0x136c9856, 0x646ba8c0, 0xfd62f97a, 0x8a65c9ec, 0x14015c4f, 0x63066cd9, 0xfa0f3d63, 0x8d080df5,
         0x3b6e20c8, 0x4c69105e, 0xd56041e4, 0xa2677172, 0x3c03e4d1, 0x4b04d447, 0xd20d85fd, 0xa50ab56b,
         0x35b5a8fa, 0x42b2986c, 0xdbbbc9d6, 0xacbcf940, 0x32d86ce3, 0x45df5c75, 0xdcd60dcf, 0xabd13d59,
         0x26d930ac, 0x51de003a, 0xc8d75180, 0xbfd06116, 0x21b4f4b5, 0x56b3c423, 0xcfba9599, 0xb8bda50f,
         0x2802b89e, 0x5f058808, 0xc60cd9b2, 0xb10be924, 0x2f6f7c87, 0x58684c11, 0xc1611dab, 0xb6662d3d,
         0x76dc4190, 0x1db7106, 0x98d220bc, 0xefd5102a, 0x71b18589, 0x6b6b51f, 0x9fbfe4a5, 0xe8b8d433,
         0x7807c9a2, 0xf00f934, 0x9609a88e, 0xe10e9818, 0x7f6a0dbb, 0x86d3d2d, 0x91646c97, 0xe6635c01,
         0x6b6b51f4, 0x1c6c6162, 0x856530d8, 0xf262004e, 0x6c0695ed, 0x1b01a57b, 0x8208f4c1, 0xf50fc457,
         0x65b0d9c6, 0x12b7e950, 0x8bbeb8ea, 0xfcb9887c, 0x62dd1ddf, 0x15da2d49, 0x8cd37cf3, 0xfbd44c65,
         0x4db26158, 0x3ab551ce, 0xa3bc0074, 0xd4bb30e2, 0x4adfa541, 0x3dd895d7, 0xa4d1c46d, 0xd3d6f4fb,
         0x4369e96a, 0x346ed9fc, 0xad678846, 0xda60b8d0, 0x44042d73, 0x33031de5, 0xaa0a4c5f, 0xdd0d7cc9,
         0x5005713c, 0x270241aa, 0xbe0b1010, 0xc90c2086, 0x5768b525, 0x206f85b3, 0xb966d409, 0xce61e49f,
         0x5edef90e, 0x29d9c998, 0xb0d09822, 0xc7d7a8b4, 0x59b33d17, 0x2eb40d81, 0xb7bd5c3b, 0xc0ba6cad,
         0xedb88320, 0x9abfb3b6, 0x3b6e20c, 0x74b1d29a, 0xead54739, 0x9dd277af, 0x4db2615, 0x73dc1683,
         0xe3630b12, 0x94643b84, 0xd6d6a3e, 0x7a6a5aa8, 0xe40ecf0b, 0x9309ff9d, 0xa00ae27, 0x7d079eb1,
         0xf00f9344, 0x8708a3d2, 0x1e01f268, 0x6906c2fe, 0xf762575d, 0x806567cb, 0x196c3671, 0x6e6b06e7,
         0xfed41b76, 0x89d32be0, 0x10da7a5a, 0x67dd4acc, 0xf9b9df6f, 0x8ebeeff9, 0x17b7be43, 0x60b08ed5,
         0xd6d6a3e8, 0xa1d1937e, 0x38d8c2c4, 0x4fdff252, 0xd1bb67f1, 0xa6bc5767, 0x3fb506dd, 0x48b2364b,
         0xd80d2bda, 0xaf0a1b4c, 0x36034af6, 0x41047a60, 0xdf60efc3, 0xa867df55, 0x316e8eef, 0x4669be79,
         0xcb61b38c, 0xbc66831a, 0x256fd2a0, 0x5268e236, 0xcc0c7795, 0xbb0b4703, 0x220216b9, 0x5505262f,
         0xc5ba3bbe, 0xb2bd0b28, 0x2bb45a92, 0x5cb36a04, 0xc2d7ffa7, 0xb5d0cf31, 0x2cd99e8b, 0x5bdeae1d,
         0x9b64c2b0, 0xec63f226, 0x756aa39c, 0x26d930a, 0x9c0906a9, 0xeb0e363f, 0x72076785, 0x5005713,
         0x95bf4a82, 0xe2b87a14, 0x7bb12bae, 0xcb61b38, 0x92d28e9b, 0xe5d5be0d, 0x7cdcefb7, 0xbdbdf21,
         0x86d3d2d4, 0xf1d4e242, 0x68ddb3f8, 0x1fda836e, 0x81be16cd, 0xf6b9265b, 0x6fb077e1, 0x18b74777,
         0x88085ae6, 0xff0f6a70, 0x66063bca, 0x11010b5c, 0x8f659eff, 0xf862ae69, 0x616bffd3, 0x166ccf45,
         0xa00ae278, 0xd70dd2ee, 0x4e048354, 0x3903b3c2, 0xa7672661, 0xd06016f7, 0x4969474d, 0x3e6e77db,
         0xaed16a4a, 0xd9d65adc, 0x40df0b66, 0x37d83bf0, 0xa9bcae53, 0xdebb9ec5, 0x47b2cf7f, 0x30b5ffe9,
         0xbdbdf21c, 0xcabac28a, 0x53b39330, 0x24b4a3a6, 0xbad03605, 0xcdd70693, 0x54de5729, 0x23d967bf,
         0xb3667a2e, 0xc4614ab8, 0x5d681b02, 0x2a6f2b94, 0xb40bbe37, 0xc30c8ea1, 0x5a05df1b, 0x2d02ef8d,
    };

        private bool IsVersionChecked { get; set; }

        private bool IsTimeChecked { get; set; }

        private bool IsCRC32Checked { get; set; }

        private bool IsMD5Checked { get; set; }

        private bool IsSHA1Checked { get; set; }

        private bool IsSHA256Checked { get; set; }

        private bool IsSHA512Checked { get; set; }

        private int processFlag = 0;

        public MainWindow()
        {
            InitializeComponent();
            label1.Content = DEFAULT_CONTENT;
        }

        private void CheckBoxVersion_Checked(object sender, RoutedEventArgs e)
        {
            IsVersionChecked = true;
        }

        private void CheckBoxVersion_UnChecked(object sender, RoutedEventArgs e)
        {
            IsVersionChecked = false;
        }

        private void CheckBoxTime_Checked(object sender, RoutedEventArgs e)
        {
            IsTimeChecked = true;
        }

        private void CheckBoxTime_UnChecked(object sender, RoutedEventArgs e)
        {
            IsTimeChecked = false;
        }


        private void CheckBoxCRC32_Checked(object sender, RoutedEventArgs e)
        {
            IsCRC32Checked = true;
        }

        private void CheckBoxCRC32_UnChecked(object sender, RoutedEventArgs e)
        {
            IsCRC32Checked = false;
        }


        private void CheckBoxMD5_Checked(object sender, RoutedEventArgs e)
        {
            IsMD5Checked = true;
        }

        private void CheckBoxMD5_UnChecked(object sender, RoutedEventArgs e)
        {
            IsMD5Checked = false;
        }

        private void CheckBoxSHA1_Checked(object sender, RoutedEventArgs e)
        {
            IsSHA1Checked = true;
        }

        private void CheckBoxSHA1_UnChecked(object sender, RoutedEventArgs e)
        {
            IsSHA1Checked = false;
        }

        private void CheckBoxSHA256_Checked(object sender, RoutedEventArgs e)
        {
            IsSHA256Checked = true;
        }

        private void CheckBoxSHA256_UnChecked(object sender, RoutedEventArgs e)
        {
            IsSHA256Checked = false;
        }

        private void CheckBoxSHA512_Checked(object sender, RoutedEventArgs e)
        {
            IsSHA512Checked = true;
        }

        private void CheckBoxSHA512_UnChecked(object sender, RoutedEventArgs e)
        {
            IsSHA512Checked = false;
        }

        private void Text_OnFileDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private void Text_OnFileDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private void Text_OnFileDrop(object sender, DragEventArgs e)
        {
            //如果拖入的对象不是文件，则不进行处理
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }
            calculteFiles((string[])e.Data.GetData(DataFormats.FileDrop));
        }

        private void calculteFiles(string[] files)
        {
            if (Interlocked.CompareExchange(ref processFlag, 1, 0) != 0)
            {
                MessageBox.Show("目前有任务正在运行，请稍后重试", "提示", MessageBoxButton.OK);
                return;
            }

            try
            {
                label1.Content = null;
                Task<List<string>> task = Task.Run(() =>
                {
                    return GetAllFiles(files);
                });
                task.ContinueWith(t =>
                {
                    List<string> fileList = t.Result;
                    Dispatcher.BeginInvoke(new Action(delegate
                    {
                        progressBar1.Minimum = 0;
                        progressBar1.Maximum = fileList.Count;
                        progressBar1.Value = 0;
                    }));

                    List<Tuple<string, bool, Func<FileStream, string>>> algoTupleList = createAlgorithmTupleList();
                    for (int i = 0; i < fileList.Count; ++i)
                    {
                        string filePath = fileList[i];
                        FileInfo fileInfo = new FileInfo(filePath);
                        string str = "文件名称：" + filePath + "\n" + "文件大小：" + getFileSizeString(fileInfo.Length) + "\n";
                        if (IsVersionChecked)
                        {
                            System.Diagnostics.FileVersionInfo fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(filePath);
                            if (fileVersionInfo != null)
                            {
                                str += "文件版本：" + fileVersionInfo.ProductVersion + "\n";
                            }
                        }
                        if (IsTimeChecked)
                        {
                            str += "修改时间：" + fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss") + "\n";
                        }

                        Dispatcher.BeginInvoke(new Action(delegate
                        {
                            var arr = from algoTuple in algoTupleList where algoTuple.Item2 select algoTuple;
                            textBox1.AppendText(str);
                            progressBar2.Minimum = 0;
                            progressBar2.Maximum = arr.ToList().Count;
                            progressBar2.Value = 0;
                        }));
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            foreach (Tuple<string, bool, Func<FileStream, string>> algoTuple in algoTupleList)
                            {
                                if (algoTuple.Item2)
                                {
                                    fileStream.Seek(0, SeekOrigin.Begin);
                                    string hash = algoTuple.Item3.Invoke(fileStream);
                                    Dispatcher.BeginInvoke(new Action(delegate
                                    {
                                        textBox1.AppendText(algoTuple.Item1 + "：" + hash + "\n");
                                        progressBar2.Value++;
                                    }));
                                }
                            }
                            Dispatcher.BeginInvoke(new Action(delegate
                            {
                                textBox1.AppendText("\n");
                                progressBar1.Value++;
                            }));
                        }
                    }
                });
            }
            finally
            {
                processFlag = 0;
            }
        }

        /// <summary>
        /// 生成所有哈希算法的执行列表
        /// </summary>
        /// <returns></returns>
        private List<Tuple<string, bool, Func<FileStream, string>>> createAlgorithmTupleList()
        {
            List<Tuple<string, bool, Func<FileStream, string>>> algoTupleList = new List<Tuple<string, bool, Func<FileStream, string>>>();
            Func<FileStream, string> crc32Func = fs => calculateCrc32(fs);
            algoTupleList.Add(Tuple.Create("CRC32", IsCRC32Checked, crc32Func));
            Func<FileStream, string> md5Func = fs => calculateHash(fs, new MD5CryptoServiceProvider());
            algoTupleList.Add(Tuple.Create("MD5", IsMD5Checked, md5Func));
            Func<FileStream, string> sha1Func = fs => calculateHash(fs, new SHA1CryptoServiceProvider());
            algoTupleList.Add(Tuple.Create("SHA1", IsSHA1Checked, sha1Func));
            Func<FileStream, string> sha256Func = fs => calculateHash(fs, new SHA256CryptoServiceProvider());
            algoTupleList.Add(Tuple.Create("SHA256", IsSHA256Checked, sha256Func));
            Func<FileStream, string> sha512Func = fs => calculateHash(fs, new SHA512CryptoServiceProvider());
            algoTupleList.Add(Tuple.Create("SHA512", IsSHA512Checked, sha512Func));
            return algoTupleList;
        }

        /// <summary>
        /// 计算 CRC32 算法
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        private string calculateCrc32(FileStream fileStream)
        {
            uint crc = 0xFFFFFFFF;
            byte[] bytes = new byte[1024];
            int readLen = -1;
            while ((readLen = fileStream.Read(bytes, 0, 1024)) != 0)
            {
                for (int i = 0; i < readLen; ++i)
                {
                    crc = ((crc >> 8) & 0x00FFFFFF) ^ CRC_TABLE[(crc ^ bytes[i]) & 0xFF];
                }
            }
            crc = crc ^ 0xFFFFFFFF;
            return crc.ToString("X");
        }

        /// <summary>
        /// 计算其它哈希算法
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="hashAlgorithm"></param>
        /// <returns></returns>
        private string calculateHash(FileStream fileStream, HashAlgorithm hashAlgorithm)
        {
            byte[] bytes = hashAlgorithm.ComputeHash(fileStream);
            return BitConverter.ToString(bytes).Replace("-", "").ToUpperInvariant();
        }

        /// <summary>
        /// 获取文件大小的描述字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private static string getFileSizeString(long length)
        {
            if (length < 1024)
            {
                return length + "B";
            }
            else if (length < 1024 * 1024)
            {
                return string.Format("{0:F2}", length / 1024d) + "KB";
            }
            else if (length < 1024 * 1024 * 1024)
            {
                return string.Format("{0:F2}", length / (1024d * 1024d)) + "MB";
            }
            else
            {
                return string.Format("{0:F2}", length / (1024d * 1024d * 1024d)) + "GB";
            }
        }

        /// <summary>
        /// 获取包含子文件夹下的所有文件列表
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private static List<string> GetAllFiles(string[] files)
        {
            List<string> fileList = new List<string>();
            foreach (string file in files)
            {
                if (File.Exists(file))
                {
                    fileList.Add(file);
                }
                else if (Directory.Exists(file))
                {
                    GetAllFiles(file, fileList);
                }
            }
            return fileList;
        }

        /// <summary>
        /// 地柜获取包含子文件夹下的所有文件列表
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private static void GetAllFiles(string folder, List<string> fileList)
        {
            string[] files = Directory.GetFiles(folder);
            foreach (string file in files)
            {
                fileList.Add(file);
            }

            string[] directories = Directory.GetDirectories(folder);
            foreach (string dir in directories)
            {
                GetAllFiles(dir, fileList);
            }
        }

        /// <summary>
        /// 浏览点击按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBrowse_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "所有文件（*.*）|*.*";
            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }
            string[] fileNames = openFileDialog.FileNames;
            calculteFiles(fileNames);
        }

        /// <summary>
        /// 复制点击按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCopy_OnClick(object sender, RoutedEventArgs e)
        {
            string text = textBox1.Text;
            if (string.IsNullOrEmpty(text))
            {
                MessageBox.Show(this, "结果数据为空", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            Clipboard.SetText(text);
            MessageBox.Show("写入剪贴板成功", "提示", MessageBoxButton.OK);
        }

        /// <summary>
        /// 清空点击按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClear_OnClick(object sender, RoutedEventArgs e)
        {
            textBox1.Text = "";
            label1.Content = DEFAULT_CONTENT;
            progressBar1.Value = 0;
            progressBar2.Value = 0;
        }

        /// <summary>
        /// 对比点击按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCompare_OnClick(object sender, RoutedEventArgs e)
        {
            string text1 = textBox1.Text;
            if (string.IsNullOrEmpty(text1))
            {
                MessageBox.Show("结果数据为空", "提示", MessageBoxButton.OK);
                return;
            }
            string text2 = textBox2.Text;
            if (string.IsNullOrEmpty(text2))
            {
                MessageBox.Show("比对数据为空", "提示", MessageBoxButton.OK);
                return;
            }
            int idx = text1.IndexOf(text2);
            if (idx == -1)
            {
                MessageBox.Show("未找到匹配数据", "提示", MessageBoxButton.OK);
                return;
            }
            Keyboard.Focus(textBox1);
            textBox1.SelectionStart = idx;
            textBox1.SelectionLength = text2.Length;
        }

        /// <summary>
        /// 保存点击按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            string text = textBox1.Text;
            if (string.IsNullOrEmpty(text))
            {
                MessageBox.Show("结果数据为空", "提示", MessageBoxButton.OK);
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "文本文档（*.txt）|*.txt|所有文件（*.*）|*.*";
            if (saveFileDialog.ShowDialog() != true)
            {
                return;
            }
            using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                fileStream.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// 关闭点击按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
