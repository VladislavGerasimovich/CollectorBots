using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBase : MonoBehaviour
{
    [SerializeField] private Base _base;
    [SerializeField] private Vector3 _mainBasePosition;

    private List<Base> _bases;
    private Unit _unitBuilder;

    private void Awake()
    {
        _bases = new List<Base>();
    }

    private void Start()
    {
        Create(_mainBasePosition);
    }

    private void Create(Vector3 position)
    {
        Base mainBase = Instantiate(_base, position, _base.transform.rotation);
        mainBase.UnitCameToBuild += WhenUnitArrived;

        if (_unitBuilder != null)
        {
            _unitBuilder.transform.SetParent(mainBase.transform);
            mainBase.AddUnit(_unitBuilder);
            _unitBuilder = null;
        }

        _bases.Add(mainBase);
    }

    private void WhenUnitArrived(Vector3 position, Unit unit)
    {
        _unitBuilder = unit;
        Create(position);
    }
}
