using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null; // static은 클래스 자체에 속함 (인스턴스가 아닌)
    public BoardManager boardScript;
    private int level = 3;
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
        boardScript = GetComponent<BoardManager>();
        InitGame();
	}
	void InitGame()
    {
        boardScript.SetupScene(level);
    }
	// Update is called once per frame
	void Update () {
		
	}
}
