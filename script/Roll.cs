using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Roll : MonoBehaviour {
    private int currentPos = 0;

    private const int CellCount = 12;

    private const int UnitDeg = 360 / CellCount;

    [SerializeField] private TMP_Text _target;
    private const int TargetRollingStep = 3 * CellCount;

    private int[] targets;

    private Status status = Status.Stop;

    [SerializeField] private AnimationCurve StartCurve;

    public int currentPoint = 3;

    int currentStep = 0;
    public int breakingStep = 6;
    public float rollingTime = 0;

    public Vector3 StartRollingRotate;

    public AnimationCurve breakingSpeedCurve;

    // Start is called before the first frame update
    void Start() { }


    private Vector3 targetRotate;

    IEnumerator roll() {
        if (status != Status.Stop) {
            yield break;
        }


        rollingTime = 0f;
        status = Status.Start;
        StartRollingRotate = transform.rotation.eulerAngles;

        int target = Random.Range(0, 6);
        Debug.Log(target);
        targets = new[] {
            target,
            target + CellCount / 2
        };
        _target.text = target.ToString();
        int endStep = target - currentPoint + TargetRollingStep;

        targetRotate = new Vector3(0, 0, endStep * UnitDeg);
        while (status != Status.Stop) {
            rollingTime += Time.deltaTime;

            switch (status) {
                case Status.Start:
                    StartMove();
                    break;
                case Status.Rolling:
                    RollingMove();
                    break;
                case Status.Breaking:
                    StopStep();
                    break;
            }

            //  Debug.Log(CurrentIndex());
            yield return null;
        }
    }


    private Vector3 breakingSpeed;

    void StopStep() {
        var samp = (deltaRotate + new Vector3(0, 0, float.Epsilon) - (targetRotate - BreakingStartLastRotate)).z /
                   BreakingStartLastRotate.z;


        breakingSpeed = breakingSpeedCurve.Evaluate(samp) * rotateSpeed;
        Debug.Log($"samp{samp} , breakingSpeed:{breakingSpeed}");

        deltaRotate += breakingSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(deltaRotate + StartRollingRotate);

        if (deltaRotate.z >= targetRotate.z) {
            currentPoint =targets[0] ;
            status = Status.Stop;
        }
    }

    float CurrentIndex() {
        return (currentPoint + ((deltaRotate.z + UnitDeg / 2F) / UnitDeg)) % 6;
    }


    public int startStatusTime = 2;


    [SerializeField] private Vector3 lastRotate;
    [SerializeField] private Vector3 rotateSpeed;
    public Vector3 deltaRotate;
    private Vector3 BreakingStartLastRotate;

    void RollingMove() {
        deltaRotate += rotateSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(deltaRotate + StartRollingRotate);

        if (targetRotate.z - deltaRotate.z <= breakingStep * UnitDeg) {
            //    transform.rotation = Quaternion.Euler(targetRotate - new Vector3(0, 0, breakingStep * UnitDeg));
            status = Status.Breaking;
            BreakingStartLastRotate = targetRotate - deltaRotate;
        }
    }

    void StartMove() {
        float sample = rollingTime / startStatusTime;
        if (sample > 1) {
            status = Status.Rolling;
            deltaRotate = new Vector3(0, 0, 12 * UnitDeg);
            transform.rotation = Quaternion.Euler(deltaRotate + StartRollingRotate);
            return;
        }

        deltaRotate = new Vector3(0, 0, StartCurve.Evaluate(sample) * 12 * UnitDeg);
        var currentRotate = deltaRotate + StartRollingRotate;
        transform.rotation = Quaternion.Euler(currentRotate);
        rotateSpeed = (currentRotate - lastRotate) / Time.deltaTime;
        lastRotate = currentRotate;
    }

    enum Status {
        Start,
        Rolling,
        Breaking,
        Stop
    }

    private Coroutine _coroutine;

    private void OnMouseDown() {
        if (_coroutine != null) {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }

        _coroutine = StartCoroutine(roll());
    }
}