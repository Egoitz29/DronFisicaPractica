using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    public GameObject platformPrefab;
    public int initialPlatforms = 5;
    public float spacing = 10;
    public float noiseScale = 0.3f; // intensidad del ruido
    public float heightAmplitude = 2f; // altura máxima

    private float lastZ;

    void Start()
    {
        lastZ = 0f;
        for (int i = 0; i < initialPlatforms; i++)
            GenerateNextPlatform();
    }

    void GenerateNextPlatform()
    {
        float noise = Mathf.PerlinNoise(0, lastZ * noiseScale);
        float yOffset = (noise - 0.5f) * heightAmplitude;
        Vector3 spawnPos = new Vector3(0, yOffset, lastZ);
        Instantiate(platformPrefab, spawnPos, Quaternion.identity);
        lastZ += spacing;
    }
}
