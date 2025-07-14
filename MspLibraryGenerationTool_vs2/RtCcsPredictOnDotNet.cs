using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XGBoost;


namespace CompMs.MspGenerator
{

    public class DescriptorResultTemp
    {
        public long ID { get; set; }
        public string InChIKey { get; set; }
        public string SMILES { get; set; }
        public Dictionary<string, double> Descriptor { get; set; }
    }

    public class RtCcsPredictOnDotNet
    {
        //public static void GenerateQsarDescriptorFileVS2(string inputFile, string outputFile)
        //{
        //    var SmilesParser = new SmilesParser();

        //    var allDescriptorResultDic = NcdkDescriptor.GenerateNCDKDescriptors("O=C(O)CCCCC"); // Header取得のためのDummy

        //    var allDescriptorHeader = new List<string>();
        //    foreach (var item in allDescriptorResultDic)
        //    {
        //        if (item.Key == "geomShape") { continue; }

        //        allDescriptorHeader.Add(item.Key);
        //    }

        //    var headerLine = string.Empty;
        //    string[] headerArray = null;
        //    var queries = new List<string[]>();

        //    var counter = 0;
        //    using (var sr = new StreamReader(inputFile, true))
        //    {
        //        headerLine = sr.ReadLine();
        //        headerArray = headerLine.ToUpper().Split('\t');
        //        int InChIKey = Array.IndexOf(headerArray, "INCHIKEY");
        //        int SMILES = Array.IndexOf(headerArray, "SMILES");

        //        var line = "";

        //        while ((line = sr.ReadLine()) != null)
        //        {
        //            if (line.Contains("SMILES")) { continue; }
        //            var lineArray = line.Split('\t');
        //            var inchikey = lineArray[InChIKey];
        //            var rawSmiles = lineArray[SMILES];

        //            queries.Add(new string[] { counter.ToString(), inchikey, rawSmiles });
        //            counter++;
        //        }
        //    }

        //    var syncObj = new object();
        //    var results = new List<DescriptorResultTemp>();
        //    //var resultArray = new DescriptorResultTemp[queries.Count];
        //    counter = 0;

        //    var descriptors = new Dictionary<long, Dictionary<string, double>>();
        //    ParallelOptions parallelOptions = new ParallelOptions();
        //    parallelOptions.MaxDegreeOfParallelism = 2;
        //    //Parallel.ForEach(queries, parallelOptions, query =>
        //    //{

        //    //    var id = long.Parse(query[0]);
        //    //    var inchikey = query[1];
        //    //    var smiles = query[2];
        //    //    //var descriptors = new Dictionary<string, double>(NcdkDescriptor.GenerateNCDKDescriptors(smiles));

        //    //    descriptors.Add(id, new Dictionary<string, double>(NcdkDescriptor.GenerateNCDKDescriptors(query)));

        //    //    var result = new DescriptorResultTemp() { ID = id, InChIKey = inchikey, SMILES = smiles, Descriptor = descriptors[id] };
        //    //    //resultArray[id] = result;

        //    //    lock (syncObj)
        //    //    {
        //    //        results.Add(result);
        //    //        counter++;
        //    //        if (!Console.IsOutputRedirected)
        //    //        {
        //    //            Console.Write("Progress {0}/{1}", counter, queries.Count);
        //    //            Console.SetCursorPosition(0, Console.CursorTop);
        //    //        }
        //    //        else
        //    //        {
        //    //            Console.WriteLine("Progress {0}/{1}", counter, queries.Count);
        //    //        }
        //    //    }
        //    //});

        //    Parallel.For(0, queries.Count, parallelOptions, i =>
        //     {
        //         var id = long.Parse(queries[i][0]);
        //         var inchikey = queries[i][1];
        //         var smiles = queries[i][2];
        //         //var descriptors = new Dictionary<string, double>(NcdkDescriptor.GenerateNCDKDescriptors(smiles));

        //         descriptors.Add(id, new Dictionary<string, double>(NcdkDescriptor.GenerateNCDKDescriptors(queries[i])));

        //         var result = new DescriptorResultTemp() { ID = id, InChIKey = inchikey, SMILES = smiles, Descriptor = descriptors[id] };
        //         //resultArray[id] = result;

        //         lock (syncObj)
        //         {
        //             results.Add(result);
        //             counter++;
        //             if (!Console.IsOutputRedirected)
        //             {
        //                 Console.Write("Progress {0}/{1}", counter, queries.Count);
        //                 Console.SetCursorPosition(0, Console.CursorTop);
        //             }
        //             else
        //             {
        //                 Console.WriteLine("Progress {0}/{1}", counter, queries.Count);
        //             }
        //         }
        //     });


        //    //results = resultArray.ToList();

        //    using (var sw = new StreamWriter(outputFile, false, Encoding.ASCII))
        //    {

        //        sw.Write(headerLine);
        //        sw.Write("\t");
        //        sw.WriteLine(string.Join("\t", allDescriptorHeader));

        //        foreach (var result in results.OrderBy(n => n.ID))
        //        {
        //            var descriptor = result.Descriptor;
        //            sw.Write(string.Join("\t", new string[] { result.InChIKey, result.SMILES }));

        //            foreach (var item in allDescriptorHeader)
        //            {
        //                sw.Write("\t");
        //                sw.Write(result.Descriptor[item]);
        //            }
        //            sw.WriteLine("");
        //        }
        //    }
        //}

        //public static void GenerateQsarDescriptorFile(string inputFile, string outputFile)
        //{
        //    var SmilesParser = new SmilesParser();

        //    var allDescriptorResultDic = NcdkDescriptor.GenerateNCDKDescriptors("O=C(O)CCCCC"); // Header取得のためのDummy

        //    var allDescriptorHeader = new List<string>();
        //    foreach (var item in allDescriptorResultDic)
        //    {
        //        if (item.Key == "geomShape") { continue; }

        //        allDescriptorHeader.Add(item.Key);
        //    }

        //    using (var sw = new StreamWriter(outputFile, false, Encoding.ASCII))
        //    {
        //        using (var sr = new StreamReader(inputFile, true))
        //        {
        //            var headerLine = sr.ReadLine();
        //            var headerArray = headerLine.ToUpper().Split('\t');
        //            int InChIKey = Array.IndexOf(headerArray, "INCHIKEY");
        //            int SMILES = Array.IndexOf(headerArray, "SMILES");

        //            sw.Write(headerLine);
        //            sw.Write("\t");
        //            sw.WriteLine(string.Join("\t", allDescriptorHeader));

        //            var line = "";
        //            while ((line = sr.ReadLine()) != null)
        //            {
        //                if (line.Contains("SMILES")) { continue; }
        //                var lineArray = line.Split('\t');
        //                var inchikey = lineArray[InChIKey];
        //                var rawSmiles = lineArray[SMILES];

        //                var iAtomContainer = SmilesParser.ParseSmiles(rawSmiles);
        //                allDescriptorResultDic =
        //                    new Dictionary<string, double>(NcdkDescriptor.GenerateNCDKDescriptors(rawSmiles));
        //                var allDescriptorResult = new List<string>();
        //                foreach (var item in allDescriptorHeader)
        //                {
        //                    if (item == "geomShape") { continue; }

        //                    if (!allDescriptorResultDic.ContainsKey(item))
        //                    {
        //                        allDescriptorResult.Add("NA");
        //                    }
        //                    else
        //                    {
        //                        allDescriptorResult.Add(allDescriptorResultDic[item].ToString());
        //                    }
        //                }
        //                sw.Write(line);
        //                sw.Write("\t");
        //                sw.WriteLine(string.Join("\t", allDescriptorResult));
        //            }
        //        }
        //    }
        //}



