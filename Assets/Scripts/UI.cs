using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UI : MonoBehaviour
{
    //Start
    public GameObject info;
    public GameObject start;

    public ToggleGroup gender;
    public TextMeshProUGUI age;
    public TextMeshProUGUI nickname;

    //Game

    //Over
    public TextMeshProUGUI reactionTime;
    public TextMeshProUGUI errorRate;
    public TextMeshProUGUI reactionTimeVariability;
    public TextMeshProUGUI stroopIndex;

    //Pilot
    public TextMeshProUGUI[] time;
    public TextMeshProUGUI[] score;

    public TextMeshProUGUI resultGender;
    public TextMeshProUGUI resultAge;
    public TextMeshProUGUI resultName;

    public GameObject warning;


    public static UI Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        warning.SetActive(false);
        resultGender.text = "여성";
    }

    public void toggleF()
    {
        resultGender.text = "여성";
    }

    public void toggleM()
    {
        resultGender.text = "남성";
    }

    public bool SetInfo()
    {

        resultName.text = nickname.text;

        if (isInt(age.text))
        {
            resultAge.text = age.text;
            return true;
        }
        else
        {
            warning.SetActive(true);
            return false;
        }
    }

    public bool isInt(string target)
    {
        foreach(char c in target)
        {
            if (c >= '0' && c <= '9' || "\0\n \u200B\t".Contains(c))
                continue;
            
            return false;
        }
        return true;
    }

    public void result(int total, int correct)
    {
        Debug.Log(correct + " / " + total);
    }

    public void resultForPilot(int[] scores, float[] times)
    {
        Debug.Log(scores + " : " + times);

        //정보 세팅
        for (int i = 0; i < scores.Length; i++)
        {
            //시간 세팅
            time[i].text = times[i].ToString("F2");

            //정답 여부 세팅
            if (scores[i] == 1)
                score[i].text = "O";
            else
                score[i].text = "X";
        }
    }
}
