using UnityEngine;
using System.Collections.Generic;
using Scripts.Models;

namespace Scripts.Models
{
    [System.Serializable]
    public class CrossingData
    {        
        public int vehicleDensity;

        public float averageSpeed;

        public string weather;

        public int totalTime;

        public List<PredictionData> predictedStatus = new List<PredictionData>();
    }
}