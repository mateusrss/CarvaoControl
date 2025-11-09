using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using CarvaoControl.Domain.Entities;
using CarvaoControl.Domain.Enums;
using CarvaoControl.Domain.Exceptions;

namespace CarvaoControl.Application.Services
{
    public class PersistenceService : IDisposable
    {
        private readonly string _basePath;
        private readonly JsonSerializerOptions _defaultOptions = new JsonSerializerOptions { WriteIndented = true };
        private readonly JsonSerializerOptions _vendaOptions = new JsonSerializerOptions { WriteIndented = true, Converters = { new JsonStringEnumConverter() } };
        private readonly Dictionary<string, FileStream> _lockFiles = new();
        private readonly Random _random = new Random();

        public PersistenceService(string? basePath = null)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _basePath = basePath ?? Path.Combine(appData, "CarvaoControl");
            Directory.CreateDirectory(_basePath);
            Directory.CreateDirectory(Path.Combine(_basePath, "locks"));
            Directory.CreateDirectory(Path.Combine(_basePath, "temp"));
        }

        private string ProdutosPath => Path.Combine(_basePath, "produtos.json");
        private string VendasPath => Path.Combine(_basePath, "vendas.json");
        private string GetLockPath(string file) => Path.Combine(_basePath, "locks", $"{Path.GetFileNameWithoutExtension(file)}.lock");
        private string GetTempPath(string file) => Path.Combine(_basePath, "temp", $"{Path.GetFileNameWithoutExtension(file)}_{DateTime.Now.Ticks}_{_random.Next(1000)}.tmp");

        private bool AcquireLock(string filePath, TimeSpan timeout)
        {
            var lockPath = GetLockPath(filePath);
            var deadline = DateTime.UtcNow.Add(timeout);
            var baseDelay = TimeSpan.FromMilliseconds(50);
            var attempt = 0;

            while (DateTime.UtcNow < deadline)
            {
                try
                {
                    var fs = new FileStream(lockPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                    _lockFiles[filePath] = fs;
                    return true;
                }
                catch (IOException) when (DateTime.UtcNow < deadline)
                {
                    // Exponential backoff with jitter
                    var delay = TimeSpan.FromMilliseconds(Math.Min(1000, baseDelay.TotalMilliseconds * Math.Pow(1.5, attempt)));
                    var jitter = TimeSpan.FromMilliseconds(_random.Next((int)delay.TotalMilliseconds));
                    Thread.Sleep(delay + jitter);
                    attempt++;
                }
            }

            return false;
        }

        private void ReleaseLock(string filePath)
        {
            if (_lockFiles.TryGetValue(filePath, out var fs))
            {
                fs.Dispose();
                _lockFiles.Remove(filePath);
                try { File.Delete(GetLockPath(filePath)); } catch { }
            }
        }

        // Improved atomic write with locking
        private void AtomicWrite(string path, string content)
        {
            if (!AcquireLock(path, TimeSpan.FromSeconds(10)))
            {
                throw new DomainException("Não foi possível obter acesso ao arquivo. Tente novamente em alguns instantes.");
            }

            try
            {
                var tempPath = GetTempPath(path);
                
                // Escreve primeiro em arquivo temporário
                File.WriteAllText(tempPath, content);
                
                // Força flush para disco
                using (var fs = new FileStream(tempPath, FileMode.Open, FileAccess.Write))
                {
                    fs.Flush(true);
                }

                // Replace atômico quando possível
                if (File.Exists(path))
                {
                    try
                    {
                        File.Replace(tempPath, path, null);
                    }
                    catch
                    {
                        File.Delete(path);
                        File.Move(tempPath, path);
                    }
                }
                else
                {
                    File.Move(tempPath, path);
                }
            }
            finally
            {
                ReleaseLock(path);
            }
        }

        private T AtomicRead<T>(string path, Func<string, T> deserialize, T defaultValue)
        {
            if (!File.Exists(path)) return defaultValue;

            if (!AcquireLock(path, TimeSpan.FromSeconds(5)))
            {
                throw new DomainException("Arquivo em uso. Tente novamente em alguns instantes.");
            }

            try
            {
                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var sr = new StreamReader(fs);
                var content = sr.ReadToEnd();
                return deserialize(content);
            }
            catch (Exception ex) when (ex is JsonException || ex is IOException)
            {
                return defaultValue;
            }
            finally
            {
                ReleaseLock(path);
            }
        }

        public void SaveProdutos(IEnumerable<Produto> produtos)
        {
            var dto = new List<ProdutoDto>();
            foreach (var p in produtos)
            {
                dto.Add(new ProdutoDto { Id = p.Id, Nome = p.Nome, Preco = p.Preco, Quantidade = p.Quantidade });
            }
            var json = JsonSerializer.Serialize(dto, _defaultOptions);
            AtomicWrite(ProdutosPath, json);
        }

        public IEnumerable<ProdutoDto> LoadProdutos()
        {
            return AtomicRead(ProdutosPath,
                json => JsonSerializer.Deserialize<List<ProdutoDto>>(json, _defaultOptions) ?? new List<ProdutoDto>(),
                new List<ProdutoDto>());
        }

        public void SaveVendas(IEnumerable<Venda> vendas)
        {
            var dto = new List<VendaDto>();
            foreach (var v in vendas)
            {
                dto.Add(new VendaDto { Id = v.Id, ProdutoId = v.ProdutoId, Quantidade = v.Quantidade, ValorTotal = v.ValorTotal, Pagamento = v.Pagamento, Data = v.Data });
            }
            var json = JsonSerializer.Serialize(dto, _vendaOptions);
            AtomicWrite(VendasPath, json);
        }

        public IEnumerable<VendaDto> LoadVendas()
        {
            return AtomicRead(VendasPath,
                json => JsonSerializer.Deserialize<List<VendaDto>>(json, _vendaOptions) ?? new List<VendaDto>(),
                new List<VendaDto>());
        }

        public void Dispose()
        {
            foreach (var fs in _lockFiles.Values)
            {
                fs.Dispose();
            }
            _lockFiles.Clear();

            // Limpa arquivos de lock órfãos
            try
            {
                var lockDir = Path.Combine(_basePath, "locks");
                if (Directory.Exists(lockDir))
                {
                    foreach (var file in Directory.GetFiles(lockDir))
                    {
                        try { File.Delete(file); } catch { }
                    }
                }
            }
            catch { }
        }

        // DTO types
        public class ProdutoDto
        {
            public int Id { get; set; }
            public string Nome { get; set; } = string.Empty;
            public decimal Preco { get; set; }
            public int Quantidade { get; set; }
        }

        public class VendaDto
        {
            public int Id { get; set; }
            public int ProdutoId { get; set; }
            public int Quantidade { get; set; }
            public decimal ValorTotal { get; set; }
            public TipoPagamento Pagamento { get; set; }
            public DateTime Data { get; set; }
        }
    }
}
