using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

[RequireComponent(typeof(Base))]
public class Pointer : MonoBehaviour
{
    [SerializeField] private Flag _flag;

    private RaycastHit _hit;
    private Flag _setFlag;
    private bool _isBaseSelected;

    public event UnityAction FlagSet;

    public Vector3 FlagPosition { get; private set; }

    private void Start()
    {
        StartCoroutine(ChoosePlace());
    }

    public void DestroyFlag()
    {
        _isBaseSelected = false;
        Destroy(_setFlag.gameObject);
    }

    private IEnumerator ChoosePlace()
    {
        while (enabled)
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _hit);

            if(_hit.collider.gameObject == transform.gameObject && Input.GetMouseButtonDown(0))
            {
                _isBaseSelected = true;
            }

            if (_isBaseSelected && Input.GetMouseButtonDown(1))
            {
                if (_setFlag == null)
                {
                    _setFlag = Instantiate(_flag, _hit.point, _flag.transform.rotation);
                    FlagPosition = _hit.point;
                    FlagSet?.Invoke();
                }
                else
                {
                    _setFlag.transform.position = _hit.point;
                    FlagPosition = _setFlag.transform.position;
                }
            }

            yield return null;
        }
    }
}
