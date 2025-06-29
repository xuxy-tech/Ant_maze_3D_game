using UnityEngine;

public class Food : MonoBehaviour
{
    public int restoreAmount = 3;
    public AntController AntController;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;

        if (other.CompareTag("Ant"))
        {
            collected = true;
            AntController.ChangeAntCount(restoreAmount);
            gameObject.SetActive(false);
        }
    }

    public void SetCollected(bool value) => collected = value;
    public bool GetCollected() => collected;
}
