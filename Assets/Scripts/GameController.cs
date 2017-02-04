﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject[] m_PlayerPrefabs;
    public List<PlayerBehaviour> m_Players = new List<PlayerBehaviour>();

    public UnityEngine.UI.Text m_DeadText;
    public UnityEngine.UI.Text m_StartText;
   
    public Camera m_DeathCam;
    public Camera m_MainCam;
    public int m_CurrentLevel = 0;

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
        PlayerTracker.SetStats(this);

        if (m_CurrentLevel >= PlayerTracker.m_LevelCount)
        { m_CurrentLevel = 0; }

        UnityEngine.SceneManagement.SceneManager.LoadScene(m_CurrentLevel, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public PlayerBehaviour GetTargetPlayer(Vector3 position)
    {
        float minDist = 1000000f;
        PlayerBehaviour closestPlayer = m_Players[0];
        foreach (PlayerBehaviour pb in m_Players)
        {
            if (pb != null)
            {

                float distance = Vector3.Distance(position, pb.transform.position);

                if (distance < minDist)
                {
                    minDist = distance;
                    closestPlayer = pb;
                }
            }
        }
        return closestPlayer;
    }

    public float GetMinPlayerDistance(Vector3 position)
    {
        float minDist = 1000000f;
        foreach (PlayerBehaviour pb in m_Players)
        {
            if (pb != null)
            {
                float distance = Vector3.Distance(position, pb.transform.position);

                if (distance < minDist)
                {
                    minDist = distance;
                }
            }
        }
        return minDist;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayerTracker.m_NumActivePlayers = 1;
            Restart();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayerTracker.m_NumActivePlayers = 2;
            Restart();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayerTracker.m_NumActivePlayers = 3;
            Restart();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayerTracker.m_NumActivePlayers = 4;
            Restart();
        }

        if (m_Init && AllPlayersDead())
        {
            AllDead();
        }

        if (Input.GetButtonDown("Restart"))
        {
            Restart();
        }
    }

    IEnumerator RestartAfterDelay()
    {
        yield return new WaitForSeconds(m_ExitTime);
        Restart();
    }

    void Restart()
    {
        PlayerTracker.Reset();
        UnityEngine.SceneManagement.SceneManager.LoadScene(m_CurrentLevel, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    bool AllPlayersDead()
    {
        foreach (PlayerBehaviour pb in m_Players)
        {
            if (pb != null && !pb.m_Dead)
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
        for (int i = 0; i < PlayerTracker.m_NumActivePlayers; i++)
        {
            GameObject player = Instantiate(m_PlayerPrefabs[i]);
            player.transform.position = startSpawn.transform.position;
            m_Players.Add(player.GetComponent<PlayerBehaviour>());

        }
        PlayerTracker.Init(this);
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
            StartCoroutine(RestartAfterDelay());
        }
    }
}
