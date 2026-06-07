using UnityEngine;
using TMPro;

public class SineWaveTextAnimation : MonoBehaviour
{
    [SerializeField] private bool isPlaying = false;
    [SerializeField] private float amplitude = 5f; // How far the letters float
    [SerializeField] private float frequency = 2f; // Speed of the float
    [SerializeField] private float waveOffset = 0.2f; // Offset between each letter
    [SerializeField] private TMP_Text textMesh;
    private Vector3[] originalVertices;

    private bool isOriginalPostion = false;

    void Awake()
    {
        textMesh = GetComponent<TMP_Text>();
    }
    void Start()
    {
        CacheOriginalVertices();
    }

    void CacheOriginalVertices()
    {
        textMesh.ForceMeshUpdate();
        originalVertices = textMesh.mesh.vertices;
        //textMesh.textInfo.CopyMeshInfoVertexData();
    }

    void LateUpdate()
    {
        if(isPlaying)
        {
            AnimateText();
        }

        if(!isPlaying && !isOriginalPostion)
        {
            RestorePosition();
        }
    }

    void RestorePosition()
    {
        var mesh = textMesh.mesh;
        mesh.vertices = originalVertices;
        textMesh.canvasRenderer.SetMesh(mesh);
        isOriginalPostion = true;
    } 

    void AnimateText()
    {
        isOriginalPostion = false;
        // Get the updated mesh and vertices
        textMesh.ForceMeshUpdate();
        var mesh = textMesh.mesh;
        var vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            int charIndex = i / 4; // Each character is represented by 4 vertices
            float wave = Mathf.Sin(Time.time * frequency + charIndex * waveOffset);
            vertices[i].y = originalVertices[i].y + wave * amplitude;
        }

        // Apply the modified vertices back to the mesh
        mesh.vertices = vertices;
        textMesh.canvasRenderer.SetMesh(mesh);
    }
}
