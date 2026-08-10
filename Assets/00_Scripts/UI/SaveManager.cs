using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public List<CategoryProgress> categories = new List<CategoryProgress>();
    public List<LevelData> levels = new List<LevelData>();
}

[Serializable]
public class CategoryProgress
{
    public int categoryID;
    public int cardCount;
}

[Serializable]
public class LevelData
{
    public int categoryID;
    public int cardID;
    public int score;
    public List<int> filledCells = new List<int>(16);
    public List<int> blockValues = new List<int>(16);
}

public class SaveManager : MonoBehaviour
{
    private string desktop;
    private string folderPath;

    void Awake()
    {
        desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        folderPath = Path.Combine(desktop, "TestVerisi.json");

        if (!File.Exists(folderPath))
        {
            GameSaveData main = new GameSaveData
            {
                categories = new List<CategoryProgress>(),
                levels = new List<LevelData>()
            };

            string saveData = JsonUtility.ToJson(main, true);
            File.WriteAllText(folderPath, saveData);
        }
    }

    public void CreateLevelScreenData(int id)
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

        main.categories.Add(createCategory);
        string saveData = JsonUtility.ToJson(main, true);
        File.WriteAllText(folderPath, saveData);
    }

    public void CreateLevelData(LevelCard cardData)
    {
        string currentJson = File.ReadAllText(folderPath);
        GameSaveData main = JsonUtility.FromJson<GameSaveData>(currentJson);

        LevelData isDataExits = main.levels.Find(x => x.categoryID == cardData.CategoryID && x.categoryID == cardData.CategoryID);
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

    public int LoadCardData(int categoryID)
    {
        string currentJson = File.ReadAllText(folderPath);
        GameSaveData main = JsonUtility.FromJson<GameSaveData>(currentJson);
        CategoryProgress category = main.categories.Find(x => x.categoryID == categoryID);

        return category.cardCount;
    }

    public LevelData LoadLevelData(int categoryID, int cardID)
    {
        string currentJson = File.ReadAllText(folderPath);
        GameSaveData main = JsonUtility.FromJson<GameSaveData>(currentJson);
        LevelData levelData = main.levels.Find(x => x.categoryID == categoryID && x.cardID == cardID);
        
        return levelData;
    }
}