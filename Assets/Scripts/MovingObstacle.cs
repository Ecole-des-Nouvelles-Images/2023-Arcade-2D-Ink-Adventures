using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MovingObstacle : MonoBehaviour
{
    [Range(0,5)]
    [SerializeField] private float _speed;
    
    [Range(0,2)]
    [SerializeField] private float waitDuration;
    
    [SerializeField] private GameObject ways;
    [SerializeField] private Transform[] wayPoints;
    
    private Vector3 _targetPos;
    private int _pointIndex;
    private int _pointCount;
    private int _direction = 1;
    
    private float speedMultiplier = 1;

    private void Awake()
    {
        _pointCount = wayPoints.Length;
        _pointIndex = 1;
        _targetPos = wayPoints[_pointIndex].transform.position;
    }

    private void Update()
    {
        var step = speedMultiplier * _speed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, _targetPos, step);

        if (transform.position == _targetPos)
        {
            NextPoint();
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
        _targetPos = wayPoints[_pointIndex].transform.position;
    }

    IEnumerator WaitNextPoint()
    {
        speedMultiplier = 0;
        yield return new WaitForSeconds(waitDuration);
        speedMultiplier = 1;
    }
}
