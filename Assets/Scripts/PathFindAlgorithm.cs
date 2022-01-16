using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DistacneClacWay
{
    MANHATTAN,
    EULER
}


/// <summary>
///  ʹ�õ���ģʽ�Ĺ�����
/// </summary>
public class PathFindAlgorithm : MonoBehaviour
{
    public Dictionary<Vector3, int> obstacleList = new Dictionary<Vector3, int>();  // �ϰ����λ������
    public int length = 5;
    public float initSeconds = 0.1f;


    private static PathFindAlgorithm instance;
    public static PathFindAlgorithm Instance 
    {
        get
        {
            if (instance == null)
                instance = new PathFindAlgorithm();
            return instance;
        }
    }



    // ������ʱ��ʹ�õ���ƫ�ƾ���
    private Vector3[] offset = { new Vector3(-1,0,0),new Vector3(0,0,-1),
    new Vector3(1,0,0), new Vector3(0,0,1)};


    private bool isFinding = false;
    private GameObject pathObj = null;
    private GameObject pathParents = null;


    private Stack<Vector3> depthStack = new Stack<Vector3>();          // ������ȱ����ĸ���ջ
    private Queue<Vector3> breadthQueue = new Queue<Vector3>();        // ������ȱ����ĸ�������

    /// <summary>
    ///  ̰������ʽ�㷨�Ķ���
    /// </summary>
    private PriorityQueue greedyQueue = new PriorityQueue();
    private PriorityQueue dijkstraQueue = new PriorityQueue();
    private PriorityQueue aStarQueue = new PriorityQueue();



    private Dictionary<Vector3, int> arriveMap = new Dictionary<Vector3, int>(); // ��¼��������ĵ�


    private void Awake()
    {
        pathParents = GameObject.Find("PathParents");
        pathObj = Resources.Load<GameObject>("Path");
        instance = this;
    }



    /// <summary>
    /// �������·������
    /// </summary>
    public IEnumerator PathFindWithDepthFirst(Vector3 currentPos, Vector3 targetPos)
    {
        ClearPath();
        if (!isFinding)
        {
            isFinding = true;
            depthStack.Clear();
            depthStack.Push(currentPos);

            while (depthStack.Count != 0)
            {
                Vector3 curr = depthStack.Pop();
                // ����������
                if (curr.x >= length || curr.x <= -length ||
                curr.z >= length || curr.z <= -length ||
                obstacleList.ContainsKey(curr) ||
                arriveMap.ContainsKey(curr))
                {
                    continue;
                }
                // ����������
                if (curr == targetPos)
                {
                    depthStack.Clear();
                    break;
                }
                yield return new WaitForSeconds(initSeconds);
                GameObject.Instantiate(pathObj, curr, Quaternion.identity, pathParents.transform);
                arriveMap.Add(curr, 1);
                for (int i = 0; i < offset.Length; i++)
                {
                    depthStack.Push(offset[i] + curr);
                }
            }
            isFinding = false;
        }
    }



    /// <summary>
    ///  �������·������
    /// </summary>
    public IEnumerator PathFindWithBreadthFirst(Vector3 currentPos, Vector3 targetPos)
    {
        ClearPath();
        if (!isFinding)
        {
            isFinding = true;
            breadthQueue.Enqueue(currentPos);

            while (breadthQueue.Count != 0)
            {
                Vector3 curr = breadthQueue.Dequeue();
                // ����������
                if (curr.x >= length || curr.x <= -length ||
                curr.z >= length || curr.z <= -length ||
                obstacleList.ContainsKey(curr) ||
                arriveMap.ContainsKey(curr))
                {
                    continue;
                }
                // ����������
                if (curr == targetPos)
                {
                    breadthQueue.Clear();
                    break;
                }
                yield return new WaitForSeconds(initSeconds);
                GameObject.Instantiate(pathObj, curr, Quaternion.identity, pathParents.transform);
                arriveMap.Add(curr, 1);
                for (int i = 0; i < offset.Length; i++)
                {
                    breadthQueue.Enqueue(offset[i] + curr);
                }
            }
            isFinding = false;
        }
    }


    /// <summary>
    ///  GBFS �㷨��̰������ʽ�㷨
    ///  ���ǿ���ʹ�������پ������ŷ��������Ϊһ�����Ƶ���
    /// </summary>
    public IEnumerator PathFindWithGreedyBestFirst(Vector3 currentPos,Vector3 targetPos, DistacneClacWay way)
    {
        ClearPath();
        if (!isFinding)
        {
            isFinding = true;
            greedyQueue.Enqueue(DisCalc(currentPos, targetPos, way), currentPos);
            while (!greedyQueue.Empty())
            {
                Vector3 curr = greedyQueue.Dequeue();
                // ����������
                if (curr.x >= length || curr.x <= -length ||
                curr.z >= length || curr.z <= -length ||
                obstacleList.ContainsKey(curr) ||
                arriveMap.ContainsKey(curr))
                {
                    continue;
                }
                // ����������
                if (curr == targetPos)
                {
                    greedyQueue.Clear();
                    break;
                }
                yield return new WaitForSeconds(initSeconds);
                GameObject.Instantiate(pathObj, curr, Quaternion.identity, pathParents.transform);
                arriveMap.Add(curr, 1);
                for (int i = 0; i < offset.Length; i++)
                {
                    greedyQueue.Size();
                    Vector3 t = curr + offset[i];
                    greedyQueue.Enqueue(DisCalc(t, targetPos, way), t);
                }
            }
            isFinding = false;
        }
    }


