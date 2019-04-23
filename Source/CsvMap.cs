using System;
using System.Collections.Generic;
using System.Linq;

namespace KazooDotNet.Utils
{

    public class FieldMap<T>
    {
        public Func<T, object> Mapping { get; set; }
        public string Header { get; set; }
    }


    public class CsvMap<T> where T : class
    {
        private List<FieldMap<T>> Mappings { get; } = new List<FieldMap<T>>();

        public IEnumerable<string> Headers => Mappings.Select(m => m.Header);

        public IEnumerable<string> Values => Mappings.Select(m => m.Mapping.Invoke(this as T)?.ToString());

        protected void Map(string header, Func<T, object> mapping)
            => Mappings.Add(new FieldMap<T>
            {
                Header = header,
                Mapping = mapping
            });
    }


}
