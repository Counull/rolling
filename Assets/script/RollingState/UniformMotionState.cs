using System;
using Unity.VisualScripting;

namespace script {
    public class UniformMotionState : RollingState {
        private float _speed;

        public float Speed {
            get => _result.SampleSpeed;
            set => _result.SampleSpeed = value;
        }

        private SampleResult _result = new() {SampleSpeed = 0};


        public override SampleResult Sample(float time) {
            return _result;
        }

        public override event Action OnComplete;
    }
}