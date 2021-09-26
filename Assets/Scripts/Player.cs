using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{ 
    private int speed = 400;

    internal void MovePlayer(Vector3 targetPos)
    {
        StartCoroutine(Move(targetPos));
    }

    IEnumerator Move(Vector3 targetPos)
    {
        while (Mathf.Abs(targetPos.y - transform.position.y) > 0.05f || Mathf.Abs(targetPos.x - transform.position.x) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
            yield return null;
        }
        GameManager.Instance.GameStatus = GameStatusEnum.PlayerMoved;
    }
}
