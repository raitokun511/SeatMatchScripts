using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Citizen : MonoBehaviour
{
    public ColorType ColorType;
    public Vector2Int Position;
    public Vector2Int Rotation;
    public CitizenState State;
    public Animator animator;
    public GameObject citizenModel;
    public bool IsSeated = false;

    Chair parentChair;
    bool isMoving = false;
    Vector3 targetMove;
    List<Vector2> listPath;

    // Start is called before the first frame update
    void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (isMoving)
        {
            GM.lastTimeMove = Time.time;
            //if (listPath.Count > 0)
            {
                if ((transform.position - targetMove).magnitude > 0.1f)
                {
                    Vector3 unitMove = (targetMove - transform.position).normalized * 0.025f;
                    transform.position += unitMove;
                    //Debug.Log($"MOVE {unitMove}");
                }
                else// if (listPath.Count > 0)
                {
                    if (listPath.Count == 0)
                    {
                        citizenModel.transform.rotation = Quaternion.Euler(0, 90, 0);
                        animator.Play(Constant.CITIZEN_ANIMATOR_SIT);
                        isMoving = false;
                        StartCoroutine(DoAfter(0.5f, () =>
                        {
                            transform.parent = parentChair.transform;
                            parentChair.LockedByCitizen = false;
                        }));
                        return;
                    }
                    //(int, int) node = listPath.Last();
                    Vector2 node = listPath.Last();
                    //Debug.Log($"MoveTo {node.Item1},{node.Item2} = {listPath.Count}");
                    Vector3 nextMove = new Vector3(node.x, transform.position.y, node.y);
                    MoveRotate(targetMove, nextMove);
                    targetMove = nextMove;
                    listPath.RemoveAt(listPath.Count - 1);
                }
            }
        }
    }

    public void MoveTo(int xtarget, int ytarget, List<Vector2> dictPath, Chair targetChair)
    {
        listPath = dictPath;
        Vector2 node = listPath.Last();
        targetMove = new Vector3(node.x, transform.position.y, node.y);
        MoveRotate(transform.position, targetMove);
        Debug.Log($"MoveTo Begin {node.x},{node.y} = {listPath.Count} Rotate: {transform.position} vs {targetMove}");
        listPath.RemoveAt(dictPath.Count - 1);
        parentChair = targetChair;
        isMoving = true;
        animator.Play(Constant.CITIZEN_ANIMATOR_RUN);
    }

    IEnumerator Move()
    {

        yield return null;
    }
    void MoveRotate(Vector3 current, Vector3 target)
    {
        if (current.x > target.x + 0.5f)
        {
            citizenModel.transform.rotation = Quaternion.Euler(0, -90, 0);//.LookRotation(new Vector3(0, 90, 0));
        }
        else if (current.x + 0.5f < target.x)
        {
            citizenModel.transform.rotation = Quaternion.Euler(0, 90, 0);//.LookRotation(new Vector3(0, -90, 0));
        }
        if (current.z > target.z + 0.5f)
        {
            citizenModel.transform.rotation = Quaternion.Euler(0, 180, 0);//.LookRotation(new Vector3(0, 0, 0));
        }
        if (current.z + 0.5f < target.z)
        {
            citizenModel.transform.rotation = Quaternion.Euler(0, 0, 0);//.LookRotation(new Vector3(0, 180, 0));
        }
    }
    IEnumerator DoAfter(float time, Action task)
    {
        yield return new WaitForSeconds(time);
        task.Invoke();
    }
}
