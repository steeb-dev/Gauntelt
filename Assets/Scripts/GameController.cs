using System;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public UnityEngine.UI.Text m_P1HealthText;
    public UnityEngine.UI.Text m_P1ScoreText;

    public GameObject m_P2UI;

    public UnityEngine.UI.Text m_P2HealthText;
    public UnityEngine.UI.Text m_P2ScoreText;

    public UnityEngine.UI.Text m_DeadText;
    public UnityEngine.UI.Text m_StartText;
    public PlayerBehaviour m_P1;
    public PlayerBehaviour m_P2;
    public int m_NumActivePlayers = 2;

    public Camera m_DeathCam;
    public Camera m_MainCam;
    private int m_CurrentLevel = 0;
    public int m_LevelCount = 1;

    private bool m_AllDead =false;
    public float m_ExitTime = 1.5f;

    public IEnumerator FinishLevel()
    {
        StartCoroutine(m_P1.FinishAnim());
        if(m_NumActivePlayers == 2) StartCoroutine(m_P2.FinishAnim());
        yield return new WaitForSeconds(m_ExitTime);
        m_CurrentLevel++;
        if(m_CurrentLevel >= m_LevelCount)
        { m_CurrentLevel = 0; }
        UnityEngine.SceneManagement.SceneManager.LoadScene(m_CurrentLevel, UnityEngine.SceneManagement.LoadSceneMode.Single);

    }

    public PlayerBehaviour GetTargetPlayer(Vector3 position)
    {
        float P1Dist =  Vector3.Distance(position, m_P1.transform.position);
        float P2Dist = Vector3.Distance(position, m_P2.transform.position);

        if(P1Dist < P2Dist)
        { return m_P1; }
        else
        { return m_P2; }
    }

    private void Update()
    {
        if (m_P1 != null)
        {
            m_P1HealthText.text = "HP: " + m_P1.m_HP.ToString();
            m_P1ScoreText.text = "Points: " + m_P1.m_Score.ToString();

            if(m_P1.m_HP < 40)
            {
                m_P1HealthText.color = UnityEngine.Random.ColorHSV();
            }
        }

        if (m_NumActivePlayers == 2 && m_P2 != null)
        {
            m_P2HealthText.text = "HP: " + m_P2.m_HP.ToString();
            m_P2ScoreText.text = "Points: " + m_P2.m_Score.ToString();

            if (m_P2.m_HP < 40)
            {
                m_P2HealthText.color = UnityEngine.Random.ColorHSV();
            }
        }

        if (m_P1.m_Dead && (m_NumActivePlayers == 1 || m_P2.m_Dead))
        {
            AllDead();
        }

        if (Input.GetButtonDown("Restart"))
            UnityEngine.SceneManagement.SceneManager.LoadScene(m_CurrentLevel, UnityEngine.SceneManagement.LoadSceneMode.Single);

        if (m_AllDead)
        {
            m_DeadText.color = UnityEngine.Random.ColorHSV();
            m_P1HealthText.color = UnityEngine.Random.ColorHSV();
            m_P2HealthText.color = UnityEngine.Random.ColorHSV();
        }
    }

    private void Start()
    {
        StartCoroutine(KillSplash());

        m_DeathCam.enabled = false;
        m_MainCam.enabled = true;
        if (m_NumActivePlayers == 1) m_P2UI.SetActive(false);
        if (m_NumActivePlayers == 1) m_P2.gameObject.SetActive(false);
    }

    IEnumerator KillSplash()
    {
        yield return new WaitForSeconds(0.25f);
        m_StartText.gameObject.SetActive(false);

    }



    void AllDead()
    {
        if (!m_AllDead)
        {
            m_AllDead = true;
            m_DeathCam.enabled = true;
            m_MainCam.gameObject.GetComponent<ThirdPersonOrbitCam>().enabled = false;
            m_MainCam.enabled = false;
            m_DeadText.gameObject.SetActive(true);
            StartCoroutine(FinishLevel());
        }
    }
}
