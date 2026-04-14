using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;

namespace SerializationLib
{
    public class JsonSerialization
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
        };

        public static void SerializeToJson<T>(T obj, string filePath)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Путь к файлу не может быть пустым или содержать только пробелы.", nameof(filePath));
            }

            EnsureDirectoryExists(filePath);

            string json = JsonSerializer.Serialize(obj, _options);
            File.WriteAllText(filePath, json);
        }

        public static async Task SerializeToJsonAsync<T>(
            T obj, string filePath, CancellationToken cancellationToken = default)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Путь к файлу не может быть пустым или содержать только пробелы.", nameof(filePath));
            }

            cancellationToken.ThrowIfCancellationRequested();

            EnsureDirectoryExists(filePath);

            string json = JsonSerializer.Serialize(obj, _options);
            await File.WriteAllTextAsync(filePath, json, cancellationToken);
        }

        public static async Task SerializeToJsonAsync<T>(
            T obj, Stream stream, CancellationToken cancellationToken = default)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            cancellationToken.ThrowIfCancellationRequested();
            await JsonSerializer.SerializeAsync(stream, obj, _options, cancellationToken);
        }

        public static T? DeserializeFromJson<T>(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Путь к файлу не может быть пустым или содержать только пробелы.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                return default;
            }

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(json, _options);
        }

        public static async Task<T?> DeserializeFromJsonAsync<T>(
            string filePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Путь к файлу не может быть пустым или содержать только пробелы.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                return default;
            }

            cancellationToken.ThrowIfCancellationRequested();

            await using var stream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<T>(stream, _options, cancellationToken);
        }

        public static async Task<T?> DeserializeFromJsonAsync<T>(
            Stream stream, CancellationToken cancellationToken = default)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            cancellationToken.ThrowIfCancellationRequested();
            return await JsonSerializer.DeserializeAsync<T>(stream, _options, cancellationToken);
        }

        private static void EnsureDirectoryExists(string filePath)
        {
            string? directory = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
}