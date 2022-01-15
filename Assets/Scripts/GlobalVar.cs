using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalVar : MonoBehaviour
{
    public static int score = 1;
    public static int deltaScore = 0;
    public static int precision = 1; // Only affect UI
    public static int quality;
    public static int maxFPSMultiplier;

    [Header("Main menu")]
    public GameObject loadingPanel;
    public Text resetHighscoreText;
    public Text precisionText;
    public Text qualityText;
    public Text fpsText;

    [Header("Game")]
    public int initialScore = 1;
    public int objectiveScore = 14;
    public GameObject poolBoxObject;
    public Text messageText;
    public Text scoreText;
    public Button restartButton;
    public static bool isWin = false;
    [Header("Static Variables: Please check script!")]
    public static int cueBallObjective = 14;
    public static bool isPenaltyCuesObjective = false;

    // MainMenu procedures
    public void ResetHighscore()
    {
        if (PlayerPrefs.HasKey("highscore"))
        {
            PlayerPrefs.DeleteKey("highscore");
        }
        resetHighscoreText.text = "Reset highscore.";
        PlayerPrefs.Save();
    }

    public void SetFrameRate(int amount)
    {
        Application.targetFrameRate = amount;
    }

    public void GetPrecision()
    {
        if (precision == 15) precisionText.text = "Pre. M";
        else precisionText.text = "Pre. " + precision;
    }

    public void SetPrecision(Text text)
    {
        if (precision >= 15)
        {
            precision = 0;
            text.text = "Pre. 0";
        }
        else if (precision == 5)
        {
            precision = 15;
            text.text = "Pre. M";
        }
        else
        {
            precision++;
            text.text = "Pre. " + precision;
        }
        PlayerPrefs.SetInt("Precision", precision);
    }

    public void GetQuality()
    {
        switch (quality)
        {
            case 0:
                {
                    qualityText.text = "Qlt. VL";
                    break;
                }
            case 1:
                {
                    qualityText.text = "Qlt. L";
                    break;
                }
            case 2:
                {
                    qualityText.text = "Qlt. M";
                    break;
                }
            case 3:
                {
                    qualityText.text = "Qlt. H";
                    break;
                }
            case 4:
                {
                    qualityText.text = "Qlt. VH";
                    break;
                }
            case 5:
                {
                    qualityText.text = "Qlt. U";
                    break;
                }
            default: break;
        }
    }
    public void SetQuality(Text text)
    {
        quality++;

        switch (quality)
        {
            case 1:
                {
                    qualityText.text = "Qlt. L";
                    break;
                }
            case 2:
                {
                    qualityText.text = "Qlt. M";
                    break;
                }
            case 3:
                {
                    qualityText.text = "Qlt. H";
                    break;
                }
            case 4:
                {
                    qualityText.text = "Qlt. VH";
                    break;
                }
            case 5:
                {
                    qualityText.text = "Qlt. U";
                    break;
                }
            case 6:
                {
                    quality = 0;
                    qualityText.text = "Qlt. VL";
                    break;
                }
            default: break;
        }

        QualitySettings.SetQualityLevel(quality);
        PlayerPrefs.SetInt("Quality", quality);
    }

    public void GetFPSLimit()
    {
        maxFPSMultiplier = PlayerPrefs.GetInt("FPSLimit");
        if (maxFPSMultiplier <= 0) fpsText.text = "FPS Max. Inf";
        else fpsText.text = "FPS Max. " + Application.targetFrameRate;
    }
    public void SetFPSLimit(Text text)
    {
        maxFPSMultiplier++;

        if (maxFPSMultiplier > 4)
        {
            maxFPSMultiplier = 0;
            fpsText.text = "FPS Max. Inf";
            SetFrameRate(-1);
        }
        else
        {
            SetFrameRate(maxFPSMultiplier * 30);
            fpsText.text = "FPS Max. " + Application.targetFrameRate;
        }
        PlayerPrefs.SetInt("FPSLimit", maxFPSMultiplier);
    }

    public void ReloadScene()
    {
        Debug.Log("Looping...");
        restartButton.interactable = false;
        LoadScene("GameScene");
    }
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadCustomSceneAsync(sceneName));
    }
    IEnumerator LoadCustomSceneAsync(string scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        restartButton.interactable = true;
    }

    private void Start()
    {
        // Is this the main menu?
        if (qualityText == null)
        {
            cueBallObjective = objectiveScore;
            score = initialScore;
            return;
        }

        precision = PlayerPrefs.GetInt("Precision", 1);
        GetPrecision();

        quality = PlayerPrefs.GetInt("Quality", 2);
        QualitySettings.SetQualityLevel(quality);
        GetQuality();

        maxFPSMultiplier = PlayerPrefs.GetInt("FPSLimit", 2);
        SetFrameRate(maxFPSMultiplier * 30);
        GetFPSLimit();
    }
}
