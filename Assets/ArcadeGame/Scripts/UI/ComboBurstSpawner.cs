using UnityEngine;

/// <summary>
/// Spawne un effet burst (shockwave, flash, explosion radiale) quand un palier combo est atteint.
/// Assigne un prefab burst par rank dans l'inspecteur, ou un burst générique si tu n'en as qu'un.
/// </summary>
public class ComboBurstSpawner : MonoBehaviour
{
    [Tooltip("Un prefab par rank (optionnel — peut être le même pour tous).")]
    [SerializeField] private GameObject[] _burstPerRank;

    [Tooltip("Position du spawn (généralement le joueur ou le centre de l'écran).")]
    [SerializeField] private Transform _spawnPoint;

    private void OnEnable()  => ComboManager.OnRankChanged += HandleRankChanged;
    private void OnDisable() => ComboManager.OnRankChanged -= HandleRankChanged;

    private void HandleRankChanged(int rankIndex, int combo)
    {
        if (_burstPerRank == null || _burstPerRank.Length == 0) return;

        // Si on a un burst par rank, on l'utilise, sinon on prend le dernier dispo
        int idx = Mathf.Min(rankIndex, _burstPerRank.Length - 1);
        GameObject burst = _burstPerRank[idx];

        if (burst == null) return;

        GameObject fx = Instantiate(burst, _spawnPoint.position, Quaternion.identity);

        // Auto-destroy après 2 secondes si pas de script dédié
        Destroy(fx, 2f);
    }
}
