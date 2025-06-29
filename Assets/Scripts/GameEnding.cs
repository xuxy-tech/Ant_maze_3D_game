using UnityEngine;

public class GameEnding : MonoBehaviour
{
    public GameObject successUI; // 通关成功时显示的 UI（可为空）
    public GameObject failUI;    // 没有钥匙时显示的 UI（可为空）

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ant"))
        {
            bool hasKey = GameManager.Instance.key.GetHasKey();

            if (hasKey)
            {
                Debug.Log("🎉 通关成功！");
                if (successUI != null) successUI.SetActive(true);
                // TODO: 添加暂停游戏、切换场景、展示动画等逻辑
            }
            else
            {
                Debug.Log("🚫 你还没有钥匙，不能通关！");
                if (failUI != null) failUI.SetActive(true);
            }
        }
    }
}
