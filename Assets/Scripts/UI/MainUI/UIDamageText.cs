using TMPro;
using UnityEngine;

public class UIDamageText : UIPoolingElement
{
    private const float NORMAL_DAMAGE_FONT_SIZE = 300;
    private const float CRITICAL_DAMAGE_FONT_SIZE = 500;

    [SerializeField] private TextMeshProUGUI _damageText;    
    [SerializeField] private float _moveSpeed = 0.5f;

    private float _positionMoveTime;
    private Transform _followTarget;
    private Camera _mainCam;
    private Vector3 _offset;
    private GameObject _prefabAsset;

    public void Init(Transform target, Vector3 offset, GameObject prefabAsset)
    {
        _followTarget = target;
        _offset = offset;

        _mainCam = Camera.main;        
        transform.position = _followTarget.position + offset;
        _prefabAsset = prefabAsset;
        _positionMoveTime = 1f;
    }

    public void SetDamage(int damage, Color color, bool isCritical)
    {
        if (isCritical)
        {
            _damageText.fontSizeMax = CRITICAL_DAMAGE_FONT_SIZE; 
            _damageText.color = Color.yellow;
        }
        else
        {
            _damageText.fontSizeMax = NORMAL_DAMAGE_FONT_SIZE;
            _damageText.color = color;
        }
        _damageText.text = damage.ToString();        
    }

    private void ResetDamageText()
    {
        _damageText.text = string.Empty;        
    }

    private void Update()
    {
        if (_positionMoveTime > 0f)
        {
            transform.Translate(Vector3.up * _moveSpeed * Time.deltaTime);
            _positionMoveTime -= Time.deltaTime;
        }
        else
        {
            ResetDamageText();
            ObjectPoolManager.Instance.ReturnToPool(_prefabAsset, this);
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
}
