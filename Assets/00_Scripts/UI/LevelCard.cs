using System.Collections.Generic;
using UnityEngine;

public class LevelCard : MonoBehaviour
{
    [SerializeField] private int _categoryID;
    [SerializeField] private int _cardID;
    [SerializeField] private int _score;
    [SerializeField] private List<int> _filledCells = new List<int>(16);
    [SerializeField] private List<int> _blockValues = new List<int>(16);

    public int CategoryID {get {return _categoryID;} set {_categoryID = value;}}
    public int CardID {get {return _cardID;} set {_cardID = value;}}
    public int Score {get {return _score;} set {_score = value;}}
    public List<int> FilledCells {get {return _filledCells;} set {_filledCells = value;}}
    public List<int> BlockValues {get {return _blockValues;} set {_blockValues = value;}}
}
