using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject startCanvas;
    public GameObject gameCanvas;
    public GameObject overCanvas;
    
    public Animator animator;

    public UI ui;
    public Stage stage;

    public Device d;

    public int roundCount = 10;

    //�̱���
    public static GameManager Instance { get; private set; }

    public void Awake()
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

        GameReady();
    }

    public void GameReady()
    {
        Debug.Log("�غ�");

        stage.retest();

        startCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        overCanvas.SetActive(false);
    }

    public void GameStart()
    {
        //if (ui.SetInfo()) //���� �Է��ϸ� ����
        //{
            StartCoroutine(WaitAndSkip()); // 대기
            //}
    }

    IEnumerator WaitAndSkip()
    {
        animator.SetBool("isSkip", true);
        yield return new WaitForSeconds(0.7f); // 대기
        
        // startCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        overCanvas.SetActive(false);

        stage.StartGame();
    }

    public void TestStart()
    {
        //if (ui.SetInfo()) //���� �Է��ϸ� ����
        //{
            Debug.Log("���Ϸ� ����");

            startCanvas.SetActive(false);
            gameCanvas.SetActive(true);
            overCanvas.SetActive(false);

            stage.StartTest();
        //}
    }

    public void GameOver(int totalQ, int correct, int[] scores, float[] times)
    {
        startCanvas.SetActive(false);
        gameCanvas.SetActive(false);
        overCanvas.SetActive(true);
        
        float totalScore = 0f;

        for (int i = 0; i < totalQ; i++)
        {
            if (scores[i] == 1)
            {
                float accuracy = (float)correct / totalQ * 100f; // 정답률 (%)
                float score = (accuracy / 20f) + (5f - times[i]);
                totalScore += score;
            }
            else
            {
                // 오답일 경우 점수 0점
                totalScore += 0f;
            }
        }

        Debug.Log("총 점수: " + totalScore.ToString("F2"));

        ui.result(totalQ, totalScore);
        //ui.resultForPilot(scores, times);
    }
}
