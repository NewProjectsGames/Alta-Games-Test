using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class BallController : MonoBehaviour
{
    [Range(0f, 10f)] [SerializeField] private float radiusBall;
    [SerializeField] private ObjectPool projectileObjectPool;
    [SerializeField] private LineRenderer road;
    [Range(0f, 5f)] [SerializeField] private float speedCharge;
    [SerializeField] private float percentForLose;
    [SerializeField] private Vector3 offsetProjectile;
    [SerializeField] private LayerMask layerObstacle;
    [SerializeField] private float distanceToObstacle;
    [SerializeField] private DoorController doorController;
    private float _minRadiusForLose;
    private GameObject _projectile;

    public static Action OnHitObstacle;
    public static Action OnDestroyObstacle;
    private int _countHitObstacle;
    float _powerProjectile;

    private void OnEnable()
    {
        OnHitObstacle = null;
        OnDestroyObstacle = null;
        OnHitObstacle += HitObstacle;
        OnDestroyObstacle += DestroyObstacle;
    }

    void HitObstacle()
    {
        _countHitObstacle++;
    }

    void DestroyObstacle()
    {
        _countHitObstacle--;
        UIManager.Instance.AddScore(1);
        if (_countHitObstacle <= 0)
        {
            JumpBall();
            _countHitObstacle = 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ResizeBall();
        _minRadiusForLose = radiusBall * percentForLose / 100f;
    }

    private void ResizeBall()
    {
        transform.localScale = Vector3.one * radiusBall;
        road.endWidth = radiusBall;
        road.startWidth = radiusBall;
    }

    // Update is called once per frame
    void Update()
    {
        Control();
    }


    void Control()
    {
        if (CheckLose())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            CreateProjectile();
        }

        if (Input.GetMouseButton(0))
        {
            Charge();
        }

        if (Input.GetMouseButtonUp(0))
        {
            Fire();
        }
    }

    void CreateProjectile()
    {
        _projectile = projectileObjectPool.GetPooledObject();
        _projectile.transform.localScale = Vector3.one * 0.1f;

        _projectile.SetActive(true);
        _powerProjectile = 0;
    }


    void Charge()
    {
        var valueCharge = speedCharge * Time.deltaTime;
        radiusBall -= valueCharge;
        _powerProjectile += valueCharge;
        _projectile.transform.position = new Vector3(transform.position.x, _powerProjectile / 2f,
            transform.position.z + (radiusBall / 2f + _powerProjectile / 2f)) + offsetProjectile;
        // _projectile.transform.position = transform.position + Vector3
        //     .forward * (radiusBall / 2f + _powerProjectile / 2f) + Vector3.up * (_powerProjectile/2f) + offsetProjectile;
        _projectile.transform.localScale = Vector3.one * _powerProjectile;
        ResizeBall();
        if (CheckLose())
        {
            Lose();
        }
    }

    void JumpBall()
    {
        if (Vector3.Distance(transform.position, doorController.transform.position) <= 5)
        {
            doorController.OpenDoor();
            transform.DOJump(transform.position + transform.forward * 7, 4, 2,
                1).SetDelay(1).OnComplete(Win);
        }
        else if (Physics.BoxCast(transform.position, Vector3.forward * radiusBall, transform.forward,
                     out RaycastHit hitInfo,
                     Quaternion.identity, 100,
                     layerObstacle))
        {
            if (hitInfo.distance > distanceToObstacle)
            {
                if (hitInfo.distance - distanceToObstacle > 0.3f)
                    transform.DOJump(transform.position + transform.forward * (hitInfo.distance - distanceToObstacle),
                        3, 2,
                        1);
            }
        }
        else
        {
            transform.DOJump(doorController.transform.position + transform.forward * 2, 4, 2,
                2).SetDelay(1).OnComplete(Win).OnUpdate(delegate
            {
                if (Vector3.Distance(transform.position, doorController.transform.position) <= 5)
                {
                    doorController.OpenDoor();
                }
            });
        }
    }

    void Fire()
    {
        _projectile.GetComponent<Projectile>().Fire(_powerProjectile);
    }

    bool CheckLose()
    {
        return _minRadiusForLose >= radiusBall;
    }

    void Win()
    {
        UIManager.Instance.Win();
    }

    void Lose()
    {
        UIManager.Instance.Lose();
        //Debug.Log("Lose");
    }
}