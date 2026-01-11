using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NpcActionIcons", menuName = "NPC/Action Icons Database")]
public class NpcActionIcons : ScriptableObject
{
    [System.Serializable]
    public struct ActionIcon
    {
        public string ActionName; // Np. "ChopWood", "Pray" lub PrettyName
        public Sprite Icon;
    }

    public List<ActionIcon> icons = new List<ActionIcon>();
    public Sprite defaultIcon; // Ikonka domyœlna (np. znak zapytania)

    public Sprite GetIcon(string actionName)
    {
        foreach (var item in icons)
        {
            // Porównujemy bez uwzglêdniania wielkoœci liter
            if (item.ActionName.ToLower().Contains(actionName.ToLower()) ||
                actionName.ToLower().Contains(item.ActionName.ToLower()))
            {
                return item.Icon;
            }
        }
        return defaultIcon;
    }
}