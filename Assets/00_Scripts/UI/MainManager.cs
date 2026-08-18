using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    [SerializeField] private GameObject _titleTemplate;
    [SerializeField] private GameObject[] _contents;
    [SerializeField] private GameObject[] _levels;
    [SerializeField] private GameObject[] _levelCurrencies;

    private float[] productionRange = {1.0f, 2.5f}; 
    private float[] counts;

    private SaveManager saveManager;
    private Dictionary<int, List<LevelCard>> levelCards = new Dictionary<int, List<LevelCard>>();
    public long[] wallet, currentScores;

    public static int CategoryID, CardID;

    void Start()
    {
        saveManager = GetComponent<SaveManager>();
        counts = new float[productionRange.Length];
        wallet = new long[_levels.GetLength(0)];
        currentScores = new long[_levels.GetLength(0)];

        LoadSaveData();
    }

    void Update()
    {
        // Tüm kategorileri aynı anda kontrol eden tek bir döngü
        for (int i = 0; i < productionRange.Length; i++)
        {
            // 1. O kategorinin kendi sayacına geçen zamanı ekle (Time.deltaTime gerçek saniyeyi ölçer)
            counts[i] += Time.deltaTime;

            // 2. Eğer o kategorinin sayacı, kendi hedeflenen süresini geçtiyse
            if (counts[i] >= productionRange[i])
            {
                // Parayı üret ve cüzdana ekle
                wallet[i] += currentScores[i];
                _levelCurrencies[i].GetComponentInChildren<TextMeshProUGUI>().text = wallet[i].ToString();

                // Sayacı sıfırla ki bir sonraki döngü için baştan saysın
                counts[i] = 0f; 
            }
        }
    }

    // Bu methodu, verilen contentName ile eşleşen içeriği etkinleştirir ve diğerlerini devre dışı bırakır.
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

    // Bu method, verilen content GameObject'ine yeni bir başlık kartı ekler ve gerekli verileri kaydeder.
    public void AddTitleCard(GameObject content)
    {
        string levelName = content.transform.parent.parent.parent.name;
        char levelNumber = levelName[levelName.Length - 1];

        GameObject newTitleCard = Instantiate(_titleTemplate, content.transform);
        Button titleButton = newTitleCard.GetComponent<Button>();
        TextMeshProUGUI cardText = titleButton.GetComponentInChildren<TextMeshProUGUI>();
        LevelCard data = newTitleCard.GetComponent<LevelCard>();

        data.CategoryID = int.Parse(levelNumber.ToString()) - 1;
        data.CardID = content.transform.childCount - 1;

        saveManager.CreateLevelData(data);
        levelCards[data.CategoryID].Add(data);

        data.Score = saveManager.LoadLevelData(data.CategoryID, data.CardID).score;
        currentScores[data.CategoryID] += data.Score;
        cardText.text = data.Score.ToString();

        titleButton.onClick.AddListener(() => {EnterToLevel(data.CategoryID, data.CardID);});
    }

    // Bu method, kaydedilen oyun verilerini siler ve ana menü sahnesine geri döner.
    public void DeleteSaveFile()
    {
        saveManager.DeleteSaveFile();
        SceneManager.LoadScene("Main");
    }

    // Bu method, belirtilen kategori ve kart ID'si ile seviye sahnesine girer.
    void EnterToLevel(int categoryID, int cardID)
    {
        CategoryID = categoryID;
        CardID = cardID;
        SceneManager.LoadScene("LevelTemplate");
    }

    // Bu method, kaydedilen oyun verilerini yükler ve her kategori için başlık kartlarını oluşturur.
    void LoadSaveData()
    {
        for (int i = 0; i < _levels.GetLength(0); i++)
        {
            saveManager.CreateCategoryData(i);
            levelCards.Add(i, new List<LevelCard>());
            wallet[i] = saveManager.LoadWalletData(i).amount;
            _levelCurrencies[i].GetComponentInChildren<TextMeshProUGUI>().text = wallet[i].ToString();

            if (saveManager.LoadCardData(i) == 0) continue;

            GameObject content = _levels[i].transform.Find("LevelCards").Find("Viewport").Find("Content").gameObject;
            for (int j = 0; j < saveManager.LoadCardData(i); j++)
            {
                AddTitleCard(content); 
            }
        }
    }

    void OnApplicationQuit()
    {
        for (int i = 0; i < wallet.Length; i++)
        {
            saveManager.SaveMain(i, wallet[i]);
        }
    }
}