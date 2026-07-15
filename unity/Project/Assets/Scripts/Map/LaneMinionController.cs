using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Controlador de minions en carril
/// Gestiona movimiento y interacción con torres
/// </summary>
public class LaneMinionController : MonoBehaviour
{
    [SerializeField] private Lane lane;
    [SerializeField] private int team;
    
    private List<MinionAI> _activeLaneMinions = new();
    
    private void Start()
    {
        // MinionManager notificará sobre minions spawneados
        MinionManager.Instance.OnMinionSpawned += OnMinionSpawned;
    }
    
    private void OnMinionSpawned(MinionAI minion)
    {
        // TODO: Determinar si el minion pertenece a este carril
        // if (minion.GetTeam() == team && IsInLane(minion.transform.position))
        // {
        //     _activeLaneMinions.Add(minion);
        // }
    }
    
    private bool IsInLane(Vector3 position)
    {
        // Verificar si posición está dentro del rango del carril
        return true;  // TODO: Implementar
    }
}
