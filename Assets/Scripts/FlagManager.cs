using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlagManager : MonoBehaviour
{
    public GameObject flagPrefab;
    public GameObject campFlagPrefab;
    public Transform ant;
    public AntController antController;
    private GameObject currentFlag;
    private NavMeshAgent agent;

    private List<GameObject> flagInstances = new List<GameObject>();
    private List<Vector3> campPositions = new List<Vector3>();
    private NavMeshPath currentPath;
    private int pathIndex = 0;
    private bool isMoving = false;
    private float currentSpeed = 0f;
    public float maxPathLength = 50f;
    public float visibleRadius = 5f; 


    public float acceleration;     // 每秒增加的速度
    public float maxSpeed;         // 速度上限

    private float CalculatePathLength(NavMeshPath path)
    {
        float length = 0f;
        if (path.corners.Length < 2)
            return length;

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            length += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }
        return length;
    }

    void Start()
    {
        agent = ant.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.enabled = false; // 禁用 NavMeshAgent 的自动移动

        // 原始营地
        Vector3 startCamp = new Vector3(0, 0, 0);
        GameObject camp = Instantiate(campFlagPrefab, startCamp, Quaternion.identity);
        flagInstances.Add(camp);
        campPositions.Add(startCamp);
        GameManager.Instance.PushSnapshot();
    }

    void Update()
    {
        

        if (Input.GetMouseButtonDown(0) && currentFlag == null && antController.antCount > 0)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 🧱 如果点击的是墙，就不放旗
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                    return;

                // 🧭 获取目标位置并对齐 Y 值
                Vector3 targetPos = hit.point;
                targetPos.y = 0;
                float clickDistance = Vector3.Distance(ant.position, targetPos);
                if (clickDistance > visibleRadius)
                {
                    Debug.Log("点击超出可见范围");
                    Destroy(currentFlag); // 删除刚创建的flag（如果你已经放了）
                    currentFlag = null;
                    return;
                }

                // 🏳️ 创建旗帜对象
                currentFlag = Instantiate(flagPrefab, targetPos, Quaternion.identity);
                flagInstances.Add(currentFlag);
                campPositions.Add(targetPos);
                antController.ChangeAntCount(-1);

                // 🧠 计算路径但不移动
                currentPath = new NavMeshPath();
                NavMesh.CalculatePath(ant.position, targetPos, NavMesh.AllAreas, currentPath);

                if (currentPath.status == NavMeshPathStatus.PathComplete)
                {
                    float pathLength = CalculatePathLength(currentPath);
                    if (pathLength > maxPathLength)
                    {
                        Debug.LogWarning("路径过长（" + pathLength.ToString("F2") + " > " + maxPathLength + "），取消寻路");
                        Destroy(currentFlag);
                        flagInstances.Remove(currentFlag);
                        campPositions.RemoveAt(campPositions.Count - 1);
                        currentFlag = null;
                    }
                    else
                    {
                        isMoving = true;
                        pathIndex = 0;
                    }
                }
                else
                {
                    Debug.LogWarning("路径计算失败！");
                    Destroy(currentFlag); // 清理失败的旗帜
                    flagInstances.Remove(currentFlag);
                    campPositions.RemoveAt(campPositions.Count - 1);
                    currentFlag = null;
                }

            }
        }
        // 🖱️ 右键取消最近的旗帜 + 停止蚂蚁
        if (Input.GetMouseButtonDown(1))
        {
            if (flagInstances.Count > 0)
            {
                // 1️⃣ 删除最新旗帜
                GameObject lastFlag = flagInstances[flagInstances.Count - 1];
                if (isMoving == true)
                {
                    Destroy(lastFlag);
                    flagInstances.RemoveAt(flagInstances.Count - 1);
                    campPositions.RemoveAt(campPositions.Count - 1);
                }
                

                // 2️⃣ 停止蚂蚁移动
                isMoving = false;
                currentSpeed = 0f;
                currentPath = null;
                pathIndex = 0;

                // 3️⃣ 清空当前目标旗帜引用
                if (lastFlag == currentFlag)
                    currentFlag = null;
            }
        }

        // 🐜 蚂蚁按路径移动（匀速）
        if (isMoving && currentPath != null && currentPath.corners.Length > 0)
        {
            Vector3 targetPoint = currentPath.corners[pathIndex];
            Vector3 direction = (targetPoint - ant.position).normalized;

            // 匀加速逻辑
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxSpeed); 

            // 移动蚂蚁
            ant.position += direction * currentSpeed * Time.deltaTime;


            // 可选：转向面向前方
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                ant.rotation = Quaternion.Slerp(ant.rotation, targetRotation, 10f * Time.deltaTime);
            }

            // 到达当前路径点，切换下一个
            if (Vector3.Distance(ant.position, targetPoint) < 0.1f)
            {
                pathIndex++;
                if (pathIndex >= currentPath.corners.Length)
                {
                    isMoving = false;
                    OnAntReachedDestination(); // 到达终点
                }
            }
        }

        // ⏪ 回退操作（按 R 或 Backspace）
        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Backspace))
        {
            GameManager.Instance.PopAndRestoreSnapshot();
        }
    }
    void OnAntReachedDestination()
    {
        if (currentFlag == null) return;

        Vector3 pos = currentFlag.transform.position;
        Destroy(currentFlag);
        flagInstances.Remove(currentFlag);
        int cost = Random.Range(1, 5); // 注意：上限是“开区间”，所以写5才包含4
        antController.ChangeAntCount(-cost);
        isMoving = false;
        currentSpeed = 0f;
        currentPath = null;
        pathIndex = 0;

        GameObject camp = Instantiate(campFlagPrefab, pos, Quaternion.identity);
        flagInstances.Add(camp);
        currentFlag = null;

        GameManager.Instance.PushSnapshot();
    }
    public List<Vector3> GetCampPositions() => new List<Vector3>(campPositions);

    public void ClearAllFlags()
    {
        foreach (var flag in flagInstances)
        {
            Destroy(flag);
        }
        flagInstances.Clear();
        campPositions.Clear();
        currentFlag = null;
    }

    public void PlaceCampAt(Vector3 pos)
    {
        GameObject camp = Instantiate(campFlagPrefab, pos, Quaternion.identity);
        flagInstances.Add(camp);
        campPositions.Add(pos);
    }
    public void RestoreDestination(Vector3 destination)
    {
        agent.SetDestination(destination); // 恢复目的地
    }

}
