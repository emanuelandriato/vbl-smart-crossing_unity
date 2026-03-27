using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Game/GameSettings")]
public class GameSettings : ScriptableObject
{
    [Header("API")]
    public string ApiBaseUrl = "http://localhost:5050/api/";        //URL da API
    public float ApiRefreshInterval = 10f;                          //tempo em segundos que a API será solicitada para buscar a próxima previsão

    [Header("Pedestre")]
    public float PlayerBaseSpeed = 5f;                              //velocidade base do jogador

    [Header("Veículos")]
    public float VehicleBaseSpeed = 20f;                            //velocidade base dos veiculos




}