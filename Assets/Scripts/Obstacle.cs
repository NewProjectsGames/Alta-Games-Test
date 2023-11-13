using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Obstacle : MonoBehaviour
{
    private Material[] _materials;

    private void Start()
    {
        _materials = GetComponent<Renderer>().materials;
    }

    public void StartDestruction(float dst)
    {
        BallController.OnHitObstacle.Invoke();
        foreach (var item in _materials)
        {
            item.DOColor(Color.gray, .5f).SetDelay(dst / 2.5f - 1).OnComplete(() => { });
            item.mainTexture = null;
        }

        transform.DOShakeScale(.4f, transform.localScale * 0.2f, 5).SetDelay(0.5f + dst / 2.5f - 1).OnComplete(() =>
        {
            transform.DOScale(0, .5f).SetEase(Ease.InBounce).OnComplete(() =>
            {
                BallController.OnDestroyObstacle.Invoke();
                Destroy(gameObject);
            });
        });
        //Destroy(gameObject);
    }
}