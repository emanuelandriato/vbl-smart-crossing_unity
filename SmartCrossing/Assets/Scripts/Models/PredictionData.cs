using UnityEngine;
using System.Collections.Generic;

namespace Scripts.Models
{
    [System.Serializable]
    public class PredictionData
    {
        public int timeMs;
        public string weather;
        public float vehicleDensity;
        public float averageSpeed;
    }
}