        public static void mergeRtAndCcsResultFilesVS2(string resultFile, string rtTrainFile, string rtTestFile, string ccsTrainFile, string ccsTestFile)
        {

            var allResultDic = new Dictionary<string, List<string>>();
            var rtResultDic = RtPredictionOnXgboost(rtTrainFile, rtTestFile);
            var ccsResultDic = CcsPredictionOnXgboost(ccsTrainFile, ccsTestFile);

            var ccsAdductHeaderList = new List<string>();
            foreach (var item in adductscoreDic)
            {
                ccsAdductHeaderList.Add(item.Key);
            }

            using (var sw = new StreamWriter(resultFile, false, Encoding.ASCII))
            {
                var headerList = new List<string>();

                headerList.Add("InChIKey");
                headerList.Add("SMILES");
                headerList.Add("RT");
                headerList.AddRange(ccsAdductHeaderList);
                var headerItem = string.Join("\t", headerList);
                sw.WriteLine(headerItem);

                foreach (var rtItem in rtResultDic)
                {
                    var writeLineItem = new List<string>();
                    writeLineItem.Add(rtItem.Key);
                    writeLineItem.Add(rtItem.Value.ToString());
                    // add CCS result 
                    if (ccsResultDic.ContainsKey(rtItem.Key))
                    {
                        var ccsResultValueList = new List<string>();
                        var ccsResult = ccsResultDic[rtItem.Key];
                        foreach (var adduct in ccsAdductHeaderList)
                        {
                            writeLineItem.Add(ccsResult[adduct].ToString());
                        }
                    }
                    sw.WriteLine(string.Join("\t", writeLineItem));
                }
                //////// to test --print ccs only item
                ////foreach (var ccsItem in ccsResultDic)
                ////{
                ////    var writeLineItem = new List<string>();
                ////    writeLineItem.Add(ccsItem.Key);

                ////    // add CCS result 
                ////    if (ccsResultDic.ContainsKey(ccsItem.Key))
                ////    {

                ////        var ccsResultValueList = new List<string>();
                ////        var ccsResult = ccsResultDic[ccsItem.Key];
                ////        foreach (var adduct in ccsAdductHeaderList)
                ////        {
                ////            writeLineItem.Add(ccsResult[adduct].ToString());
                ////        }
                ////    }
                ////    sw.WriteLine(string.Join("\t", writeLineItem));
                ////}

            }

        }

        public static Dictionary<string, float> RtPredictionOnXgboost(string rtTrainModel, string testFile)
        {
            var inchikeyList = new List<string>();

            // read testFile and set to array
            var testArrayList = new List<XGBArray>();
            var vectorsTest = new List<XGVector<Array>>();

            using (var sr = new StreamReader(testFile, true))
            {
                var headerLine = sr.ReadLine();
                headerLine = headerLine.Replace(",", "\t");
                headerLine = headerLine.Replace("-", "_");
                headerLine = headerLine.Replace(".", "_");
                headerLine = headerLine.Replace("*", "_asterisk");

                var headerArray = headerLine.Split('\t');
                var headerArrayUpper = headerLine.ToUpper().Split('\t');

                int smilesOrder = Array.IndexOf(headerArrayUpper, "SMILES");
                int inchikeyOrder = Array.IndexOf(headerArrayUpper, "INCHIKEY");

                int descriptorStartOrder = Math.Max(smilesOrder, inchikeyOrder) + 1;

                var line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Replace(",", "\t");
                    var lineArray = line.Split('\t');
                    var itemlist = new List<float>();
                    var target = 0.0f;
                    if (smilesOrder > -1)
                    {
                        inchikeyList.Add(lineArray[inchikeyOrder] + '\t' + lineArray[smilesOrder]);
                    }
                    else
                    {
                        inchikeyList.Add(lineArray[inchikeyOrder] + '\t');
                    }

                    for (var i = descriptorStartOrder; i < lineArray.Length; i++)
                    {
                        var item = float.Parse(lineArray[i]);
                        if (headerArray[i] == "RT")
                        {
                            target = item;
                        }
                        else
                        {
                            itemlist.Add(item);
                        }
                    }
                    XGVector<Array> newVector = new XGVector<Array>();
                    float[] recordsList = (float[])itemlist.ToArray();
                    //newVector.Original = recordsList;
                    newVector.Features = recordsList;
                    newVector.Target = target;
                    vectorsTest.Add(newVector);
                }
            }

            var xgbc = XGBRegressor.LoadRegressorFromFile(rtTrainModel);

            XGBArray arrTest = ConvertToXGBArray(vectorsTest);
            var outcomeTest = xgbc.Predict(arrTest.Vectors);

            var rtDic = new Dictionary<string, float>();
            for (int i = 0; i < inchikeyList.Count; i++)
            {
                if (rtDic.ContainsKey(inchikeyList[i])) { continue; };
                rtDic.Add(inchikeyList[i], outcomeTest[i]);
            }

            //Console.ReadKey();
            return rtDic;

        }

        public static Dictionary<string, Dictionary<string, float>> CcsPredictionOnXgboost(string ccsTrainModel, string ccsTestFile)
        {
            var inchikeyList = new List<string>();

            // read testFile and set to array
            var testArrayList = new List<XGBArray>();
            var vectorsTest = new List<XGVector<Array>>();

            using (var sr = new StreamReader(ccsTestFile, true))
            {
                var headerLine = sr.ReadLine();
                headerLine = headerLine.Replace(",", "\t");
                headerLine = headerLine.Replace("-", "_");
                headerLine = headerLine.Replace(".", "_");
                headerLine = headerLine.Replace("*", "_asterisk");

                var headerArray = headerLine.Split('\t');
                var headerArrayUpper = headerLine.ToUpper().Split('\t');

                int smilesOrder = Array.IndexOf(headerArrayUpper, "SMILES");
                int inchikeyOrder = Array.IndexOf(headerArrayUpper, "INCHIKEY");


                var line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Replace(",", "\t");
                    var lineArray = line.Split('\t');
                    var itemlist = new List<float>();
                    var target = 0.0f;
                    int descriptorStartOrder = Math.Max(smilesOrder, inchikeyOrder) + 1;

                    if (smilesOrder > -1)
                    {
                        inchikeyList.Add(lineArray[inchikeyOrder] + '\t' + lineArray[smilesOrder]);
                    }
                    else
                    {
                        inchikeyList.Add(lineArray[inchikeyOrder] + '\t');
                    }

                    for (var i = descriptorStartOrder; i < lineArray.Length; i++)
                    {
                        var item = float.Parse(lineArray[i]);
                        if (headerArray[i] == "AdductScore")
                        {
                            continue;
                        }
                        if (headerArray[i] == "CCS")
                        {
                            target = item;
                        }
                        else
                        {
                            itemlist.Add(item);
                        }

                    }
                    foreach (var adductscore in adductscoreDic)
                    {
                        XGVector<Array> newVector = new XGVector<Array>();
                        var itemlist2 = new List<float>(itemlist);
                        itemlist2.Insert(0, adductscore.Value);
                        float[] recordsList = (float[])itemlist2.ToArray();
                        newVector.Original = recordsList;
                        newVector.Features = recordsList;
                        //newVector.Target = target;
                        vectorsTest.Add(newVector);

                    }
                }
            }

            var xgbc = XGBRegressor.LoadRegressorFromFile(ccsTrainModel);

            XGBArray arrTest = ConvertToXGBArray(vectorsTest);
            var outcomeTest = xgbc.Predict(arrTest.Vectors);

            var ccsResultDic = new Dictionary<string, Dictionary<string, float>>();
            var count = 0;

            for (int i = 0; i < inchikeyList.Count; i++)
            {
                if (ccsResultDic.ContainsKey(inchikeyList[i]))
                {
                    count = count + adductscoreDic.Count;
                    continue;
                };
                var ccsAdductResult = new Dictionary<string, float>();
                foreach (var item in adductscoreDic)
                {
                    ccsAdductResult.Add(item.Key, outcomeTest[count]);
                    count = count + 1;
                }
                ccsResultDic.Add(inchikeyList[i], ccsAdductResult);
            }

