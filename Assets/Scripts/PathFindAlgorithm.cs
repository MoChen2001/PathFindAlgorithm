using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DistacneClacWay
{
    MANHATTAN,
    EULER
}


/// <summary>
///  使用单例模式的工具类
/// </summary>
public class PathFindAlgorithm : MonoBehaviour
{
    public Dictionary<Vector3, int> obstacleList = new Dictionary<Vector3, int>();  // 障碍物的位置链表
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



    // 遍历的时候使用到的偏移矩阵
    private Vector3[] offset = { new Vector3(-1,0,0),new Vector3(0,0,-1),
    new Vector3(1,0,0), new Vector3(0,0,1)};


    private bool isFinding = false;
    private GameObject pathObj = null;
    private GameObject pathParents = null;


    private Stack<Vector3> depthStack = new Stack<Vector3>();          // 深度优先遍历的辅助栈
    private Queue<Vector3> breadthQueue = new Queue<Vector3>();        // 广度优先遍历的辅助队列

    /// <summary>
    ///  贪心启发式算法的队列
    /// </summary>
    private PriorityQueue greedyQueue = new PriorityQueue();
    private PriorityQueue dijkstraQueue = new PriorityQueue();
    private PriorityQueue aStarQueue = new PriorityQueue();



    private Dictionary<Vector3, int> arriveMap = new Dictionary<Vector3, int>(); // 记录到到达过的点


    private void Awake()
    {
        pathParents = GameObject.Find("PathParents");
        pathObj = Resources.Load<GameObject>("Path");
        instance = this;
    }



    /// <summary>
    /// 深度优先路径搜索
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
                // 跳过的条件
                if (curr.x >= length || curr.x <= -length ||
                curr.z >= length || curr.z <= -length ||
                obstacleList.ContainsKey(curr) ||
                arriveMap.ContainsKey(curr))
                {
                    continue;
                }
                // 结束的条件
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
    ///  广度优先路径搜索
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
                // 跳过的条件
                if (curr.x >= length || curr.x <= -length ||
                curr.z >= length || curr.z <= -length ||
                obstacleList.ContainsKey(curr) ||
                arriveMap.ContainsKey(curr))
                {
                    continue;
                }
                // 结束的条件
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
    ///  GBFS 算法，贪婪启发式算法
    ///  我们可以使用曼哈顿距离或者欧拉距离作为一个控制的量
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
                // 跳过的条件
                if (curr.x >= length || curr.x <= -length ||
                curr.z >= length || curr.z <= -length ||
                obstacleList.ContainsKey(curr) ||
                arriveMap.ContainsKey(curr))
                {
                    continue;
                }
                // 结束的条件
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
    ///  Dijkstra 算法
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
                // 跳过的条件
                if (curr.x >= length || curr.x <= -length ||
                curr.z >= length || curr.z <= -length ||
                obstacleList.ContainsKey(curr) ||
                arriveMap.ContainsKey(curr))
                {
                    continue;
                }
                // 结束的条件
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
    ///  A* 算法
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
                // 跳过的条件
                if (curr.x >= length || curr.x <= -length ||
                curr.z >= length || curr.z <= -length ||
                obstacleList.ContainsKey(curr) ||
                arriveMap.ContainsKey(curr))
                {
                    continue;
                }
                // 结束的条件
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
    ///  清空路径的算法
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
    ///  计算距离
    /// </summary>
    /// <param name="way">距离的计算方式，曼哈顿距离或者欧拉距离</param>
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
    ///  dijkstra 算法的消耗计算
    ///  由于是 Dijkstra 适用于带权重的图，这里没移动一步权重都设置为1，
    ///  实际上就是使用当前点到出发点的曼哈顿距离作为优先队列的权重进行处理
    /// </summary>
    private float DijkstraCostCalc(Vector3 pos1, Vector3 pos2)
    {
        return Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y) + Mathf.Abs(pos1.z - pos2.z);
    }


    /// <summary>
    ///  A* 算法的消耗计算
    /// </summary>
    private float AStarCostCalc(Vector3 curr,Vector3 begin,Vector3 target)
    {
        float manhattn = Mathf.Abs(curr.x - begin.x) + Mathf.Abs(curr.y - begin.y) + Mathf.Abs(curr.z - begin.z);
        float euler = Vector3.Distance(curr, target);

        return 0.3f * manhattn + 0.7f * euler;
    }
}
