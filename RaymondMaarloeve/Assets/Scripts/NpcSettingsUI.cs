using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NpcSettingsUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField narratorInputField;
    [SerializeField] private Transform npcListContainer;
    [SerializeField] private GameObject npcRowPrefab;
    [SerializeField] private Button addNpcButton;

    private readonly List<GameObject> npcRows = new List<GameObject>();

    private string defaultModel = "Describe npc character";

    private void Start()
    {
        SetupNarratorInput();
        addNpcButton.onClick.AddListener(OnAddNpcClicked);

        AddNpcRow("NPC1");
        AddNpcRow("NPC2");
    }

    private void SetupNarratorInput()
    {
        narratorInputField.text = defaultModel;
    }

    private void AddNpcRow(string npcName)
    {
        var row = Instantiate(npcRowPrefab, npcListContainer);
        npcRows.Add(row);

        var label = row.transform.Find("NpcLabel").GetComponent<TMP_Text>();
        label.text = npcName;

        var modelInput = row.transform.Find("ModelInputField").GetComponent<TMP_InputField>();
        modelInput.text = defaultModel;

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
        string npcName = $"NPC{npcRows.Count + 1}";
        AddNpcRow(npcName);
    }

    public void LogCurrentSettings()
    {
        Debug.Log($"Narrator Model: {narratorInputField.text}");
        foreach (var row in npcRows)
        {
            var name = row.transform.Find("NpcLabel").GetComponent<TMP_Text>().text;
            var model = row.transform.Find("ModelInputField").GetComponent<TMP_InputField>().text;
            Debug.Log($"NPC: {name}, Model: {model}");
        }
    }
}