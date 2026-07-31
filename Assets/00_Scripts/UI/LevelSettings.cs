using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSettings : MonoBehaviour
{
    public LevelTheme levelTheme;

    private GameObject _background, _visualGrid, _backBtn, _score, _block;
    private Image _backgroundImage, _visualGridImage, _blockImage;
    private Button _backBtnButton;
    private TextMeshProUGUI _backBtnText, _scoreText;

    void Awake()
    {
        _background = transform.Find("Background").gameObject;
        _visualGrid = transform.Find("VisualGrid").gameObject;
        _backBtn = transform.Find("BackBTN").gameObject;
        _score = transform.Find("Score").gameObject;
        _block = transform.Find("Block").gameObject;

        _backgroundImage = _background.GetComponent<Image>();
        _visualGridImage = _visualGrid.GetComponent<Image>();
        _blockImage = _block.GetComponent<Image>();
        _backBtnButton = _backBtn.GetComponent<Button>();
        _scoreText = _score.GetComponent<TextMeshProUGUI>();
        _backBtnText = _backBtn.GetComponentInChildren<TextMeshProUGUI>();

        Image[] cellImages = _visualGrid.GetComponentsInChildren<Image>();
        foreach (Image gridCellImage in cellImages)
        {
            gridCellImage.color = levelTheme.gridCellColor;
        }

        _backgroundImage.color = levelTheme.backgroundColor;
        _visualGridImage.color = levelTheme.gridColor;
        _blockImage.color = levelTheme.blockColor0;

        ColorBlock backBtnColors = _backBtnButton.colors;
        backBtnColors.normalColor = levelTheme.buttonColor;
        backBtnColors.pressedColor = levelTheme.buttonPressedColor;
        backBtnColors.highlightedColor = levelTheme.buttonColor;
        backBtnColors.selectedColor = levelTheme.buttonColor;
        backBtnColors.disabledColor = levelTheme.buttonColor;
        _backBtnButton.colors = backBtnColors;

        _backBtnText.font = levelTheme.font;
        _scoreText.font = levelTheme.font;
    }

}
