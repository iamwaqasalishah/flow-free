using TMPro;
using UnityEngine;

public class UpdateLevelNumberView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelNumberText;

    private void OnEnable()
    {
        EventManager.OnLevelNumberUpdate += UpdateLevelNumber;
        UpdateLevelNumber();
    }

    private void OnDisable()
    {
        EventManager.OnLevelNumberUpdate -= UpdateLevelNumber;
    }

    private void UpdateLevelNumber()
    {
        if (_levelNumberText != null)
            _levelNumberText.text = $"Level Number {DB.LevelNumber}";
    }
}