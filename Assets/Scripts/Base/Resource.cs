using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    private Vector3 _point;

    private void Awake()
    {
        _point = new Vector3(1, 1, 1);
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = _point;
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}