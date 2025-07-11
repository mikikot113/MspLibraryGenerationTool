using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace NistMspToMsDialMsp.property
{
    // use http://json2csharp.com/
    [DataContract]
    public class MetaData
    {
        [DataMember]
        public string category { get; set; }
        [DataMember]
        public bool computed { get; set; }
        [DataMember]
        public bool hidden { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public object value { get; set; }
    }
    [DataContract]
    public class Name
    {
        [DataMember]
        public bool computed { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public double score { get; set; }
    }
    [DataContract]
    public class Classification
    {
        [DataMember]
        public string category { get; set; }
        [DataMember]
        public bool computed { get; set; }
        [DataMember]
        public bool hidden { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string value { get; set; }
    }

    [DataContract]
    public class Compound
    {
        [DataMember]
        public string inchi { get; set; }
        [DataMember]
        public string inchiKey { get; set; }
        [DataMember]
        public List<MetaData> metaData { get; set; }
        [DataMember]
        public string molFile { get; set; }
        [DataMember]
        public List<Name> names { get; set; }
        [DataMember]
        public List<object> tags { get; set; }
        [DataMember]
        public bool computed { get; set; }
        [DataMember]
        public string kind { get; set; }
        [DataMember]
        public List<Classification> classification { get; set; }
    }

    [DataContract]
    public class MetaData2
    {
        [DataMember]
        public string category { get; set; }
        [DataMember]
        public bool computed { get; set; }
        [DataMember]
        public bool hidden { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public object value { get; set; }
        [DataMember]
        public string unit { get; set; }
    }

    [DataContract]
    public class Impact
    {
        [DataMember]
        public double value { get; set; }
        [DataMember]
        public string reason { get; set; }
    }

    [DataContract]
    public class Score
    {
        [DataMember]
        public List<Impact> impacts { get; set; }
        [DataMember]
        public double score { get; set; }
    }

    [DataContract]
    public class Splash
    {
        [DataMember]
        public string block1 { get; set; }
        [DataMember]
        public string block2 { get; set; }
        [DataMember]
        public string block3 { get; set; }
        [DataMember]
        public string block4 { get; set; }
        [DataMember]
        public string splash { get; set; }
    }

    [DataContract]
    public class Submitter
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public string emailAddress { get; set; }
        [DataMember]
        public string firstName { get; set; }
        [DataMember]
        public string lastName { get; set; }
        [DataMember]
        public string institution { get; set; }
    }

    [DataContract]
    public class Tag
    {
        [DataMember]
        public bool ruleBased { get; set; }
        [DataMember]
        public string text { get; set; }
    }

    [DataContract]
    public class Tag2
    {
        [DataMember]
        public bool ruleBased { get; set; }
        [DataMember]
        public string text { get; set; }
    }

    [DataContract]
    public class Library
    {
        [DataMember]
        public string library { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string link { get; set; }
        [DataMember]
        public Tag2 tag { get; set; }
    }

    [DataContract]
    public class RootObject
    {
        [DataMember]
        public List<Compound> compound { get; set; }
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public long dateCreated { get; set; }
        [DataMember]
        public long lastUpdated { get; set; }
        [DataMember]
        public long lastCurated { get; set; }
        [DataMember]
        public List<MetaData2> metaData { get; set; }
        [DataMember]
        public Score score { get; set; }
        [DataMember]
        public string spectrum { get; set; }
        [DataMember]
        public Splash splash { get; set; }
        [DataMember]
        public Submitter submitter { get; set; }
        [DataMember]
        public List<Tag> tags { get; set; }
        [DataMember]
        public Library library { get; set; }
    }

    public class Peak
    {
        private double mz;
        private double intensity;

        public Peak() { }

        public double Mz
        {
            get { return Math.Round(mz, 6); }
            set { mz = Math.Round(value, 6); }
        }

        public double Intensity
        {
            get { return Math.Round(intensity, 6); }
            set { intensity = Math.Round(value, 6); }
        }

    }
    class jsonFromMoNAClass
    {
    }
}
