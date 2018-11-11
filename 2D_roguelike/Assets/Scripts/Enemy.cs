using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

    public int playerDamage; // 적이 플레이어를 공격할때 뺄 음식 포인트
    private Animator animator;
    private Transform target; // 플레이어 위치 저장, 적이 어디로 향할지 알려줌
    private bool skipMove; // 적이 턴마다 움직이게 하는데 사용
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

	// Use this for initialization
	protected override void Start () {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
	}

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if(skipMove)
        {
            skipMove = false; // 턴이 돌아올때만 움직이도록
            return;
        }
        base.AttemptMove<T>(xDir, yDir);

        skipMove = true; // 적이 움직였으므로 true로
    }
    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon) // x좌표가 대충 비슷한지 확인
        {
            yDir = target.position.y > transform.position.y ? 1 : -1;

        }
        else
            xDir = target.position.x > transform.position.x ? 1 : -1;
        AttemptMove<Player>(xDir, yDir);
    }
    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player; // component 값을 player로 변환해서 넣음

        hitPlayer.LoseFood(playerDamage); // 플레이어의 LoseFood 호출
        animator.SetTrigger("enemyAttack");

        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }
}
