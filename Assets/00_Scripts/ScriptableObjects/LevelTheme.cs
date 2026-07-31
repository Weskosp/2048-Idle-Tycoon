using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelTheme", menuName = "Scriptable Objects/LevelTheme")]
public class LevelTheme : ScriptableObject
{
    [Header("Genel Renkler")]
    public Color backgroundColor = Color.white;
    public Color gridColor = Color.white;
    public Color gridCellColor = Color.white;

    [Header("Düğme Renkleri")]
    public Color buttonColor = Color.white;
    public Color buttonPressedColor = Color.white;

    [Header("Blok Renkleri")]
    public Color blockColor0 = Color.white;
    public Color blockColor1 = Color.white;
    public Color blockColor2 = Color.white;
    public Color blockColor3 = Color.white;
    public Color blockColor4 = Color.white;
    public Color blockColor5 = Color.white;
    public Color blockColor6 = Color.white;
    public Color blockColor7 = Color.white;
    public Color blockColor8 = Color.white;
    public Color blockColor9 = Color.white;
    public Color blockColor10 = Color.white;

    [Header("Genel Sprite")]
    public Sprite backgroundSprite;
    public Sprite gridSprite;
    public Sprite gridCellSprite;
    public Sprite buttonSprite;

    [Header("Block Sprite")]
    public Sprite blockSprite0;
    public Sprite blockSprite1;
    public Sprite blockSprite2;
    public Sprite blockSprite3;
    public Sprite blockSprite4;
    public Sprite blockSprite5;
    public Sprite blockSprite6;
    public Sprite blockSprite7;
    public Sprite blockSprite8;
    public Sprite blockSprite9;
    public Sprite blockSprite10;

    [Header("Yazı Tipi")]
    public TMP_FontAsset font;
}
