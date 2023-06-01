using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pen : MonoBehaviour
{
    [Header("Unity Objects")]
    [SerializeField] Camera _mainCamera;
    [SerializeField] GameObject _linePrefab;
    [SerializeField] Transform _lineRoot;
    private Coroutine draw;

    public Dictionary<int, Rect> TabletAreaDic = new();

    public Action DrawCompleteCallback = null;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDraw();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopDraw();
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            StopDraw();
    }

    void StartDraw()
    {
        // Won't draw while clicking buttons
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (!CheckPointerInsideTabletArea(Input.mousePosition))
            return;

        if (draw != null)
        {
            StopCoroutine(draw);
        }

        draw = StartCoroutine(DrawLine());
    }

    private bool CheckPointerInsideTabletArea(Vector2 pointerPos)
    {
        bool isPointerOverTabletArea = false;
        foreach (var rect in TabletAreaDic.Values)
        {
            if (rect.Contains(pointerPos))
            {
                isPointerOverTabletArea |= true;
                break;
            }
        }

        return isPointerOverTabletArea;
    }

    IEnumerator DrawLine()
    {
        var lineObj = Instantiate(_linePrefab, Vector3.zero, Quaternion.identity, _lineRoot);
        var line = lineObj.GetComponent<LineRenderer>();
        line.positionCount = 0;
        lineObj.SetActive(true);

        while (true)
        {
            var mousePos = Input.mousePosition;
            if (!CheckPointerInsideTabletArea(mousePos))
            {
                yield break;
            }

            var pos = _mainCamera.ScreenToWorldPoint(mousePos);
            pos.z = 0;
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, pos);
            yield return null;
        }
    }

    void StopDraw()
    {
        if (draw != null)
        {
            StopCoroutine(draw);
        }

        DrawCompleteCallback?.Invoke();
    }

    public void Undo()
    {
        if (_lineRoot.childCount > 0)
        {
            Destroy(_lineRoot.GetChild(_lineRoot.childCount - 1).gameObject);
        }
    }

    public void Clear()
    {
        var childCount = _lineRoot.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Destroy(_lineRoot.GetChild(i).gameObject);
        }
    }
}