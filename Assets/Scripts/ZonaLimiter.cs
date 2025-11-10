using UnityEngine;

public class ZonaLimiter : MonoBehaviour
{
    private MissionManager mission;
    private float exitCooldown = 1f; // segundos de tolerancia tras cambiar de modo

    void Start()
    {
        mission = FindObjectOfType<MissionManager>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Ignora salidas si el dron acaba de cambiar de modo
        DroneController_Visual drone = other.GetComponent<DroneController_Visual>();
        if (drone && Time.time - drone.lastModeSwitchTime < exitCooldown)
            return;

        mission.RestartMission();
    }
}
