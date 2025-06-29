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

    void Start()
    {
        agent = ant.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

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
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                    return;

                Vector3 targetPos = hit.point;
                targetPos.y = 0;

                currentFlag = Instantiate(flagPrefab, targetPos, Quaternion.identity);


                flagInstances.Add(currentFlag);
                campPositions.Add(targetPos);

                antController.ChangeAntCount(-1);
                agent.SetDestination(targetPos);

            }
        }

        if (currentFlag != null && Vector3.Distance(ant.position, currentFlag.transform.position) < 1f)
        {
            Vector3 pos = currentFlag.transform.position;
            Destroy(currentFlag);
            flagInstances.Remove(currentFlag);

            GameObject camp = Instantiate(campFlagPrefab, pos, Quaternion.identity);
            flagInstances.Add(camp);

            currentFlag = null;
            GameManager.Instance.PushSnapshot();
        }

        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Backspace))
        {
            GameManager.Instance.PopAndRestoreSnapshot();
        }
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
