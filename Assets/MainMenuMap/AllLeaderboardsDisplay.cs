using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Affiche les 3 leaderboards côte à côte en 3 colonnes égales.
/// Tout est généré par code — pas besoin de ScrollView.
///
/// Setup Canvas :
///   _container = un RectTransform vide qui occupe toute la zone d'affichage.
///   Assigne _sectionTitlePrefab et _entryPrefab.
/// </summary>
public class AllLeaderboardsDisplay : MonoBehaviour
{
    [Header("Conteneur principal")]
    [Tooltip("RectTransform parent qui occupe la zone d'affichage (ex: un Panel plein écran)")]
    [SerializeField] private RectTransform _container;

    [Header("Prefabs")]
    [SerializeField] private TMP_Text _sectionTitlePrefab;
    [SerializeField] private TMP_Text _entryPrefab;

    [Header("Options")]
    [SerializeField] private int   _topN            = 10;
    [SerializeField] private float _columnPadding   = 20f;  // marge intérieure de chaque colonne
    [SerializeField] private float _entrySpacing     = 8f;   // espace entre les lignes
    [SerializeField] private float _titleBottomSpace = 16f;  // espace titre → première entrée

    [SerializeField] private string _titleArcade       = "⚔  ARCADE";
    [SerializeField] private string _titleRunner       = "🏃  RUNNER";
    [SerializeField] private string _titleGameAndWatch = "🎮  GAME & WATCH";

    private void OnEnable() => Refresh();

    public void Refresh()
    {
        // Vide le contenu précédent
        foreach (Transform child in _container)
            Destroy(child.gameObject);

        // Crée les 3 colonnes
        RectTransform col0 = CreateColumn(0);
        RectTransform col1 = CreateColumn(1);
        RectTransform col2 = CreateColumn(2);

        PopulateColumn(col0, _titleArcade,       new LeaderboardController().GetTopEntries(_topN));
        PopulateColumn(col1, _titleRunner,        new LeaderboardControllerRunner().GetTopEntries(_topN));
        PopulateColumn(col2, _titleGameAndWatch,  new LeaderboardControllerGameAndWatch().GetTopEntries(_topN));
    }

    // ── Création d'une colonne ───────────────────────────────────────────────

    /// <summary>Crée un RectTransform qui occupe exactement 1/3 du conteneur.</summary>
    private RectTransform CreateColumn(int index)
    {
        GameObject go = new GameObject($"Column_{index}", typeof(RectTransform));
        go.transform.SetParent(_container, false);

        RectTransform rt = go.GetComponent<RectTransform>();

        // Ancres : chaque colonne couvre 1/3 horizontal, toute la hauteur
        float anchorMinX = index       / 3f;
        float anchorMaxX = (index + 1) / 3f;

        rt.anchorMin = new Vector2(anchorMinX, 0f);
        rt.anchorMax = new Vector2(anchorMaxX, 1f);
        rt.offsetMin = new Vector2( _columnPadding,  _columnPadding);
        rt.offsetMax = new Vector2(-_columnPadding, -_columnPadding);

        // Séparateur vertical (ligne fine) entre colonnes 0-1 et 1-2
        if (index > 0)
            CreateVerticalDivider(index);

        return rt;
    }

    private void CreateVerticalDivider(int index)
    {
        GameObject go = new GameObject($"Divider_{index}", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(_container, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        float anchorX = index / 3f;

        rt.anchorMin = new Vector2(anchorX, 0.02f);
        rt.anchorMax = new Vector2(anchorX, 0.98f);
        rt.sizeDelta = new Vector2(1f, 0f); // 1px de large

        go.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.15f); // ligne subtile
    }

    // ── Remplissage d'une colonne ────────────────────────────────────────────

    private void PopulateColumn(RectTransform column, string title, List<LeaderboardEntry> entries)
    {
        float currentY = 0f; // commence en haut (on descend)

        // Titre
        TMP_Text titleText = Instantiate(_sectionTitlePrefab, column);
        PlaceText(titleText, column, ref currentY, _titleBottomSpace);
        titleText.text = title;

        // Entrées
        if (entries == null || entries.Count == 0)
        {
            TMP_Text empty = Instantiate(_entryPrefab, column);
            PlaceText(empty, column, ref currentY, _entrySpacing);
            empty.text = "— Aucun score —";
            return;
        }

        for (int i = 0; i < entries.Count; i++)
        {
            TMP_Text entry = Instantiate(_entryPrefab, column);
            PlaceText(entry, column, ref currentY, _entrySpacing);

            string medal = i == 0 ? "1" : i == 1 ? "2" : i == 2 ? "3" : $"{i + 1}.";
            entry.text = $"{medal}  {entries[i].PlayerName}  —  {entries[i].Score:D6}";
        }
    }

    /// <summary>
    /// Positionne un TMP_Text en haut de la colonne, puis avance currentY vers le bas.
    /// </summary>
    private void PlaceText(TMP_Text txt, RectTransform column, ref float currentY, float spacingAfter)
    {
        RectTransform rt = txt.GetComponent<RectTransform>();

        // Ancre en haut de la colonne
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot     = new Vector2(0.5f, 1f);

        // Force le recalcul pour connaître la hauteur préférée
        txt.ForceMeshUpdate();
        float height = txt.preferredHeight;

        rt.anchoredPosition = new Vector2(0f, -currentY);
        rt.sizeDelta        = new Vector2(0f, height);

        currentY += height + spacingAfter;
    }
}