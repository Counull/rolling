using System;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace script {
    public struct SampleResult {
        public bool IsOverSample;
        public float SampleSpeed;
    }

    public interface ISampleStrategy {
        public SampleResult Sample(float time);
      
    }
}