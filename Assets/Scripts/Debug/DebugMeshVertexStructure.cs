using UnityEngine;

public class DebugMeshVertexStructure : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Debug.Log($"[DEBUG] Has Vertex Colors? {mesh.colors.Length > 0}");
    }
}
