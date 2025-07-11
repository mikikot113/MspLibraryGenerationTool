using MspLibraryGenerationTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MspLibraryGenerationTool_vs2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //このツールは、公開ライブラリからMS-DIAL用のMSPファイルを変換生成するためのものです。

            //手順
            //・クリーニング
            //  不整合のあるmspレコードを抽出します。
            //  ***_Exclude.txtというファイルを書き出しますので、それを見てプログラムの判定がおかしい場合はプログラムを調整してください。
            //・Ontologyの付与
            //  http://classyfire.wishartlab.com/
            //  ClassyFireClassというものを使用します。
            //  すでにあるライブラリにデータが無く、Ontologyが入力できないものについては***_noOntology.txtというファイルを書き出しますので、
            //  ClassyOntologyUtil.csの中にある説明に従って上記WebPageからデータを取得してください。
            //・SPLASHの付与
            //  クリーニングとOntologyの付与が問題なくなったmspに対して実行してください。すごく時間がかかります。
            //・RT CCSの付与(未実装)
            //  mspに必須ではありません。コードを整理して共有しますので少々お待ちください...

            //編集したいmspファイルを保存したディレクトリを指定(複数のファイルを処理できます)
            var mspFilePath = @"d:\mikikot\Desktop\Tsugawa-san_work\20250710_v20\check";
            //出力先のディレクトリを指定
            var outputFilePath = @"d:\mikikot\Desktop\Tsugawa-san_work\20250710_v20\v20_2";
            if (!Directory.Exists(outputFilePath))
            {
                Directory.CreateDirectory(outputFilePath);
            }
            //Ontologyのライブラリのパスを指定
            var ontologyFilePath = @"F:\takahashi\~central\code\MspLibraryGenerationTool\Library\ClassyFire";


            //mspファイルを処理
            //mspFilePathに保存されたmspファイルのファイル名を取得
            var mspFiles = Directory.GetFiles(mspFilePath, "*.msp");
            foreach (var file in mspFiles)
            {
                var mspStorage = MspParser.MspFileParser(file);
                //クリーニング
                mspStorage = MspCleaningUtil.MspCleaner(mspStorage, Path.GetFileName(file), outputFilePath);
                //Ontologyの付与
                var ontologyList = Directory.GetFiles(ontologyFilePath);
                mspStorage = ClassyOntologyUtil.AddOntology(mspStorage, ontologyList, Path.GetFileName(file), outputFilePath);
                ////SPLASHの付与
                //mspStorage = SplashUtil.SetSplashToMsp(mspStorage); // SPLASHの付与は時間がかかるので、必要な場合のみ実行してください。

                //出力先のディレクトリにmspファイルを保存
                var DateTimeString = DateTime.Now.ToString("yyyyMMddHHmmss");
                var outputFileName = $"{Path.GetFileNameWithoutExtension(file)}_{DateTimeString}.msp";
                using (var sw = new StreamWriter(
                   Path.Combine(outputFilePath, outputFileName), false, Encoding.ASCII))
                {
                    foreach (var item in mspStorage)
                    {
                        if (item != null)
                        {
                            MspParser.writeMspFields(item, sw);
                        }
                    }
                }
            }



            ///misc

            //// posとnegが混在しているmspファイルを分割します。(mspファイルをひとつ指定すると、その保存ディレクトリにpos/negのmspが出力されます)
            //MspCleaningUtil.ModeSeparator("msp-file-path");



            ////複数のmspファイルを読み込んでひとつのmspファイルを作成します。
            //var mspDirectry = @"C:\Users\Public\Documents\mspFiles"; // ここにmspファイルが格納されているディレクトリを指定してください。
            //jointMspFiles.JointMspFiles(mspDirectry);
        }
    }
}
