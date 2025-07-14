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
            //  misc(下のほう)にある説明に従って上記WebPageからデータを取得してください。
            //・SPLASHの付与
            //  クリーニングとOntologyの付与が問題なくなったmspに対して実行してください。すごく時間がかかります。
            //・RT CCSの付与
            //  mspに必須ではありません。RT、CCSの予測をこのファイルの下のほうに書きましたので、参考にしてください。

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

            /// noOntology.txtの処理
            /// webを利用する場合
            /// http://classyfire.wishartlab.com/#chemical-structure-query-file　にnoOntoligyListを投げる
            /// sdfをDL
            // ClassyFireのsdfファイルから InChIKeyとOntology部分を抜き出す
            //var sdfFolder = @"d:\mikikot\Desktop\Tsugawa-san_work\20231219_MSMS_VS19\MoNAjson\classyfire\";
            //var filename = "11289315.sdf";
            //var outName01 = sdfFolder + filename.Replace(".sdf", "_classy.txt");
            //ClassyOntologyUtil.GetClassyOntologydataFromSDF(sdfFolder + filename,outName01);
            ////// classyFire  InchikeyClassyfireDB - VS * に追加する行を作成
            //////入力形式： [ID(not use)] \t [InChIKey] \t [Direct parent name]
            //////出力形式： [InChIKey] \t [Direct parent name] \t [Direct parent ID]
            //var path2 = @"F:\takahashi\~central\code\MspLibraryGenerationTool\Library\";
            //var chemontIDList = path2 + "CHEMONTID.txt";// oboファイルから抽出したCHEMONTID-ClassOntologyのリスト
            //var classyOnt = outName01; //追加するデータ
            //var outputFileName2 = outName01.Replace(".txt", "_toDB.txt"); // 出力ファイル名
            //ClassyOntologyUtil.GenerateInchikeyClassyfireDBList(classyOnt, chemontIDList, outputFileName2);


            ///RT、CCSの予測
            ///C#上のXGBoostでRT、CCSのpredictionをおこなう
            // NCDKを利用したdescriptorの出力 (string inputFile, string outputFile)
            // inputFile <- InChIKeyとSMILESを含んだテーブルデータを渡す。
            // 1行目(ヘッダー行)が"SMILES"となっている列を認識してdescriptorを算出する。
            // RtCcsPredictOnDotNet.GenerateQsarDescriptorFileVS2();//--old
            //var workingFolder = @"D:\mikikot\Desktop\Tsugawa-san_work\20230906_re-prediction_TUAT\LC25min_lipidomics\ToPrediction\";

            //qsarDescriptorOnNcdk.GenerateQsarDescriptorFileVS4
            //    (workingFolder + @"\LC25min_all_ToPrediction.txt",
            //     workingFolder + @"\LC25min_all_ToPrediction.txt_descriptor_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".txt");

            //qsarDescriptorOnNcdk.GenerateQsarDescriptorFileVS4
            //    (@"E:\6_Projects\PROJECT_CASMI2022\PFP_DB\InChIKeySmilesRtList.txt",
            //     @"E:\6_Projects\PROJECT_CASMI2022\PFP_DB\InChIKeySmilesRtDescriptorList.txt");


            ////モデル作成
            ////training file
            //// 1. header行にRTまたはCCS（目的変数）を含むこと（どちらかひとつ）
            //// 2. 目的変数の列より右にDescriptorを入力したタブ区切りテキストを用意する。（目的変数より列番号の小さい列は予測に使用されない）
            ///
            ////すでにチューニング済みのパラメータを使用する場合
            //var parameters = new RtCcsPredictOnDotNet.TuningParameter()
            //{
            //    nEstimators = 1000, //nrounds int
            //    maxDepth = 7, //int
            //    learningRate = 0.03F, //eta float
            //    gamma = 0F, //float
            //    colSampleByTree = 0.75F, //float
            //    minChildWeight = 10,//int
            //    subsample = 0.5F, //float
            //};
            //var workingFolder =
            //    @"D:\takahashi\desktop\Tsugawa-san_work\20210430_RTprediction\calc";
            //var trainFile = workingFolder + @"\masterRT_NCDK_20210104113720.txt";
            //var output = workingFolder + @"\masterRT_NCDK_20210104113720.model";
            //RtCcsPredictOnDotNet.GeneratePredictionModel("RT", trainFile, output, parameters);
            //RtCcsPredictOnDotNet.GeneratePredictionModel("CCS", trainFile, output, parameters);

            ////tuningして最善解でモデルファイルを作成（指標はRMSE）
            //// 計算後に生成されるtxtファイルに各パラメータ使用時の統計値が出力されている。RMSE(Mean)が最小のパラメータセットが採用されている。
            ////  ほかのパラメータセットを試したい時は上の"チューニング済みのパラメータを使用"を利用
            ////tuningするパラメーターはコード内を参照 
            //var workingFolder =
            // @"E:\6_Projects\PROJECT_CASMI2022\PFP_DB";
            //var trainFile = @"D:\mikikot\Desktop\Tsugawa-san_work\20230906_re-prediction_TUAT\LC25min_lipidomics\ToPrediction\LC25min_all_ToPrediction.txt_descriptor_202309071512.txt";
            //var output = @"D:\mikikot\Desktop\Tsugawa-san_work\20230906_re-prediction_TUAT\LC25min_lipidomics\ToPrediction\NCDK_TUAT_RT_LC25min_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".model";
            //RtCcsPredictOnDotNet.GeneratePredictionModelVS2("RT", trainFile, output);


        }
    }
}
