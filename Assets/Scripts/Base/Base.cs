using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ResourceStorage))]
[RequireComponent(typeof(Pointer))]
[RequireComponent(typeof(Scanner))]
public class Base : MonoBehaviour
{
    [SerializeField] private Unit _unitPrefab;
    [SerializeField] private PlaceForUnits _placeForUnits;
    [SerializeField] private Vector3 _displacementVector;

    private ResourceStorage _resourceStorage;
    private Pointer _placeForBuild;
    private Scanner _scanner;
    private List<Unit> _units;
    private int _maxCountUnits;
    private Resource _resource;
    private bool _isBuildingBase;
    private Unit _unitBuilder;

    private Vector3 _unitStartPosition;

    public event UnityAction<Vector3, Unit> UnitCameToBuild;

    private void OnEnable()
    {
        _resourceStorage.CanBuildUnit += CreateUnit;
        _resourceStorage.CanBuildBase += SendUnitToBuild;
        _scanner.ReceivedResource += SendUnitToMine;
    }

    private void OnDisable()
    {
        _resourceStorage.CanBuildUnit -= CreateUnit;
        _resourceStorage.CanBuildBase -= SendUnitToBuild;
        _scanner.ReceivedResource -= SendUnitToMine;

        foreach (var unit in _units)
        {
            unit.BroughtMaterial -= GiveAwayResource;
        }
    }

    private void Awake()
    {
        _units = new List<Unit>();   
        _resourceStorage = GetComponent<ResourceStorage>();
        _placeForBuild = GetComponent<Pointer>();
        _scanner = GetComponent<Scanner>();
        _unitStartPosition = _placeForUnits.transform.position + _displacementVector;
        _maxCountUnits = 5;
    }

    private void Start()
    {
        CreateUnits(3);
    }

    public void AddUnit(Unit unit)
    {
        _units.Add(unit);
        unit.BroughtMaterial += GiveAwayResource;
    }

    private void CreateUnit(int count, int price)
    {
        if (_units.Count < _maxCountUnits)
        {
            _resourceStorage.SpendResources(price);
            CreateUnits(count);
        }
    }

    private void CreateUnits(int count)
    {
        if(_units.Count < _maxCountUnits)
        {
            for (int i = 0; i < count; i++)
            {
                Unit unit = Instantiate(_unitPrefab, _unitStartPosition, Quaternion.identity, transform);
                unit.MountStartPosition(_unitStartPosition);
                _unitStartPosition += _displacementVector;
                unit.BroughtMaterial += GiveAwayResource;
                _units.Add(unit);
            }
        }
    }

    private void GiveAwayResource()
    {
        _resourceStorage.IncreaseCount();
        _resourceStorage.BuildThing();
    }

    private void StartCreateBase()
    {
        _unitBuilder.CameToBuild -= StartCreateBase;
        UnitCameToBuild?.Invoke(_placeForBuild.FlagPosition, _unitBuilder);
        _placeForBuild.DestroyFlag();
    }

    private void SendUnitToBuild()
    {
        _isBuildingBase = true;
        StartCoroutine(GoToBuild());
    }

    private void SendUnitToMine(Resource resource)
    {
        if(_isBuildingBase == false)
        {
            _resource = resource;
            StartCoroutine(Mining(_resource));
            _resourceStorage.RequestResource();
        }
    }

    private bool TryGetUnit(out Unit unit)
    {
        for (int i = 0; i < _units.Count; i++)
        {
            if (_units[i].IsBusy == false)
            {
                unit = _units[i];

                return true;
            }
        }

        unit = null;
        return false;
    }

    private IEnumerator GoToBuild()
    {
        bool isWork = true;

        while (isWork)
        {
            if (TryGetUnit(out Unit unit))
            {
                unit.BroughtMaterial -= GiveAwayResource;
                _units.Remove(unit);
                _unitBuilder = unit;
                _unitBuilder.CameToBuild += StartCreateBase;
                unit.CreateBase(_placeForBuild.FlagPosition);
                _resourceStorage.SetStatus();
                isWork = false;
                _isBuildingBase = false;
            }

            yield return null;
        }
    }

    private IEnumerator Mining(Resource resource)
    {
        bool isWork = true;

        while(isWork)
        {
            if (TryGetUnit(out Unit unit))
            {
                unit.Mining(resource);
                isWork = false;
            }

            yield return null;
        }
    }
}