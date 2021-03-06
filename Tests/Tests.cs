using System;
using System.Collections.Generic;
using System.Linq;
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
                ["unknownField"] = 3,
                ["time"] = "2018-10-31",
                ["String"] = "asdf",
                ["Int"] = "123",
                ["Decimal"] = "123.52",
                ["StringArray"] = new List<string> { "a", "b", "cde"},
                ["StringList"] = new[] { "a", "b", "cde"},
                ["SubPoco2"] = new List<KeyValuePair<string, object>>
                {
                      new KeyValuePair<string, object>("Int", 3),
                      new KeyValuePair<string, object>("IntArray", new[] { "1", "2", "3"})
                },
                ["SubPoco"] = new NestedDictionary
                {
                    ["Int"] = 3,
                    ["IntArray"] = new[] { "1", "2", "3"}
                },
                ["SubPocos"] = new[]
                {
                    new NestedDictionary
                    {
                        
                        ["int"] = "3",
                        ["IntArray"] = new[] { 1, 2, 3}
                    },
                    new NestedDictionary
                    {
                        
                        ["Int"] = "4",
                        ["intArray"] = new[] { "1", "2", "3", "4"}
                    }
                },
                ["SubPocos2"] = new List<KeyValuePair<string,object>>[]
                {
                    new List<KeyValuePair<string,object>>
                    {
                        new KeyValuePair<string, object>("Int", 3),
                        new KeyValuePair<string, object>("IntArray", new[] { "1", "2", "3"})
                    }
                }
            };

            var poco = new Poco();
            poco.Assign(dict);
            Assert.Equal(DateTime.Parse(dict["time"].ToString()), poco.Time);
            Assert.Equal("asdf", poco.String);
            Assert.Equal(123, poco.Int);
            Assert.Equal(123.52m, poco.Decimal);
            // TODO: list/array checking
            Assert.Equal(3, poco.SubPoco.Int);
            Assert.Equal(3, poco.SubPocos.First().Int);
            Assert.Equal(4, poco.SubPocos.Skip(1).First().Int);
            
        }
    }
}