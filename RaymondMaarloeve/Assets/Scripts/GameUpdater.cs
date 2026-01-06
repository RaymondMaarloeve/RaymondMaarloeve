using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Octokit;
using System.Diagnostics; 

public class GameUpdater : MonoBehaviour
{
    [Header("GitHub Settings")]
    public string repoOwner = "RaymondMaarloeve";
    public string repoName = "RaymondMaarloeve";

    [Header("UI Components")]
    public Button downloadButton;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI progressText;
    public Slider progressBar;

    private GitHubClient _client;
    private string _extractPath;
    private float _visualProgress = 0;
    private string _visualStatus = "";
    private bool _localhost = false;

    [Serializable]
    private class LocalConfigData
    {
        public bool Localhost;
    }

    void Start()
    {
        _client = new GitHubClient(new ProductHeaderValue("UnityLauncher"));
        _extractPath = Path.Combine(UnityEngine.Application.persistentDataPath, "LatestBuild");
        _visualStatus = "";
    }

    void Update()
    {
        if (progressBar != null) progressBar.value = _visualProgress;
        if (progressText != null && _visualStatus != "") progressText.text = $"{(_visualProgress * 100):F1}%";
        if (statusText != null) statusText.text = _visualStatus;
    }

    public void Click_DownloadUpdate()
    {
        _ = DownloadAndInstallLatestRelease();
    }

    private async Task DownloadAndInstallLatestRelease()
    {
        try
        {
            if (downloadButton != null) downloadButton.interactable = false;
            _visualStatus = "Looking for updates";

            var latestRelease = await _client.Repository.Release.GetLatest(repoOwner, repoName);

            string targetAsset = UnityEngine.Application.platform == RuntimePlatform.LinuxPlayer
                ? "Build-StandaloneLinux64.zip" : "Build-StandaloneWindows64.zip";

            var asset = latestRelease.Assets.FirstOrDefault(a => a.Name == targetAsset)
                        ?? latestRelease.Assets.FirstOrDefault(a => a.Name.EndsWith(".zip"));

            if (asset == null)
            {
                _visualStatus = "File not found";
                return;
            }

            _visualStatus = "Downloading...";
            string tempPath = Path.Combine(UnityEngine.Application.temporaryCachePath, asset.Name);
            await DownloadFileAsync(asset.BrowserDownloadUrl, tempPath);

            _visualStatus = "Extracting...";
            await Task.Run(() => {
                if (Directory.Exists(_extractPath)) Directory.Delete(_extractPath, true);
                Directory.CreateDirectory(_extractPath);
                ZipFile.ExtractToDirectory(tempPath, _extractPath);
                if (File.Exists(tempPath)) File.Delete(tempPath);
            });

            _visualStatus = "Ready";

            LaunchGame();
        }
        catch (Exception ex)
        {
            _visualStatus = $"Error: {ex.Message}";
            UnityEngine.Debug.LogError(ex);
        }
        finally
        {
            if (downloadButton != null) downloadButton.interactable = true;
        }
    }

    private async Task DownloadFileAsync(string url, string filePath)
    {
        using var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        var totalBytes = response.Content.Headers.ContentLength ?? -1L;
        using var remoteStream = await response.Content.ReadAsStreamAsync();
        using var localFileStream = File.Create(filePath);
        var buffer = new byte[81920];
        long totalRead = 0;
        int bytesRead;
        while ((bytesRead = await remoteStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            await localFileStream.WriteAsync(buffer, 0, bytesRead);
            totalRead += bytesRead;
            if (totalBytes != -1) _visualProgress = (float)totalRead / totalBytes;
        }
    }

    private void LaunchGame()
    {
        var gameDir = _extractPath;
        var isWindows = UnityEngine.Application.platform == RuntimePlatform.WindowsPlayer || UnityEngine.Application.platform == RuntimePlatform.WindowsEditor;

        var exePath = Path.Combine(gameDir, isWindows ? "StandaloneWindows64.exe" : "StandaloneLinux64");
        var dataPath = Path.Combine(gameDir, isWindows ? "StandaloneWindows64_Data" : "StandaloneLinux64_Data");

        var serverDir = Path.Combine(gameDir, "Server");
        var serverExePath = Path.Combine(serverDir, isWindows ? "LLMServer.exe" : "LLMServer");

        if (!Directory.Exists(gameDir))
        {
            _visualStatus = "Game directory doesn't exist.";
            return;
        }

        const string configName = "game_config.json";
        var sourceConfig = Path.Combine(UnityEngine.Application.streamingAssetsPath, configName);
        var targetConfig = Path.Combine(dataPath, configName);

        if (File.Exists(sourceConfig))
        {
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            File.Copy(sourceConfig, targetConfig, true);
            _localhost = ReadLocalhostFromConfig(sourceConfig);
        }

        if (!File.Exists(exePath))
        {
            _visualStatus = "Executable not found.";
            return;
        }

        if (UnityEngine.Application.platform == RuntimePlatform.LinuxPlayer)
        {
            try
            {
                Process.Start("chmod", $"+x \"{exePath}\"")?.WaitForExit();
                if (File.Exists(serverExePath)) Process.Start("chmod", $"+x \"{serverExePath}\"")?.WaitForExit();
            }
            catch { /* ignore permissions error */ }
        }

        try
        {
            Process serverProcess = null;
            if (_localhost && File.Exists(serverExePath))
            {
                serverProcess = Process.Start(new ProcessStartInfo
                {
                    FileName = serverExePath,
                    WorkingDirectory = serverDir,
                    UseShellExecute = false
                });
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                WorkingDirectory = gameDir,
                UseShellExecute = false
            });

            UnityEngine.Application.Quit();
        }
        catch (Exception ex)
        {
            _visualStatus = $"Launch error: {ex.Message}";
        }
    }

    private bool ReadLocalhostFromConfig(string path)
    {
        try
        {
            if (!File.Exists(path)) return false;
            string json = File.ReadAllText(path);
            var config = JsonUtility.FromJson<LocalConfigData>(json);
            return config != null && config.Localhost;
        }
        catch
        {
            return false;
        }
    }
}