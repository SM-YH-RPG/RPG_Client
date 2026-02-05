using System;
using UnityEngine;
using UnityEngine.UI;

public class UINormalEnemyHP : UIPoolingElement
{
    [SerializeField]
    private Image _hpBarImage = null;

    private Transform _followTarget;
    private Camera _mainCam;

    [SerializeField] //.. debug
    private Vector3 _offset;

    public void Init(Transform target, Vector3 offset)
    {
        _followTarget = target;
        _offset = offset;

        _mainCam = Camera.main;
    }

    private void Update()
    {
        if (_followTarget != null && _mainCam != null)
        {
            transform.position = _followTarget.position + _offset;
        }
    }

    private void LateUpdate()
    {
        if (_mainCam != null)
        {
            transform.LookAt(transform.position + _mainCam.transform.rotation * Vector3.forward,
                             _mainCam.transform.rotation * Vector3.up);
        }
    }

    public void UpdateHPBar(int currentHP, int maxHP)
    {
        if (currentHP <= 0)
        {
            _hpBarImage.fillAmount = 0;            
            return;
        }
        else
        {
            _hpBarImage.fillAmount = (float)currentHP / maxHP;            
        }
    }

    public void ResetHPBar()
    {
        _hpBarImage.fillAmount = 1;
        _followTarget = null;
        _mainCam = null;
    }
}
