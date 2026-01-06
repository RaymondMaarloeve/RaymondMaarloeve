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
        }
        catch (Exception ex)
        {
            _visualStatus = $"Error {ex.Message}";
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
}