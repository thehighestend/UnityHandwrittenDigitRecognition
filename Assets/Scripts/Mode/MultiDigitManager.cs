using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public class MultiDigitManager : MonoBehaviour
{
    [Header("Unity UI")]
    [SerializeField] TextMeshProUGUI _figures;

    [Header("Modules")]
    [SerializeField] Pen _pen;
    [SerializeField] TabletManager[] _tablets;

    private StringBuilder _strBulider = new StringBuilder();

    private void Start()
    {
        _pen._DrawCompleteCallback += OnDrawComplete;
    }

    private void OnDestroy()
    {
        _pen._DrawCompleteCallback -= OnDrawComplete;
    }

    private void OnDrawComplete()
    {
        StartCoroutine(DetectMultiDigits());
    }

    private IEnumerator DetectMultiDigits()
    {
        _strBulider.Clear();

        bool isFirstDigit = true;

        foreach (var tablet in _tablets)
        {
            yield return tablet.ChainDetection();

            if (tablet.NumberSaved < 0 || (isFirstDigit && tablet.NumberSaved == 0))
                continue;

            _strBulider.Append(tablet.NumberSaved);
            isFirstDigit = false;
        }

        _figures.text = _strBulider.ToString();
    }
}