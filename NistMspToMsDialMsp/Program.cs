using NistMspToMsDialMsp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NistMspToMsDialMsp
{
    class Program
    {
        static void Main(string[] args)
        {
            var workingFolder = @"D:\takahashi\desktop\Tsugawa-san_work\20200721_addLibrary\MonaVolatileGC\";
            // NIST lib2nist.exe でconvertしたSDFファイルからMS-DIAL仕様のmspファイルを作成します。
            // NCDKを利用してInChIKey-SMILESを出力します。
            // InChIKey-SMILESが生成できなかったものの名前が"*_noInchikey.txt"として出力されます。
            //NistSdfToMspConvert.Convert(workingFolder + "apci_msms_nist.SDF", workingFolder + "apci_msms_nist.msp"); 
            //NistSdfToMspConvert.Convert(workingFolder + "biopep_msms_nist.SDF", workingFolder + "biopep_msms_nist.msp");
            //NistSdfToMspConvert.Convert(workingFolder + "hr_msms_nist.SDF", workingFolder + "hr_msms_nist.msp");
            //NistSdfToMspConvert.Convert(workingFolder + "lr_msms_nist.SDF", workingFolder + "lr_msms_nist.msp");

            //ConvertでInChIKey-SMILESが生成できなかったものを、元のSDFファイルから抽出する
            //NistSdfToMspConvert.pickupNoInchikeyCompoundFromSdf(workingFolder + "lr_msms_nist.msp_noInchikey.txt", workingFolder + "lr_msms_nist.SDF", workingFolder + "lr_msms_nist_loss.SDF");
            //NistSdfToMspConvert.pickupNoInchikeyCompoundFromSdf(workingFolder + "hr_msms_nist.msp_noInchikey.txt", workingFolder + "hr_msms_nist.SDF", workingFolder + "hr_msms_nist_loss.SDF");

            /*
             * memo 
             * molconvert smiles hr_msms_nist_loss.SDF -o hr_msms_nist_loss.smiles.txt -g -Y
             * molconvert inchikey hr_msms_nist_loss.SDF -o hr_msms_nist_loss.inchikey.txt -g -Y
             */
            //ConvertでInChIKey-SMILESが生成できなかった化合物のInChIKey-SMILESをどうにかして取ってきて、mspにmergeする
            //NistSdfToMspConvert.mergeInchikeyAndSmilesToMsp(workingFolder + "lr_msms_nist.msp", workingFolder + "\\Name_InChIKey_SMILES.txt");
            //NistSdfToMspConvert.mergeInchikeyAndSmilesToMsp(workingFolder + "hr_msms_nist.msp", workingFolder + "\\Name_InChIKey_SMILES.txt");

            // classyfire ontologyを取得する
            // 
            workingFolder = @"D:\takahashi\desktop\Tsugawa-san_work\20200721_addLibrary\NIST20\ontology_classyfire\";
            //var dictionaryfile = workingFolder + @"\Merged-vs9.txt";// InChiKey-classyfireOntologyのDictionary
            //NistSdfToMspConvert.extractOntologies(workingFolder + "all.inchikey", dictionaryfile, workingFolder + "all.classy");
            // dictionaryに無かったものはMarkUpLanguageExtractionToolのClassyFireAPIを利用して取得
            // 取得できなかったものはClassyFireのHPでSDFを取ってくる
            // sdfからDirect parent nameを取得

            workingFolder = @"d:\mikikot\Desktop\Riken-metadatabase\19_metaboliteClass\annotationlist\temp\";

            //NistSdfToMspConvert.getClassyOntologydataFromSDF(workingFolder + "\\6884856.sdf", workingFolder + "\\6884856.classy");
            // MsFinderDbGeneratorを使用して新しいInchikeyClassyfireDB - VS *を作成する

            //mspにmerge
            //NistSdfToMspConvert.mergeOntologyToMsp(workingFolder + "\\apci_msms_nist.msp", workingFolder + "\\Merged-vs10.txt") ;



            // RT の予測は20200728現在、Rを用いてRetipで計算
            // RTの予測結果のファイル
            workingFolder = @"D:\takahashi\desktop\Tsugawa-san_work\20200721_addLibrary\NIST20\RTCCSPrediction\";
            var RtPredictedFile = workingFolder + "\\DB_pred.txt";

            //CCSは津川さんのライブラリとマッチングを取る。
            var CcsLibraryPos = workingFolder + @"\msdial_ccs_textlibrary_pos.txt";
            var CcsLibraryneg = workingFolder + @"\msdial_ccs_textlibrary_neg.txt";
            //RtCcsMatching.mergeRTandCCSintoMsp(workingFolder + "hr_msms_nist.msp_mergedOntology.msp", RtPredictedFile, CcsLibraryPos, CcsLibraryneg, workingFolder + "hr_msms_nist_final.msp");

            //PosNeg分ける
            workingFolder = @"D:\takahashi\desktop\Tsugawa-san_work\20200721_addLibrary\NIST20\generatedLibrary\";

            //NistSdfToMspConvert.SeparatePositiveNegative(workingFolder + "apci_msms_nist_20200803.msp.txt", workingFolder + "apci_msms_nist_20200803_pos.msp.txt", workingFolder + "apci_msms_nist_20200803_neg.msp.txt");//(string inputfile, string outputPositive, string outputNegative);
            //NistSdfToMspConvert.SeparatePositiveNegative(workingFolder + "lr_msms_nist_20200803.msp.txt", workingFolder + "lr_msms_nist_20200803_pos.msp.txt", workingFolder + "lr_msms_nist_20200803_neg.msp.txt");//(string inputfile, string outputPositive, string outputNegative);
            //NistSdfToMspConvert.SeparatePositiveNegative(workingFolder + "hr_msms_nist_20200803.msp.txt", workingFolder + "hr_msms_nist_20200803_pos.msp.txt", workingFolder + "hr_msms_nist_20200803_neg.msp.txt");//(string inputfile, string outputPositive, string outputNegative);
            //NistSdfToMspConvert.SeparatePositiveNegative(workingFolder + "biopep_msms_nist_20200803.msp.txt", workingFolder + "biopep_msms_nist_20200803_pos.msp.txt", workingFolder + "biopep_msms_nist_20200803_neg.msp.txt");//(string inputfile, string outputPositive, string outputNegative);



            //MoNAのJSONをMS-DIALのMSPに変換する

            ////  1.必要なjsonのデータをtsvに出力 & SMILESのリストを出力
            workingFolder = @"D:\takahashi\desktop\Tsugawa-san_work\20200721_addLibrary\MonaVolatileGC\";
            //MonaGcJsonToMsp.extractMonaJsonToTsv(workingFolder + "MoNA-export-Volatile_FiehnLib.json", workingFolder + "MoNA-Volatile.tsv");

            /// 2.SMILESのリストをmolconvertで必要な情報に変換  <- 構造のクリーニングのために必要
            ///      molconvert smiles (OutFileName).tsv.smiles -o (OutFileName).smiles2 -g -Y -F  //付加イオンを除いたSMILESを生成
            ///      molconvert inchikey  (OutFileName).smiles2 -o (OutFileName).inchikey -g -Y
            ///      molconvert sdf (OutFileName).smiles2 -o (OutFileName).sdf -g -Y
            /// 3.SDFからcxcalcで必要な情報に変換
            ///      cxcalc (OutFileName).sdf -o (OutFileName).formula.txt exactmass formula
            ///
            ///  4.InChiKeyからOntologyを引いてきて加えて出力
            //var dictionaryfile = @"D:\takahashi\desktop\Tsugawa-san_work\20200721_addLibrary\NIST20\ontology_classyfire\Merged-vs10.txt";// InChiKey-classyfireOntologyのDictionary
            //MonaGcJsonToMsp.extractOntologies(workingFolder + "MoNA-Volatile.inchikey", dictionaryfile, workingFolder + "MoNA-Volatile.classy");
            ///  5.チェック用ファイルとして2-4で作成されたデータをMerge
            ///   生成された*.calced.tsvともとのtsvファイルをキュレーション
            var OutFile = workingFolder + "\\MoNA-Volatile";
            //MonaGcJsonToMsp.mergeCalcedData(OutFile + ".inchikey", OutFile + ".classy", OutFile + ".smiles2", OutFile + ".formula.txt", OutFile + ".calced.tsv");

            ///  6.mspデータ出力
            ///  今回は元データがキレイだったのでjsonからそのまま作成する
            //MonaGcJsonToMsp.exportMspFile(workingFolder + "MoNA-export-Volatile_FiehnLib.json", OutFile + ".msp");　// exportMspFile(もとtsvファイル,出力mspファイル名)

            //other 
            //mspからSPLASHをとる
            var inputMsp = @"d:\mikikot\Desktop\Tsugawa-san_work\2023\20231219_MSMS_VS19\check\BioMSMS-Pos-PlaSMA.msp";
            var outputTsv = inputMsp+".splash.txt";
            MspCheck.GetSpectrumSplashFromMsp(inputMsp,outputTsv);

            // File1にFile2のSPLASHが含まれているかチェック。
            var file1 = @"D:\takahashi\desktop\Tsugawa-san_work\20200721_addLibrary\NIST20\nist14_chk\MSMS-Pos-NIST14.msp.txt";
            var file2 = @"D:\takahashi\desktop\Tsugawa-san_work\20200721_addLibrary\NIST20\nist14_chk\NIST20_splash_all.splash";
            var output = @"D:\takahashi\desktop\Tsugawa-san_work\20200721_addLibrary\NIST20\nist14_chk\NIST14-20-compair.txt";
            //MspCheck.compairSplash(file1, file2, output);

            //intensityを整数に
            var path = @"D:\takahashi\desktop\Tsugawa-san_work\20200721_addLibrary\NIST20\generatedLibrary\"; 
            var inputFile = path + @"\old\biopep_msms_nist_20200730.msp.txt";
            var outputFile = path + "biopep_msms_nist_20200803.msp.txt";
            //MspCheck.peakIntensityFroatToInt(inputFile, outputFile);

            //mspのionmodeとadductの整合性チェック
            var checkFolder = @"d:\mikikot\Desktop\Tsugawa-san_work\20231219_MSMS_VS19\check\";
            var files = Directory.GetFiles(checkFolder);
            foreach(var file in files)
            {
                MspCheck.IonModeChecker(file);
            }


        }
    }
}
