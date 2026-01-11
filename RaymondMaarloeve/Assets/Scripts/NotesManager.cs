using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotesManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject notesParent;       // Panel notatek
    [SerializeField] private TMP_InputField inputNote;
    [SerializeField] private TMP_Dropdown dropdown;

    private Dictionary<NPC, string> notes = new Dictionary<NPC, string>();

    private void Start()
    {
        if (notesParent != null)
            notesParent.SetActive(false); // domyœlnie ukrywamy panel notatek

        StartCoroutine(InitializeWhenNPCsReady());
    }



    private IEnumerator InitializeWhenNPCsReady()
    {
        while (GameManager.Instance == null || GameManager.Instance.npcs == null || GameManager.Instance.npcs.Count == 0)
        {
            yield return null;
        }

        List<NPC> npcList = GameManager.Instance.npcs;

        dropdown.ClearOptions();

        List<string> optionNames = new List<string>();
        foreach (var npc in npcList)
        {
            optionNames.Add(npc.Name);
            notes[npc] = "";
        }

        dropdown.AddOptions(optionNames);
        dropdown.value = 0;
        dropdown.RefreshShownValue();

        inputNote.onValueChanged.AddListener(OnTextChanged);
        dropdown.onValueChanged.AddListener(OnDropdownChanged);

        UpdateInputField();
    }

    private void OnTextChanged(string text)
    {
        NPC selected = GetSelectedNPC();
        notes[selected] = text;
    }

    private void OnDropdownChanged(int index)
    {
        UpdateInputField();
        ActivateInputAtEnd();
    }

    private void UpdateInputField()
    {
        NPC selected = GetSelectedNPC();
        inputNote.text = notes[selected];
    }

    // -----------------------------
    // Ustawia kursor na koñcu tekstu bez zaznaczania
    public void ActivateInputAtEnd()
    {
        inputNote.Select();
        inputNote.ActivateInputField();
        int textLength = inputNote.text.Length;
        inputNote.caretPosition = textLength;
        inputNote.selectionAnchorPosition = textLength;
        inputNote.selectionFocusPosition = textLength;
    }

    private NPC GetSelectedNPC()
    {
        return GameManager.Instance.npcs[dropdown.value];
    }
}
