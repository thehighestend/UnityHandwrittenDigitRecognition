using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;
using Unity.Barracuda;
using UnityEditor;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;

public class TabletManager : MonoBehaviour
{
    [Header("Unity Component")]
    [SerializeField] Camera _mainCamera;

    [Header("Unity UI")]
    [SerializeField] RectTransform _tabletImageRT;
#if UNITY_EDITOR
    [SerializeField] RawImage _debugRawImage;
#endif
    private Vector2 _tabletSize = Vector2.zero;

    [Header("Virtual Device")]
    [SerializeField] Pen _pen;

    [Header("Module")]
    [SerializeField] InferenceManager _inferenceManager;

    public Action<int> _postDetectionCallback = null;

    // Coroutine flag
    private WaitForEndOfFrame _wfEOF = new WaitForEndOfFrame();

    private void Start()
    {
        _tabletSize = _tabletImageRT.sizeDelta;
        _pen._DrawCompleteCallback += Detect;
    }

    private void OnDestroy()
    {
        _pen._DrawCompleteCallback -= Detect;
    }

#if UNITY_EDITOR_WIN
    public void ShowDebug()
    {
        StartCoroutine(TakeScreenshotTabletArea((texture) =>
        {
            _debugRawImage.texture = texture;
            _debugRawImage.gameObject.SetActive(true);

            _inferenceManager.InferNumber(texture);
        }));
    }
#endif

    private void Detect()
    {
        StartCoroutine(TakeScreenshotTabletArea((texture) =>
        {
            var number = _inferenceManager.InferNumber(texture);
            _postDetectionCallback?.Invoke(number);
        }));
    }

    private IEnumerator TakeScreenshotTabletArea(Action<Texture2D> finishCallback)
    {
        var tex = new Texture2D((int)_tabletSize.x, (int)_tabletSize.y, TextureFormat.RGB24, false);
        var tabletScreenPos = _mainCamera.WorldToScreenPoint(_tabletImageRT.position);
        var rect = new Rect(tabletScreenPos.x, tabletScreenPos.y, _tabletSize.x, _tabletSize.y);

        yield return _wfEOF;
        tex.ReadPixels(rect, 0, 0);
        tex.Apply();

        var rt = RenderTexture.GetTemporary(28, 28, 0);
        RenderTexture.active = rt;
        Graphics.Blit(tex, rt);
        Texture2D resized = new Texture2D(28, 28);
        resized.ReadPixels(new Rect(0, 0, 28, 28), 0, 0);
        resized.Apply();

        RenderTexture.ReleaseTemporary(rt); 

        finishCallback?.Invoke(resized);
    }
}