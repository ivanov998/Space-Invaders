using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Game : MonoBehaviour
{
    int points = 0;
    int lives = 2;
    int level = 1;

    private Dictionary<int,int> colToInvaderType = new Dictionary<int,int>() {
        {0, 0}, {1, 1}, {2, 1}, {3, 2}, {4, 2}
    };

    private Dictionary<int,float> bunkerLocation = new Dictionary<int,float>() { {0, -1.5f}, {1, 0f}, {2, 1.5f} };

    private int rows = 5;
    private int cols = 6;

    public float leftWorldBorder, rightWorldBorder;

    private Vector3 invadersDirection;

    private TextMeshProUGUI levelIndicator, scoreBoard, overallScore;

    public GameObject bunker;
    private GameObject[] bunkers = new GameObject[3];

    public GameObject laser;

    public GameObject[] invaderVariants = new GameObject[3];
    private List<GameObject> invaders = new List<GameObject>();

    public GameObject mysteryShip;
    private GameObject activeMysteryShip;

    public GameObject gameOverPanel;

    void Start()
    {
        GetWorldBorders();

        levelIndicator = GameObject.Find("Level").GetComponent<TextMeshProUGUI>();
        scoreBoard = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();

        SpawnInvaders();
        SpawnBunkers();
    }

    public float CalculateLeftWorldBorder() 
    {
        Camera cam = Camera.main;
        Vector3 p = cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight, cam.nearClipPlane));

        return p[0];
    }

    private void GetWorldBorders() 
    {
        leftWorldBorder = CalculateLeftWorldBorder();
        rightWorldBorder = Mathf.Abs(leftWorldBorder);
    }

    void SpawnInvaders() 
    {
        float interval = 1.0f - (level * 0.075f);
        
        for (int col = 0; col < cols; col++) {
            for (int row = 0; row < rows; row++) {
                GameObject inv = Instantiate(invaderVariants[colToInvaderType[row]], 
                    new Vector3(leftWorldBorder + 0.5f + (col*0.5f),3 - (row * 0.5f) , 0), Quaternion.identity);
                inv.GetComponent<Invader>().altTime = interval;
                invaders.Add(inv);
            }
        }

        invadersDirection = Vector3.right;

        InvokeRepeating("MoveInvaders", 1.0f, interval);
        InvokeRepeating("ShootLasers", 0.5f, interval);

        Invoke("SpawnMysteryShip", Random.Range(5.0f, 8.0f));
    }

    void SpawnBunkers() 
    {
        RemoveBunkers();

        for (int i = 0; i < 3; i++) {
            bunkers[i] = Instantiate(bunker,new Vector3(bunkerLocation[i],-2.5f, 0), Quaternion.identity);
        }
    }

    void SpawnMysteryShip() 
    {
        activeMysteryShip = Instantiate(mysteryShip, new Vector3(leftWorldBorder - 0.5f, 4.0f, 0), Quaternion.identity);
        StartCoroutine("MoveMysteryShip");
    }

    void MoveInvaders() 
    {
        foreach(GameObject inv in invaders)
            inv.transform.position += invadersDirection * 0.15f;
        CheckAndInvertInvadersDirection();
        CheckIfInvadersReachedBottom();
    }

    void CheckAndInvertInvadersDirection() 
    {
        float posX = getFurthestInvaderForDirection().transform.position.x;
        if ((invadersDirection == Vector3.right && rightWorldBorder - posX < 0.25f) ||
            (invadersDirection == Vector3.left && -leftWorldBorder + posX < 0.25f)) {
            invadersDirection *= -1;
            DropInvadersDown();
        }
    }

    void CheckIfInvadersReachedBottom() 
    {
        if (getFurthestDownInvader().transform.position.y < -2.2f) {
            RemoveBunkers();
        }

        if (getFurthestDownInvader().transform.position.y < -3.1f) {
            EndGame();
        }
    }

    void DropInvadersDown() 
    {
        foreach(GameObject inv in invaders)
            inv.transform.position += Vector3.down * 0.25f;
    }

    IEnumerator MoveMysteryShip() 
    {
        while (true) {
            
            yield return new WaitForSeconds(Time.deltaTime);

            if (activeMysteryShip.transform.position.x > rightWorldBorder + 0.5f)
                KillMysteryShip(0);

            activeMysteryShip.transform.position += Vector3.right * Time.deltaTime * 2.0f;
        }
    }

    public void KillInvader(GameObject invader, int reward) 
    {

        AddPoints(reward);

        invaders.Remove(invader);

        if (invaders.Count == 0) {
            NextLevel();
        }

        Destroy(invader);
    }

    public void KillMysteryShip(int reward) 
    {
        if (reward > 0)
            AddPoints(reward);

        Destroy(activeMysteryShip);
        StopCoroutine("MoveMysteryShip");
        Invoke("SpawnMysteryShip", Random.Range(5.0f,8.0f));
    }

    void RemoveBunkers() 
    {
        if (bunkers.Length == 0) return;

        foreach(GameObject bnk in bunkers)
            Destroy(bnk);
    }

    public void RemoveLife() 
    {
        if (lives > 0) {
            Destroy(GameObject.Find("Live Icon"));
            lives--;
        } else {
            EndGame();
        }
    }
    
    GameObject getFurthestInvaderForDirection () =>
        invaders.OrderBy(inv => inv.transform.position.x * (invadersDirection == Vector3.right ? -1 : 1)).First();

    GameObject getFurthestDownInvader () => invaders.OrderBy(inv => inv.transform.position.y).First();

    void ShootLasers() 
    {
        int index = Random.Range(0 , invaders.Count);
        Vector3 pos = invaders[index].transform.position;

        GameObject activeLaser = Instantiate(laser, pos, Quaternion.identity);
        activeLaser.GetComponent<Laser>().isFromInvader = true;
    }

    void AddPoints(int reward) 
    {
        points += reward;
        scoreBoard.text = "Score: " + points;
    }

    void NextLevel() 
    {
        level++;

        CancelInvoke();
        
        KillMysteryShip(0);
        
        Invoke("SpawnInvaders",1.5f);
        Invoke("SpawnBunkers",1.0f);

        levelIndicator.text = "Level: " + level;
    }
    
    void EndGame() 
    {
        gameOverPanel.SetActive(true);

        overallScore = GameObject.Find("Highscore").GetComponent<TextMeshProUGUI>();
        overallScore.text = "Score: " + points;

        int highscore = PlayerPrefs.GetInt("highscore");

        if (points > highscore) {
            overallScore.text += "\n(new record)";
            PlayerPrefs.SetInt("highscore", points);
        }

        CancelInvoke();
    }

    public void PlayAgain() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu() 
    {
        SceneManager.LoadScene("Menu");
    }
}
