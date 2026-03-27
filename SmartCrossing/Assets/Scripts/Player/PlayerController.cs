using UnityEngine;

public class PlayerController : MonoBehaviour
{    
    public float CurrentSpeed;
    private bool isMoving = false;
    public GameSettings Settings;    

    void Update()
    {
        if (!isMoving) return;
        UpdateSpeedFromWeather();
        Move();
    }

    void Move()
    {
        transform.Translate(Vector3.forward * CurrentSpeed * Time.deltaTime);
    }

    public void StartCrossing()
    {
        UpdateSpeedFromWeather();
        isMoving = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Colidiu com: " + other.name);
        if (other.CompareTag("Finish"))
        {         
            isMoving = false;
            GameManager.Instance.OnPlayerFinished();
        }
        else if (other.CompareTag("Vehicles"))
        {            
            isMoving = false;
            GameManager.Instance.OnPlayerColliderWithCar();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    public void UpdateSpeed()
    {
        UpdateSpeedFromWeather();
    }

    void UpdateSpeedFromWeather()
    {
        string weather = GameManager.Instance.GetCurrentWeather();
        float multiplier = GetWeatherMultiplier(weather);
        CurrentSpeed = Settings.PlayerBaseSpeed * multiplier;
    }

    float GetWeatherMultiplier(string weather)
    {
        /*
        Tempo limpo (sunny): 1.0x (Velocidade total). 
        Nublado/Neblina (clouded/foggy): 0.8x (Um pouco mais lento). 
        Chuva leve (light rain): 0.6x (Cuidado com o chão liso!). 
        Chuva pesada (heavy rain): 0.4x (Movimentação severamente afetada). 
        */
        switch (weather)
        {
            case "sunny":
                return 1.0f;

            case "clouded_foggy":            
                return 0.8f;

            case "light_rain":
                return 0.6f;

            case "heavy_rain":
                return 0.4f;

            default:
                return 1.0f;
        }
    }

}