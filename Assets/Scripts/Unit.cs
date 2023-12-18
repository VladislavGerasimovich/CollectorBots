using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : MonoBehaviour
{
    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _speed;
    private float _permissibleValue;

    private Resource _resource;

    public event UnityAction BroughtMaterial;
    public event UnityAction CameToBuild;

    public bool IsBusy { get; private set; }

    private void Awake()
    {
        _speed = 30f;
        _permissibleValue = 0.3f;
    }

    public void Mining(Resource resource)
    {
        _resource = resource;
        IsBusy = true;
        _targetPosition = _resource.transform.position;
        StartCoroutine(Mining());
    }

    public void CreateBase(Vector3 position)
    {
        IsBusy = true;
        _targetPosition = position;
        StartCoroutine(CreateBase());
    }

    public void MountStartPosition(Vector3 position)
    {
        _startPosition = position;
    }

    private void Move(Vector3 position)
    {
        transform.position = Vector3.MoveTowards(transform.position, position, _speed * Time.deltaTime);
    }
    
    private IEnumerator CreateBase()
    {
        bool _isAchieved = false;
        bool isWork = true;

        while (isWork)
        {
            Move(_targetPosition);

            if (Vector3.Distance(_targetPosition, transform.position) < _permissibleValue)
            {
                CameToBuild?.Invoke();
                _isAchieved = true;
                _targetPosition = _startPosition;
            }

            if (_isAchieved == true && transform.position == _startPosition)
            {
                IsBusy = false;
                _isAchieved = false;
                isWork = false;
            }
            
            yield return null;
        }
    }

    private IEnumerator Mining()
    {
        bool _isAchieved = false;
        bool isWork = true;

        while (isWork)
        {
            Move(_targetPosition);

            if(Vector3.Distance(_targetPosition, transform.position) < _permissibleValue)
            {
                _isAchieved = true;
                _targetPosition = _startPosition;
                _resource.SetParent(transform);
            }

            if(_isAchieved == true && Vector3.Distance(transform.position, _targetPosition) < _permissibleValue)
            {
                BroughtMaterial?.Invoke();
                _resource.Die();
                IsBusy = false;
                _isAchieved = false;
                isWork = false;
            }

            yield return null;
        }
    }
}