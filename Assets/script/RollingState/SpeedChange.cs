using System;
using Unity.VisualScripting;
using UnityEngine;

namespace script {
    public sealed class SpeedChange : RollingState {
        private readonly ISampleStrategy _sampleStrategy;

        private int _target;


        public SpeedChange(AnimationCurve curve, float minSpeed, float maxSpeed, float during, Action onComplete) {
            if (minSpeed > maxSpeed) {
                (minSpeed, maxSpeed) = (maxSpeed, minSpeed);
            }

            _sampleStrategy = new SampleSpeedByTime(curve, minSpeed, maxSpeed, during);
            OnComplete = onComplete;
        }

        public SpeedChange(AnimationCurve curve, float minSpeed, float maxSpeed, float currentDeg,
            float targetDeg, Action onComplete) {
            if (minSpeed > maxSpeed) {
                (minSpeed, maxSpeed) = (maxSpeed, minSpeed);
            }


            var deltaDeg = targetDeg - currentDeg;
            _sampleStrategy = new SampleSpeedByDistance(curve, deltaDeg, minSpeed, maxSpeed);
            OnComplete = onComplete;
        }


        public override SampleResult Sample(float time) {
            var ret = _sampleStrategy.Sample(time);
            if (ret.IsOverSample) {
                OnComplete?.Invoke();
            }

            return ret;
        }

        public override event Action OnComplete;
    }
}