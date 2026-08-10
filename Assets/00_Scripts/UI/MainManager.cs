using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _contents;
    [SerializeField] private GameObject _titleTemplate;
    [SerializeField] private GameObject[] _levels;

    private SaveManager saveManager;

    public static int CategoryID, CardID;

    void Start()
    {
        saveManager = GetComponent<SaveManager>();
        
        for (int i = 0; i < _levels.GetLength(0); i++)
        {
            saveManager.CreateLevelScreenData(i);

            if (saveManager.LoadCardData(i) == 0) continue;

            GameObject content = _levels[i].transform.Find("LevelCards").Find("Viewport").Find("Content").gameObject;
            for (int j = 0; j < saveManager.LoadCardData(i); j++)
            {
                AddTitleCard(content);   
            }
        }
    }

    public void ChangeContent(string contentName)
    {
        for (int i = 0; i < _contents.GetLength(0); i++)
        {
            if (contentName + "Screen" == _contents[i].name)
            {
                _contents[i].SetActive(true);
                continue;
            }
            _contents[i].SetActive(false);
        }
    }

    public void AddTitleCard(GameObject content)
    {
        string levelName = content.transform.parent.parent.parent.name;
        char levelNumber = levelName[levelName.Length - 1];

        GameObject newTitleCard = Instantiate(_titleTemplate, content.transform);
        Button titleButton = newTitleCard.GetComponent<Button>();
        LevelCard data = newTitleCard.GetComponent<LevelCard>();

        data.CategoryID = int.Parse(levelNumber.ToString()) - 1;
        data.CardID = content.transform.childCount - 1;

        saveManager.CreateLevelData(data);

        titleButton.onClick.AddListener(() => {EnterToLevel(data.CategoryID, data.CardID);});
        Destroy(newTitleCard.transform.GetChild(0).gameObject);
    }

    void EnterToLevel(int categoryID, int cardID)
    {
        CategoryID = categoryID;
        CardID = cardID;
        SceneManager.LoadScene("LevelTemplate");
    }
}