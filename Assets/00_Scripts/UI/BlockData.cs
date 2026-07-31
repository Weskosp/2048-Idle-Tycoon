using UnityEngine;

public class BlockData : MonoBehaviour
{
    [SerializeField] private int _value;
    [SerializeField] private int _currentIndex;
    [SerializeField] private bool _isMerge;

    public int BlockValue {get {return _value;} set {_value = value;}}
    public int CurrentIndex {get {return _currentIndex;} set {_currentIndex = value;}}
    public bool IsMerge {get {return _isMerge;} set {_isMerge = value;}}
}
