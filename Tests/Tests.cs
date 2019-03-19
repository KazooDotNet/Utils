using System;
using System.Collections.Generic;
using KazooDotNet.Utils;
using Xunit;

namespace Tests
{
    public class Tests
    {
        [Fact]
        public void Assigners()
        {
            var dict = new NestedDictionary
            {
                ["unknownField"] = "3",
                ["time"] = "2018-10-31",
                ["String"] = "asdf",
                ["Int"] = "123",
                ["Decimal"] = "123.52",
                ["StringArray"] = new List<string> { "a", "b", "cde"},
                ["StringList"] = new[] { "a", "b", "cde"},
                ["SubPoco"] = new NestedDictionary
                {
                    ["Int"] = "3",
                    ["IntArray"] = new[] { "1", "2", "3"}
                }
                ["SubPocos"] = new[]
                {
                    new NestedDictionary
                    {
                        
                        ["int"] = "3",
                        ["IntArray"] = new[] { "1", "2", "3"}
                    },
                    new NestedDictionary
                    {
                        
                        ["Int"] = "4",
                        ["intArray"] = new[] { "1", "2", "3", "4"}
                    }
                }
            };

            var poco = new Poco();
            poco.Assign(dict);
            Assert.Equal(DateTime.Parse(dict["time"].ToString()), poco.Time);
            
        }
    }
}