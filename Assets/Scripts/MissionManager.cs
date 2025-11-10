using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public HUDManager hud;
    public int enemiesToDestroy = 3;
    public float startDelay = 5f;

    private int destroyedCount = 0;
    private bool missionStarted = false;

    void Start()
    {
        hud.ShowMessage("Presiona TAB para alternar modo de vuelo\nClic izquierdo para disparar", 4f);
    }


    void SpawnEnemies()
    {
        for (int i = 0; i < enemiesToDestroy; i++)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemyPrefab, point.position, Quaternion.identity);
        }
    }

    public void EnemyDestroyed()
    {
        destroyedCount++;
        if (destroyedCount >= enemiesToDestroy)
        {
            hud.ShowMessage("¡Objetivo cumplido! Entrenamiento completado.", 6f);
            Invoke(nameof(EndMission), 5f);
        }
    }

    public void RestartMission()
    {
        hud.ShowMessage("Zona abandonada. Reiniciando...", 3f);
        Invoke(nameof(ReloadScene), 3f);
    }

    void EndMission()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
