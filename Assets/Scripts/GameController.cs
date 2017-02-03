using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject[] m_PlayerPrefabs;
    public List<PlayerBehaviour> m_Players = new List<PlayerBehaviour>();

    public UnityEngine.UI.Text m_DeadText;
    public UnityEngine.UI.Text m_StartText;

    public int m_NumActivePlayers = 2;

    public Camera m_DeathCam;
    public Camera m_MainCam;
    private int m_CurrentLevel = 0;
    public int m_LevelCount = 1;

    private bool m_AllDead =false;
    public float m_ExitTime = 1.5f;
    private bool m_Init = false;

    public IEnumerator FinishLevel()
    {
        foreach (PlayerBehaviour pb in m_Players)
        {
            StartCoroutine(pb.FinishAnim());
        }
        yield return new WaitForSeconds(m_ExitTime);
        m_CurrentLevel++;
        if(m_CurrentLevel >= m_LevelCount)
        { m_CurrentLevel = 0; }
        UnityEngine.SceneManagement.SceneManager.LoadScene(m_CurrentLevel, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public PlayerBehaviour GetTargetPlayer(Vector3 position)
    {
        float minDist = 1000000f;
        PlayerBehaviour closestPlayer = m_Players[0];
        foreach (PlayerBehaviour pb in m_Players)
        {
            float distance = Vector3.Distance(this.transform.position, pb.transform.position);

            if (distance < minDist)
            {
                minDist = distance;
                closestPlayer = pb;
            }
        }
        return closestPlayer;
    }

    private void Update()
    {

        if (m_Init && AllPlayersDead())
        {
            AllDead();
        }

        if (Input.GetButtonDown("Restart"))
            UnityEngine.SceneManagement.SceneManager.LoadScene(m_CurrentLevel, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    bool AllPlayersDead()
    {
        foreach (PlayerBehaviour pb in m_Players)
        {
            if (!pb.m_Dead)
            { return false; }
        }
        return true;
    }

    private void Awake()
    {
        StartCoroutine(KillSplash());

        m_DeathCam.enabled = false;
        m_MainCam.enabled = true;
        GameObject startSpawn = GameObject.FindGameObjectWithTag("Spawn");
        for (int i = 0; i < m_NumActivePlayers; i++)
        {
            GameObject player = Instantiate(m_PlayerPrefabs[i]);
            player.transform.position = startSpawn.transform.position;
            m_Players.Add(player.GetComponent<PlayerBehaviour>());

        }
        m_Init = true;
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
