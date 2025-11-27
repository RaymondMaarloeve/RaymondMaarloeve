using UnityEngine;

public class SettingsTab : MonoBehaviour
{
    [SerializeField] private GameObject generalPanel;
    [SerializeField] private GameObject npcPanel;

    private void Start()
    {
        ShowGeneral();   // na starcie poka¿ ogólne
    }

    public void ShowGeneral()
    {
        if (generalPanel != null) generalPanel.SetActive(true);
        if (npcPanel != null) npcPanel.SetActive(false);
    }

    public void ShowNpc()
    {
        if (generalPanel != null) generalPanel.SetActive(false);
        if (npcPanel != null) npcPanel.SetActive(true);
    }
}
