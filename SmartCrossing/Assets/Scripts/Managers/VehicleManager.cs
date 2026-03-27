using UnityEngine;
using Scripts.Models;

public class VehicleManager : MonoBehaviour
{
    public GameObject VehiclePrefab;
    public Transform SpawnPoint;
    public GameSettings Settings;

    private float MinSpacingTime = 3f; // tempo mínimo entre carros (em segundos)
    private float LastSpawnTime = -Mathf.Infinity;
    private float SpawnTimer = 0f;

    void Update()
    {
        var prediction = GameManager.Instance.GetCurrentPrediction();
        if (prediction == null) return;

        float density = prediction.vehicleDensity;

        if (density <= 0f) return;

        float spawnInterval = 1f / density;

        SpawnTimer -= Time.deltaTime;

        if (SpawnTimer <= 0f)
        {
            float speedFactor = GetCurrentSpeedFactor();

            float dynamicSpacing = MinSpacingTime * speedFactor;

            if (Time.time - LastSpawnTime >= dynamicSpacing && CanSpawn())
            {
                SpawnVehicle();
                SpawnTimer = spawnInterval;
                LastSpawnTime = Time.time;
            }
        }
    }

    float GetCurrentSpeedFactor()
    {
        var prediction = GameManager.Instance.GetCurrentPrediction();
        if (prediction == null) return 1f;
        float normalizedSpeed = prediction.averageSpeed / 100f;        
        return Mathf.Lerp(0.5f, 2f, normalizedSpeed);
    }

    void SpawnVehicle()
    {
        var obj = Instantiate(VehiclePrefab, SpawnPoint.position, SpawnPoint.rotation);

        var vehicle = obj.GetComponent<VehicleController>();

        if (vehicle == null)
        {
            Debug.LogError("VehiclePrefab não tem VehicleController!");
            return;
        }

        // 👇 NOVO: injeta Settings no veículo
        vehicle.Settings = Settings;
    }

    bool CanSpawn()
    {
        float checkRadius = 2f;

        Collider[] hits = Physics.OverlapSphere(SpawnPoint.position, checkRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Vehicles"))
            {
                return false;
            }
        }

        return true;
    }
}