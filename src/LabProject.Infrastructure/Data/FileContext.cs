using System.Text.Json;

namespace LabProject.Infrastructure.Data
{
    public class FileContext
    {
        private static readonly object _fileLock = new();
        private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

        public List<T> Read<T>(string filePath)
        {
            lock (_fileLock)
            {
                if (!File.Exists(filePath))
                {
                    return new List<T>();
                }

                var json = File.ReadAllText(filePath);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<T>();
                }

                return JsonSerializer.Deserialize<List<T>>(json, _options) ?? new List<T>();
            }
        }

        public void Write<T>(string filePath, List<T> data)
        {
            lock (_fileLock)
            {
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonSerializer.Serialize(data, _options);
                File.WriteAllText(filePath, json);
            }
        }
    }
}
