using System;
using UnityEngine;

namespace script {
    public class SampleSpeedByDistance : ISampleStrategy {
        private readonly AnimationCurve _animationCurve;
        private readonly float _deltaDistance;
        private readonly float _baseSpeed;
        private readonly float _deltaSpeed;
        private float _currentDistance = 0;

        public SampleSpeedByDistance(AnimationCurve curve, float deltaDistance, float minSpeed, float maxSpeed) {
            if (maxSpeed < minSpeed) {
                throw new Exception("maxSpeed < minSpeed");
            }

            _deltaSpeed = maxSpeed - minSpeed;
            _baseSpeed = minSpeed;
            _deltaDistance = deltaDistance;
            _animationCurve = curve;
        }

        public SampleResult Sample(float time) {
            bool isOverSample = false;
            var normalizedDistance = _currentDistance / _deltaDistance;
            if (normalizedDistance > 1) {
                normalizedDistance = 1;
                isOverSample = true;
            }

            var samp = _animationCurve.Evaluate(normalizedDistance);

            var speed = _baseSpeed + _deltaSpeed * samp;
            _currentDistance += speed * time;


            return new SampleResult() {SampleSpeed = speed, IsOverSample = isOverSample};
        }
    }
}