using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using DefaultNamespace;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    private string gameId = FindGameButton.gameId.ToString();
    private string start;
    private string end;
    private LastMoveResponse lastMove;
    private bool myTurn = true;
    private string myColor = "RED";
    private GameObject checker;
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.Find("PlayerManager").GetComponent<CameraController>().isBlack)
        {
            myTurn = false;
            myColor = "BLACK";
            StartCoroutine(getLastMove());
        }
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void RegisterStart(string clickNum, GameObject checker)
    {
        if (!myTurn)
        {
            return;
        }
        end = null;
        start = clickNum;
        this.checker = checker;
    }
    
    public void RegisterEnd(string clickNum)
    {
        if (!myTurn)
        {
            return;
        }
        end = clickNum;
        if (start != null)
        {
            //Call the server to register a move.
            var postData = "{\"gameId\": \"" + gameId + "\", \"start\": \"" + start + "\",\"end\":\"" + end + "\"}";
            var data = Encoding.ASCII.GetBytes(postData);
            var request = WebRequest.Create(String.Format("https://checker-game-manager.herokuapp.com/api/play/move"));
            string result;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;
            Debug.Log("Sent: "+ System.Text.Encoding.Default.GetString(data));
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            var response = (HttpWebResponse)request.GetResponse();
            
            
            try
            {
                
                StreamReader streamReader = new StreamReader(response.GetResponseStream(), true);
                try
                {
                    result = streamReader.ReadToEnd();
                }
                finally
                {
                    streamReader.Close();
                }
                
            }
            finally
            {
                response.Close();
            }
            MoveResponse moveResponse = JsonUtility.FromJson<MoveResponse>(result);
            Debug.Log("Recieved: " + result);

            if (Boolean.Parse(moveResponse.legalMove))
            {
                isLegalMove(moveResponse);
            }

            if (Boolean.Parse(moveResponse.gameOver))
            {
                isGameOver();
            }
        }
        
        
        start = null;
        end = null;
        checker = null;
    }

    private void isLegalMove(MoveResponse moveResponse)
    {
        //Move our checker to the end square.
        checker.transform.position = new Vector3(
            GameObject.Find(end).transform.position.x,
            checker.transform.position.y,
            GameObject.Find(end).transform.position.z);
        
        //If we jumped a checker then Destroy the checker we jumped.
        if (Int32.Parse(moveResponse.jump) != -1)
        {
            var checkers = GameObject.FindGameObjectsWithTag("Checker");
            foreach (var foundChecker in checkers)
            {
                if ((foundChecker.transform.position.x == GameObject.Find(moveResponse.jump).transform.position.x) &&
                    (foundChecker.transform.position.z == GameObject.Find(moveResponse.jump).transform.position.z))
                {
                    Destroy(foundChecker);
                    Debug.Log("Destroyed: " + moveResponse.jump);
                    break;
                }
            }
        }
        //If there are no additional jumps then switch sides and wait for the oponent to move.
        if (!Boolean.Parse(moveResponse.additionalJumps))
        {
            myTurn = !myTurn;
            StartCoroutine(getLastMove());
        }
    }
    private void isGameOver()
    {
        
    }

    private IEnumerator getLastMove()
    {
        LastMoveResponse move = callGetLastMove(gameId);
        while(move.color == this.myColor)
        {
            yield return new WaitForSeconds(2f);
            move = callGetLastMove(gameId);
        }
        lastMove = move;
        var checkers = GameObject.FindGameObjectsWithTag("Checker");
        foreach (var foundChecker in checkers)
        {
            if((foundChecker.transform.position.x == GameObject.Find(Convert.ToString(lastMove.start)).transform.position.x) &&
               (foundChecker.transform.position.z == GameObject.Find(Convert.ToString(lastMove.start)).transform.position.z))
            {
                foundChecker.transform.position = new Vector3(
                    GameObject.Find(Convert.ToString(lastMove.end)).transform.position.x,
                    foundChecker.transform.position.y,
                    GameObject.Find(Convert.ToString(lastMove.end)).transform.position.z);
                
            }

        }
        if(lastMove.jump != -1)
        {
            checkers = GameObject.FindGameObjectsWithTag("Checker");
            foreach (var foundChecker in checkers)
            {
                if ((foundChecker.transform.position.x == GameObject.Find(Convert.ToString(lastMove.jump)).transform.position.x) &&
                    (foundChecker.transform.position.z == GameObject.Find(Convert.ToString(lastMove.jump)).transform.position.z))
                {
                    Destroy(foundChecker);
                    Debug.Log("Destroyed: " + lastMove.jump);
                    break;
                }
            }
        }
        myTurn = !myTurn;
        Debug.Log("LastMoveResponse: (" + lastMove.start + ", " + lastMove.end + ", " + lastMove.jump + ", " + lastMove.color + ")");
    }

    private LastMoveResponse callGetLastMove(string gameId)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format("https://checker-game-manager.herokuapp.com/api/play/lastmove/{0}", gameId));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            return JsonUtility.FromJson<LastMoveResponse>(reader.ReadToEnd());
        } catch (Exception e)
        {
            Debug.Log("Exception: " + e.ToString() + " caught in callGetLastMove().");
            return null;
        }
        
    }
}

