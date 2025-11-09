param(
  [string]$ExePath = "c:\\Users\\teste\\CarvaoControl\\MeuAppWinForms\\publish\\win-x64\\self-contained_v1.0.1\\CarvaoControl.exe",
  [string]$CertPath = "c:\\path\\to\\certificate.pfx",
  [string]$TimestampUrl = "http://timestamp.digicert.com",
  [string]$Description = "Carvão Control",
  [string]$Traces = "CarvaoControl",
  [SecureString]$CertPassword = $null # opcional: para execução não interativa
)

Write-Host "==> Assinando $ExePath"
if (!(Test-Path $ExePath)) { throw "Arquivo não encontrado: $ExePath" }
if (!(Test-Path $CertPath)) { throw "Certificado .pfx não encontrado: $CertPath" }

# Obtém senha do certificado
if (-not $CertPassword) {
  $SecurePwd = Read-Host -AsSecureString -Prompt "Senha do certificado PFX"
} else {
  $SecurePwd = $CertPassword
}

# Localizar signtool
$signTool = & where.exe signtool 2>$null | Select-Object -First 1
if (-not $signTool) { throw "signtool.exe não encontrado. Instale o Windows SDK." }

# Primeiro: assinatura SHA256 com carimbo de tempo
& $signTool sign /fd SHA256 /tr $TimestampUrl /td SHA256 /f $CertPath /p $SecurePwd /d $Description /du "https://www.chamadistribuidora.com" $ExePath

Write-Host "Assinatura concluída."
