using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("引用")]
    public FlagManager flagManager;
    public GameObject ant;
    public AntController antController;

    [Header("道具")]
    public Food food;
    public TelescopeItem telescope;
    public KeyItem key;

    private Stack<GameSnapshot> snapshotStack = new Stack<GameSnapshot>();

    private void Awake()
    {
        Instance = this;
    }

    public void PushSnapshot()
    {
        GameSnapshot snapshot = new GameSnapshot();
        snapshot.antPosition = ant.transform.position;
        snapshot.antCount = antController.antCount;
        snapshot.antDestination = antController.GetDestination();

        // Food 状态
        snapshot.foodActive = food.gameObject.activeSelf;
        snapshot.foodCollected = food.GetCollected();

        // Telescope 状态
        snapshot.telescopeActive = telescope.gameObject.activeSelf;
        snapshot.telescopeCollected = telescope.GetCollected();
        snapshot.cameraDistance = telescope.GetCameraDistance();

        // Key 状态
        snapshot.keyActive = key.gameObject.activeSelf;
        snapshot.keyCollected = key.GetCollected();
        snapshot.hasKey = key.GetHasKey();

        // 营地旗帜位置
        snapshot.campPositions = new List<Vector3>(flagManager.GetCampPositions());

        snapshotStack.Push(snapshot);
        Debug.Log("📸 快照已保存");
    }

    public void PopAndRestoreSnapshot()
    {
        if (snapshotStack.Count <= 1)
        {
            Debug.LogWarning("⛔ 已退回最初状态，无法继续回退！");
            return;
        }

        snapshotStack.Pop();
        GameSnapshot snapshot = snapshotStack.Peek();

        // 蚂蚁状态
        ant.transform.position = snapshot.antPosition;
        antController.SetAntCount(snapshot.antCount);
        // 恢复蚂蚁的目的地
        flagManager.RestoreDestination(snapshot.antDestination); 



        // Food 恢复
        food.gameObject.SetActive(snapshot.foodActive);
        food.SetCollected(snapshot.foodCollected);

        // Telescope 恢复
        telescope.gameObject.SetActive(snapshot.telescopeActive);
        telescope.SetCollected(snapshot.telescopeCollected);
        telescope.SetCameraDistance(snapshot.cameraDistance);

        // Key 恢复
        key.gameObject.SetActive(snapshot.keyActive);
        key.SetCollected(snapshot.keyCollected);
        key.SetHasKey(snapshot.hasKey);

        // 旗帜恢复
        flagManager.ClearAllFlags();
        foreach (var pos in snapshot.campPositions)
        {
            flagManager.PlaceCampAt(pos);
        }

        Debug.Log("⏪ 回退成功");
    }
}

[System.Serializable]
public class GameSnapshot
{
    public Vector3 antPosition;
    public Vector3 antDestination;
    public int antCount;

    public bool foodActive;
    public bool foodCollected;

    public bool telescopeActive;
    public bool telescopeCollected;
    public float cameraDistance;

    public bool keyActive;
    public bool keyCollected;
    public bool hasKey;

    public List<Vector3> campPositions;
}
