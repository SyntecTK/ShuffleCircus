using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Zentrale Stelle zum Erstellen und Befuellen von Karten-Views (Prefab-Instanzen mit CardData + Image).
/// Wird sowohl von HandDisplay als auch von DiscardDisplay (und anderen Kartenanzeigen) genutzt,
/// um doppelten Code zu vermeiden.
/// </summary>
public static class CardViewFactory
{
    /// <summary>
    /// Instanziiert das Karten-Prefab unter dem angegebenen Parent und weist Sprite/Identity zu.
    /// </summary>
    /// <param name="cardPrefab">Das Karten-Prefab (muss eine CardData-Komponente enthalten).</param>
    /// <param name="parent">Der Transform, unter dem die Karte erzeugt werden soll.</param>
    /// <param name="card">Die Kartenidentitaet (Rang/Farbe).</param>
    /// <param name="spriteDataBase">Datenbank fuer die Karten-Sprites.</param>
    /// <param name="showFace">Ob die Vorderseite gezeigt werden soll (true) oder der Kartenrücken (false).</param>
    /// <param name="interactable">Ob Hover-/Drag-Komponenten auf der Karte aktiv bleiben sollen (false fuer reine Anzeige-Karten).</param>
    public static RectTransform CreateCardView(RectTransform cardPrefab, Transform parent, CardIdentity card, CardSpriteDataBase spriteDataBase, bool showFace = true, bool interactable = true)
    {
        if (cardPrefab == null)
        {
            Debug.LogError("CardViewFactory: cardPrefab ist nicht zugewiesen.");
            return null;
        }

        RectTransform cardInstance = Object.Instantiate(cardPrefab, parent);
        ApplyCardVisual(cardInstance, card, spriteDataBase, showFace);

        if (!interactable)
        {
            SetInteractable(cardInstance, false);
        }

        return cardInstance;
    }

    /// <summary>
    /// Aktiviert/deaktiviert Hover- und Drag-Verhalten auf einer Karten-Instanz.
    /// Nuetzlich fuer reine Anzeige-Panels (z. B. Discard), die keine Interaktion benoetigen.
    /// </summary>
    public static void SetInteractable(RectTransform cardInstance, bool interactable)
    {
        if (cardInstance == null)
        {
            return;
        }

        CardHover hover = cardInstance.GetComponent<CardHover>();
        if (hover != null)
        {
            hover.enabled = interactable;
        }

        CardDrag drag = cardInstance.GetComponent<CardDrag>();
        if (drag != null)
        {
            drag.enabled = interactable;
        }
    }

    /// <summary>
    /// Weist einer bereits instanziierten Karte Identity und Sprite zu.
    /// </summary>
    public static void ApplyCardVisual(RectTransform cardInstance, CardIdentity card, CardSpriteDataBase spriteDataBase, bool showFace = true)
    {
        if (cardInstance == null)
        {
            return;
        }

        CardData cardData = cardInstance.GetComponent<CardData>();
        if (cardData == null)
        {
            Debug.LogWarning("CardViewFactory: Karten-Prefab hat keine CardData-Komponente.");
            return;
        }

        cardData.SetCard(card);
        cardInstance.name = $"Card_{card.rank}_of_{card.suit}";

        Image cardImage = cardInstance.GetComponentInChildren<Image>();
        if (cardImage == null || spriteDataBase == null)
        {
            return;
        }

        if (showFace)
        {
            Sprite sprite = spriteDataBase.GetSprite(card);
            if (sprite != null)
            {
                cardImage.sprite = sprite;
            }
        }
        else
        {
            cardImage.sprite = spriteDataBase.GetEnemyBackSprite();
        }
    }
}
