using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class AntController : MonoBehaviour
{
    [Header("Ant Count Settings")]
    public int antCount = 5;
    public TextMeshProUGUI antCountText;
    public NavMeshAgent navMeshAgent;

    void Start()
    {
        UpdateAntCountDisplay();
    }

    /// <summary>
    /// 修改蚂蚁数量，并更新 UI 显示
    /// </summary>
    public void ChangeAntCount(int delta)
    {
        antCount += delta;
        antCount = Mathf.Max(0, antCount);
        UpdateAntCountDisplay();
    }

    /// <summary>
    /// 设置蚂蚁数量（用于快照恢复）
    /// </summary>
    public void SetAntCount(int value)
    {
        antCount = Mathf.Max(0, value);
        UpdateAntCountDisplay();
    }
    public Vector3 GetDestination()
    {
        return GetComponent<NavMeshAgent>().destination;
    }

    /// <summary>
    /// 更新 UI 上显示的蚂蚁数量
    /// </summary>
    private void UpdateAntCountDisplay()
    {
        if (antCountText != null)
        {
            antCountText.text = $"antcount: {antCount}";
        }
    }
}
