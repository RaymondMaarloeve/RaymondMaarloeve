using UnityEngine;
using UnityEngine.UI;

public class MinimapIndicator : MonoBehaviour
{
    [Header("Minimap setup")]
    public RectTransform minimapRect;     // referencja do UI minimapy
    public Camera minimapCamera;          // kamera minimapy
    public GameObject indicatorPrefab;    // prefab wskaźnika (kropka)

    private Transform[] npcs;
    private RectTransform[] indicators;

    void Start()
    {
        // --- CHECK CONFIG ---
        if (minimapRect == null)
        {
            Debug.LogWarning("[MinimapIndicator] ❌ Brak przypisanego minimapRect (RectTransform minimapy).");
        }

        if (minimapCamera == null)
        {
            Debug.LogWarning("[MinimapIndicator] ❌ Brak przypisanej kamery minimapy.");
        }

        if (indicatorPrefab == null)
        {
            Debug.LogWarning("[MinimapIndicator] ❌ Brak przypisanego prefabu wskaźnika (indicatorPrefab).");
        }

        // --- FIND NPCs ---
        GameObject[] found = GameObject.FindGameObjectsWithTag("NPC");
        if (found.Length == 0)
        {
            Debug.LogWarning("[MinimapIndicator] ⚠️ Nie znaleziono żadnych obiektów z tagiem 'NPC'.");
        }
        else
        {
            Debug.Log($"[MinimapIndicator] ✅ Znaleziono {found.Length} NPC.");
            foreach (GameObject go in found)
            {
                Debug.Log($"NPC name : {go.name}");
            }
        }

        npcs = new Transform[found.Length];
        indicators = new RectTransform[found.Length];

        for (int i = 0; i < found.Length; i++)
        {
            npcs[i] = found[i].transform;

            GameObject go = Instantiate(indicatorPrefab, minimapRect);
            indicators[i] = go.GetComponent<RectTransform>();

            if (indicators[i] == null)
                Debug.LogWarning($"[MinimapIndicator] ⚠️ Prefab '{indicatorPrefab.name}' nie ma komponentu RectTransform.");

            indicators[i].gameObject.SetActive(true);
        }

        Debug.Log("[MinimapIndicator] 🔧 Inicjalizacja zakończona.");
    }

    void Update()
    {
        if (npcs == null || npcs.Length == 0)
        {
            Debug.LogWarning("[MinimapIndicator] ⚠️ Brak listy NPC (może nie znaleziono żadnych?).");
            return;
        }

        if (minimapCamera == null || minimapRect == null)
        {
            Debug.LogWarning("[MinimapIndicator] ⚠️ Brak kamery lub RectTransform minimapy — nie mogę aktualizować wskaźników.");
            return;
        }

        for (int i = 0; i < npcs.Length; i++)
        {
            if (npcs[i] == null)
            {
                indicators[i].gameObject.SetActive(false);
                Debug.LogWarning($"[MinimapIndicator] ⚠️ NPC nr {i} został zniszczony lub nie istnieje.");
                continue;
            }

            UpdateIndicator(i);
        }
    }

    void UpdateIndicator(int i)
    {
        Transform npc = npcs[i];
        Vector3 vp = minimapCamera.WorldToViewportPoint(npc.position);

        // NPC w kadrze minimapy
        if (vp.z > 0f && vp.x >= 0f && vp.x <= 1f && vp.y >= 0f && vp.y <= 1f)
        {
            indicators[i].gameObject.SetActive(false);
            return;
        }

        // Poza kadrem — pokaż wskaźnik
        indicators[i].gameObject.SetActive(true);

        // Kierunek od środka (0.5, 0.5)
        Vector2 dir = ((Vector2)vp - new Vector2(0.5f, 0.5f)).normalized;

        if (float.IsNaN(dir.x) || float.IsNaN(dir.y))
        {
            Debug.LogWarning($"[MinimapIndicator] ⚠️ Błąd normalizacji pozycji viewport NPC {npc.name} (pozycja: {vp}).");
            return;
        }

        // Pozycja na krawędzi prostokąta
        float halfW = minimapRect.rect.width / 2f;
        float halfH = minimapRect.rect.height / 2f;

        float t = Mathf.Min(
            Mathf.Abs(halfW / dir.x),
            Mathf.Abs(halfH / dir.y)
        );

        Vector2 edgePos = dir * t;
        edgePos.x = Mathf.Clamp(edgePos.x, -halfW, halfW);
        edgePos.y = Mathf.Clamp(edgePos.y, -halfH, halfH);

        indicators[i].anchoredPosition = edgePos;

        // Debug log dla wizualizacji (opcjonalny)
        Debug.DrawLine(minimapCamera.transform.position, npc.position, Color.red);
    }
}
