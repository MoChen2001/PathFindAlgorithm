using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleControl : MonoBehaviour
{

    public Dictionary<Vector3, int> obstacleList;

    private ObstacleMV m_MV;    // 视图层和模型层

    [SerializeField]
    private int obstacleNumber = 10; // 场景中随机生成的障碍物数量

    [SerializeField]
    private int length = 5;
    
    public int Length { get => length; }

    void Awake()
    {
        obstacleList = new Dictionary<Vector3, int>();
        m_MV = gameObject.GetComponent<ObstacleMV>();

        CreateEdgeWall();
        CreateRandomObstacle();
    }

    // 生成周围的障碍围墙
    private void CreateEdgeWall()
    {

        for(int i = -length; i <= length; i++)
        {
            for(int j = -length; j <= length; j++)
            {
                if( i == length || i == -length || j == length || j == -length)
                {
                    Vector3 pos = new Vector3(i, 0.5f, j);
                    GameObject.Instantiate(m_MV.Obstacle_Prefab, pos, 
                        Quaternion.identity,gameObject.transform);
                    if(!obstacleList.ContainsKey(pos))
                    {
                        obstacleList.Add(pos, 1);
                    }
                }
            }
        }

    }


    // 随机生成场景中的障碍墙
    private void CreateRandomObstacle()
    {
        obstacleNumber = obstacleNumber > 20 ? 20 : obstacleNumber;

        while(obstacleNumber >= 0)
        {
            obstacleNumber--;
            int randomX = Random.Range(-length + 1, length - 1);
            int randomZ = Random.Range(-length + 1, length - 1);

            Vector3 pos = new Vector3(randomX, 0.5f, randomZ);    
            if (!obstacleList.ContainsKey(pos) && !(pos.x == 0 && pos.z == 0))
            {
                GameObject.Instantiate(m_MV.Obstacle_Prefab, pos,Quaternion.identity, 
                    gameObject.transform);
                obstacleList.Add(pos, 1);
            }
        }
    }


}
