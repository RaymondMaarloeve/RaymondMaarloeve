using UnityEngine;

public class SettingsTab : MonoBehaviour
{
    [SerializeField] private GameObject generalPanel;
    [SerializeField] private GameObject npcPanel;

    private void Start()
    {
        ShowGeneral();
    }

    public void ShowGeneral()
    {
        generalPanel.SetActive(true);
        npcPanel.SetActive(false);
    }

    public void ShowNpc()
    {
        generalPanel.SetActive(false);
        npcPanel.SetActive(true);
    }
}
