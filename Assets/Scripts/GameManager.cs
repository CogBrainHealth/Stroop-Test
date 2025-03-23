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
            Debug.Log("����");
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
        Debug.Log("����");

        startCanvas.SetActive(false);
        gameCanvas.SetActive(false);
        overCanvas.SetActive(true);

        ui.result(totalQ, correct);
        //ui.resultForPilot(scores, times);
    }
}
