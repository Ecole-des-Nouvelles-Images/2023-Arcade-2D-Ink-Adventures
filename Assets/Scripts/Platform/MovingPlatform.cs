using System;
using System.Collections;
using System.Collections.Generic;
using Noah.Scripts.Player;
using UnityEngine;
using UnityEngine.Serialization;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject ways; 
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private float waitDuration;

    private int _pointIndex;
    private int _pointCount;
    private int _direction = 1;
    
    private Vector3 _targetPos;
    private PlayerController _playerController;
    private Rigidbody2D _rb;
    private Vector2 _moveDirection;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (ways != null)
        {
            wayPoints = new Transform[ways.transform.childCount];

            for (int i = 0; i < ways.gameObject.transform.childCount; i++)
            {
                wayPoints[i] = ways.transform.GetChild(i).gameObject.transform;
            }
        }
    }
    private void Start()
    {
        _pointIndex = 1;
        _pointCount = wayPoints.Length;
        if (ways != null)
        {
            _targetPos = wayPoints[1].transform.position;
        }
        DirectionCalculate();
    }

    private void FixedUpdate()
    {
        _rb.velocity = _moveDirection * speed;
    }

    private void Update()
    {
        if (Vector2.Distance(transform.position, _targetPos) < 0.1f)
        {
            NextPoint();
        }
    }
    private void DirectionCalculate()
    {
        _moveDirection = (_targetPos - transform.position).normalized;
    }

    private void NextPoint()
    {
        transform.position = _targetPos;
        _moveDirection = Vector3.zero;

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
        StartCoroutine(WaitNextPoint());
    }

    IEnumerator WaitNextPoint()
    {
        yield return new WaitForSeconds(waitDuration);
        DirectionCalculate();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Elias.Scripts.Components.PlayerController.Instance.IsOnPlatform = true;
            Elias.Scripts.Components.PlayerController.Instance.PlatformRb = _rb;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.instance.IsOnPlatform = false;
        }
    }
    
}
