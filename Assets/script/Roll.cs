using System;
using System.Collections;
using System.Collections.Generic;
using script;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Roll : MonoBehaviour {
    private int currentPos = 0;

    private const int CellCount = 12;

    private const int UnitDeg = 360 / CellCount;
    private RollingState _rollingState;
    private RollingState _NextState;
    [SerializeField] private AnimationCurve _speedUp;
    [SerializeField] private AnimationCurve _breaking;


    private float _currentSpeed = 0;

    private void Update() {
        if (_rollingState != null) {
            var current = gameObject.transform.rotation.eulerAngles;
            var ret = _rollingState.Sample(Time.deltaTime);
            _currentSpeed = ret.SampleSpeed;

            if (ret.IsOverSample) {
                _rollingState = _NextState;
                _NextState = null;
                return;
            }

            current.z += _currentSpeed * Time.deltaTime;
            gameObject.transform.rotation = Quaternion.Euler(current);
        }
    }


    private void OnMouseDown() {
        _rollingState = new SpeedChange(_speedUp, 0, 1800, 3f, () => {
            _NextState = new SpeedChange(_breaking, 10, _currentSpeed,
                gameObject.transform.rotation.eulerAngles.z,
                UnitDeg * 3 + 360 * 5, null);
        });
    }
}