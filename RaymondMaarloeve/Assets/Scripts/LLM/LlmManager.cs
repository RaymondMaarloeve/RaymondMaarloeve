using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LlmManager : MonoBehaviour
{
    public static LlmManager Instance;
    private string BaseUrl;
    
    public bool IsConnected { get; private set; }

    private Queue<IEnumerator> postRequestQueue = new Queue<IEnumerator>();
    private bool isProcessingQueue = false;

    public void Setup(string api)
    {
        BaseUrl = api;
    }
    
    private void Awake()
    {
        Instance = this;
    }
    
    /// <summary>
    /// Sends a GET request and deserializes the response to type T
    /// </summary>
    public IEnumerator Get<T>(string endpoint, Action<T> onSuccess, Action<string> onError) where T : class
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{BaseUrl}/{endpoint}"))
        {
            Debug.Log($"LlmManager: Get request: /{endpoint}");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke($"Web request failed: {request.error}");
                yield break;
            }

            var content = request.downloadHandler.text;
            Debug.Log($"LlmManager: Get response: {content}");
            var result = JsonUtility.FromJson<T>(content);
            onSuccess?.Invoke(result);
        }
    }

    /// <summary>
    /// Queues one POST request, ensuring that only one executes at a time.
    /// </summary>
    public void QueuePostRequest<T, TRequest>(string endpoint, TRequest data, Action<T> onSuccess, Action<string> onError) 
        where T : class 
        where TRequest : class
    {
        // Dodajemy żądanie do kolejki
        postRequestQueue.Enqueue(Post<T, TRequest>(endpoint, data, onSuccess, onError));
        if (!isProcessingQueue)
        {
            StartCoroutine(ProcessPostQueue());
        }
    }

    private IEnumerator ProcessPostQueue()
    {
        isProcessingQueue = true;

        while (postRequestQueue.Count > 0)
        {
            var request = postRequestQueue.Dequeue();
            yield return StartCoroutine(request);
        }

        isProcessingQueue = false;
    }

    /// <summary>
    /// Sends a POST request with data and deserializes the response to type T
    /// </summary>
    private IEnumerator Post<T, TRequest>(string endpoint, TRequest data, Action<T> onSuccess, Action<string> onError) 
        where T : class 
        where TRequest : class
    {
        using (UnityWebRequest request = new UnityWebRequest($"{BaseUrl}/{endpoint}", "POST"))
        {
            var json = JsonUtility.ToJson(data);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log($"LlmManager: Post request: {json}");
            
            yield return request.SendWebRequest();

            var responseContent = request.downloadHandler.text;
            
            if (request.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke($"LlmManager: Post request failed ({request.error}): {responseContent}");
                yield break;
            }
            Debug.Log($"LlmManager: Post response: {responseContent}");

            var result = JsonUtility.FromJson<T>(responseContent);
            onSuccess?.Invoke(result);
        }
    }

    #region Endpoint handlers
    public void Status(Action<StatusDTO> onComplete, Action<string> onError)
    {
        StartCoroutine(Get<StatusDTO>("status", onComplete, onError));
    }

    public void LoadModel(string modelID, string path, Action<MessageDTO> onComplete, Action<string> onError)
    {
        var data = new LoadModelDTO()
        {
            model_id = modelID,
            model_path = path,
            f16_kv = true,
            n_ctx = 1024,
            n_parts = -1,
            seed = 42, // TODO: Make it randomized
            n_gpu_layers = -1,
        };
        
        QueuePostRequest<MessageDTO, LoadModelDTO>("load", data, onComplete, onError);
    }

    public void UnloadModel(string modelID, Action<MessageDTO> onComplete, Action<string> onError)
    {
        var data = new UnloadModelRequestDTO()
        {
            model_id = modelID
        };
        
        QueuePostRequest<MessageDTO, UnloadModelRequestDTO>("unload", data, onComplete, onError);
    }
    
    public void Chat(string modelID, List<Message> messages, Action<ChatResponseDTO> onComplete, Action<string> onError)
    {
        var data = new ChatRequestDTO()
        {
            model_id = modelID,
            messages = messages,
            max_tokens = 1500,
            temperature = 0.5f,
            top_p = 0.95f,
        };
        QueuePostRequest<ChatResponseDTO, ChatRequestDTO>("chat", data, onComplete, onError);
    }
    #endregion
    
    public void Connect(Action<bool> onComplete)
    {
        Status(
            healthData => 
            {
                IsConnected = healthData.healthy;
                
                foreach (var model in healthData.models)
                    UnloadModel(model, GenericComplete, Debug.LogError);
                
                onComplete?.Invoke(IsConnected);
            },
            error => 
            {
                Debug.LogError("Could not connect to LLM Server: " + error);
                IsConnected = false;
                onComplete?.Invoke(IsConnected);
            });
    }

    public void GenericComplete(MessageDTO message)
    {
        if (message.success)
            Debug.Log(message.message);
        else 
            Debug.LogError(message.message);
    }
    
    #region Console Commands
    
    [ConsoleCommand("llmstatus", "Checks the status of the LLM server")]
    public static bool StatusCommand()
    {
        Instance.Status(statusData => 
                Debug.Log($"LLM Server status: healthy: {statusData.healthy}\nLoaded models: {string.Join("\n", statusData.models)}"),
            error => 
                Debug.LogError(error));
        return true;
    }
    
    #endregion
}