using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PathFindControl : MonoBehaviour
{
    private int length;                             // ����ĳ���
    private GameObject currentTarget;                      // ���õ�Ŀ����λ�ã�ÿ�θ��ĵ�ʱ�����Ҫ����
    private PlayerControl playerControl;            // ��ɫ�Ŀ��ƽű�
    private GameObject playerObj;                   // ��ɫ�������
    private Dictionary<Vector3, int> obstacleList;


    private Button depthBegin;
    private Button breadthBegin;
    private Button greedyBestBegin;
    private Button dijkstraBegin;
    private Button aStarBegin;
    private Dropdown calcDisWay;


    void Start()
    {
        currentTarget = GameObject.Find("TargetControl").GetComponent<TargetControl>().CurrTarget;

        obstacleList = GameObject.Find("ObstacleParents").GetComponent<ObstacleControl>().obstacleList;
        


        PathFindAlgorithm.Instance.obstacleList = obstacleList;


        length = GameObject.Find("ObstacleParents").GetComponent<ObstacleControl>().Length;
        PathFindAlgorithm.Instance.length = length;
        playerControl = GameObject.Find("PlayerControl").GetComponent<PlayerControl>();
        
        InitPlayerPos();


        depthBegin = GameObject.Find("Canvas/DepthFirstPathFind").GetComponent<Button>();
        depthBegin.onClick.AddListener(PathFindWithDepthFirst);


        breadthBegin = GameObject.Find("Canvas/BreadthFirstPathFind").GetComponent<Button>();
        breadthBegin.onClick.AddListener(PathFindWithBreadthFirst);

        greedyBestBegin = GameObject.Find("Canvas/Greedy Best First Search").GetComponent<Button>();
        greedyBestBegin.onClick.AddListener(PathFindWithGreedyBestFirst);

        dijkstraBegin = GameObject.Find("Canvas/Dijkstra Search").GetComponent<Button>();
        dijkstraBegin.onClick.AddListener(PathFindWithDijkstraFirst);

        aStarBegin = GameObject.Find("Canvas/AStarSearch").GetComponent<Button>();
        aStarBegin.onClick.AddListener(PathFindWithAStarFirst);


        calcDisWay = GameObject.Find("Canvas/CalcDisWay").GetComponent<Dropdown>();
    }



    private void PathFindWithDepthFirst()
    {
        Vector3 pos = new Vector3((int)playerObj.transform.position.x, 0.5f, (int)playerObj.transform.position.z);

        StartCoroutine(PathFindAlgorithm.Instance.PathFindWithDepthFirst(pos, currentTarget.transform.position));
    }


    private void PathFindWithBreadthFirst()
    {
        Vector3 pos = new Vector3((int)playerObj.transform.position.x, 0.5f, (int)playerObj.transform.position.z);

        StartCoroutine(PathFindAlgorithm.Instance.PathFindWithBreadthFirst(pos, currentTarget.transform.position));
    }


    private void PathFindWithGreedyBestFirst()
    {
        Vector3 pos = new Vector3((int)playerObj.transform.position.x, 0.5f, (int)playerObj.transform.position.z);
        StartCoroutine(PathFindAlgorithm.Instance.PathFindWithGreedyBestFirst(pos, 
            currentTarget.transform.position, GetCalcWay()));
    }


    private void PathFindWithDijkstraFirst()
    {
        Vector3 pos = new Vector3((int)playerObj.transform.position.x, 0.5f, (int)playerObj.transform.position.z);
        StartCoroutine(PathFindAlgorithm.Instance.PathFindDijkstraFind(pos, currentTarget.transform.position));
    }

    /// <summary>
    ///  A ��Ѱ·�㷨
    /// </summary>
    private void PathFindWithAStarFirst()
    {
        Vector3 pos = new Vector3((int)playerObj.transform.position.x, 0.5f, (int)playerObj.transform.position.z);
        StartCoroutine(PathFindAlgorithm.Instance.PathFindAStarFind(pos, 
            currentTarget.transform.position, GetCalcWay()));
    }


    /// <summary>
    ///  ��ʼ����ɫλ��
    /// </summary>
    private void InitPlayerPos()
    {
        playerObj = playerControl.Player;

        for (int i = -length + 1; i < length; i++)
        {
            for(int j = -length + 1; j < length; j++)
            {
                Vector3 pos = new Vector3(i, 0.5f, j);

                if(!obstacleList.ContainsKey(pos))
                {
                    playerObj.transform.position = pos;
                    return;
                }
            }
        }
    }



    private DistacneClacWay GetCalcWay()
    {
        if(calcDisWay.value == 0)
        {
            return DistacneClacWay.MANHATTAN;
        }
        else if(calcDisWay.value == 1)
        {
            return DistacneClacWay.EULER;
        }
        return DistacneClacWay.MANHATTAN;
    }


    
}
