using System;
using System.IO;
using System.IO.Compression;

namespace CarvaoControl.Infrastructure.Services
{
    public class BackupService
    {
        private readonly string _dataPath;
        private readonly string _backupPath;
        private readonly LoggingService _log;

        public BackupService(string? basePath = null)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var baseDir = basePath ?? Path.Combine(appData, "CarvaoControl");
            _dataPath = baseDir;
            _backupPath = Path.Combine(baseDir, "backups");
            _log = new LoggingService(basePath);
            Directory.CreateDirectory(_backupPath);
        }

        public string CreateBackup()
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var backupFile = Path.Combine(_backupPath, $"backup_{timestamp}.zip");

                using (var zip = ZipFile.Open(backupFile, ZipArchiveMode.Create))
                {
                    // Backup das configurações
                    var configFile = Path.Combine(_dataPath, "config.json");
                    if (File.Exists(configFile))
                    {
                        zip.CreateEntryFromFile(configFile, "config.json");
                    }

                    // Backup dos logs
                    var logsDir = Path.Combine(_dataPath, "logs");
                    if (Directory.Exists(logsDir))
                    {
                        foreach (var logFile in Directory.GetFiles(logsDir, "*.log"))
                        {
                            var fileName = Path.GetFileName(logFile);
                            zip.CreateEntryFromFile(logFile, Path.Combine("logs", fileName));
                        }
                    }
                }

                _log.LogAudit("Backup", $"Backup criado: {backupFile}");
                return backupFile;
            }
            catch (Exception ex)
            {
                _log.LogError("Falha ao criar backup", ex);
                throw;
            }
        }

        public void RestoreBackup(string backupFile)
        {
            try
            {
                using (var zip = ZipFile.OpenRead(backupFile))
                {
                    foreach (var entry in zip.Entries)
                    {
                        var targetPath = Path.Combine(_dataPath, entry.FullName);
                        Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
                        
                        // Restaura normalmente
                        entry.ExtractToFile(targetPath, true);
                    }
                }

                _log.LogAudit("Restore", $"Backup restaurado: {backupFile}");
            }
            catch (Exception ex)
            {
                _log.LogError("Falha ao restaurar backup", ex);
                throw;
            }
        }

        public void CleanOldBackups(int keepDays = 30)
        {
            try
            {
                var cutoff = DateTime.Now.AddDays(-keepDays);
                foreach (var file in Directory.GetFiles(_backupPath, "backup_*.zip"))
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < cutoff)
                    {
                        File.Delete(file);
                        _log.LogInfo($"Backup antigo removido: {file}");
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError("Falha ao limpar backups antigos", ex);
            }
        }
    }
}