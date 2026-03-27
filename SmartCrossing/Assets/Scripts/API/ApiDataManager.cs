using UnityEngine;
using System.Threading.Tasks;
using Scripts.Models;
using System;
using System.Threading;
using System.Collections.Generic;

public class ApiDataManager : MonoBehaviour
{    
    public static ApiDataManager Instance { get; private set; }
    public GameSettings Settings;
    private readonly object _lock = new object();
    private bool _RefreshingData = false;
    private CancellationTokenSource _cts;    
    private List<CrossingData> CrossingDataList = new List<CrossingData>();     //Lista privada para manter o carregamento das previsões da api    
    public CrossingData CurrentCrossingData = new CrossingData();               //dados atuais da previsão    
    public bool IsReady { get; private set; } = false;                          //o jogo somente pode iniciar depois que possuir dados da API, variavel para controlar isso

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

   private async void Start()
    {
        _cts = new CancellationTokenSource();
        await FetchDataFromApi();                
        UseNextPrediction();        
        _ = RefreshLoop(_cts.Token);
    }

    private void OnDestroy()
    {
        // Cancela o loop quando o objeto for destruído ou quando der stop
        if (_cts != null)
            _cts.Cancel();
    }

    private async Task RefreshLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await FetchDataFromApi();
            await Task.Delay(System.TimeSpan.FromSeconds(Settings.ApiRefreshInterval), token);
        }
    }
    
    private async Task FetchDataFromApi()
    {
        if (_RefreshingData == true)
        {
            return;
        }

        if (CrossingDataList.Count >= 2)
        {
            lock (_lock)
            {
                CrossingDataList.RemoveAt(CrossingDataList.Count-1);
            }
        }

        try
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                string baseUrl = Settings.ApiBaseUrl;
                Uri baseUri = new Uri(baseUrl, UriKind.Absolute);
                Uri fullUri = new Uri(baseUri, "crossing");

                var response = await client.GetStringAsync(fullUri);                

                CrossingData data = JsonUtility.FromJson<CrossingData>(response);
                //Debug.Log(JsonUtility.ToJson(data, true));

                if (data != null)
                {                    
                    CrossingDataList.Add(data);
                    if (CrossingDataList.Count > 0)
                        IsReady = true;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Erro ao buscar API: {ex.Message}");
        }        
    }

    //método para clonar a proxima previsao para o objeto utilizado no jogo
    private void ApplyNextPrediction()
    {        
        var data = CrossingDataList[0];
        CurrentCrossingData.vehicleDensity = data.vehicleDensity;
        CurrentCrossingData.averageSpeed = data.averageSpeed;
        CurrentCrossingData.weather = data.weather;
        CurrentCrossingData.totalTime = data.totalTime;

        CurrentCrossingData.predictedStatus.Clear();
        foreach (var p in data.predictedStatus)
        {
            CurrentCrossingData.predictedStatus.Add(new PredictionData
            {
                timeMs = p.timeMs,
                vehicleDensity = p.vehicleDensity,
                averageSpeed = p.averageSpeed,
                weather = p.weather
            });
        }
    }

    //metodo publico para que seja chamado sempre que um ciclo de previsoes for utilizado e um novo deve ser carregado
    public void UseNextPrediction()
    {
        lock (_lock)
        {
            if (CrossingDataList.Count == 0)
                return;
            
            _RefreshingData = true;
            ApplyNextPrediction();
            if (CrossingDataList.Count > 0)
                CrossingDataList.RemoveAt(0);
            _RefreshingData = false;
        }
    }

}