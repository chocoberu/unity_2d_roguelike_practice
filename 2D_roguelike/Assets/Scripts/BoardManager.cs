using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;


public class BoardManager : MonoBehaviour {

    [Serializable]
    public class Count
    {
        public int minium;
        public int maxium;

        public Count(int min,int max)
        {
            minium = min;
            maxium = max;
        }
    }
    public int columns = 8;
    public int rows = 8; // 8*8의 게임 보드
    public Count wallCount = new Count(5, 9); // 벽의 개수 min~max
    public Count foodCount = new Count(1, 5); // 음식 개수 min~max
    public GameObject exit; // 출구, 게임상에 하나만 존재
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder; // Hierarchy 정리용
    private List<Vector3> gridPositions = new List<Vector3>();
    // 게임판의 오브젝트들의 위치를 추적하기 위함

    void InitialiseList()
    {
        gridPositions.Clear();
        //6*6 배열 위치 추적을 위해 초기화
        //가장자리까지 벽이 생길경우 게임이 끝나지 못하므로
        for(int x = 1 ; x < columns - 1 ; x++)
        {
            for(int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }
    void BoardSetup() // 바깥벽과 게임보드의 바닥을 짓기 위해서 사용
    {
        boardHolder = new GameObject("Board").transform;
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)]; // 바닥 타일중 랜덤하게 고름
                if (x == -1 || x == columns || y == -1 || y == rows) // 바깥벽이라면
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                // 고른 프리팹을 toInstantiate에 넣고 (x,y,0)의 위치로 회전값 없이 인스턴스
                instance.transform.SetParent(boardHolder); // 새로 생성된 인스턴스의 부모를 boardHolder로

            }
        }
    }
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0,gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex); // 중복된 위치 선정을 막기 위해 제거
        return randomPosition;
    }
    void LayoutObjectRandom(GameObject[] tileArray,int minium,int maxium) // 선택한 장소에 실제로 타일을 소환
    {
        int objectCount = Random.Range(minium, maxium + 1);
        // 오브젝트를 얼마나 스폰할지 조정
        for(int i=0;i<objectCount;i++)
        {
            Vector3 randomPosition = RandomPosition(); // randomPosition을 함수로 지정
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            // tileArray에서 랜덤 타일을 선택
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
            // 선택한 타일을 해당 위치에 인스턴스화
        }
    }
   public void SetupScene(int level)
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectRandom(wallTiles, wallCount.minium, wallCount.maxium);
        LayoutObjectRandom(foodTiles, foodCount.minium, foodCount.maxium);
        LayoutObjectRandom(foodTiles, foodCount.minium, foodCount.maxium);
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }
}
