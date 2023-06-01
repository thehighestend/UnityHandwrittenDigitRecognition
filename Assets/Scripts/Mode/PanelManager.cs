using UnityEngine;
using TMPro;
using System.Text;

public class PanelManager : MonoBehaviour
{
    [Header("Unity UI")]
    [SerializeField] private TextMeshProUGUI _guideText;
    [SerializeField] private TextMeshProUGUI _lcdText;

    [Header("Module")]
    [SerializeField] private TabletManager _tablet;

    private StringBuilder _stringBuilder = new StringBuilder();
    private string _numberString = string.Empty;

    private void Start()
    {
        _tablet.PostDetectionCallback += OnInferenceComplete;
    }

    private void OnDestroy()
    {
        _tablet.PostDetectionCallback -= OnInferenceComplete;
    }

    private void OnInferenceComplete(int number)
    {
        if (number < 0)
            return;

        _numberString = number.ToString();

        _guideText.text = _numberString;
    }

    public void EnterLCD()
    {
        _stringBuilder.Append(_numberString);
        UpdateLCD();
    }

    public void Backspace()
    {
        if (_stringBuilder.Length > 0)
        {
            _stringBuilder.Remove(_stringBuilder.Length - 1, 1);
            UpdateLCD();
        }
    }

    private void UpdateLCD()
    {
        _lcdText.text = _stringBuilder.ToString();
    }
}