            //Console.ReadKey();
            return ccsResultDic;
        }

        public static Dictionary<string, float> RtPredictionOnXgboostWithFit(string rtTrainFile, string testFile)
        {
            // read trainFile and set to array
            var vectorsTrain = new List<XGVector<Array>>();
            var inchikeyList = new List<string>();

            using (var sr = new StreamReader(rtTrainFile, true))
            {
                var headerLine = sr.ReadLine();
                headerLine = headerLine.Replace("-", "_");
                headerLine = headerLine.Replace(".", "_");
                headerLine = headerLine.Replace("*", "_asterisk");

                var headerArray = headerLine.Split('\t');
                var headerArrayUpper = headerLine.ToUpper().Split('\t');
                int descriptorStartOrder = Array.IndexOf(headerArrayUpper, "RT");

                var line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    var lineArray = line.Split('\t');
                    var itemlist = new List<float>();
                    var target = 0.0f;
                    for (var i = descriptorStartOrder; i < lineArray.Length; i++)
                    {
                        var item = float.Parse(lineArray[i]);
                        if (headerArray[i] == "RT")
                        {
                            target = item;
                        }
                        else
                        {
                            itemlist.Add(item);
                        }
                    }
                    XGVector<Array> newVector = new XGVector<Array>();
                    float[] recordsList = (float[])itemlist.ToArray();
                    //newVector.Original = recordsList;
                    newVector.Features = recordsList;
                    newVector.Target = target;
                    vectorsTrain.Add(newVector);
                }
            }

            // read testFile and set to array
            var testArrayList = new List<XGBArray>();
            var vectorsTest = new List<XGVector<Array>>();

            using (var sr = new StreamReader(testFile, true))
            {
                var headerLine = sr.ReadLine();
                headerLine = headerLine.Replace("-", "_");
                headerLine = headerLine.Replace(".", "_");
                headerLine = headerLine.Replace("*", "_asterisk");

                var headerArray = headerLine.Split('\t');
                var headerArrayUpper = headerLine.ToUpper().Split('\t');

                int smilesOrder = Array.IndexOf(headerArrayUpper, "SMILES");
                int inchikeyOrder = Array.IndexOf(headerArrayUpper, "INCHIKEY");

                int descriptorStartOrder = Math.Max(smilesOrder, inchikeyOrder) + 1;

                var line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    var lineArray = line.Split('\t');
                    var itemlist = new List<float>();
                    var target = 0.0f;
                    if (smilesOrder > -1)
                    {
                        inchikeyList.Add(lineArray[inchikeyOrder] + '\t' + lineArray[smilesOrder]);
                    }
                    else
                    {
                        inchikeyList.Add(lineArray[inchikeyOrder]);
                    }

                    for (var i = descriptorStartOrder; i < lineArray.Length; i++)
                    {
                        var item = float.Parse(lineArray[i]);
                        if (headerArray[i] == "RT")
                        {
                            target = item;
                        }
                        else
                        {
                            itemlist.Add(item);
                        }


                    }
                    XGVector<Array> newVector = new XGVector<Array>();
                    float[] recordsList = (float[])itemlist.ToArray();
                    //newVector.Original = recordsList;
                    newVector.Features = recordsList;
                    newVector.Target = target;
                    vectorsTest.Add(newVector);
                }
            }
            int maxDepth = 5; //default=3; use tune result
            float learningRate = 0.02F;
            int nEstimators = 1000;
            bool silent = true;
            string objective = "reg:linear";
            int nThread = -1;
            float gamma = 1; // default=0 on R 
            int minChildWeight = 10; // default=1 on R 
            int maxDeltaStep = 0;
            float subsample = 0.5F; // default=1 on R 
            float colSampleByTree = 1; // default=1 on R 
            float colSampleByLevel = 1;
            float regAlpha = 0; // default=0 on R 
            float regLambda = 1; // default=1 on R 
            float scalePosWeight = 1;
            float baseScore = 0.5F;
            int seed = 0;
            float missing = float.NaN;

            var xgbc = new XGBoost.XGBRegressor(maxDepth, learningRate, nEstimators, silent,
                objective, nThread, gamma, minChildWeight, maxDeltaStep,
                subsample, colSampleByTree, colSampleByLevel, regAlpha, regLambda,
                scalePosWeight, baseScore, seed, missing);
            XGBArray arrTrain = ConvertToXGBArray(vectorsTrain);
            xgbc.Fit(arrTrain.Vectors, arrTrain.Target);


            XGBArray arrTest = ConvertToXGBArray(vectorsTest);
            var outcomeTest = xgbc.Predict(arrTest.Vectors);

            var rtDic = new Dictionary<string, float>();
            for (int i = 0; i < inchikeyList.Count; i++)
            {
                if (rtDic.ContainsKey(inchikeyList[i])) { continue; };
                rtDic.Add(inchikeyList[i], outcomeTest[i]);
            }

            //Console.ReadKey();
            return rtDic;

        }

        public static Dictionary<string, Dictionary<string, float>> CcsPredictionOnXgboostWithFit(string ccsTrainFile, string ccsTestFile)
        {
            // read trainFile and set to array
            var trainArrayList = new List<XGBArray>();
            var vectorsTrain = new List<XGVector<Array>>();
            var inchikeyList = new List<string>();

            using (var sr = new StreamReader(ccsTrainFile, true))
            {
                var headerLine = sr.ReadLine();
                headerLine = headerLine.Replace("-", "_");
                headerLine = headerLine.Replace(".", "_");
                headerLine = headerLine.Replace("*", "_asterisk");

                var headerArray = headerLine.Split('\t');
                var headerArrayUpper = headerLine.ToUpper().Split('\t');
                int descriptorStartOrder = Array.IndexOf(headerArrayUpper, "CCS");

                var line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    var lineArray = line.Split('\t');
                    var itemlist = new List<float>();
                    var target = 0.0f;
                    for (var i = descriptorStartOrder; i < lineArray.Length; i++)
                    {
                        var item = float.Parse(lineArray[i]);
                        if (headerArray[i] == "CCS")
                        {
                            target = item;
                        }
                        else
                        {
                            itemlist.Add(item);
                        }
                    }
                    XGVector<Array> newVector = new XGVector<Array>();
                    float[] recordsList = (float[])itemlist.ToArray();
                    newVector.Original = recordsList;
                    newVector.Features = recordsList;
                    newVector.Target = target;
                    vectorsTrain.Add(newVector);
                }
            }

            // read testFile and set to array
            var testArrayList = new List<XGBArray>();
            var vectorsTest = new List<XGVector<Array>>();

            using (var sr = new StreamReader(ccsTestFile, true))
            {
                var headerLine = sr.ReadLine();
                headerLine = headerLine.Replace("-", "_");
                headerLine = headerLine.Replace(".", "_");
                headerLine = headerLine.Replace("*", "_asterisk");

                var headerArray = headerLine.Split('\t');
                var headerArrayUpper = headerLine.ToUpper().Split('\t');

                int smilesOrder = Array.IndexOf(headerArrayUpper, "SMILES");
                int inchikeyOrder = Array.IndexOf(headerArrayUpper, "INCHIKEY");


                var line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    var lineArray = line.Split('\t');
                    var itemlist = new List<float>();
                    var target = 0.0f;
                    int descriptorStartOrder = Math.Max(smilesOrder, inchikeyOrder) + 1;

                    if (smilesOrder > -1)
                    {
                        inchikeyList.Add(lineArray[inchikeyOrder] + '\t' + lineArray[smilesOrder]);
                    }
                    else
                    {
                        inchikeyList.Add(lineArray[inchikeyOrder]);
                    }

                    for (var i = descriptorStartOrder; i < lineArray.Length; i++)
                    {
                        var item = float.Parse(lineArray[i]);
                        if (headerArray[i] == "AdductScore")
                        {
                            continue;
                        }
                        if (headerArray[i] == "CCS")
                        {
                            target = item;
                        }
                        else
                        {
                            itemlist.Add(item);
                        }

                    }
                    foreach (var adductscore in adductscoreDic)
                    {
                        XGVector<Array> newVector = new XGVector<Array>();
                        var itemlist2 = new List<float>(itemlist);
                        itemlist2.Insert(0, adductscore.Value);
                        float[] recordsList = (float[])itemlist2.ToArray();
                        newVector.Original = recordsList;
                        newVector.Features = recordsList;
                        //newVector.Target = target;
                        vectorsTest.Add(newVector);

                    }
                }
            }

            int maxDepth = 7; //default=3; use tune result
            float learningRate = 0.025F;
            int nEstimators = 500;
            bool silent = true;
            string objective = "reg:linear";
            int nThread = -1;
            float gamma = 0.00F;
            int minChildWeight = 0;
            int maxDeltaStep = 0;
            float subsample = 1F;
            float colSampleByTree = 0.75F;
            float colSampleByLevel = 1;
            float regAlpha = 0;
            float regLambda = 1;// default=1 on R 
            float scalePosWeight = 1;
            float baseScore = 0.5F;
            int seed = 0;
            float missing = float.NaN;

            var xgbc = new XGBoost.XGBRegressor(maxDepth, learningRate, nEstimators, silent,
                objective, nThread, gamma, minChildWeight, maxDeltaStep, subsample,
                colSampleByTree, colSampleByLevel, regAlpha, regLambda,
                scalePosWeight, baseScore, seed, missing);
            XGBArray arrTrain = ConvertToXGBArray(vectorsTrain);
            xgbc.Fit(arrTrain.Vectors, arrTrain.Target);


            XGBArray arrTest = ConvertToXGBArray(vectorsTest);
            var outcomeTest = xgbc.Predict(arrTest.Vectors);

            var ccsResultDic = new Dictionary<string, Dictionary<string, float>>();
            var count = 0;

            for (int i = 0; i < inchikeyList.Count; i++)
            {
                if (ccsResultDic.ContainsKey(inchikeyList[i]))
                {
                    count = count + adductscoreDic.Count;
                    continue;
                };
                var ccsAdductResult = new Dictionary<string, float>();
                foreach (var item in adductscoreDic)
                {
                    ccsAdductResult.Add(item.Key, outcomeTest[count]);
                    count = count + 1;
                }
                ccsResultDic.Add(inchikeyList[i], ccsAdductResult);
            }

            //xgbc.SaveModelToFile(ccsTrainFile + ".model");

            //Console.ReadKey();
            return ccsResultDic;
        }

        public static void ExtractDescriptorToPredict(string descriptorFile, string descriptorListFile)
        {
            var descriptorList = new List<string>();
            var newFileName = Path.GetDirectoryName(descriptorFile) + "\\" + Path.GetFileNameWithoutExtension(descriptorFile) + "_Extracted.tsv";
            using (var srList = new StreamReader(descriptorListFile, true))
            {
                var headerLine = srList.ReadLine();
                var line = "";
                while ((line = srList.ReadLine()) != null)
                {
                    var lineArray = line.Split(' ');
                    descriptorList.Add(lineArray[1]);
                }
                descriptorList.Add("RT");
                descriptorList.Add("CCS");
                descriptorList.Add("AdductScore");

            }

            using (var sw = new StreamWriter(newFileName, false, Encoding.ASCII))
            {
                using (var sr = new StreamReader(descriptorFile, true))
                {
                    var headerLine = sr.ReadLine();
                    headerLine = headerLine.Replace("-", ".");
                    headerLine = headerLine.Replace("*", ".");

                    var headerArray = headerLine.Split('\t');
                    var headerArrayUpper = headerLine.ToUpper().Split('\t');
                    var descriptorStartOrder = Math.Max(Array.IndexOf(headerArrayUpper, "INCHIKEY"), Array.IndexOf(headerArrayUpper, "SMILES")) + 1;

                    var newHeaderList = new List<string>();
                    for (int i = 0; i < descriptorStartOrder; i++)
                    {
                        newHeaderList.Add(headerArray[i]);
                    }
                    for (int i = descriptorStartOrder; i < headerArray.Length; i++)
                    {
                        if (descriptorList.Contains(headerArray[i]))
                        {
                            newHeaderList.Add(headerArray[i]);
                        }
                    }

                    var newHeaderLine = string.Join("\t", newHeaderList);
                    sw.WriteLine(newHeaderLine);

                    var line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        var lineArray = line.Split('\t');
                        var newLineList = new List<string>();
                        for (int i = 0; i < descriptorStartOrder; i++)
                        {
                            newLineList.Add(lineArray[i]);
                        }

                        for (int i = descriptorStartOrder; i < lineArray.Length; i++)
                        {
                            if (descriptorList.Contains(headerArray[i]))
                            {
                                newLineList.Add(lineArray[i]);
                            }
                        }

                        sw.WriteLine(string.Join("\t", newLineList));


                    }
                }

            }

        }

        public static void ExtractDescriptorToPredictFromPadel(string padelOutFileName, string rtDescriptorListFile, string ccsDescriptorListFile)
        {
            var newRtDescriptorFileName = Path.GetDirectoryName(padelOutFileName) + "\\" + Path.GetFileNameWithoutExtension(padelOutFileName) + "_ExtractedFromPadelResult_RT.tsv";
            var newCcsDescriptorFileName = Path.GetDirectoryName(padelOutFileName) + "\\" + Path.GetFileNameWithoutExtension(padelOutFileName) + "_ExtractedFromPadelResult_CCS.tsv";

            var rtSelectedDescriptorHeader = "Name\tRT";
            var ccsSelectedDescriptorHeader = "Name\tCCS\tMass";

            using (var sr = new StreamReader(rtDescriptorListFile, true))
            {
                while (sr.Peek() > -1)
                {
                    var line = sr.ReadLine();
                    rtSelectedDescriptorHeader = rtSelectedDescriptorHeader + "\t" + line;
                }
            }
            using (var sr = new StreamReader(ccsDescriptorListFile, false))
            {
                while (sr.Peek() > -1)
                {
                    var line = sr.ReadLine();
                    ccsSelectedDescriptorHeader = ccsSelectedDescriptorHeader + "\t" + line;
                }
            }

            using (var sw = new StreamWriter(newRtDescriptorFileName, false, Encoding.ASCII))
            {
                using (var sr = new StreamReader(padelOutFileName, true))
                {
                    var headerLine = sr.ReadLine();
                    var headerArray = headerLine.Split(',');
                    var selectDescriptorArray = rtSelectedDescriptorHeader.Split('\t');

                    sw.WriteLine(rtSelectedDescriptorHeader.Replace("Name", "InChIKey"));

                    var line = "";

                    while ((line = sr.ReadLine()) != null)
                    {
                        var lineArray = line.Split(',');
                        var newLineList = new List<string>();
                        var lineDic = new Dictionary<string, string>();

                        for (int i = 0; i < lineArray.Length; i++)
                        {
                            lineDic.Add(headerArray[i], lineArray[i]);
                        }
                        foreach (var item in selectDescriptorArray)
                        {
                            if (lineDic.ContainsKey(item))
                            {
                                newLineList.Add(lineDic[item]);
                            }
                            else
                            {
                                newLineList.Add("0"); // maybe case of "RT"
                            }
                        }

                        sw.WriteLine(string.Join("\t", newLineList).Replace("\"", ""));
                    }
                }
            }
            using (var sw = new StreamWriter(newCcsDescriptorFileName, false, Encoding.ASCII))
            {
                using (var sr = new StreamReader(padelOutFileName, true))
                {
                    var headerLine = sr.ReadLine();
                    var headerArray = headerLine.Split(',');
                    var selectDescriptorArray = ccsSelectedDescriptorHeader.Split('\t');

                    sw.WriteLine(ccsSelectedDescriptorHeader.Replace("Name", "InChIKey"));

                    var line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        var lineArray = line.Split(',');
                        var newLineList = new List<string>();
                        var lineDic = new Dictionary<string, string>();

                        for (int i = 0; i < lineArray.Length; i++)
                        {
                            lineDic.Add(headerArray[i], lineArray[i]);
                        }
                        foreach (var item in selectDescriptorArray)
                        {
                            if (lineDic.ContainsKey(item))
                            {
                                newLineList.Add(lineDic[item]);
                            }
                            else if (item == "Mass")
                            {
                                newLineList.Add(lineDic["MW"]);
                            }
                            else
                            {
                                newLineList.Add("0");
                            }
                        }

                        sw.WriteLine(string.Join("\t", newLineList).Replace("\"", ""));
                    }
                }
            }
        }

        public static void GeneratePredictionModel(string prediction, string trainFile, string modelFile, TuningParameter parameters)
        {
            // read trainFile and set to array
            var trainArrayList = new List<XGBArray>();
            var vectorsTrain = new List<XGVector<Array>>();
            var inchikeyList = new List<string>();

            using (var sr = new StreamReader(trainFile, true))
            {
                var headerLine = sr.ReadLine();
                headerLine = headerLine.Replace("-", "_");
                headerLine = headerLine.Replace(".", "_");
                headerLine = headerLine.Replace("*", "_asterisk");

                var headerArray = headerLine.Split('\t');
                var headerArrayUpper = headerLine.ToUpper().Split('\t');
                int descriptorStartOrder = Array.IndexOf(headerArrayUpper, prediction);

                var line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    var lineArray = line.Split('\t');
                    var itemlist = new List<float>();
                    var target = 0.0f;
                    for (var i = descriptorStartOrder; i < lineArray.Length; i++)
                    {
                        var item = float.Parse(lineArray[i]);
                        if (headerArray[i] == prediction)
                        {
                            target = item;
                        }
                        else
                        {
                            itemlist.Add(item);
                        }
                    }
                    XGVector<Array> newVector = new XGVector<Array>();
                    float[] recordsList = (float[])itemlist.ToArray();
                    newVector.Original = recordsList;
                    newVector.Features = recordsList;
                    newVector.Target = target;
                    vectorsTrain.Add(newVector);
                }
            }
            // use tune result
            int nEstimators = parameters.nEstimators; //nrounds
            int maxDepth = parameters.maxDepth; //default=3; use tune result
            float learningRate = parameters.learningRate; //eta
            float gamma = parameters.gamma;
            float colSampleByTree = parameters.colSampleByTree;
            int minChildWeight = parameters.minChildWeight;
            float subsample = parameters.subsample;
            bool silent = true;
            string objective = "reg:linear";
            int nThread = -1;
            int maxDeltaStep = 0;
            float colSampleByLevel = 1;
            float regAlpha = 0;
            float regLambda = 1;// default=1 on R 
            float scalePosWeight = 1;
            float baseScore = 0.5F;
            int seed = 0;
            float missing = float.NaN;

            var xgbc = new XGBoost.XGBRegressor(maxDepth, learningRate, nEstimators, silent,
                objective, nThread, gamma, minChildWeight, maxDeltaStep,
                subsample, colSampleByTree, colSampleByLevel,
                regAlpha, regLambda, scalePosWeight, baseScore, seed, missing);
            XGBArray arrTrain = ConvertToXGBArray(vectorsTrain);
            xgbc.Fit(arrTrain.Vectors, arrTrain.Target);

            xgbc.SaveModelToFile(modelFile);

        }

        public static void GeneratePredictionModelVS2(string prediction, string trainFile, string output)
        {
            // read trainFile and set to array
            var vectorsTrain = new List<XGVector<Array>>();
            //var trainArrayList = new List<XGBArray>();
            //var inchikeyList = new List<string>();

            using (var sr = new StreamReader(trainFile, true))
            {
                var headerLine = sr.ReadLine();
                headerLine = headerLine.Replace("-", "_");
                headerLine = headerLine.Replace(".", "_");
                headerLine = headerLine.Replace("*", "_asterisk");

                var headerArray = headerLine.Split('\t');
                var headerArrayUpper = headerLine.ToUpper().Split('\t');
                int descriptorStartOrder = Array.IndexOf(headerArrayUpper, prediction);

                var line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    var lineArray = line.Split('\t');
                    var itemlist = new List<float>();
                    var target = 0.0f;
                    for (var i = descriptorStartOrder; i < lineArray.Length; i++)
                    {
                        var item = float.Parse(lineArray[i]);
                        if (headerArray[i] == prediction)
                        {
                            target = item;
                        }
                        else
                        {
                            itemlist.Add(item);
                        }
                    }
                    XGVector<Array> newVector = new XGVector<Array>();
                    float[] recordsList = (float[])itemlist.ToArray();
                    newVector.Original = recordsList;
                    newVector.Features = recordsList;
                    newVector.Target = target;
                    vectorsTrain.Add(newVector);
                }
            }

            var parameters = XgbTuneVS2(vectorsTrain, output);

            // use tune result
            int nEstimators = parameters.nEstimators; //nrounds
            int maxDepth = parameters.maxDepth; //default=3; use tune result
            float learningRate = parameters.learningRate; //eta
            float gamma = parameters.gamma;
            float colSampleByTree = parameters.colSampleByTree;
            int minChildWeight = parameters.minChildWeight;
            float subsample = parameters.subsample;
            bool silent = true;
            string objective = "reg:linear";
            int nThread = -1;
            int maxDeltaStep = 0;
            float colSampleByLevel = 1;
            float regAlpha = 0;
            float regLambda = 1;// default=1 on R 
            float scalePosWeight = 1;
            float baseScore = 0.5F;
            int seed = 0;
            float missing = float.NaN;

            var xgbc = new XGBoost.XGBRegressor(maxDepth, learningRate, nEstimators, silent,
                objective, nThread, gamma, minChildWeight, maxDeltaStep,
                subsample, colSampleByTree, colSampleByLevel,
                regAlpha, regLambda, scalePosWeight, baseScore, seed, missing);
            XGBArray arrTrain = ConvertToXGBArray(vectorsTrain);
            xgbc.Fit(arrTrain.Vectors, arrTrain.Target);

            xgbc.SaveModelToFile(output);

        }


        public static TuningParameter XgbTune(List<XGVector<Array>> vectorsTrain, string output)
        {
            var cval = 7;
            var cvalTrainset = new Dictionary<int, List<XGVector<Array>>>();
            var cvalTestset = new Dictionary<int, List<XGVector<Array>>>();
            var correctListSet = new Dictionary<int, List<float>>();

            for (int cvset = 0; cvset < cval; cvset++)
            {
                var trainset = new List<XGVector<Array>>();
                var testset = new List<XGVector<Array>>();
                var correctList = new List<float>();

                for (int i = 0; i < vectorsTrain.Count; i++)
                {
                    if (i + cvset < vectorsTrain.Count)
                    {
                        if (i % cval == 0)
                        {
                            testset.Add(vectorsTrain[i + cvset]);
                            correctList.Add(vectorsTrain[i + cvset].Target);
                        }
                        else
                        {
                            trainset.Add(vectorsTrain[i]);
                        }
                    }
                }

                cvalTrainset.Add(cvset, trainset);
                cvalTestset.Add(cvset, testset);
                correctListSet.Add(cvset, correctList);

            }

            var nEstimatorsTune = new List<int>() { 500, 600, 700, 800, 900, 1000 };
            var maxDepthTune = new List<int>() { 5, 6, 7, 8, 9, 10 };
            var learningRateTune = new List<float>() { 0.025F, 0.05F, 0.1F };
            var gammaTune = new List<float>() { 0, 0.01F };
            var colSampleByTreeTune = new List<float>() { 0.75F, 1 };
            var minChildWeightTune = new List<int>() { 1 };
            var subsampleTune = new List<float>() { 1 };

            ////to check
            //var nEstimatorsTune = new List<int>() { 500 };
            //var maxDepthTune = new List<int>() { 7 };
            //var learningRateTune = new List<float>() { 0.025F, 0.05F };
            //var gammaTune = new List<float>() { 0, 0.01F };
            //var colSampleByTreeTune = new List<float>() { 0.75F, 1 };
            //var minChildWeightTune = new List<int>() { 0, 1 };
            //var subsampleTune = new List<float>() { 0.5F };

            var id2Param = new Dictionary<int, TuningParameter>();
            var id2Result = new Dictionary<int, double>();
            var id2ResultStatSet = new Dictionary<int, Dictionary<string, double>>();

            var icounter = 0;

            ////check result output
            using (var sw = new StreamWriter(output + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt", false, Encoding.ASCII))
            {
                sw.WriteLine("CV=" + cval);

                sw.WriteLine(string.Join("\t", new string[]
                    {
                        "id", "learningRate", "maxDepth", "gamma", "colSampleByTree", "minChildWeight", "subsample", "nEstimators",
                        "RMSE(Mean)","RSquared(Mean)","MAE(Mean)","RMSESD","RSquaredSD","MAESD"
                     }));

                for (int i = 0; i < nEstimatorsTune.Count; i++)
                {
                    for (int j = 0; j < maxDepthTune.Count; j++)
                    {
                        for (int k = 0; k < learningRateTune.Count; k++)
                        {
                            for (int l = 0; l < gammaTune.Count; l++)
                            {
                                for (int m = 0; m < colSampleByTreeTune.Count; m++)
                                {
                                    for (int n = 0; n < minChildWeightTune.Count; n++)
                                    {
                                        for (int o = 0; o < subsampleTune.Count; o++)
                                        {
                                            var tParam = new TuningParameter()
                                            {
                                                nEstimators = nEstimatorsTune[i],
                                                maxDepth = maxDepthTune[j],
                                                learningRate = learningRateTune[k],
                                                gamma = gammaTune[l],
                                                colSampleByTree = colSampleByTreeTune[m],
                                                minChildWeight = minChildWeightTune[n],
                                                subsample = subsampleTune[o],
                                            };
                                            // 
                                            int nEstimators = tParam.nEstimators; //nrounds
                                            int maxDepth = tParam.maxDepth; //
                                            float learningRate = tParam.learningRate; //eta
                                            float gamma = tParam.gamma;
                                            float colSampleByTree = tParam.colSampleByTree;
                                            int minChildWeight = tParam.minChildWeight;
                                            float subsample = tParam.subsample;
                                            bool silent = true;
                                            string objective = "reg:linear";
                                            int nThread = -1;
                                            int maxDeltaStep = 0;
                                            float colSampleByLevel = 1;
                                            float regAlpha = 0;
                                            float regLambda = 1;// default=1 on R 
                                            float scalePosWeight = 1;
                                            float baseScore = 0.5F;
                                            int seed = 0;
                                            float missing = float.NaN;

                                            var xgbc = new XGBoost.XGBRegressor(maxDepth, learningRate, nEstimators, silent,
                                                objective, nThread, gamma, minChildWeight, maxDeltaStep,
                                                subsample, colSampleByTree, colSampleByLevel,
                                                regAlpha, regLambda, scalePosWeight, baseScore, seed, missing);

                                            double[] rmseValues = new double[cval];
                                            double[] rsquaredValues = new double[cval];
                                            double[] maeValues = new double[cval];
                                            for (int cvset = 0; cvset < cval; cvset++)
                                            {

                                                XGBArray arrTrain = ConvertToXGBArray(cvalTrainset[cvset]);
                                                xgbc.Fit(arrTrain.Vectors, arrTrain.Target);
                                                XGBArray arrTest = ConvertToXGBArray(cvalTestset[cvset]);
                                                var outcomeTest = xgbc.Predict(arrTest.Vectors).ToList();
                                                var correctList = new List<float>(correctListSet[cvset]);

                                                if (outcomeTest.Count != correctList.Count)
                                                {
                                                    Console.Write("correctList and outcomeTest is not same number");
                                                }
                                                rmseValues[cvset] = getRmse(outcomeTest, correctList);
                                                rsquaredValues[cvset] = getRsquared(outcomeTest, correctList);
                                                maeValues[cvset] = getMae(outcomeTest, correctList);
                                            }
                                            var rmse = rmseValues.Average();
                                            var rsquared = rsquaredValues.Average();
                                            var mae = maeValues.Average();
                                            var rmsesd = getStdev(rmseValues);
                                            var rsquaredsd = getStdev(rsquaredValues);
                                            var maesd = getStdev(maeValues);
                                            var id2ResultStat = new Dictionary<string, double>();
                                            id2ResultStat.Add("RMSE", rmse);
                                            id2ResultStat.Add("RSquared", rsquared);
                                            id2ResultStat.Add("MAE", mae);

                                            id2ResultStat.Add("RMSESD", rmsesd);
                                            id2ResultStat.Add("RSquaredSD", rsquaredsd);
                                            id2ResultStat.Add("MAESD", maesd);

                                            id2ResultStatSet.Add(icounter, id2ResultStat);

                                            id2Param[icounter] = tParam;
                                            id2Result[icounter] = rmse;


                                            {
                                                var item = id2Param[icounter];
                                                var itemStatResult = id2ResultStatSet[icounter];
                                                sw.WriteLine(string.Join(
                                                    "\t", new string[] {
                                                    icounter.ToString(),
                                                    item.learningRate.ToString(),
                                                    item.maxDepth.ToString(),
                                                    item.gamma.ToString(),
                                                    item.colSampleByTree.ToString(),
                                                    item.minChildWeight.ToString(),
                                                    item.subsample.ToString(),
                                                    item.nEstimators.ToString(),
                                                    itemStatResult["RMSE"].ToString(),
                                                    itemStatResult["RSquared"].ToString(),
                                                    itemStatResult["MAE"].ToString(),
                                                    itemStatResult["RMSESD"].ToString(),
                                                    itemStatResult["RSquaredSD"].ToString(),
                                                    itemStatResult["MAESD"].ToString(),
                                                }));
                                            }

                                            icounter++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


            ////check result output
            using (var sw = new StreamWriter(output + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt", false, Encoding.ASCII))
            {
                sw.WriteLine("CV=" + cval);

                sw.WriteLine(string.Join("\t", new string[]
                                {
                    "id", "learningRate", "maxDepth", "gamma", "colSampleByTree", "minChildWeight", "subsample", "nEstimators",
                    "RMSE(Mean)","RSquared(Mean)","MAE(Mean)","RMSESD","RSquaredSD","MAESD"

                                }));
                for (int i = 0; i < id2Param.Count; i++)
                {
                    var item = id2Param[i];
                    var itemStatResult = id2ResultStatSet[i];
                    sw.WriteLine(string.Join(
                        "\t", new string[] {
                            i.ToString(),
                            item.learningRate.ToString(),
                            item.maxDepth.ToString(),
                            item.gamma.ToString(),
                            item.colSampleByTree.ToString(),
                            item.minChildWeight.ToString(),
                            item.subsample.ToString(),
                            item.nEstimators.ToString(),
                            itemStatResult["RMSE"].ToString(),
                            itemStatResult["RSquared"].ToString(),
                            itemStatResult["MAE"].ToString(),
                            itemStatResult["RMSESD"].ToString(),
                            itemStatResult["RSquaredSD"].ToString(),
                            itemStatResult["MAESD"].ToString(),
                        }));
                }
            }

            //var minID = id2Result.Argmin(n => n.Value).Key;
            var minID = id2Result
                .Aggregate(
                    (minPair, nextPair) => nextPair.Value < minPair.Value ? nextPair : minPair
                )
                .Key;
            var optParam = id2Param[minID];

            return optParam;

        }

        public static TuningParameter XgbTuneVS2(List<XGVector<Array>> vectorsTrain, string output)
        {
            var cval = 7;
            var cvalTrainset = new Dictionary<int, List<XGVector<Array>>>();
            var cvalTestset = new Dictionary<int, List<XGVector<Array>>>();
            var correctListSet = new Dictionary<int, List<float>>();

            for (int cvset = 0; cvset < cval; cvset++)
            {
                var trainset = new List<XGVector<Array>>();
                var testset = new List<XGVector<Array>>();
                var correctList = new List<float>();

                for (int i = 0; i < vectorsTrain.Count; i++)
                {
                    if (i + cvset < vectorsTrain.Count)
                    {
                        if (i % cval == 0)
                        {
                            testset.Add(vectorsTrain[i + cvset]);
                            correctList.Add(vectorsTrain[i + cvset].Target);
                        }
                        else
                        {
                            trainset.Add(vectorsTrain[i]);
                        }
                    }
                }

                cvalTrainset.Add(cvset, trainset);
                cvalTestset.Add(cvset, testset);
                correctListSet.Add(cvset, correctList);

            }

            ////case1
            //var nEstimatorsTune = new List<int>() { 800, 900, 1000 };
            //var maxDepthTune = new List<int>() { 8, 9, 10 };
            //var learningRateTune = new List<float>() { 0.1F,0.05F };
            //var gammaTune = new List<float>() { 0 };
            //var colSampleByTreeTune = new List<float>() { 0.75F, 1 };
            //var minChildWeightTune = new List<int>() { 0, 1 };
            //var subsampleTune = new List<float>() { 0.5F, 1 };

            ////case2 RT
            var nEstimatorsTune = new List<int>() { 1000 };
            var maxDepthTune = new List<int>() { 5, 6, 7, 8, 9 };
            var learningRateTune = new List<float>() { 0.3F, 0.1F, 0.05F, 0.02F };
            var gammaTune = new List<float>() { 0F, 1F };
            var colSampleByTreeTune = new List<float>() { 0.75F };
            var minChildWeightTune = new List<int>() { 10 };
            var subsampleTune = new List<float>() { 0.5F };


            ////full set
            //var nEstimatorsTune = new List<int>() { 500, 600, 700, 800, 900, 1000 };
            //var maxDepthTune = new List<int>() { 5, 6, 7, 8, 9, 10 };
            //var learningRateTune = new List<float>() { 0.025F, 0.05F, 0.1F };
            //var gammaTune = new List<float>() { 0, 0.01F };
            //var colSampleByTreeTune = new List<float>() { 0.75F, 1 };
            //var minChildWeightTune = new List<int>() { 0, 1 };
            //var subsampleTune = new List<float>() { 0.5F, 1 };

            ////to check set
            //var nEstimatorsTune = new List<int>() { 500, 1000 };
            //var maxDepthTune = new List<int>() { 5,10 };
            //var learningRateTune = new List<float>() { 0.025F, 0.1F };
            //var gammaTune = new List<float>() { 0, 0.01F };
            //var colSampleByTreeTune = new List<float>() { 0.75F, 1 };
            //var minChildWeightTune = new List<int>() { 0, 1 };
            //var subsampleTune = new List<float>() { 0.5F, 1 };

            var id2Param = new Dictionary<int, TuningParameter>();
            var id2Result = new Dictionary<int, double>();
            var id2ResultStatSet = new Dictionary<int, Dictionary<string, double>>();

            var icounter = 0;
            var tParamDic = new Dictionary<int, TuningParameter>();
            for (int i = 0; i < nEstimatorsTune.Count; i++)
            {
                for (int j = 0; j < maxDepthTune.Count; j++)
                {
                    for (int k = 0; k < learningRateTune.Count; k++)
                    {
                        for (int l = 0; l < gammaTune.Count; l++)
                        {
                            for (int m = 0; m < colSampleByTreeTune.Count; m++)
                            {
                                for (int n = 0; n < minChildWeightTune.Count; n++)
                                {
                                    for (int o = 0; o < subsampleTune.Count; o++)
                                    {
                                        var tParam = new TuningParameter()
                                        {
                                            nEstimators = nEstimatorsTune[i],
                                            maxDepth = maxDepthTune[j],
                                            learningRate = learningRateTune[k],
                                            gamma = gammaTune[l],
                                            colSampleByTree = colSampleByTreeTune[m],
                                            minChildWeight = minChildWeightTune[n],
                                            subsample = subsampleTune[o],
                                        };
                                        tParamDic.Add(icounter, tParam);
                                        icounter++;

                                        // 

                                    }
                                }
                            }
                        }
                    }
                }

            }
            var lockObject = new object();
            Parallel.For(0, tParamDic.Count, i =>
            {
                var id2ResultStat = getXgboostTuningResult(tParamDic[i], cvalTrainset, cvalTestset, correctListSet, cval);
                lock (lockObject)
                {
                    id2ResultStatSet.Add(i, id2ResultStat);
                }
                id2Param[i] = tParamDic[i];
                id2Result[i] = id2ResultStat["RMSE"];
            }
            );
            //for (int i = 0; i < tParamDic.Count; i++)
            //{

            //    var id2ResultStat = getXgboostTuningResult(tParamDic[i], cvalTrainset, cvalTestset, correctListSet, cval);

            //    id2ResultStatSet.Add(i, id2ResultStat);

            //    id2Param[i] = tParamDic[i];
            //    id2Result[i] = id2ResultStat["RMSE"];
            //}

            ////check result output
            using (var sw = new StreamWriter(output + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt", false, Encoding.ASCII))
            {
                sw.WriteLine("CV=" + cval);

                sw.WriteLine(string.Join("\t", new string[]
                                {
                    "id", "learningRate", "maxDepth", "gamma", "colSampleByTree", "minChildWeight", "subsample", "nEstimators",
                    "RMSE(Mean)","RSquared(Mean)","MAE(Mean)"
                    ,"RMSESD","RSquaredSD","MAESD"
                                }));
                for (int i = 0; i < id2Param.Count; i++)
                {
                    var item = id2Param[i];
                    var itemStatResult = id2ResultStatSet[i];
                    sw.WriteLine(string.Join(
                        "\t", new string[] {
                            i.ToString(),
                            item.learningRate.ToString(),
                            item.maxDepth.ToString(),
                            item.gamma.ToString(),
                            item.colSampleByTree.ToString(),
                            item.minChildWeight.ToString(),
                            item.subsample.ToString(),
                            item.nEstimators.ToString(),
                            itemStatResult["RMSE"].ToString(),
                            itemStatResult["RSquared"].ToString(),
                            itemStatResult["MAE"].ToString(),
                            itemStatResult["RMSESD"].ToString(),
                            itemStatResult["RSquaredSD"].ToString(),
                            itemStatResult["MAESD"].ToString(),
                        }));
                }
            }

            //var minID = id2Result.Argmin(n => n.Value).Key;
            var minID = id2Result
                .Aggregate(
                    (minPair, nextPair) => nextPair.Value < minPair.Value ? nextPair : minPair
                )
                .Key;
            var optParam = id2Param[minID];

            return optParam;

        }

        public static Dictionary<string, double> getXgboostTuningResult(
            TuningParameter tParam, Dictionary<int, List<XGVector<Array>>> cvalTrainset, Dictionary<int, List<XGVector<Array>>> cvalTestset,
             Dictionary<int, List<float>> correctListSet, int cval
            )
        {
            int nEstimators = tParam.nEstimators; //nrounds
            int maxDepth = tParam.maxDepth; //
            float learningRate = tParam.learningRate; //eta
            float gamma = tParam.gamma;
            float colSampleByTree = tParam.colSampleByTree;
            int minChildWeight = tParam.minChildWeight;
            float subsample = tParam.subsample;
            bool silent = true;
            string objective = "reg:linear";
            int nThread = 4; // default -1;
            int maxDeltaStep = 0;
            float colSampleByLevel = 1;
            float regAlpha = 0;
            float regLambda = 1;// default=1 on R 
            float scalePosWeight = 1;
            float baseScore = 0.5F;
            int seed = 0;
            float missing = float.NaN;

            var xgbc = new XGBoost.XGBRegressor(maxDepth, learningRate, nEstimators, silent,
                objective, nThread, gamma, minChildWeight, maxDeltaStep,
                subsample, colSampleByTree, colSampleByLevel,
                regAlpha, regLambda, scalePosWeight, baseScore, seed, missing);


            double[] rmseValues = new double[cval];
            double[] rsquaredValues = new double[cval];
            double[] maeValues = new double[cval];
            for (int cvset = 0; cvset < cval; cvset++)
            {
                XGBArray arrTrain = ConvertToXGBArray(cvalTrainset[cvset]);
                xgbc.Fit(arrTrain.Vectors, arrTrain.Target);
                XGBArray arrTest = ConvertToXGBArray(cvalTestset[cvset]);

                var outcomeTest = xgbc.Predict(arrTest.Vectors).ToList();


                var correctList = new List<float>(correctListSet[cvset]);

                if (outcomeTest.Count != correctList.Count)
                {
                    Console.Write("correctList and outcomeTest is not same number");
                }
                rmseValues[cvset] = getRmse(outcomeTest, correctList);
                rsquaredValues[cvset] = getRsquared(outcomeTest, correctList);
                maeValues[cvset] = getMae(outcomeTest, correctList);
            }

            var id2ResultStat = new Dictionary<string, double>();
            var rmse = rmseValues.Average();
            var rsquared = rsquaredValues.Average();
            var mae = maeValues.Average();
            id2ResultStat.Add("RMSE", rmse);
            id2ResultStat.Add("RSquared", rsquared);
            id2ResultStat.Add("MAE", mae);

            var rmsesd = getStdev(rmseValues);
            var rsquaredsd = getStdev(rsquaredValues);
            var maesd = getStdev(maeValues);
            id2ResultStat.Add("RMSESD", rmsesd);
            id2ResultStat.Add("RSquaredSD", rsquaredsd);
            id2ResultStat.Add("MAESD", maesd);

            xgbc.Dispose();
            GC.Collect();

            return id2ResultStat;

        }


        public static double getRmse(List<float> outcomeTest, List<float> correct)
        {
            var diffList = new List<float>();
            for (int i = 0; i < correct.Count; i++)
            {
                diffList.Add(outcomeTest[i] - correct[i]);
            }

            float[] diff = diffList.ToArray();

            double squareSum = diff.Select(n => n * n).Sum();

            // Calculate Mean
            double mean = (squareSum / diff.Length);

            // Calculate Root
            double rmse = Math.Sqrt(mean);

            return rmse;

        }
        public static double getMae(List<float> outcomeTest, List<float> correct)
        {
            var diffList = new List<float>();
            for (int i = 0; i < correct.Count; i++)
            {
                diffList.Add(outcomeTest[i] - correct[i]);
            }

            float[] diff = diffList.ToArray();
            double tot = 0;

            for (int i = 0; i < diff.Length; i++)
            {
                tot += Math.Abs(diff[i]);
            }

            double mae = tot / diff.Length;

            return mae;
        }
        public static double getRsquared(List<float> outcomeTest, List<float> correct)
        {
            var diffList = new List<float>();
            for (int i = 0; i < correct.Count; i++)
            {
                diffList.Add(outcomeTest[i] - correct[i]);
            }

            float[] diff = diffList.ToArray();

            double ssres = 0;
            double sstot = 0;

            for (int i = 0; i < diff.Length; i++)
            {
                ssres += Math.Pow(diff[i], 2);
            }

            double correctMean = correct.Average();
            for (int i = 0; i < correct.Count; i++)
            {
                sstot += Math.Pow(correct[i] - correctMean, 2);
            }

            double rsquared = 1 - (ssres / sstot);

            return rsquared;

        }

        public static double getStdev(double[] items)
        {
            double mean = items.Average();
            double squareSum = items.Select(n => n * n).Sum();
            double variance = (squareSum / items.Length) - mean * mean;

            double stdev = Math.Sqrt(variance);

            return stdev;

        }




        public static XGBArray ConvertToXGBArray(List<XGVector<Array>> vectorsTrain)
        {
            var arr = new XGBArray();
            arr.Target = vectorsTrain.Select(v => v.Target).ToArray();
            arr.Vectors = vectorsTrain.Select(v => v.Features).ToArray();
            return arr;
        }

        public static Dictionary<string, float>
            adductscoreDic = new Dictionary<string, float>()
            {
            //  "[M]+" "[M+H]+" "[M+NH4]+" "[M+Na]+"  
            //"[M-H]-"  "[M+HCOO]-"  "[M+CH3COO]-" "[M+H-H2O]+" "[M-2H]2-"
                { "[M]+", -0.00054858f },
                { "[M+H]+",1.00727642f},
                { "[M+NH4]+",18.03382555f},
                { "[M+Na]+",22.9892207f},
                { "[M-H]-" ,-1.00727642f},
                { "[M+HCOO]-",44.99820285f},
                {"[M+CH3COO]-" ,59.01385292f},
                {"[M+H-H2O]+" ,-17.00328358f},
                {"[M-2H]2-" ,-1.00727642f},
                { "[M+2H]2+",1.00727642f},
                { "[M+2NH4]2+",18.03382555f},
                // 20221219 add
                { "[M+K]+",38.96315791f},
                { "[M+Li]+",7.01545486f},
                // 20250123 add
                { "[M+HCO3]-",(float)(MassDictionary.CarbonMass * 1 + MassDictionary.OxygenMass * 3 + MassDictionary.HydrogenMass + MassDictionary.Electron)},
            };
        public class XGVector<T>
        {
            /// <summary>
            /// The original object
            /// </summary>
            public T Original { get; set; }
            /// <summary>
            /// Attributes of the feature vector
            /// </summary>
            public float[] Features { get; set; }
            public float Target { get; set; }
        }
        public class XGBArray
        {
            public float[][] Vectors { get; set; }
            public float[] Target { get; set; }
        }

        public class TuningParameter
        {
            public int nEstimators;
            public int maxDepth;
            public float learningRate;
            public float gamma;
            public float colSampleByTree;
            public int minChildWeight;
            public float subsample;

        }

    }
    public sealed class MassDictionary
    {
        private MassDictionary() { }

        //public static double C13_C12 = 1.003354838;
        //public static double H2_H1 = 1.006276746;
        //public static double N15_N14 = 0.997034893;
        //public static double O17_O16 = 1.00421708;
        //public static double Si29_Si28 = 0.999568168;
        //public static double S33_S32 = 0.99938776;

        //public static double C13_12_Plus_H2_H1 = 2.009631584;
        //public static double C13_12_Plus_N15_N14 = 2.000389731;
        //public static double C13_12_Plus_O17_O16 = 2.007571918;
        //public static double C13_12_Plus_S33_S32 = 2.002742598;
        //public static double C13_12_Plus_Si29_Si28 = 2.002923006;
        //public static double H2_H1_Plus_N15_N14 = 2.003311639;
        //public static double H2_H1_Plus_O17_O16 = 2.010493826;
        //public static double H2_H1_Plus_S33_S32 = 2.005664506;
        //public static double H2_H1_Plus_Si29_Si28 = 2.005844914;
        //public static double N15_N14_Plus_O17_O16 = 2.001251973;
        //public static double N15_N14_Plus_S33_S32 = 1.996422653;
        //public static double N15_N14_Plus_Si29_Si28 = 1.996603061;
        //public static double O17_O16_Plus_S33_S32 = 2.00360484;
        //public static double O17_O16_Plus_Si29_Si28 = 2.003785248;
        //public static double S33_S32_Plus_Si29_Si28 = 1.998955928;
        //public static double C13_C12_Plus_C13_C12 = 2.006709676;
        //public static double H2_H1_Plus_H2_H1 = 2.012553492;
        //public static double N15_N14_Plus_N15_N14 = 1.994069786;
        //public static double O17_O16_Plus_O17_O16 = 2.00843416;
        //public static double S33_S32_Plus_S33_S32 = 1.99877552;
        //public static double Si29_Si28_Plus_Si29_Si28 = 1.999136336;

        //public static double S34_S32 = 1.9957959;
        //public static double Si30_Si28 = 1.996843638;
        //public static double O18_O16 = 2.00424638;
        //public static double Cl37_Cl35 = 1.99704991;
        //public static double Br81_Br79 = 1.9979535;

        public static double CarbonMass = 12.00000000000;
        public static double HydrogenMass = 1.00782503207;
        public static double NitrogenMass = 14.00307400480;
        public static double OxygenMass = 15.99491461956;
        public static double SulfurMass = 31.97207100000;
        public static double PhosphorusMass = 30.97376163000;
        public static double FluorideMass = 18.99840322000;
        public static double SiliconMass = 27.97692653250;
        public static double ChlorideMass = 34.96885268000;
        public static double BromineMass = 78.91833710000;
        public static double IodineMass = 126.90447300000;
        public static double H2OMass = 18.010564684;
        public static double Proton = 1.00727641974;
        public static double Electron = 0.00054858026;
        public static double NH4Adduct = 18.033825553;
        public static double NaAdduct = 22.9892207;
        public static double KAdduct = 38.96315791f;
        public static double LiAdduct = 7.01545486f;
        public static double HCOOadduct = 44.998202852;
        public static double HCO3adduct = CarbonMass * 1 + OxygenMass * 3 + HydrogenMass + Electron;
        public static double CH3COOadduct = 59.013852917;
        public static double C6H10O5 = CarbonMass * 6 + HydrogenMass * 10 + OxygenMass * 5;
        public static readonly double Carbon13Mass = 13.00335484;
        public static readonly double Hydrogen2Mass = 2.014101778;
        public static readonly double Nitrogen15Mass = 15.0001089;
        public static readonly double Oxygen18Mass = 17.999161;
        public static readonly double Sulfur34Mass = 33.9678669;
        public static readonly double Chloride37Mass = 36.96590259;
        public static readonly double Bromine81Mass = 80.9162906;
    }

}