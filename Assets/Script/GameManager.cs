using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    [Header("Grid Setup")]
    public RectTransform gridContainer;   // UI Panel where cards will be placed
    public GameObject cardPrefab;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;

    [Header("Difficulty")]
    public int rows;
    public int cols;


    private SaveData loadedData;

    private List<Card> allCards = new List<Card>();
    private List<Sprite> faceImages = new List<Sprite>();
    private int totalMatchesNeeded;
    private int matchesFound = 0;

    bool gameOver = false;

    void Awake()
    {
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        if (SaveLoadManager.SaveExists())
        {
            loadedData = SaveLoadManager.LoadGame();
            if (loadedData != null)
            {
                rows = loadedData.rows;
                cols = loadedData.cols;

                Debug.Log("Loaded JSON: " + File.ReadAllText(Application.persistentDataPath + "/save.json"));

                LoadFaceImagesFromSave();
                CreateGrid(fromSave: true);

                ScoreManager.Instance.RestoreScore(loadedData.score);

                CheckAllMatched();

                 CompareManager.Instance.EnableInput();
                return;
            }
            else
            {
                SaveLoadManager.DeleteSave();
                Debug.LogError("Failed to load save data.");
            }
        }


        Debug.Log("No save found, starting new game.");
        rows = GameSettings.rows;
        cols = GameSettings.cols;

        LoadFaceImages();
        CreateGrid(fromSave: false);
        SaveCurrentState();

        StartCoroutine(PreviewCards());
    }

    // Load images for the cards
    public void LoadFaceImages()
    {
        Sprite[] loadedSprites = Resources.LoadAll<Sprite>("Images");
        List<Sprite> all = new List<Sprite>(loadedSprites);
        Shuffle(all);

        int pairCount = (rows * cols) / 2;
        faceImages.Clear();

        for (int i = 0; i < pairCount; i++)
        {
            Debug.Log("Loaded sprite: " + all[i].name);

            faceImages.Add(all[i]);
            faceImages.Add(all[i]);
        }

        Shuffle(faceImages);

        totalMatchesNeeded = pairCount;
        matchesFound = 0;
    }

    // Load images matching saved indexes (for continuing game)
    void LoadFaceImagesFromSave()
    {
        Sprite[] images = Resources.LoadAll<Sprite>("Images");
        faceImages.Clear();

        if (loadedData == null)
        {
            Debug.LogWarning("⚠ loadedData was NULL. Starting new game.");
            LoadFaceImages();
            return;
        }

        if (loadedData.faceIndexes == null)
        {
            Debug.LogWarning("⚠ Save data missing faceIndexes, starting new level.");
            LoadFaceImages();
            return;
        }

        foreach (int index in loadedData.faceIndexes)
        {
            if (index >= 0 && index < images.Length)
                faceImages.Add(images[index]);
            else
                faceImages.Add(images[0]); // fallback safe
        }

        totalMatchesNeeded = (rows * cols) / 2;
        matchesFound = CountMatched(loadedData.matched);

        Debug.Log("Restored matches found: " + matchesFound);
        Debug.Log("Total matches needed: " + totalMatchesNeeded);
    }

    // Count already matched cards on restore
    int CountMatched(List<bool> matched)
    {
        if(loadedData == null || loadedData.matched == null)
            return 0;

        int trueCount = 0;
        for (int i = 0; i < loadedData.matched.Count; i++)
        {
            if (loadedData.matched[i])
                trueCount++;
        }

        return trueCount / 2;
    }


    // Create the grid of cards
    public void CreateGrid(bool fromSave)
    {
        float width = gridContainer.rect.width;
        float height = gridContainer.rect.height;

        float cardWidth = width / cols;
        float cardHeight = height / rows;

        float cardSize = Mathf.Min(cardWidth, cardHeight);
        float spacing = 10f; // adjustable spacing


        // Total grid dimensions using chosen card size
        float gridW = cols * cardSize;
        float gridH = rows * cardSize;

        // Start positions so grid  is centered
        float startX = -gridW / 2f + cardSize / 2f;
        float startY = gridH / 2f - cardSize / 2f;

        int faceIndex = 0;
        allCards.Clear();

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GameObject cardObj = Instantiate(cardPrefab, gridContainer);
                RectTransform rt = cardObj.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(cardSize - spacing, cardSize - spacing);

                float posX = startX + c * cardSize;
                float posY = startY - r * cardSize;
                rt.anchoredPosition = new Vector2(posX, posY);

                Card card = cardObj.GetComponent<Card>();
                card.SetCardFace(faceImages[faceIndex]);

                if (fromSave && loadedData != null && loadedData.matched != null &&
                    loadedData.matched.Count > faceIndex && loadedData.matched[faceIndex])
                {
                    card.FlipOpenInstant();
                }

                allCards.Add(card);
                faceIndex++;
            }
        }
    }

    // Preview all cards at start
    IEnumerator PreviewCards()
    {
        yield return new WaitForSeconds(1.0f); 
        // Show all cards
        foreach (var card in allCards)
        {
            card.ShowImmediate();
        }

        yield return new WaitForSeconds(1.5f); // duration

        // Hide all unmatched cards
        foreach (var card in allCards)
        {
            if (!card.isMatched)
                card.HideImmediate();
        }

        CompareManager.Instance.EnableInput(); 
    }


    // Check if game over
    public void CheckGameOver()
    {
        matchesFound++;
        SaveCurrentState();
        if (matchesFound >= totalMatchesNeeded)
        {
            Debug.Log("Game Over! All matches found.");

            gameOver = true;
            Invoke(nameof(ShowGameOverUI), 1.0f);
            SaveLoadManager.DeleteSave();
        }
    }

    // Check if all cards are matched
    void CheckAllMatched()
    {
        foreach (Card card in allCards)
        {
            if (!card.isMatched)
                return;
        }

        Debug.Log("Game Over! All matches found.");
        SaveLoadManager.DeleteSave();
        
        LoadFaceImages();
        CreateGrid(fromSave: false);
    }

    // Return to menu
    public void BackToMenu()
    {
        AudioManager.Instance.PlayButtonTap();
        SceneManager.LoadScene("MenuScreen");
    }

    // Show Game Over UI
    public void ShowGameOverUI()
    {
        AudioManager.Instance.PlayGameOver();

        gameOverPanel.SetActive(true);
    }

    // Back button click handler
    public void BackButtonClick()
    {
        AudioManager.Instance.PlayButtonTap();
        if (!gameOver)
            SceneManager.LoadScene("MenuScreen");
    }

    // Restart the game
    public void RestartGame()
    {
        AudioManager.Instance.PlayButtonTap();
        int prevRows = rows;
        int prevCols = cols;

        SaveLoadManager.DeleteSave();

        GameSettings.rows = prevRows;
        GameSettings.cols = prevCols;

        SceneManager.LoadScene("GamePlay");
    }

    // Shuffle a list of sprites
    void Shuffle(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int r = Random.Range(i, list.Count);
            Sprite temp = list[i];
            list[i] = list[r];
            list[r] = temp;
        }
    }
    

    // Save the current game state
    public void SaveCurrentState()
    {
        SaveData data = new SaveData();
        data.rows = rows;
        data.cols = cols;

        data.faceIndexes = new List<int>();
        data.matched = new List<bool>();

        Sprite[] images = Resources.LoadAll<Sprite>("Images");

        foreach (Card card in allCards)
        {
            int index = System.Array.IndexOf(images, card.faceSprite);
            if (index < 0) index = 0;
            data.faceIndexes.Add(index);
            data.matched.Add(card.isMatched);
        }

        data.score = ScoreManager.Instance.GetScore();
        SaveLoadManager.SaveGame(data);
    }

}
