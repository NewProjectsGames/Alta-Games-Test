using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Transform pointDoor;


    public void OpenDoor()
    {
        pointDoor.DOLocalRotate(new Vector3(0, 90, 0), 1);
    }
}