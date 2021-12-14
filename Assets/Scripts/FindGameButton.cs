using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
public class FindGameButton : MonoBehaviour
{
    public static Guid PlayerID;
    public static Guid redPlayer;
    public static Guid blackPlayer;
    public static Guid gameId;
    
    public void findGame()
    {
        if (GameObject.Find("Button").GetComponentInChildren<Text>().text == "Cancel Search")
        {
            var request = WebRequest.Create(String.Format("https://checker-game-matcher.herokuapp.com/api/delete/{0}", PlayerID));
            request.Method = "POST";
            request.ContentType = "application/json";
            request.GetResponse();
            SceneManager.LoadScene(0);
        }
        GameObject.Find("Button").GetComponentInChildren<Text>().text = "Cancel Search";
        PlayerID = Guid.NewGuid();
        InvokeRepeating("getMatch", 1f, 3f);
    }

    public void getMatch()
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(String.Format("https://checker-game-matcher.herokuapp.com/api/game/{0}", PlayerID));
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonResponse = reader.ReadToEnd();
            MatcherResponse matcherResponse = JsonUtility.FromJson<MatcherResponse>(jsonResponse);
            if (!String.IsNullOrEmpty(matcherResponse.gameId))
            {
                redPlayer = Guid.Parse(matcherResponse.player1);
                blackPlayer = Guid.Parse(matcherResponse.player2);
                gameId = Guid.Parse(matcherResponse.gameId);
                SceneManager.LoadScene(1);
            }
        }
        catch
        {
            SceneManager.LoadScene(0);        
        }
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
