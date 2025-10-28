using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class CombineMeshWithCollider : MonoBehaviour
{
    void Start()
    {
        // Pobiera wszystkie meshe z dzieci
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }

        // Tworzy nowy mesh po³¹czony z wszystkich dzieci
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine);

        // Ustawia mesh na rodzicu
        var mf = GetComponent<MeshFilter>();
        mf.mesh = combinedMesh;
        GetComponent<MeshCollider>().sharedMesh = combinedMesh;

        // Wy³¹cza renderowanie dzieci (nie usuwa ich)
        foreach (var rend in GetComponentsInChildren<MeshRenderer>())
        {
            if (rend.transform != transform)
                rend.enabled = false;
        }
    }
}
