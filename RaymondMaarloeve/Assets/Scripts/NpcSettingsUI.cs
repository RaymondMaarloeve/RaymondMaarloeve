using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NpcSettingsUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField llmApiInputField; // Nowe pole na adres API
    [SerializeField] private Transform npcListContainer;
    [SerializeField] private GameObject npcRowPrefab;
    [SerializeField] private Button addNpcButton;
    [SerializeField] private Button saveConfigButton;

    private readonly List<GameObject> npcRows = new List<GameObject>();

    private void Start()
    {
        addNpcButton.onClick.AddListener(OnAddNpcClicked);
        saveConfigButton.onClick.AddListener(SaveConfig);

        if (llmApiInputField != null)
            llmApiInputField.text = "http://127.0.0.1:5000/";

        AddNpcRow("NPC 1");
        AddNpcRow("NPC 2");
    }

    private void AddNpcRow(string npcName)
    {
        var row = Instantiate(npcRowPrefab, npcListContainer);
        npcRows.Add(row);

        var label = row.transform.Find("NpcLabel").GetComponent<TMP_Text>();
        label.text = npcName;

        var descriptionInput = row.transform.Find("ModelInputField").GetComponent<TMP_InputField>();
        descriptionInput.text = "Wpisz opis modelu...";

        var deleteButton = row.transform.Find("DeleteButton").GetComponent<Button>();
        deleteButton.onClick.AddListener(() => RemoveNpcRow(row));
    }

    private void RemoveNpcRow(GameObject row)
    {
        npcRows.Remove(row);
        Destroy(row);
    }

    private void OnAddNpcClicked()
    {
        string npcName = $"NPC {npcRows.Count + 1}";
        AddNpcRow(npcName);
    }

    public void SaveConfig()
    {
        NpcSettingsConfig config = new NpcSettingsConfig();

        // Pobieramy adres API z pola tekstowego
        config.LlmServerApi = llmApiInputField.text;

        // Pakujemy listê NPC
        for (int i = 0; i < npcRows.Count; i++)
        {
            var descriptionInput = npcRows[i].transform.Find("ModelInputField").GetComponent<TMP_InputField>();

            config.Models.Add(new NpcModelData
            {
                Id = i,
                Description = descriptionInput.text
            });
        }

        string json = JsonUtility.ToJson(config, true);
        string filePath = Path.Combine(Application.persistentDataPath, "game_config.json");

        File.WriteAllText(filePath, json);

        Debug.Log($"<b>[Zapisano]</b> {filePath}");
        Debug.Log(json);
    }
}

// --- KLASY DANYCH ---

[System.Serializable]
public class NpcModelData
{
    public int Id;
    public string Description;
}

[System.Serializable]
public class NpcSettingsConfig
{
    public int Revision = 1;
    public string LlmServerApi; // Wartoœæ pobierana z UI
    public bool Localhost = true;
    public bool FullScreen = false;
    public int GameWindowWidth = 1920;
    public int GameWindowHeight = 1080;
    public List<NpcModelData> Models = new List<NpcModelData>();
}