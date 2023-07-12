using System;
using UnityEngine;

namespace script {
    public class SampleSpeedByTime : ISampleStrategy {
        private readonly AnimationCurve _curve;
        private readonly float _baseSpeed;
        private readonly float _deltaSpeed;
        private readonly float _during;

        private float _currentTime;

        public SampleSpeedByTime(AnimationCurve curve, float minSpeed, float maxSpeed, float during
        ) {
            _curve = curve;
            _deltaSpeed = maxSpeed - minSpeed;

            _during = during;
        }

        public SampleResult Sample(float time) {
            _currentTime += time;
            var normalized = _currentTime / _during;
            if (normalized >= 1) {
                return new SampleResult() {SampleSpeed = _baseSpeed + _deltaSpeed, IsOverSample = true};
            }

            var samp = _curve.Evaluate(normalized);
            var sampSpeed = _baseSpeed + samp * _deltaSpeed;
            return new SampleResult() {SampleSpeed = sampSpeed, IsOverSample = false};
        }
    }
}