    /// <summary>
    ///  Dijkstra �㷨
    /// </summary>
    public IEnumerator PathFindDijkstraFind(Vector3 currentPos, Vector3 targetPos)
    {
        ClearPath();
        if (!isFinding)
        {
            isFinding = true;
            Vector3 beginPos = currentPos;
            dijkstraQueue.Enqueue(DijkstraCostCalc(currentPos, beginPos), currentPos);
            while (!dijkstraQueue.Empty())
            {
                Vector3 curr = dijkstraQueue.Dequeue();
                // ����������
                if (curr.x >= length || curr.x <= -length ||
                curr.z >= length || curr.z <= -length ||
                obstacleList.ContainsKey(curr) ||
                arriveMap.ContainsKey(curr))
                {
                    continue;
                }
                // ����������
                if (curr == targetPos)
                {
                    dijkstraQueue.Clear();
                    break;
                }
                yield return new WaitForSeconds(initSeconds);
                GameObject.Instantiate(pathObj, curr, Quaternion.identity, pathParents.transform);
                arriveMap.Add(curr, 1);
                for (int i = 0; i < offset.Length; i++)
                {
                    dijkstraQueue.Size();
                    Vector3 t = curr + offset[i];
                    dijkstraQueue.Enqueue(DijkstraCostCalc(t, beginPos), t);
                }
            }
            isFinding = false;
        }
    }


    /// <summary>
    ///  A* �㷨
    /// </summary>
    public IEnumerator PathFindAStarFind(Vector3 currentPos, Vector3 targetPos, DistacneClacWay way)
    {
        ClearPath();
        if (!isFinding)
        {
            isFinding = true;
            Vector3 beginPos = currentPos;
            aStarQueue.Enqueue(AStarCostCalc(currentPos, beginPos, targetPos), currentPos);
            while (!aStarQueue.Empty())
            {
                Vector3 curr = aStarQueue.Dequeue();
                // ����������
                if (curr.x >= length || curr.x <= -length ||
                curr.z >= length || curr.z <= -length ||
                obstacleList.ContainsKey(curr) ||
                arriveMap.ContainsKey(curr))
                {
                    continue;
                }
                // ����������
                if (curr == targetPos)
                {
                    aStarQueue.Clear();
                    break;
                }
                yield return new WaitForSeconds(initSeconds);
                GameObject.Instantiate(pathObj, curr, Quaternion.identity, pathParents.transform);
                arriveMap.Add(curr, 1);
                for (int i = 0; i < offset.Length; i++)
                {
                    aStarQueue.Size();
                    Vector3 t = curr + offset[i];
                    aStarQueue.Enqueue(AStarCostCalc(t, beginPos, targetPos), t);
                }
            }
            isFinding = false;
        }
    }


    /// <summary>
    ///  ���·�����㷨
    /// </summary>
    private void ClearPath()
    {

        if(!isFinding)
        {
            arriveMap.Clear();
            Transform[] t = pathParents.GetComponentsInChildren<Transform>();
            for (int i = 1; i < t.Length; i++)
            {
                GameObject.Destroy(t[i].gameObject);
            }
        }
    }


    /// <summary>
    ///  �������
    /// </summary>
    /// <param name="way">����ļ��㷽ʽ�������پ������ŷ������</param>
    /// <returns></returns>
    private float DisCalc(Vector3 pos1, Vector3 pos2,DistacneClacWay way)
    {
        if(way == DistacneClacWay.MANHATTAN)
        {
            return Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y) + Mathf.Abs(pos1.z - pos2.z);
        }
        else if(way == DistacneClacWay.EULER)
        {
            return Vector3.Distance(pos2, pos1);
        }
        return 0;
    }


    /// <summary>
    ///  dijkstra �㷨�����ļ���
    ///  ������ Dijkstra �����ڴ�Ȩ�ص�ͼ������û�ƶ�һ��Ȩ�ض�����Ϊ1��
    ///  ʵ���Ͼ���ʹ�õ�ǰ�㵽������������پ�����Ϊ���ȶ��е�Ȩ�ؽ��д���
    /// </summary>
    private float DijkstraCostCalc(Vector3 pos1, Vector3 pos2)
    {
        return Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y) + Mathf.Abs(pos1.z - pos2.z);
    }


    /// <summary>
    ///  A* �㷨�����ļ���
    /// </summary>
    private float AStarCostCalc(Vector3 curr,Vector3 begin,Vector3 target)
    {
        float manhattn = Mathf.Abs(curr.x - begin.x) + Mathf.Abs(curr.y - begin.y) + Mathf.Abs(curr.z - begin.z);
        float euler = Vector3.Distance(curr, target);

        return 0.3f * manhattn + 0.7f * euler;
    }
}
