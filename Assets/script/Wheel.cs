using System;
using System.Collections;
using System.Collections.Generic;
using script;
using script.RollingState;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Wheel : MonoBehaviour
{
    private RollingState _rollingState;
    private RollingState _nextState;
    [SerializeField] private AnimationCurve speedUp;
    [SerializeField] private AnimationCurve breaking;


    private LinkedList<RollingState> _statesPlanning = new LinkedList<RollingState>();

    private readonly UniformMotionState _uniformMotionStateInf = new UniformMotionState(1.0f, true);

    private float _currentSpeed = 0;

    //反转
    public bool reverse = false;


    float CheckRevers()
    {
        return reverse ? -1f : 1f;
    }

    private void Update()
    {
        if (_rollingState != null)
        {
            var current = gameObject.transform.rotation.eulerAngles;
            var ret = _rollingState.Sample(Time.deltaTime);
            _currentSpeed = ret.SampleSpeed;

            if (ret.IsOverSample)
            {
                _rollingState = _nextState;
                _nextState = null;
                return;
            }

            current.z += _currentSpeed * Time.deltaTime * CheckRevers();
            Debug.Log(_currentSpeed);
            gameObject.transform.rotation = Quaternion.Euler(current);
        }
    }


    #region 示例

#if UNITY_EDITOR
    bool _testRolling = false;


    //自动测试结果是否准确
    IEnumerator Test()
    {
        while (true)
        {
            if (_testRolling)
            {
                yield return null;
                continue;
            }

            Debug.Log("=============================");
            _testRolling = true;

            reverse = Random.value >= 0.5F;
            Debug.Log($"Reverse:{reverse}");

            var targetSpeed = Random.Range(100, 1800);
            Debug.Log($"target Speed:{targetSpeed}");

            //加速阶段
            _rollingState = new SpeedChange(speedUp, 10, targetSpeed, 3f, null);
            //匀速状态
            _uniformMotionStateInf.Speed = targetSpeed;
            _nextState = _uniformMotionStateInf;

            //随机切换减速至结果
            var time = Random.Range(0.01f, 5);
            Debug.Log($"Shutdown time:{time}");
            yield return new WaitForSeconds(time);

            //随即目标
            float targetDeg = Random.Range(0, 360);
            Debug.Log($"Shutdown Speed：{_currentSpeed}");
            _nextState = null;


            //切换状态为有目标的减速
            _rollingState = new SpeedChange(breaking, 10, _currentSpeed, gameObject.transform.rotation.eulerAngles.z,
                targetDeg + 3 * 360f, reverse, () =>
                {
                    var curr = gameObject.transform.rotation.eulerAngles.z;
                    Debug.Log($"target:{targetDeg}  result:{curr}  bias:{targetDeg - curr}");
                    Assert.IsTrue(curr > targetDeg - 0.15 && curr < targetDeg + 0.15);
                    _testRolling = false;
                });
        }
    }

    public bool testPlanning = false;

    private void OnMouseDown()
    {
        if (testPlanning)
        {
            PlanningState();
            _rollingState = _statesPlanning.First.Value;
            return;
        }

        StartCoroutine(Test());
    }

    private void PlanningState()
    {
        float[] targetSpeed = { 1200f, 360f, 15 };
        float targetDeg = 75f;
        _statesPlanning.AddLast(_rollingState = new SpeedChange(speedUp, 10, targetSpeed[0], 3f, NextStep));
        _statesPlanning.AddLast(new UniformMotionState(targetSpeed[0], 2f, NextStep));

        void OnNeedStop()
        {
            _statesPlanning.AddLast(new SpeedChange(breaking, targetSpeed[2], _currentSpeed,
                transform.rotation.eulerAngles.z, targetDeg + 360F, reverse, NextStep));
            NextStep();
        }

        _statesPlanning.AddLast(new SpeedChange(breaking, targetSpeed[1], targetSpeed[0], 2f, OnNeedStop));
    }


    void NextStep()
    {
        ;
        _statesPlanning.RemoveFirst();
        if (_statesPlanning.Count <= 0)
        {
            return;
        }

        _nextState = _statesPlanning.First.Value;
        Debug.LogWarning($"NextStep：{_nextState.GetType().Name}==========================");
    }

#endif

    #endregion
}