using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plantation : MonoBehaviour
{
    [SerializeField] private Resource _resourcePrefab;

    private Vector3 _startPosition;

    private WaitForSeconds _delay;

    private Queue<Resource> _resources;
    private int _maxCountResources;

    private void Awake()
    {
        _resources = new Queue<Resource>();
        _maxCountResources = 5;
        _delay = new WaitForSeconds(1);
    }

    private void Start()
    {
        StartCoroutine(CreateResources());
    }

    public Resource GetResource()
    {
        if (_resources.Count == 0)
        {
            return null;
        }

        Resource resource = _resources.Dequeue();
        return resource;
    }

    private void CreateResource()
    {
        GenerateRandomPosition();
        Resource resource = Instantiate(_resourcePrefab, _startPosition, Quaternion.identity, transform);
        _resources.Enqueue(resource);
    }

    private void GenerateRandomPosition()
    {
        int _maxPositionX = 90;
        int _maxPositionZ = 60;
        int _randomPositionX = Random.Range(0, _maxPositionX);
        int _randomPositionZ = Random.Range(0, _maxPositionZ);
        _startPosition = transform.TransformPoint(_randomPositionX, 0, _randomPositionZ);
    }

    private IEnumerator CreateResources()
    {
        while (true)
        {
            if(_resources.Count < _maxCountResources)
            {
                CreateResource();
            }

            yield return _delay;
        }
    }
}
