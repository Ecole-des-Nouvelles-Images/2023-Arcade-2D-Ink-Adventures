using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MovingObstacle : MonoBehaviour
{
    [SerializeField] private GameObject _obstacle;
    [SerializeField] private Transform[] _wayPoints;

    [Range(0,5)]
    [SerializeField] private float _speed;
    
    [Range(0,2)]
    [SerializeField] private float _waitDuration;
    
    private Vector3 _targetPos;
    private int _pointIndex;
    private int _pointCount;
    private int _direction = 1;
    private float _speedMultiplier = 1;

    private bool _isWaiting;
    private void Awake()
    {
        _pointCount = _wayPoints.Length;
        _targetPos = _wayPoints[_pointIndex].transform.position;
    }

    private void Update()
    {
        var step = _speedMultiplier * _speed * Time.deltaTime;
        _obstacle.transform.position = Vector2.MoveTowards(_obstacle.transform.position, _targetPos, step);

        if (Vector2.Distance(_obstacle.transform.position, _targetPos) < 0.01f && !_isWaiting)
        {
            StartCoroutine(WaitNextPoint());
        }
    }

    private void NextPoint()
    {
        if (_pointIndex == _pointCount - 1)
        {
            _direction = -1;
        }

        if (_pointIndex == 0)
        {
            _direction = 1;
        }

        _pointIndex += _direction;
        _targetPos = _wayPoints[_pointIndex].transform.position;
    }

    IEnumerator WaitNextPoint()
    {
        _isWaiting = true;
        _speedMultiplier = 0;
        yield return new WaitForSeconds(_waitDuration);
        _speedMultiplier = 1;
        NextPoint();
        _isWaiting = false;
    }
}
