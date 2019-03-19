using System;
using System.Collections.Generic;

namespace Tests
{
    public class Poco
    {
        public DateTime? Time { get; set; }
        public string String { get; set; }
        public int? Int { get; set; }
        public float Float { get; set; }
        public decimal Decimal { get; set; }
        public string[] StringArray { get; set; }
        public List<string> StringList { get; set; }
        public SubPoco SubPoco { get; set; }
        public ICollection<SubPoco> SubPocos { get; set; }
    }

    public class SubPoco
    {
        public int Int { get; set; }
        public int IntArray { get; set; }
    }
}