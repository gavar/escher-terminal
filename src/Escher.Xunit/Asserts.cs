using System;
using System.Collections;
using System.IO;
using System.Text.Json;

namespace Escher.Xunit
{
    public abstract class Asserts : global::Xunit.Assert
    {
        private static readonly JsonSerializerOptions SERIALIZER_OPTIONS = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        public static void FileLines(string path, string[] expected)
        {
            var expect = string.Join(Environment.NewLine, expected) + Environment.NewLine;
            Equal(expect, File.ReadAllText(path), ignoreLineEndingDifferences: true);
        }

        public static void FileJson<T>(string path, IDictionary expected)
        {
            var actual = NormalizeJsonText<T>(File.ReadAllText(path));
            var expect = NormalizeJsonData(expected);
            Equal(expect, actual, false, true, true);
        }
        
        public static void FileJson<T>(string path, T expected)
        {
            var actual = NormalizeJsonText<T>(File.ReadAllText(path));
            var expect = NormalizeJsonData(expected);
            Equal(expect, actual, false, true, true);
        }

        private static string NormalizeJsonText<T>(string json)
        {
            var data = JsonSerializer.Deserialize<T>(json);
            return NormalizeJsonData(data);
        }

        private static string NormalizeJsonData<T>(T data)
        {
            return JsonSerializer.Serialize(data, SERIALIZER_OPTIONS);
        }
    }
}