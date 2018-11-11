//using System;
using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    // MovingObject를 상속
    // Use this for initialization

    public int wallDamage = 1; // 플레이어가 벽을 부술 때 벽 오브젝트에 적용될 데미지
    public int pointsPerFood = 10; // 보드 위에서 음식이나 소다를 집었을 때 플레이어 스코어에 더해질 점수
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text foodText;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;


    private Animator animator;
    private int food;
    private Vector2 touchOrigin = -Vector2.one; // touchOrigin에 터치가 시작되는 지점을 저장, 스크린 밖을 의미하는 -Vector2.one 으로 초기화

    protected override void Start()
    { // MovingObject의 start()와 구분
        animator = GetComponent<Animator>();

        food = GameManager.instance.playerFoodPoints; // 해당 레벨 동안 음직 스코어 관리, 레벨이 바뀔 때 GameManager로 다시 저장

        foodText.text = "Food : " + food;
        base.Start(); // MovingObject의 start() 호출
    }
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;
        foodText.text = "Food : " + food;

        base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D hit; // move 함수에서 이루어진 라인캐스트 충돌 결과를 가져올 변수
        if (Move(xDir, yDir, out hit))
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }
        CheckIfGameOver();
        GameManager.instance.playersTurn = false; // 플레이어의 턴이 끝남
    }
    protected override void OnCantMove<T>(T component) // 플레이어가 이동하려는 공간에 벽이 있고 이에 막히는 경우의 행동
    {
        Wall hitWall = component as Wall; // component 값을 wall로 변환해서 넣음
        hitWall.DamageWall(wallDamage); // 플레이어가 벽에 얼만큼의 데미지를 줄지 wallDamage로 알림
        animator.SetTrigger("playerChop");
        //throw new NotImplementedException();
    }
    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
    }
    // Update is called once per frame
    private void Update()
    {
        if (!GameManager.instance.playersTurn) return; // 플레이어의 턴이 아니라면 이하 코드 실행 방지

        int horizontal = 0;
        int vertical = 0; // 플레이어가 움직이려는 방향

#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR

        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));
        if (horizontal != 0)
            vertical = 0; // 대각선 움직임 방지
#else
        if(Input.touchCount > 0)
        {
            Touch myTouch = Input.touches[0]; // 처음 터치하느 지점을 myTouch에 저장
            if(myTouch.phase == TouchPhase.Began)
            {
                touchOrigin = myTouch.position;
            }
            else if(myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                Vector2 touchEnd = myTouch.position;
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;
                touchOrigin.x = -1;
                if(Mathf.Abs(x) > Mathf.Abs(y))
                {
                    horizontal = x > 0 ? 1 : -1;
                }
                else
                {
                    vertical = y > 0 ? 1 : -1;
                }
            }
        }
#endif
        if (horizontal != 0 || vertical != 0) // 플레이어가 움직이려함
            AttemptMove<Wall>(horizontal, vertical); // 플레이어가 벽과 대면할수도 있으므로

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit") // 출구에 닿았다면
        {
            Invoke("Restart", restartLevelDelay); //Restart 함수를 Invoke, 지연시간을 입력 즉 1초 후 Restart() 호출
            enabled = false; // 레벨이 끝났으므로 enable은 false
        }
        else if (other.tag == "Food")
        {
            food += pointsPerFood;
            foodText.text = "+" + pointsPerFood + " Food : " + food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false); //충돌한 오브젝트를 비활성화
        }
        else if (other.tag == "Soda")
        {
            food += pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + " Food : " + food;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
        }
    }

    private void Restart()
    {
        // 레벨을 다시 로드, 출구에 도착했을 때 호출
        //Application.LoadLevel(Application.loadedLevel); // 마지막으로 로드된 scene을 로드

        SceneManager.LoadScene(0);
    }
    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = "-" + loss + " Food : " + food;
        CheckIfGameOver();
    }
    private void CheckIfGameOver()
    {
        //음식 스코어가 0 이하인지 체크
        if (food <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }


}
