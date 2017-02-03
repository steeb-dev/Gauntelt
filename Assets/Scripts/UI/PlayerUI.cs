using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{

    public UnityEngine.UI.Text m_PlayerText;
    public UnityEngine.UI.Text m_HealthText;
    public UnityEngine.UI.Text m_ScoreText;
    public PlayerBehaviour m_Player;

    private void Update()
    {
        m_PlayerText.text = m_Player.m_PlayerPrefix;
        m_HealthText.text = "HP: " + m_Player.m_HP.ToString();
        m_ScoreText.text = "Score: " + m_Player.m_Score.ToString();
    }
}