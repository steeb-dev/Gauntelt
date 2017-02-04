using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public static class PlayerTracker
{
    public static int m_NumActivePlayers = 2;
    public static int m_LevelCount = 3;
    public static PlayerStats[] m_Stats;
    public static bool m_Init = false;

    public static void Reset()
    {
        m_Init = false;
    }

    public static void Init(GameController m_Contoller)
    {
        if (!m_Init)
        {
            m_Stats = new PlayerStats[PlayerTracker.m_NumActivePlayers];
            SetStats(m_Contoller);
            m_Init = true;
        }
        else
        {
            for (int i = 0; i < PlayerTracker.m_NumActivePlayers; i++)
            {
                m_Contoller.m_Players[i].m_HP = m_Stats[i].m_HP;
                m_Contoller.m_Players[i].m_Score = m_Stats[i].m_Score;  
            }
        }
    }

    public static void SetStats(GameController m_Controller)
    {
        for (int i = 0; i < PlayerTracker.m_NumActivePlayers; i++)
        {
            m_Stats[i] = new PlayerStats { m_HP = m_Controller.m_Players[i].m_HP, m_Score = m_Controller.m_Players[i].m_Score };
        }
    }
}

public struct PlayerStats
{
    public int m_HP;
    public int m_Score;
}