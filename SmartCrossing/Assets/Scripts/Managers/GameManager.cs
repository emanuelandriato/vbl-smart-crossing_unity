using UnityEngine;
using TMPro;
using System.Collections;
using Scripts.Models;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject PlayerPrefab;
    public Transform SpawnPointPlayer;
    public GameSettings Settings;
    private PlayerController CurrentPlayer;
    
    //Audio
    public AudioSource AudioSource;
    public AudioClip WinSound;
    public AudioClip LoseSound;

    // HUD
    public TextMeshProUGUI Txt_timer;
    public TextMeshProUGUI Txt_score;
    public TextMeshProUGUI Txt_NextWeather;
    public TextMeshProUGUI Txt_Weather;
    public TextMeshProUGUI Txt_VehicleDensity;
    public TextMeshProUGUI Txt_VehicleAverageSpeed;
    public TextMeshProUGUI Txt_PlayerSpeed;

    private float RemainingTime;
    private bool IsCounting;
    private int Score = 0;

    // previsão
    private int CurrentPredictionIndex = 0;
    private float PredictionTimer = 5f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(WaitForApi());        
    }

    void Update()
    {
        if (IsCounting)
        {
            UpdateWeatherForecast();
            UpdateTimer();
        }        
    }

    private IEnumerator WaitForApi()
    {     
        while (!ApiDataManager.Instance.IsReady)
        {
            yield return null;
        }     
        SpawnPlayer();
        UpdateHUB();
    }

    public void SpawnPlayer()
    {
        var obj = Instantiate(PlayerPrefab, SpawnPointPlayer.position, SpawnPointPlayer.rotation);
        CurrentPlayer = obj.GetComponent<PlayerController>();

        StartRound();
    }

    public void StartCrossing()
    {
        if (CurrentPlayer == null) return;

        CurrentPlayer.StartCrossing();
    }

    public void OnPlayerFinished()
    {
        AudioSource.PlayOneShot(WinSound);
        Destroy(CurrentPlayer.gameObject);
        Score++;
        UpdateScoreUI();
        IsCounting = false;
        Invoke(nameof(SpawnPlayer), 2f);
    }

    public void OnPlayerColliderWithCar()
    {     
        AudioSource.PlayOneShot(LoseSound);
        Destroy(CurrentPlayer.gameObject);
        IsCounting = false;
        Invoke(nameof(SpawnPlayer), 2f);
    }

    void StartRound()
    {        
        ApiDataManager.Instance.UseNextPrediction();
        var data = ApiDataManager.Instance.CurrentCrossingData;

        if (data == null)
        {
            //Debug.LogWarning("Ainda não há dados da API!");
            RemainingTime = 15f;
            return;
        }

        RemainingTime = data != null ? (data.totalTime / 1000f) : 15f;
        CurrentPredictionIndex = 0;
        GameManager.Instance.NotifyWeatherChanged();
        PredictionTimer = 5f;
        IsCounting = true;

        UpdateTimerUI();
    }

    void UpdateTimer()
    {        
        RemainingTime -= Time.deltaTime;

        if (RemainingTime <= 0f && IsCounting)
        {            
            AudioSource.PlayOneShot(LoseSound);

            RemainingTime = 0f;
            IsCounting = false;

            if (CurrentPlayer != null)
                Destroy(CurrentPlayer.gameObject);

            Invoke(nameof(SpawnPlayer), 2f);
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        int seconds = Mathf.CeilToInt(RemainingTime);

        if (seconds > 0)
        {
            Txt_timer.text = $"{seconds}";
        }
        else
        {
            Txt_timer.text = string.Empty;
        }
    }

    void UpdateScoreUI()
    {
        Txt_score.text = $"Score: {Score}";
    }

    void UpdateWeatherForecast()
    {        
        var data = ApiDataManager.Instance.CurrentCrossingData;

        if (data == null || data.predictedStatus.Count == 0)
            return;

        PredictionTimer -= Time.deltaTime;

        if (PredictionTimer <= 0f)
        {
            CurrentPredictionIndex++;
            GameManager.Instance.NotifyWeatherChanged();

            if (CurrentPredictionIndex >= data.predictedStatus.Count)
            {
                CurrentPredictionIndex = data.predictedStatus.Count - 1;
            }            
            PredictionTimer = 5f;
        }
        //Debug.Log($"index:{CurrentPredictionIndex}");

        UpdateHUB();
    }

    void UpdateHUB()
    {
        var data = ApiDataManager.Instance.CurrentCrossingData;

        if (data == null || data.predictedStatus.Count == 0)
            return;
        
        var currentPrediction = data.predictedStatus[CurrentPredictionIndex];

        int seconds = Mathf.CeilToInt(PredictionTimer);

        Txt_NextWeather.text = $"Next Change: {seconds}s";
        Txt_Weather.text = $"Climate: {currentPrediction.weather}";
        Txt_VehicleDensity.text = $"Vehicle Density: {currentPrediction.vehicleDensity}%";
        Txt_VehicleAverageSpeed.text = $"Vehicle Speed: {Mathf.RoundToInt(currentPrediction.averageSpeed)} Km/h";

        if (CurrentPlayer != null)
        {
            Txt_PlayerSpeed.text = $"Player Speed: {Mathf.RoundToInt(CurrentPlayer.CurrentSpeed)}";
        }        
    }

    public string GetCurrentWeather()
    {
        var data = ApiDataManager.Instance.CurrentCrossingData;

        if (data == null || data.predictedStatus.Count == 0)
            return "sunny";

        return data.predictedStatus[CurrentPredictionIndex].weather;
    }

    public void NotifyWeatherChanged()
    {
        if (CurrentPlayer != null)
            CurrentPlayer.UpdateSpeed();
    }

    public PredictionData GetCurrentPrediction()
    {
        var data = ApiDataManager.Instance.CurrentCrossingData;

        if (data == null || data.predictedStatus.Count == 0)
            return null;

        return data.predictedStatus[
            Mathf.Clamp(CurrentPredictionIndex, 0, data.predictedStatus.Count - 1)
        ];
    }
}