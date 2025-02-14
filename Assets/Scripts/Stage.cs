using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Stage:MonoBehaviour
{
    public GameManager gm;

    //상수
    public const float questTime = 3f; //문제 제시 후 카운트다운
    const float TimerSize = 1600f / 10f; //바의 원래 사이즈 1600f,  전체 시간 10f 

    //데이터
    StroopData[] data; //Scriptable 오브젝트

    //카드
    public int cardCount = 4; //현재 스테이지의 카드 수
    public GameObject[] cardPanel;

    public Card[] list; //현재 스테이지의 카드 오브젝트들

    //문제 관리
    public TextMeshProUGUI stage;
    public RectTransform timerBar;

    int roundFlag = 0;
    int totalQuest = 0; //전체 문제 수
    int thisQuestNum = 0; //현재 문제 순서
    int answer = -1; //현재 정답

    //수집 데이터
    int correct = 0; //정답 수

    //파일럿
    bool pilotFlag = false;

    int[] score = new int[10];
    float[] time = new float[10];
    float tempTime = 0f;

    List<PilotData> pilotData = new List<PilotData>();  

    class PilotData
    {
        public int[] cardData;
        public bool played = false;
        public questOption option = questOption.TEXT;

        public PilotData(int[] c, bool p, questOption q)
        {
            this.cardData = c;
            this.played = p;
            this.option = q;
        }
    }

    //UI
    public TextMeshProUGUI questIntroduce;
    public TextMeshProUGUI countDown;

    //싱글톤
    public static Stage Instance { get; private set; }

    enum questOption
    {
        TEXT,
        IMAGE
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }

        //Scriptable 오브젝트 모두 읽어오기. (Resource/Card)
        data = Resources.LoadAll<StroopData>("Card");
        timerBar.gameObject.SetActive(false);
        pilotDummy();
    }

    void pilotDummy()
    {
        //정답 카드번호, 각 카드의 text, image 번호
        var t1 = new PilotData(new int[] { 1, 3, 2, 2, 3}, false, questOption.TEXT);
        var t2 = new PilotData(new int[] { 0, 3, 4, 1, 3 }, false, questOption.IMAGE);
        var t3 = new PilotData(new int[] { 2, 2, 1, 2, 3, 1, 2, 4, 4 }, false, questOption.TEXT);
        var t4 = new PilotData(new int[] { 2, 1, 4, 2, 3, 3, 2, 1, 2 }, false, questOption.TEXT);
        var t5 = new PilotData(new int[] { 3, 3, 4, 2, 6, 1, 1, 6, 2 }, false, questOption.IMAGE);
        var t6 = new PilotData(new int[] { 1, 3, 4, 2, 1, 1, 6, 4, 2 }, false, questOption.IMAGE);
        var t7 = new PilotData(new int[] { 5, 3, 4, 2, 2, 1, 6, 3, 1, 1, 6, 6, 3}, false, questOption.TEXT);
        var t8 = new PilotData(new int[] { 5, 3, 4, 5, 2, 5, 1, 3, 4, 1, 5, 2, 1}, false, questOption.TEXT);
        var t9 = new PilotData(new int[] { 4, 2, 4, 1, 0, 6, 3, 4, 3, 3, 1, 5, 5}, false, questOption.IMAGE);
        var t10 = new PilotData(new int[] { 3, 2, 0, 3, 0, 2, 2, 1, 3, 0, 2, 6, 6}, false, questOption.IMAGE);

        pilotData.Add(t1);
        pilotData.Add(t2);
        pilotData.Add(t3);
        pilotData.Add(t4);
        pilotData.Add(t5);
        pilotData.Add(t6);
        pilotData.Add(t7);
        pilotData.Add(t8);
        pilotData.Add(t9);
        pilotData.Add(t10);
    }

    void Update()
    {
        tempTime += Time.deltaTime;

        if (timerBar.gameObject.activeSelf)
        {
            float size = (10f - tempTime) * TimerSize;
            timerBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
        }
    }

    void cardRead() //카드 오브젝트 읽어오기
    {
        for (int i = 0; i < cardPanel.Length; i++)
            cardPanel[i].SetActive(false);

        Transform t;
        
        t = cardActive();

        list = new Card[cardCount];

        for (int i = 0; i < cardCount; i++)
        {
            list[i] = t.GetChild(i).GetComponent<Card>();
        }
    }

    Transform cardActive() //오브젝트 풀링 - 다른 카드 판은 비활성화
    {
        switch (cardCount) //case 카드 수, 그에 맞게 세팅된 카드판의 인덱스
        {
            case 2:
                cardPanel[0].SetActive(true);
                return cardPanel[0].transform;
            case 4:
                cardPanel[1].SetActive(true);
                return cardPanel[1].transform;
            case 6:
                cardPanel[2].SetActive(true);
                return cardPanel[2].transform;
            default:
                cardPanel[1].SetActive(true);
                Debug.Log("카드 수 세팅 바람");
                return cardPanel[1].transform;
        }
    }

    public void StartGame()
    {
        roundFlag++;

        switch (roundFlag)
        {
            case 1:
                startRound(2, 2);
                break;
            case 2:
                startRound(4, 4);
                break;
            case 3:
                startRound(6, 4);
                break;
            default:
                gm.GameOver(totalQuest, correct, score, time);
                break;
        }
    }

    public void startRound(int card, int stage)
    {
        cardCount = card;
        cardRead();

        totalQuest += stage;
        startQuest();
    }

    public void startQuest()
    {
        stage.text = (thisQuestNum + 1).ToString() + "/10";

        //문제가 남았으면 다음 문제
        if (thisQuestNum < totalQuest)
        {
            int option = Random.Range(0, System.Enum.GetValues(typeof(questOption)).Length);
            quest(option);
        }
        else //문제가 안 남았다면 게임 종료
            StartGame();
    }

    private void quest(int option)
    {
        //답이 될 값
        int correctData = Random.Range(0, data.Length);

        //답이 될 카드
        answer = Random.Range(0, list.Length);

        if (option == (int)questOption.TEXT) //일치하는 글자 맞추기
        {
            //문제 출제
            questIntroduce.text = data[correctData].text + " 글자를 찾아주세요!";

            //정답 카드 세팅
            list[answer].setCard(data[correctData].text, data[unequalData(correctData)].image);

            //함정 카드 선택
            int imageFake = unequalAnswer(answer);

            //오답 카드 텍스트 세팅
            for (int i = 0; i < list.Length; i++)
            {
                if (answer == i) //정답 카드는 이미 세팅됨.
                    continue;
                else //오답 카드 세팅
                {
                    //텍스트 세팅 - 답안이랑 달라야 함.
                    string text = data[unequalData(correctData)].text;

                    //이미지 세팅 - 답안과 같은 것 하나, 나머지 랜덤
                    Sprite image;

                    if (i == imageFake)
                        image = data[correctData].image;
                    else
                        image = data[Random.Range(0, data.Length)].image;

                    list[i].setCard(text, image);
                }
            }
        }
        else if (option == (int)questOption.IMAGE) //일치하는 이미지 맞추기
        {
            //문제 출제
            questIntroduce.text = data[correctData].text + " 이미지를 찾아주세요!";

            //정답 카드 세팅
            list[answer].setCard(data[unequalData(correctData)].text, data[correctData].image);

            //오답 카드 텍스트 세팅
            for (int i = 0; i < list.Length; i++)
            {
                if (answer == i) //정답 카드는 이미 세팅됨.
                    continue;
                else //오답 카드 세팅
                {
                    //오브젝트 세팅 -  답안이랑 달라야 함.
                    Sprite image = data[unequalData(correctData)].image;

                    //텍스트 세팅 - 답안과 같은 것 하나, 나머지 랜덤
                    string text;
                    int textFake = unequalAnswer(answer);

                    if (i == textFake)
                        text = data[correctData].text;
                    else
                        text = data[Random.Range(0, data.Length)].text;

                    list[i].setCard(text, image);
                }
            }
        }
        else
        {
            Debug.Log("아직 추가되지 않은 문제 옵션.");
        }

        StartCoroutine(questTimer());
    }

    public void StartTest()
    {
        pilotFlag = true;

        roundFlag++;

        switch (roundFlag)
        {
            case 1:
                testRound(2, 2);
                break;
            case 2:
                testRound(4, 4);
                break;
            case 3:
                testRound(6, 4);
                break;
            default:
                gm.GameOver(totalQuest, correct, score, time);
                pilotFlag=false;
                break;
        }
    }

    public void testRound(int card, int stage)
    {
        cardCount = card;
        cardRead();

        totalQuest += stage;
        startTestQuest();
    }

    public void startTestQuest()
    {
        stage.text = (thisQuestNum + 1).ToString() + "/10";

        if (thisQuestNum < totalQuest)
        {
            int i = 0;

            switch (cardCount)
            {
                case 2:
                    do { i = Random.Range(0, 2); }
                    while (pilotData[i].played);
                    break;
                case 4:
                    do { i = Random.Range(2, 6); }
                    while (pilotData[i].played);
                    break;
                case 6:
                    do { i = Random.Range(6, 10); }
                    while (pilotData[i].played);
                    break;
                default:
                    Debug.Log("그런 거 없다");
                    break;
            }

            Debug.Log("문제 번호: " + i.ToString());

            pilotData[i].played = true;
            testQuest(pilotData[i].cardData, pilotData[i].option);
        }
        else
            StartTest();
    }

    private void testQuest(int[] questData, questOption option)
    {
        if (questData.Length != (list.Length * 2 + 1))
        {
            Debug.Log("잘못된 퀘스트 데이터");
            return;
        }

        //questData 할당
        answer = questData[0]; //정답 번호

        for (int i = 0; i < list.Length; i++) //카드 리스트
        {
            string text = data[questData[i * 2 + 1]].text;
            Sprite image = data[questData[i * 2 + 2]].image;

            list[i].setCard(text, image);

            if (i == answer)
                if (option == questOption.TEXT)
                   questIntroduce.text = text + " 글자를 찾아주세요!";
                else if (option == questOption.IMAGE)
                    questIntroduce.text = data[questData[i * 2 + 2]].text + " 이미지를 찾아주세요";
        }

        StartCoroutine(questTimer());
    }

    private IEnumerator questTimer()
    {
        //카운트 ui 세팅
        for (int i = 0; i < cardPanel.Length; i++)
            cardPanel[i].SetActive(false);

        countDown.gameObject.SetActive(true);

        //카운트 다운
        float second = questTime;

        while (second >= 0f)
        {
            second -= Time.deltaTime;
            countDown.text = Mathf.CeilToInt(second).ToString();
            yield return null;
        }

        //게임 ui 세팅
        countDown.gameObject.SetActive(false);
        tempTime = 0f;
        timerBar.gameObject.SetActive(true);
        cardActive();
    }

    public void Choice(int num)
    {
        time[thisQuestNum] = tempTime; //초기화는 startQuest에서
        timerBar.gameObject.SetActive(false);

        if (answer == num)
        {
            Debug.Log("정답");
            score[thisQuestNum] = 1;

            correct++;
        }
        else
        {
            Debug.Log("오답");
            score[thisQuestNum] = 0;
        }

        thisQuestNum++; //문제 번호 

        if (pilotFlag)
            startTestQuest();
        else
            startQuest();
    }

    public void retest()
    {
        //문제
        roundFlag = 0;
        totalQuest = 0; //전체 문제 수
        thisQuestNum = 0; //현재 문제 순서

        //수집 데이터
        correct = 0; //정답 수
        pilotFlag = false;
    }

    //데이터 중 정답이 아닌 번호 생성
    int unequalData(int correct)
    {
        int temp;
        do { temp = Random.Range(0, data.Length); }
        while (temp == correct);

        return temp;
    }

    //카드 중 정답이 아닌 번호 생성
    int unequalAnswer(int correct)
    {
        int temp;
        do { temp = Random.Range(0, list.Length);}
        while (temp == correct);

        return temp;
    }

    //읽어온 데이터 모두 출력
    void testPrint()
    {
        for (int i = 0; i < data.Length; i++){
            Debug.Log("number: " + data[i].number + " // text: " + data[i].text + " // image: " + data[i].image);
        }
    }
}
