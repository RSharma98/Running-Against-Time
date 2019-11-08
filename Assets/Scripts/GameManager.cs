//Game Manager class handles the UI and timer system

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public float maxTime;
    private float timeRemaining, speedMultiplier;
    private bool gameOver, inTutorial;

    [Header("Particles")]
    public ParticleSystem timeParticles;

    [Header("UI Elements")]
    public GameObject timerBackground;
    public Text timerText;
    public GameObject tutorialStuff;
    public Image gameOverPanel;
    public Text titleText;
    public Text subText;
    public Text retryText;

    public static GameManager instance;
    void Awake(){
        if(instance == null) instance = this;
        else Destroy(this);

        timeRemaining = maxTime;
        speedMultiplier = 1;
        gameOver = false;
        gameOverPanel.enabled = false;
        titleText.enabled = subText.enabled = retryText.enabled = false;
    }

    void Update(){
        if(timeRemaining > 0 && !gameOver && !inTutorial) timeRemaining -= Time.deltaTime * speedMultiplier;
        else if(timeRemaining <= 0) GameOver(false, false);

        if(gameOver) {
            if(Input.GetKeyDown(KeyCode.R)) LoadScene("MainLevel");
            else if(Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        }
        
        timerText.text = timeRemaining.ToString("F1");
    }

    private void LoadScene(string levelName){
        Time.timeScale = 1;
        SceneManager.LoadScene(levelName);
    }

    public void RefillTimer(Vector2 pos){
        timeRemaining = maxTime;
        timeParticles.transform.position = pos;
        timeParticles.Play();
    }

    public void ShowTutorial(bool show){
        inTutorial = show;
        tutorialStuff.SetActive(show);
    }

    public void GameOver(bool gameComplete, bool playerDied){
        this.gameOver = true;
        gameOverPanel.enabled = true;
        titleText.enabled = subText.enabled = retryText.enabled = true;
        if(gameComplete){
            timerBackground.SetActive(false);
            titleText.text = "GAME COMPLETE";
            titleText.color = Color.green;
            subText.text = "I AM PROUD OF YOU";
            retryText.text = "PRESS R TO PLAY AGAIN OR ESC TO QUIT";
        } else{ 
            titleText.text = "GAME OVER";
            titleText.color = Color.red;
            subText.text = playerDied ? "YOU DIED" : "YOU RAN OUT OF TIME";
            retryText.text = "PRESS R TO RETRY OR ESC TO QUIT";
            speedMultiplier = 0;
            Time.timeScale = 0;
        }
    }

    //These multiplier values allow the game to freeze for a short time while transitioning levels
    public void SetMultiplier(float multiplier){
        this.speedMultiplier = multiplier;
    }

    public float GetMultiplier(){
        return speedMultiplier;
    }
}
