using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Scanner))]
[RequireComponent(typeof(Pointer))]
public class ResourceStorage : MonoBehaviour
{
    private Pointer _placeForBuild;
    private Scanner _scanner;
    private int _resources;
    private int _maxCountResourcesReceived;
    private int _unitCost;
    private int _baseCost;
    private int _count;
    private bool _isBuildingBase;

    public event UnityAction<int, int> CanBuildUnit;
    public event UnityAction CanBuildBase;

    private void OnEnable()
    {
        _placeForBuild.FlagSet += SpendResourcesToBuildingBase;
    }

    private void OnDisable()
    {
        _placeForBuild.FlagSet -= SpendResourcesToBuildingBase;
    }

    private void Awake()
    {
        _unitCost = 3;
        _baseCost = 5;
        _maxCountResourcesReceived = 5;
        _scanner = GetComponent<Scanner>();
        _placeForBuild = GetComponent<Pointer>();
    }

    private void Start()
    {
        RequestResource();
    }

    public void SetStatus()
    {
        _isBuildingBase = false;
    }

    public void SpendResources(int count)
    {
        _count -= count;
        _resources -= count;
        RequestResource();
    }

    public void IncreaseCount()
    {
        _resources++;
    }

    public void BuildThing()
    {
        if (_isBuildingBase)
        {
            if (_resources >= _baseCost)
            {
                CanBuildBase?.Invoke();
                SpendResources(_baseCost);
            }
        }
        
        if(_isBuildingBase == false)
        {
            if(_resources >= _unitCost)
            {
                CanBuildUnit?.Invoke(1, _unitCost);
            }
        }
    }

    public void RequestResource()
    {
        if(_count < _maxCountResourcesReceived)
        {
            _count++;
            _scanner.StartScanCoroutine();
        }
    }

    private void SpendResourcesToBuildingBase()
    {
        _isBuildingBase = true;
        BuildThing();
    }
}
