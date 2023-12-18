using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Scanner : MonoBehaviour
{
    private Plantation _plantation;

    public event UnityAction<Resource> ReceivedResource;

    private void Awake()
    {
        _plantation = GameObject.FindWithTag("Plantation").GetComponent<Plantation>();
    }

    public void StartScanCoroutine()
    {
        StartCoroutine(Scan());
    }

    private IEnumerator Scan()
    {
        bool isWork = true;

        while (isWork)
        {
            Resource resource = _plantation.GetResource();

            if (resource != null)
            {
                ReceivedResource?.Invoke(resource);
                isWork = false;
            }

            yield return null;
        }
    }
}
