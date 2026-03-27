using UnityEngine;
using Scripts.Models;

public class VehicleController : MonoBehaviour
{
    public GameSettings Settings;
    private float currentSpeed;

    void Update()
    {
        UpdateSpeed();
        Move();
    }

    void Move()
    {
        transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);
    }

    void UpdateSpeed()
    {
        var prediction = GameManager.Instance.GetCurrentPrediction();
        if (prediction == null) return;

        currentSpeed = (prediction.averageSpeed / 100f) * Settings.VehicleBaseSpeed;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}