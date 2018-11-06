using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// public abstrace 클래스
// 플레이어와 적이 MovingObject 상속
// 플레이어는 벽과 상호작용, 적은 플레이어와 상호작용
// 상호작용 하는 hitComponent의 종류을 알 수 없으므로 generic으로 선언
public abstract class MovingObject : MonoBehaviour {

    public float moveTime = 0.1f;
    public LayerMask blockingLayer; // 이동할 공간이 열려있고, 그곳으로 이동하려 할때 충돌이 일어났는지 체크
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime; //움직임을 더 효율적으로 계산하는데 사용
                                   // Use this for initialization
    protected virtual void Start() { // 자식 클래스가 덮어써서 재정의 (오버라이드)
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime; // moveTime의 역수를 넣어 나누기 대신에 계산에 효율적인 곱하기를 사용

    }
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit) // out은 입력을 레퍼런스로 받게함
    {
        // move 함수가 두개 이상의 값을 리턴하기 위해서, 함수에 의해 리턴되는 bool과 hit라는 RaycastHit2D 또한 리턴
        Vector2 start = transform.position; // 시작하는 위치
        Vector2 end = start + new Vector2(xDir, yDir); // 끝나는 위치
        boxCollider.enabled = false; // Ray 사용할때 자신의 충돌체에 부딪치지 않게 하기 위해 boxCollider 해제
        hit = Physics2D.Linecast(start, end, blockingLayer); // 시작에서 끝까지의 라인을 가져와 BlockingLayer와 충돌을 검사
        boxCollider.enabled = true;
        if (hit.transform == null) // 라인으로 검사한 공간이 열려 있고 그곳으로 이동할 수 있음
        {
            StartCoroutine(SmoothMovement(end)); // SmoothMovement 코루틴 시작
            return true;
        }
        return false;

    }
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqRemainingDistance = (transform.position - end).sqrMagnitude; // sqrMagintude 벡터 길이 제곱, Magintude 벡터 길이
        while (sqRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            // Vector3.MoveTowards는 현재 포인트를 직선상에 목표 포인트로 이동
            // 이동 시킬 포인트는 newPosition
            // 현재 위치는 rb2D.position에서 목적지인 end로 이동, inverseMoveTime * Time.deltaTime 만큼 가까워짐
            rb2D.MovePosition(newPosition);
            sqRemainingDistance = (transform.position - end).sqrMagnitude; // 다시 계산
            yield return null; // 루프를 갱신하기 전에 다음 프레임을 기다림
        }
    }
    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component  // T가 컴포넌트 종류를 가리키게 함
    {
        // generic T는 막혔을 때, 유닛이 반응할 컴포넌트 타입을 가리키기 위해 사용
        // 적에 적용된 경우 상대는 플레이어, 플레이어의 경우 상대는 벽들, 플레이어가 벽을 공격하고 파괴할 수 있음
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit); // 이동 성공 시 true, 실패하면 false
        if (hit.transform == null) // move에서 라인 캐스트가 다른 것과 부딪치지 않았다면
            return; // 리턴
        T hitComponet = hit.transform.GetComponent<T>(); // 부딪혔다면 충돌한 오브젝트의 컴포넌트의 레퍼런스를 T타입의 컴포넌트에 할당
        if (!canMove && hitComponet != null) //움직이던 오브젝트가 막혔고, 상호작용할 수 있는 오브젝트와 충돌했을 때
            OnCantMove(hitComponet); 
    }
    protected abstract void OnCantMove<T>(T component)
        where T : Component;
    
}
