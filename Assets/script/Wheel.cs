using System;
using System.Collections;
using System.Collections.Generic;
using script;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Wheel : MonoBehaviour {
    private int currentPos = 0;

    private const int CellCount = 12;

    private const int UnitDeg = 360 / CellCount;
    private RollingState _rollingState;
    private RollingState _NextState;
    [SerializeField] private AnimationCurve _speedUp;
    [SerializeField] private AnimationCurve _breaking;

    private UniformMotionState _uniformMotionState = new UniformMotionState();

    private float _currentSpeed = 0;

    public bool reverse = false;


    float CheckRevers() {
        return reverse ? -1f : 1f;
    }

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

            current.z += _currentSpeed * Time.deltaTime * CheckRevers();
            gameObject.transform.rotation = Quaternion.Euler(current);
        }
    }

    bool rolling = false;

    IEnumerator Test() {
        while (true) {
            if (rolling) {
                yield return null;
                continue;
            }

            Debug.Log("=============================");
            rolling = true;

            var targetSpeed = Random.Range(100, 1800);
            Debug.Log($"target Speed:{targetSpeed}");
            _rollingState = new SpeedChange(_speedUp, 10, targetSpeed, 3f, null);
            _uniformMotionState.Speed = targetSpeed;
            _NextState = _uniformMotionState;

            var time = Random.Range(0.01f, 5);
            Debug.Log($"Shutdown time:{time}");
            yield return new WaitForSeconds(time);
            float targetDeg = Random.Range(0, 360);

            Debug.Log($"Shutdown Speed：{_currentSpeed}");
            _NextState = null;

            _rollingState = new SpeedChange(_breaking, 10, _currentSpeed, gameObject.transform.rotation.eulerAngles.z,
                targetDeg + 3 * 360f, reverse, () => {
                    var curr = gameObject.transform.rotation.eulerAngles.z;

                    Debug.Log($"target:{targetDeg}  result:{curr}  bias:{targetDeg - curr}");
                    Assert.IsTrue(curr > targetDeg - 0.15 && curr < targetDeg + 0.15);
                    rolling = false;
                });
        }
    }

    private void OnMouseDown() {
        StartCoroutine(Test());
    }
}