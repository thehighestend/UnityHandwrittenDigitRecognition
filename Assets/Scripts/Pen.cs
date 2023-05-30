using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pen : MonoBehaviour
{
    [Header("Unity Objects")]
    [SerializeField] Camera _mainCamera;
    [SerializeField] GameObject _linePrefab;
    [SerializeField] Transform _lineRoot;
    private Coroutine draw;

    [Header("Tablet")]
    [SerializeField] TabletManager _tablet;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            StartDraw();
        }
        else if(Input.GetMouseButtonUp(0))
        {
            StopDraw();
        }
    }

    void StartDraw()
    {
        if(draw != null)
        {
            StopCoroutine(draw);
        }

        draw = StartCoroutine(DrawLine());
    }

    IEnumerator DrawLine()
    {
        var lineObj = Instantiate(_linePrefab, Vector3.zero, Quaternion.identity, _lineRoot);
        var line = lineObj.GetComponent<LineRenderer>();
        line.positionCount = 0;
        lineObj.SetActive(true);

        while(true)
        {
            var pos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, pos);
            yield return null;
        }
    }

    void StopDraw()
    {
        StopCoroutine(draw);
    }
}
