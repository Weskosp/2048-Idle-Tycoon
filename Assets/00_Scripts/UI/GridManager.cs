using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    /*
        bir hücrenin hangi konumda olduğunu bulmak için
        satır = index / sutunSayisi 
        sütun = index % sutunSayisi

        bir konumun hangi hücreye denk olduğunu bulmak için
        index = mevcutSatir * sutunSayisi + mevcutSutun

        BU FORMÜLLERİ UNUTMA!!!!
    */

    [SerializeField] private int _blockCount;
    [SerializeField] private int _baseBlockValue;
    [SerializeField] private float _blockMoveSpeed;
    [SerializeField] private List<GameObject> _gridCells = new List<GameObject>();

    public enum Direction {Up, Down, Left, Right};
    private int[,] _gridData;
    private int _totalScore;
    private bool isSwipe;

    private LevelSettings _levelSettings;
    private GridGenerator _gridGenerator;
    private GameObject _blockPrefab;
    private GameObject _scoreObject;

    void Start()
    {
        _gridGenerator = GetComponent<GridGenerator>();
        _blockPrefab = transform.Find("Block").gameObject;
        _scoreObject = transform.Find("Score").gameObject;
        _gridData = new int[_gridGenerator.RowValue, _gridGenerator.ColumnValue];
        _levelSettings = GetComponent<LevelSettings>();

        AddBlockToGrid();
    }

    public void AddCellPositionToList(GameObject cell)
    {
        _gridCells.Add(cell);
    }

    public void AddBlockToGrid()
    {
        RandomGridDataGenerator();
    }

    void AddBlock(int cellIndex, int blockValue)
    {
        GameObject newBlock = Instantiate(_blockPrefab, _gridCells[cellIndex].transform, false);

        newBlock.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        newBlock.GetComponentInChildren<TextMeshProUGUI>().text = blockValue.ToString();
        newBlock.GetComponent<BlockData>().BlockValue = blockValue;
        newBlock.GetComponent<BlockData>().CurrentIndex = cellIndex;

        StartCoroutine(PopEffect(newBlock, Vector2.one, 0.2f));
        AddToScore(newBlock);
    }

    void RandomGridDataGenerator()
    {
        for (int i = 0; i < _blockCount; i++)
        {
            List<int> emptyGridCells = new List<int>();

            for (int j = 0; j < _gridCells.Count; j++)
            {
                if(_gridCells[j].transform.childCount == 0) emptyGridCells.Add(j);
            }

            if (emptyGridCells.Count == 0) continue;  
            int randomIndex = Random.Range(0, emptyGridCells.Count);
            if (_gridCells[emptyGridCells[randomIndex]].transform.childCount == 0) AddBlock(emptyGridCells[randomIndex], _baseBlockValue);
        }
    }

    void ManuelGridDataGenerator()
    {
        _gridData[0,0] = 4;
        _gridData[1,0] = 2;
        _gridData[2,0] = 0;
        _gridData[3,0] = 2;
        _gridData[0,1] = 4;
        _gridData[1,1] = 2;
        _gridData[2,1] = 0;
        _gridData[3,1] = 2;
    }

    public void MoveGrid(Direction direction)
    {
        if (!isSwipe)
        {            
            List<int> backup = new List<int>();
            List<int> current = new List<int>();
            bool changed = false;
            _totalScore = 0;

            for (int i = 0; i < _gridCells.Count; i++)
            {
                if (_gridCells[i].transform.childCount != 0) backup.Add(1);
                else backup.Add(0);
            }

            switch (direction)
            {
                case Direction.Up:
                    for (int i = 0; i < _gridGenerator.ColumnValue; i++)
                    {
                        List<GameObject> columnBlocks = new List<GameObject>();

                        for (int j = 0; j < _gridCells.Count; j++)
                        {
                            if(_gridCells[j].transform.childCount == 0) continue;
                            BlockData blockData = _gridCells[j].transform.GetChild(0).GetComponent<BlockData>();
                            int column = blockData.CurrentIndex % _gridGenerator.ColumnValue;
                            if (i == column)
                            {
                                columnBlocks.Add(blockData.gameObject);
                            }
                        }

                        for (int k = 0; k < columnBlocks.Count - 1; k++)
                        {
                            BlockData blockData = columnBlocks[k].GetComponent<BlockData>();
                            BlockData blockData2 = columnBlocks[k + 1].GetComponent<BlockData>();
                            if (blockData.BlockValue == blockData2.BlockValue && blockData.BlockValue != -1 && !blockData.IsMerge && !blockData2.IsMerge)
                            {
                                blockData2.BlockValue += blockData.BlockValue;
                                blockData.BlockValue = -1; 
                                blockData2.IsMerge = true;

                                columnBlocks[k + 1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = blockData2.BlockValue.ToString();
                            }
                        }

                        int realCount = 0;
                        for (int l = 0; l < columnBlocks.Count; l++)
                        {
                            BlockData bd = columnBlocks[l].GetComponent<BlockData>();
                            if (bd.BlockValue == -1) continue;

                            AddToScore(columnBlocks[l]);
                            BlockMultiplier(bd);
                            
                            int g = realCount;
                            int cellIndex = g * _gridGenerator.ColumnValue + i;

                            columnBlocks[l].transform.SetParent(_gridCells[cellIndex].transform, true);
                            bd.CurrentIndex = cellIndex;
                            bd.IsMerge = false;
                            StartCoroutine(MovingBlock(columnBlocks[l], _blockMoveSpeed, bd.BlockValue));

                            realCount++;
                        }

                        for (int m = 0; m < columnBlocks.Count; m++)
                        {
                            BlockData bd = columnBlocks[m].GetComponent<BlockData>();
                            if (bd.BlockValue != -1) continue;

                            Transform hedefParent = columnBlocks[m + 1].transform.parent;
                            columnBlocks[m].transform.SetParent(hedefParent, true);
                            StartCoroutine(MovingBlock(columnBlocks[m], _blockMoveSpeed, bd.BlockValue));
                        }                
                    }
                    break;
                case Direction.Down:
                    for (int i = 0; i < _gridGenerator.ColumnValue; i++)
                    {
                        List<GameObject> columnBlocks = new List<GameObject>();

                        for (int j = 0; j < _gridCells.Count; j++)
                        {
                            if(_gridCells[j].transform.childCount == 0) continue;
                            BlockData blockData = _gridCells[j].transform.GetChild(0).GetComponent<BlockData>();
                            int column = blockData.CurrentIndex % _gridGenerator.ColumnValue;
                            if (i == column)
                            {
                                columnBlocks.Add(blockData.gameObject);
                            }
                        }
                        
                        for (int k = columnBlocks.Count - 1; k > 0; k--)
                        {
                            BlockData blockData = columnBlocks[k].GetComponent<BlockData>();
                            BlockData blockData2 = columnBlocks[k - 1].GetComponent<BlockData>();
                            if (blockData.BlockValue == blockData2.BlockValue && blockData.BlockValue != -1 && !blockData.IsMerge && !blockData2.IsMerge)
                            {
                                blockData2.BlockValue += blockData.BlockValue;
                                blockData.BlockValue = -1; 
                                blockData2.IsMerge = true;

                                columnBlocks[k - 1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = blockData2.BlockValue.ToString();
                            }
                        }

                        int realCount = 0;
                        for (int l = columnBlocks.Count - 1; l >= 0; l--)
                        {
                            BlockData bd = columnBlocks[l].GetComponent<BlockData>();
                            if (bd.BlockValue == -1) continue;

                            AddToScore(columnBlocks[l]);
                            BlockMultiplier(bd);

                            int g = _gridGenerator.ColumnValue - 1 - realCount;
                            int cellIndex = g * _gridGenerator.ColumnValue + i;

                            columnBlocks[l].transform.SetParent(_gridCells[cellIndex].transform, true);
                            bd.CurrentIndex = cellIndex;
                            bd.IsMerge = false;
                            StartCoroutine(MovingBlock(columnBlocks[l], _blockMoveSpeed, bd.BlockValue));

                            realCount++;
                        }

                        for (int m = columnBlocks.Count - 1; m > 0; m--)
                        {
                            BlockData bd = columnBlocks[m].GetComponent<BlockData>();
                            if (bd.BlockValue != -1) continue;

                            Transform hedefParent = columnBlocks[m - 1].transform.parent;
                            columnBlocks[m].transform.SetParent(hedefParent, true);
                            StartCoroutine(MovingBlock(columnBlocks[m], _blockMoveSpeed, bd.BlockValue));
                        }                
                    }
                    break;
                case Direction.Left:
                    for (int i = 0; i < _gridGenerator.RowValue; i++)
                    {
                        List<GameObject> rowBlocks = new List<GameObject>();

                        for (int j = 0; j < _gridCells.Count; j++)
                        {
                            if(_gridCells[j].transform.childCount == 0) continue;
                            BlockData blockData = _gridCells[j].transform.GetChild(0).GetComponent<BlockData>();
                            int row = blockData.CurrentIndex / _gridGenerator.ColumnValue;
                            if (i == row)
                            {
                                rowBlocks.Add(blockData.gameObject);
                            }
                        }
                        
                        for (int k = 0; k < rowBlocks.Count - 1; k++)
                        {
                            BlockData blockData = rowBlocks[k].GetComponent<BlockData>();
                            BlockData blockData2 = rowBlocks[k + 1].GetComponent<BlockData>();
                            if (blockData.BlockValue == blockData2.BlockValue && blockData.BlockValue != -1 && !blockData.IsMerge && !blockData2.IsMerge)
                            {
                                blockData2.BlockValue += blockData.BlockValue;
                                blockData.BlockValue = -1; 
                                blockData2.IsMerge = true;

                                rowBlocks[k + 1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = blockData2.BlockValue.ToString();
                            }
                        }

                        int realCount = 0;
                        for (int l = 0; l < rowBlocks.Count; l++)
                        {
                            BlockData bd = rowBlocks[l].GetComponent<BlockData>();
                            if (bd.BlockValue == -1) continue;

                            AddToScore(rowBlocks[l]);
                            BlockMultiplier(bd);

                            int g = realCount;
                            int cellIndex = i * _gridGenerator.ColumnValue + g;

                            rowBlocks[l].transform.SetParent(_gridCells[cellIndex].transform, true);
                            bd.CurrentIndex = cellIndex;
                            bd.IsMerge = false;
                            StartCoroutine(MovingBlock(rowBlocks[l], _blockMoveSpeed, bd.BlockValue));

                            realCount++;
                        }

                        for (int m = 0; m < rowBlocks.Count; m++)
                        {
                            BlockData bd = rowBlocks[m].GetComponent<BlockData>();
                            if (bd.BlockValue != -1) continue;

                            Transform hedefParent = rowBlocks[m + 1].transform.parent;
                            rowBlocks[m].transform.SetParent(hedefParent, true);
                            StartCoroutine(MovingBlock(rowBlocks[m], _blockMoveSpeed, bd.BlockValue));
                        }                
                    }
                    break;
                case Direction.Right:
                    for (int i = 0; i < _gridGenerator.RowValue; i++)
                    {
                        List<GameObject> rowBlocks = new List<GameObject>();

                        for (int j = 0; j < _gridCells.Count; j++)
                        {
                            if(_gridCells[j].transform.childCount == 0) continue;
                            BlockData blockData = _gridCells[j].transform.GetChild(0).GetComponent<BlockData>();
                            int row = blockData.CurrentIndex / _gridGenerator.ColumnValue;
                            if (i == row)
                            {
                                rowBlocks.Add(blockData.gameObject);
                            }
                        }
                        
                        /*
                            Sağa yaslancağı için, listeyi sondan başa gez. Komşu iki eleman eşitse ve ikisi de daha önce
                            birleşmemişse topla, sonucu önde ki(k-1) elamanda tut, sonrakini(k) sil, öndekini işaretle
                        */
                        for (int k = rowBlocks.Count - 1; k > 0; k--)
                        {
                            BlockData blockData = rowBlocks[k].GetComponent<BlockData>();
                            BlockData blockData2 = rowBlocks[k - 1].GetComponent<BlockData>();
                            if (blockData.BlockValue == blockData2.BlockValue && blockData.BlockValue != -1 && !blockData.IsMerge && !blockData2.IsMerge)
                            {
                                blockData2.BlockValue += blockData.BlockValue;
                                blockData.BlockValue = -1; 
                                blockData2.IsMerge = true;

                                rowBlocks[k - 1].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = blockData2.BlockValue.ToString();
                            }
                        }

                        int realCount = 0;
                        // 1. GEÇİŞ: Sadece gerçek (silinmemiş) blokları sağa yasla
                        for (int l = rowBlocks.Count - 1; l >= 0; l--)
                        {
                            BlockData bd = rowBlocks[l].GetComponent<BlockData>();
                            if (bd.BlockValue == -1) continue;

                            AddToScore(rowBlocks[l]);
                            BlockMultiplier(bd);

                            int g = _gridGenerator.ColumnValue - 1 - realCount;
                            int cellIndex = i * _gridGenerator.ColumnValue + g;

                            rowBlocks[l].transform.SetParent(_gridCells[cellIndex].transform, true);
                            bd.CurrentIndex = cellIndex;
                            bd.IsMerge = false;
                            StartCoroutine(MovingBlock(rowBlocks[l], _blockMoveSpeed, bd.BlockValue));

                            realCount++;
                        }

                        // 2. GEÇİŞ: -1 olan (kaybolan) blokları, komşusunun (k-1) gittiği hücreye gönder
                        for (int m = rowBlocks.Count - 1; m > 0; m--)
                        {
                            BlockData bd = rowBlocks[m].GetComponent<BlockData>();
                            if (bd.BlockValue != -1) continue;

                            Transform hedefParent = rowBlocks[m - 1].transform.parent;
                            rowBlocks[m].transform.SetParent(hedefParent, true);
                            StartCoroutine(MovingBlock(rowBlocks[m], _blockMoveSpeed, bd.BlockValue));
                        }                
                    }
                    break;
                default:
                    break;
            }

            for (int i = 0; i < _gridCells.Count; i++)
            {
                if (_gridCells[i].transform.childCount != 0) current.Add(1);
                else current.Add(0);

                if (current[i] != backup[i])
                {
                    changed = true;
                    break;
                }
            }

            if(changed) RandomGridDataGenerator();
            isSwipe = true;
        }
    }

    private void AddToScore(GameObject targetBlock)
    {
        int score = targetBlock.GetComponent<BlockData>().BlockValue;

        _totalScore += score;
        _scoreObject.GetComponent<TextMeshProUGUI>().text = _totalScore.ToString();
    }

    private IEnumerator PopEffect(GameObject targetBlock ,Vector2 targetScale, float count)
    {
        RectTransform targetBlockRect = targetBlock.GetComponent<RectTransform>();
        Vector2 startScale = Vector2.zero;
        float elapsedTime = 0f;

        while (elapsedTime < count)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / count;

            // Lerp fonksiyonu gibi aynı mantığa sahip ama lerp gibi lineer(düz) hızla değil daha yumuşak şekilde değer geçişi yapar
            float smoothTransition = Mathf.SmoothStep(0f, 1f, t);

            targetBlockRect.localScale = Vector2.Lerp(startScale, targetScale, smoothTransition);

            // Bir sonraki kareyi(frame) bekle 
            yield return null;
        }

        // Döngü bittiğinde küsüratlı hataları önlemek için objeyi tam hedefe oturt
        targetBlockRect.anchoredPosition = targetScale;
    }

    private IEnumerator MovingBlock(GameObject targetBlock , float count, int blockValue)
    {
        RectTransform targetBlockRect = targetBlock.GetComponent<RectTransform>();
        Vector2 startScale = targetBlockRect.anchoredPosition;
        float elapsedTime = 0f;

        while (elapsedTime < count)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / count;
            float smoothTransition = Mathf.SmoothStep(0f, 1f, t);

            targetBlockRect.localPosition = Vector2.Lerp(startScale, Vector2.zero, smoothTransition);

            yield return null;
        }

        targetBlockRect.anchoredPosition = Vector2.zero;
        if (blockValue == -1) Destroy(targetBlock);
        isSwipe = false;
    }

    private void BlockMultiplier(BlockData blockData)
    {
        int blockLevel = (int)Mathf.Log(blockData.BlockValue / _baseBlockValue, 2);
        string hexCode;

        switch (blockLevel)
        {
            case 0:
                hexCode = "#" + ColorUtility.ToHtmlStringRGB(_levelSettings.levelTheme.blockColor0);
                ChangeBlockColor(blockData, hexCode);
                break;
            case 1:
                hexCode = "#" + ColorUtility.ToHtmlStringRGB(_levelSettings.levelTheme.blockColor1);
                ChangeBlockColor(blockData, hexCode);
                break;
            case 2:
                hexCode = "#" + ColorUtility.ToHtmlStringRGB(_levelSettings.levelTheme.blockColor2);
                ChangeBlockColor(blockData, hexCode);
                break;
            case 3:
                hexCode = "#" + ColorUtility.ToHtmlStringRGB(_levelSettings.levelTheme.blockColor3);
                ChangeBlockColor(blockData, hexCode);
                break;
            case 4:
                hexCode = "#" + ColorUtility.ToHtmlStringRGB(_levelSettings.levelTheme.blockColor4);
                ChangeBlockColor(blockData, hexCode);
                break;
            case 5:
                hexCode = "#" + ColorUtility.ToHtmlStringRGB(_levelSettings.levelTheme.blockColor5);
                ChangeBlockColor(blockData, hexCode);
                break;
            case 6:
                hexCode = "#" + ColorUtility.ToHtmlStringRGB(_levelSettings.levelTheme.blockColor6);
                ChangeBlockColor(blockData, hexCode);
                break;
            case 7:
                hexCode = "#" + ColorUtility.ToHtmlStringRGB(_levelSettings.levelTheme.blockColor7);
                ChangeBlockColor(blockData, hexCode);
                break;
            case 8:
                hexCode = "#" + ColorUtility.ToHtmlStringRGB(_levelSettings.levelTheme.blockColor8);
                ChangeBlockColor(blockData, hexCode);
                break;

            case 9:
                hexCode = "#" + ColorUtility.ToHtmlStringRGB(_levelSettings.levelTheme.blockColor9);
                ChangeBlockColor(blockData, hexCode);
                break;

            case 10:
                hexCode = "#" + ColorUtility.ToHtmlStringRGB(_levelSettings.levelTheme.blockColor10);
                ChangeBlockColor(blockData, hexCode);
                break;
        }
    }

    private void ChangeBlockColor(BlockData blockData, string hexValue)
    {
        Color renk;
        ColorUtility.TryParseHtmlString(hexValue, out renk);

        blockData.gameObject.GetComponent<Image>().color = renk;
    }
}