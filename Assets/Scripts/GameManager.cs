using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject startCanvas;
    public GameObject gameCanvas;
    public GameObject overCanvas;

    public UI ui;
    public Stage stage;

    public Device d;

    public int roundCount = 10;

    //싱글톤
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
        Debug.Log("준비");

        stage.retest();

        startCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        overCanvas.SetActive(false);
    }

    public void GameStart()
    {
        //if (ui.SetInfo()) //나이 입력하면 시작
        //{
            Debug.Log("시작");

            startCanvas.SetActive(false);
            gameCanvas.SetActive(true);
            overCanvas.SetActive(false);

            stage.StartGame();
        //}
    }

    public void TestStart()
    {
        //if (ui.SetInfo()) //나이 입력하면 시작
        //{
            Debug.Log("파일럿 시작");

            startCanvas.SetActive(false);
            gameCanvas.SetActive(true);
            overCanvas.SetActive(false);

            stage.StartTest();
        //}
    }

    public void GameOver(int totalQ, int correct, int[] scores, float[] times)
    {
        Debug.Log("종료");

        startCanvas.SetActive(false);
        gameCanvas.SetActive(false);
        overCanvas.SetActive(true);

        ui.result(totalQ, correct);
        //ui.resultForPilot(scores, times);
    }
}
