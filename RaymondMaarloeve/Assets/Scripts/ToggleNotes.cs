using UnityEngine;

public class ToggleNotes : MonoBehaviour
{
    [SerializeField] private GameObject notesParent;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (notesParent != null)
            {
                notesParent.SetActive(!notesParent.activeSelf);
            }
        }
    }
}
