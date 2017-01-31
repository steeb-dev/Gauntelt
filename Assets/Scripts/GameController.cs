using System;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public UnityEngine.UI.Text m_HealthText;
    public UnityEngine.UI.Text m_ScoreText;
    public UnityEngine.UI.Text m_DeadText;
    public UnityEngine.UI.Text m_StartText;
    public GameObject m_Rasta;

    public Camera m_DeathCam;
    public Camera m_MainCam;
    public PlayerBehaviour m_Player;
    private int m_CurrentLevel = 0;
    private bool m_AllDead =false;

    private void Update()
    {
        if (m_Player != null)
        {
            m_HealthText.text = "HP: " + m_Player.m_HP.ToString();
            m_ScoreText.text = "Points: " + m_Player.m_Score.ToString();

            if(m_Player.m_Score == 420 && m_Rasta != null && m_Rasta.activeSelf == false)
            {
                m_Rasta.SetActive(true);
            }

            if(m_Player.m_HP < 40)
            {
                m_HealthText.color = UnityEngine.Random.ColorHSV();
            }
        }
        if (m_Player.m_Dead)
        {
            AllDead();
        }

        if (Input.GetButtonDown("Restart"))
            UnityEngine.SceneManagement.SceneManager.LoadScene(m_CurrentLevel, UnityEngine.SceneManagement.LoadSceneMode.Single);

        if (m_AllDead)
        {
            m_DeadText.color = UnityEngine.Random.ColorHSV();
            m_HealthText.color = UnityEngine.Random.ColorHSV();
        }
    }

    private void Start()
    {
        StartCoroutine(KillChuck());

        m_DeathCam.enabled = false;
        m_MainCam.enabled = true;
    }

    IEnumerator KillChuck()
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

        }
    }
}
