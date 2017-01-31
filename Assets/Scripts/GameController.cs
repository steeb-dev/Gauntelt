using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public UnityEngine.UI.Text m_HealthText;
    public UnityEngine.UI.Text m_DeadText;

    public Camera m_DeathCam;
    public Camera m_MainCam;
    public PlayerBehaviour m_Player;
    private int m_CurrentLevel = 0;
    private bool m_AllDead =false;

    private void Update()
    {
        m_HealthText.text = "HP: " + m_Player.m_HP.ToString();
        if(m_Player.m_Dead)
        {
            AllDead();
        }

        if (Input.GetButtonDown("Restart"))
            UnityEngine.SceneManagement.SceneManager.LoadScene(m_CurrentLevel, UnityEngine.SceneManagement.LoadSceneMode.Single);

        if(m_AllDead)
        { m_DeadText.color = UnityEngine.Random.ColorHSV(); }
        m_HealthText.color = UnityEngine.Random.ColorHSV(); 
    }

    private void Start()
    {
        m_DeathCam.enabled = false;
        m_MainCam.enabled = true;
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
