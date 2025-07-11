using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MspLibraryGenerationTool
{
    public class jointMspFiles
    {

        public static void JointMspFiles(string directoryPath)
        {
            List<string> subdirectories = new List<string>(Directory.GetDirectories(directoryPath));
            foreach (var path in subdirectories)
            {
                var FileName = directoryPath + Path.GetFileName(path) + ".msp";
                MspFilesIO(path, FileName);
            }
        }
        private static void MspFilesIO(string path, string outFilename)
        {
            var txtFiles = Directory.GetFiles(path, "*.msp");
            using (var wfs = new FileStream(outFilename, FileMode.Create, FileAccess.Write))
            {
                // 結合するファイルを順に読んで、結果ファイルに書き込む
                foreach (var mspFile in txtFiles)
                {
                    var rbuf = new byte[1024 * 1024];

                    using (var rfs = new FileStream(mspFile, FileMode.Open, FileAccess.Read))
                    {
                        var readByte = 0;
                        var leftByte = rfs.Length;
                        while (leftByte > 0)
                        {
                            // 指定のサイズずつファイルを読み込む
                            readByte = rfs.Read(rbuf, 0, (int)Math.Min(rbuf.Length, leftByte));

                            // 読み込んだ内容を結果ファイルに書き込む
                            wfs.Write(rbuf, 0, readByte);

                            // 残りの読み込みバイト数を更新
                            leftByte -= readByte;
                        }
                    }
                }
            }

        }

    }
}
