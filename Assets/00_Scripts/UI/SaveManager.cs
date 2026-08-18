using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Bu sınıf, oyun verilerini temsil eder ve JSON formatında kaydedilir.
[Serializable]
public class GameSaveData
{
    public List<CategoryProgress> categories = new List<CategoryProgress>();
    public List<LevelCurrency> wallet = new List<LevelCurrency>();
    public List<LevelData> levels = new List<LevelData>();
}


// Bu sınıf, her kategori için kart sayısını temsil eder.
[Serializable]
public class CategoryProgress
{
    public int categoryID;
    public int cardCount;
}

// Bu sınıf, her seviye için gerekli verileri temsil eder.
[Serializable]
public class LevelData
{
    public int categoryID;
    public int cardID;
    public int score;
    public List<int> filledCells = new List<int>(16);
    public List<int> blockValues = new List<int>(16);
}

// Bu sınıf, her kategori için para miktarını temsil eder.
[Serializable]
public class LevelCurrency
{
    public int categoryID;
    public long amount;
}

public class SaveManager : MonoBehaviour
{
    private string desktop;
    private string folderPath;

    void Awake()
    {
        #if UNITY_EDITOR
            desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            folderPath = Path.Combine(desktop, "TestVerisi.json");
        #else
            folderPath = Path.Combine(Application.persistentDataPath, "TestVerisi.json");
        #endif

        if (!File.Exists(folderPath))
        {
            GameSaveData main = new GameSaveData
            {
                categories = new List<CategoryProgress>(),
                wallet = new List<LevelCurrency>(),
                levels = new List<LevelData>()
            };

            string saveData = JsonUtility.ToJson(main, true);
            File.WriteAllText(folderPath, saveData);
        }
    }

    // Bu method, belirtilen kategori ID'si ile yeni bir kategori verisi oluşturur ve kaydeder.
    public void CreateCategoryData(int id)
    {
        string currentJson = File.ReadAllText(folderPath);
        GameSaveData main = JsonUtility.FromJson<GameSaveData>(currentJson);

        CategoryProgress isCategoryExits = main.categories.Find(x => x.categoryID == id);
        if (isCategoryExits != null) return;

        CategoryProgress createCategory = new CategoryProgress
        {
            categoryID = id,
            cardCount = 0
        };

        LevelCurrency createCurrency = new LevelCurrency
        {
          categoryID = id,
          amount = 0  
        };

        main.categories.Add(createCategory);
        main.wallet.Add(createCurrency);

        string saveData = JsonUtility.ToJson(main, true);
        File.WriteAllText(folderPath, saveData);
    }

    // Bu method, verilen LevelCard verisi ile yeni bir seviye verisi oluşturur ve kaydeder.
    public void CreateLevelData(LevelCard cardData)
    {
        string currentJson = File.ReadAllText(folderPath);
        GameSaveData main = JsonUtility.FromJson<GameSaveData>(currentJson);

        LevelData isDataExits = main.levels.Find(x => x.categoryID == cardData.CategoryID && x.cardID == cardData.CardID);
        if (isDataExits != null) return;
        
        LevelData levelData = new LevelData()
        {
            categoryID = cardData.CategoryID,
            cardID = cardData.CardID,
            score = cardData.Score
        };

        CategoryProgress category = main.categories.Find(x => x.categoryID == cardData.CategoryID);
        category.cardCount += 1;
        
        main.levels.Add(levelData);
        
        string saveData = JsonUtility.ToJson(main, true);
        File.WriteAllText(folderPath, saveData);
    }

    // Bu method, verilen kategori ID'si, kart ID'si, skor ve hücre verileri ile mevcut seviye verisini günceller ve kaydeder.
    public void SaveLevel(int categoryID, int cardID, int score, List<int> cellsIndex, List<int> blockValues)
    {
        string currentJson = File.ReadAllText(folderPath);
        GameSaveData main = JsonUtility.FromJson<GameSaveData>(currentJson);

        LevelData levelData = main.levels.Find(x => x.categoryID == categoryID && x.cardID == cardID);
        if (levelData == null) return;

        levelData.filledCells.Clear();
        levelData.blockValues.Clear();

        levelData.score = score;
        levelData.filledCells.AddRange(cellsIndex);
        levelData.blockValues.AddRange(blockValues);

        string saveData = JsonUtility.ToJson(main, true);
        File.WriteAllText(folderPath, saveData);
    }

    public void SaveMain(int categoryID, long amount)
    {
        string currentJson = File.ReadAllText(folderPath);
        GameSaveData main = JsonUtility.FromJson<GameSaveData>(currentJson);

        LevelCurrency levelCurrency = main.wallet.Find(x => x.categoryID == categoryID);
        levelCurrency.amount = amount;

        string saveData = JsonUtility.ToJson(main, true);
        File.WriteAllText(folderPath, saveData);
    }

    // Bu method, verilen kategori ID'si ile kaydedilen kart sayısını döndürür.
    public int LoadCardData(int categoryID)
    {
        string currentJson = File.ReadAllText(folderPath);
        GameSaveData main = JsonUtility.FromJson<GameSaveData>(currentJson);
        CategoryProgress category = main.categories.Find(x => x.categoryID == categoryID);

        return category.cardCount;
    }

    // Bu method, verilen kategori ID'si ve kart ID'si ile kaydedilen seviye verisini döndürür.
    public LevelData LoadLevelData(int categoryID, int cardID)
    {
        string currentJson = File.ReadAllText(folderPath);
        GameSaveData main = JsonUtility.FromJson<GameSaveData>(currentJson);
        LevelData levelData = main.levels.Find(x => x.categoryID == categoryID && x.cardID == cardID);
        
        return levelData;
    }

    public LevelCurrency LoadWalletData(int categoryID)
    {
        string currentJson = File.ReadAllText(folderPath);
        GameSaveData main = JsonUtility.FromJson<GameSaveData>(currentJson);

        LevelCurrency levelCurrency = main.wallet.Find(x => x.categoryID == categoryID);

        return levelCurrency;
    }

    // Bu method, kaydedilen oyun verilerini siler.
    public void DeleteSaveFile()
    {
        if (File.Exists(folderPath))
        {
            File.Delete(folderPath);
            Debug.Log("Kayıt dosyası başarıyla silindi!");
        }
        else
        {
            Debug.LogWarning("Silinecek dosya zaten yok.");
        }    
    }
}