using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MovingObject {
    // MovingObject를 상속
    // Use this for initialization

    public int wallDamage = 1; // 플레이어가 벽을 부술 때 벽 오브젝트에 적용될 데미지
    public int pointsPerFood = 10; // 보드 위에서 음식이나 소다를 집었을 때 플레이어 스코어에 더해질 점수
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;

    private Animator animator;
    private int food;

	protected override void Start () { // MovingObject의 start()와 구분
        animator = GetComponent<Animator>();

        food = GameManager.instance.playerFoodPoints; // 해당 레벨 동안 음직 스코어 관리, 레벨이 바뀔 때 GameManager로 다시 저장
        base.Start(); // MovingObject의 start() 호출
	}
	private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
    }
    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.playersTurn) return; // 플레이어의 턴이 아니라면 이하 코드 실행 방지

        int horizontal = 0;
        int vertical = 0; // 플레이어가 움직이려는 방향

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");
        if (horizontal != 0)
            vertical = 0; // 대각선 움직임 방지
        if (horizontal != 0 || vertical != 0) // 플레이어가 움직이려함
            AttemptMove<Wall>(horizontal, vertical); // 플레이어가 벽과 대면할수도 있으므로

    }
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;

        base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D hit; // move 함수에서 이루어진 라인캐스트 충돌 결과를 가져올 변수
        CheckIfGameOver();
        GameManager.instance.playersTurn = false; // 플레이어의 턴이 끝남
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Exit") // 출구에 닿았다면
        {
            Invoke("Restart", restartLevelDelay); //Restart 함수를 Invoke, 지연시간을 입력 즉 1초 후 Restart() 호출
            enabled = false; // 레벨이 끝났으므로 enable은 false
        }
        else if(other.tag == "Food")
        {
            food += pointsPerFood;
            other.gameObject.SetActive(false); //충돌한 오브젝트를 비활성화
        }
        else if(other.tag == "Soda")
        {
            food += pointsPerSoda;
            other.gameObject.SetActive(false);
        }
    }
    protected override void OnCantMove<T>(T component) // 플레이어가 이동하려는 공간에 벽이 있고 이에 막히는 경우의 행동
    {
        Wall hitWall = component as Wall; // component 값을 wall로 변환해서 넣음
        hitWall.DamageWall(wallDamage); // 플레이어가 벽에 얼만큼의 데미지를 줄지 wallDamage로 알림
        animator.SetTrigger("playerChop");
        //throw new NotImplementedException();
    }
    private void Restart()
    {
        // 레벨을 다시 로드, 출구에 도착했을 때 호출
        Application.LoadLevel(Application.loadedLevel); // 마지막으로 로드된 scene을 로드
    }
    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        CheckIfGameOver();
    }
    private void CheckIfGameOver()
    {
        //음식 스코어가 0 이하인지 체크
        if (food < 0)
            GameManager.instance.GameOver();
    }
	
	
}
