using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NpcSettingsUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Dropdown narratorDropdown;
    [SerializeField] private Transform npcListContainer;
    [SerializeField] private GameObject npcRowPrefab;
    [SerializeField] private Button addNpcButton;

    private readonly List<GameObject> npcRows = new List<GameObject>();

    // przyk³adowe nazwy modeli – póŸniej podmienisz na swoje
    private readonly List<string> modelOptions = new List<string>
    {
        "unsloth.Q4_K_M.gguf",
        "model_2.gguf",
        "model_3.gguf"
    };

    private void Start()
    {
        SetupNarratorDropdown();
        addNpcButton.onClick.AddListener(OnAddNpcClicked);

        // Na start dodajmy np. dwa NPC jak na screenie
        AddNpcRow("NPC1");
        AddNpcRow("NPC2");
    }

    private void SetupNarratorDropdown()
    {
        narratorDropdown.ClearOptions();
        narratorDropdown.AddOptions(modelOptions);
        narratorDropdown.value = 0;
    }

    private void AddNpcRow(string npcName)
    {
        var row = Instantiate(npcRowPrefab, npcListContainer);
        npcRows.Add(row);

        // Ustaw label
        var label = row.transform.Find("NpcLabel").GetComponent<TMP_Text>();
        label.text = npcName;

        // Ustaw dropdown z modelami
        var dropdown = row.transform.Find("ModelDropdown").GetComponent<TMP_Dropdown>();
        dropdown.ClearOptions();
        dropdown.AddOptions(modelOptions);

        // Pod³¹cz przycisk kasowania
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
}
