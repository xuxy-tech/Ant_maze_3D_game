using UnityEngine;

public class KeyItem : MonoBehaviour
{
    private bool collected = false;
    private bool hasKey = false;

    public bool GetCollected() => collected;
    public void SetCollected(bool value) => collected = value;

    public bool GetHasKey() => hasKey;
    public void SetHasKey(bool value) => hasKey = value;

    private void OnTriggerEnter(Collider other)
    {
        if (collected || !other.CompareTag("Ant")) return;

        collected = true;
        hasKey = true;
        gameObject.SetActive(false);

        Debug.Log("🔑 钥匙已获得！");
    }
}
