using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// GameManager가 인스턴스화 되었는지 체크한 후
// 그렇지 않으면 프리팹으로부터 하나 인스턴스화
public class Loarder : MonoBehaviour {

    public GameManager gameManager;
	// Use this for initialization
	void Awake () {
        if (GameManager.instance == null)
            Instantiate(gameManager);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
