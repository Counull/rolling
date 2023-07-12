using System;
using Unity.VisualScripting;

namespace script {
    public abstract class RollingState {
        protected RollingState() { }
        public abstract SampleResult Sample(float time);
        public abstract event Action OnComplete;
    }
}