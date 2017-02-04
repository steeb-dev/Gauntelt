using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitLevel : MonoBehaviour {
    public MeshRenderer m_HighlightRenderer;
    private List<PlayerBehaviour> m_WaitingPlayers = new List<PlayerBehaviour>();
    public GameController m_GameController;
    public Color m_FinishColor;

    private void OnTriggerEnter(Collider other)
    {
        PlayerBehaviour pb = other.gameObject.GetComponent<PlayerBehaviour>();
        if (pb != null)
        {
            if (!m_WaitingPlayers.Contains(pb))
            {
                m_WaitingPlayers.Add(pb);
            }
        }
        if (m_WaitingPlayers.Count == m_GameController.m_NumActivePlayers)
        {
            m_HighlightRenderer.materials[0].color = m_FinishColor;
            StartCoroutine(m_GameController.FinishLevel(true));
        }
    }


    private void OnTriggerExit(Collider other)
    {
        PlayerBehaviour pb = other.gameObject.GetComponent<PlayerBehaviour>();
        if (pb != null)
        {
            if (!m_WaitingPlayers.Contains(pb))
            {
                m_WaitingPlayers.Remove(pb);
            }
        }
    }
}
