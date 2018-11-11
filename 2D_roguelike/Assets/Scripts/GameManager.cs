using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public float levelStartDelay = 2f; // 레벨 시작전 대기시간 
    public float turnDelay = 0.1f; // 턴 사이 대기 시간 
    public static GameManager instance = null; // static은 클래스 자체에 속함 (인스턴스가 아닌)
    
    public int playerFoodPoints = 100;
    [HideInInspector]public bool playersTurn = true; // 변수는 public이지만 에디터에서 숨김

    private BoardManager boardScript;
    private Text levelText;
    private GameObject levelImage;
    private bool doingSetup;
    private int level = 1;
    private List<Enemy> enemies; // 적들의 위치 기록, 움직이도록 명령
    private bool enemiesMoving;
	// Use this for initialization
	void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        // GameManager는 하나만 필요하므로 2개이상 생겼으면 제거
        DontDestroyOnLoad(gameObject);
        // 우리가 새로운 신을 로드할 때 일반적으로 하이라키의 모든 게임 오브젝트가 소멸
        // 하지만 GameManager는 그대로 존재해야하므로 파괴를 막음
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();
	}
    void OnLevelWasLoaded(int index)
    {
        //Add one to our level number.
        level++;
        //Call InitGame to initialize our level.
        InitGame();
    }
    void InitGame()
    {
        doingSetup = true;
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay); // 타이틀 카드 보여진 뒤 꺼질 때 까지 2초 기다림

        enemies.Clear(); // 적 리스트 초기화
        boardScript.SetupScene(level);
    }
    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup)
            return;
        StartCoroutine(MoveEnemies());
    }
    public void GameOver()
    {
        levelText.text = "After " + level + " days, you starved.";
        levelImage.SetActive(true);
        enabled = false;
    }
	
	
    public void AddEnemyToList(Enemy script)
    {
        // 적들이 자신을 게임 매니저에 등록, 게임 매니저는 적들을 움직이도록
        enemies.Add(script);
    }
    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if(enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }
        for(int i=0;i<enemies.Count;i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime); // 다음 적을 호출하기 전 yield

        }
        playersTurn = true;
        enemiesMoving = false;
    }
}
