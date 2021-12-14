using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackSquareInputController : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnMouseUpAsButton()
    {
        var checkers = GameObject.FindGameObjectsWithTag("Checker");
        foreach (var checker in checkers)
        {
            if ((checker.transform.position.x == gameObject.transform.position.x) &&
                (checker.transform.position.z == gameObject.transform.position.z))
            {
                GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<MoveController>().RegisterStart(gameObject.name, checker);
                return;
            }
        }

        GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<MoveController>().RegisterEnd(gameObject.name);
    }
}
