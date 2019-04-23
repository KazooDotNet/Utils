using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KazooDotNet.Utils
{
    public class CsvWriter : IDisposable
    {
        private readonly StreamWriter _stream;
        private bool _headerWritten;
        private static readonly char[] EscapeChars = {'"', '\n', ',', '\''};

        public CsvWriter(StreamWriter stream)
        {
            _stream = stream;
        }

        public void Dispose()
        {
            _stream?.Dispose();
        }

        public void WriteRecord<T>(CsvMap<T> record) where T : class
        {
            if (!_headerWritten)
            {
                _stream.WriteLine(EscapeAndJoin(record.Headers));
                _headerWritten = true;
            }
            _stream.WriteLine(EscapeAndJoin(record.Values));
        }

        public void WriteRecords<T>(IEnumerable<CsvMap<T>> records) where T : class
        {
            var list = records.ToList();
            if (!list.Any()) return;
            if (!_headerWritten)
            {
                _stream.WriteLine(EscapeAndJoin(list.First().Headers));
                _headerWritten = true;
            }
            foreach(var r in list)
                _stream.WriteLine(EscapeAndJoin(r.Values));
        }

        private static string EscapeAndJoin(IEnumerable<string> vals) =>
            string.Join(",", vals.Select(Escape));


        private static string Escape(string val)
        {
            var escape = false;
            foreach (var c in EscapeChars)
                if (val?.Contains(c) ?? false) escape = true;
            if (escape)
                val = $"\"{val.Replace("\"", "\\\"")}\"";
            return val;
        }

    }
}
