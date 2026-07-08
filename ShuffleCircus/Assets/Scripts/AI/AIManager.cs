using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI Opponent Manager: orchestrates opponent turns with board evaluation and move selection.
/// 
/// Phase 1: Turn orchestration & Reentrancy safety
/// Phases 2-4: Hooks for move generation, evaluation, and execution (implemented in later steps)
/// </summary>
public class AIManager : MonoBehaviour
{
    private const int MinDifficulty = 2;
    private const int MaxDifficulty = 3;
    private const float OpeningPhaseFillThreshold = 0.45f;

    // === INSPECTOR CONFIG ===
    [SerializeField] private float thinkDelaySeconds = 0.5f;
    [SerializeField] private float cardPlayDelaySeconds = 0.3f;
    
    // === REFERENCES (cached at Start) ===
    private GameplayManager gameplayManager;
    private HandManager handManager;
    private GameBoard playerBoard;
    private GameBoard opponentBoard;
    private int aiDifficultyLevel = MinDifficulty;
    
    // === STATE ===
    private bool isAIThinking = false;  // Reentrancy guard: prevent overlapping AI turns
    
    // === LIFECYCLE ===
    private void OnEnable()
    {
        EventManager.OnTurnEnded += HandleAITurnTriggered;
    }
    
    private void OnDisable()
    {
        EventManager.OnTurnEnded -= HandleAITurnTriggered;
        isAIThinking = false;  // Safety cleanup on disable
    }
    
    private void Start()
    {
        CacheReferences();
        ResolveDifficultyFromGameState();
    }
    
    private void OnDestroy()
    {
        // Ensure cleanup if scene changes or object destroyed mid-turn
        isAIThinking = false;
    }
    
    // === MAIN ENTRY POINT ===
    /// <summary>
    /// Called when EventManager.OnTurnEnded fires.
    /// Only acts if it's opponent's turn AND not already thinking.
    /// </summary>
    private void HandleAITurnTriggered()
    {
        if (IsMultiplayerMode()) return;
        if (GameManager.Instance.IsGameOver) return;

        // Only execute if it's the opponent's turn and we're not already thinking
        if (GameManager.Instance.IsPlayerTurn || isAIThinking)
        {
            return;
        }
        
        StartCoroutine(AIThinkAndPlayCoroutine());
    }
    
    // === ORCHESTRATION: Think & Play Loop ===
    private IEnumerator AIThinkAndPlayCoroutine()
    {
        isAIThinking = true;
        int aiCardsPlayedThisTurn = 0;
        
        try
        {
            if (IsMultiplayerMode()) yield break;

            // Brief thinking delay for UX
            yield return new WaitForSeconds(thinkDelaySeconds);

            if (IsMultiplayerMode() || GameManager.Instance.IsGameOver) yield break;
            
            // Play up to 4 cards this turn
            while (GameManager.Instance.CanPlayCardThisTurn() && isActiveAndEnabled)
            {
                if (IsMultiplayerMode() || (GameManager.Instance != null && GameManager.Instance.IsGameOver))
                {
                    yield break;
                }

                // Step 5: Get legal moves for current board state
                List<AIMove> legalMoves = GetLegalMoves();
                
                if (legalMoves.Count == 0)
                {
                    //Debug.Log("AI: No legal moves remaining this turn.");
                    break;
                }
                
                // Step 7-8: Evaluate and pick best move
                AIMove bestMove = EvaluateAndPickMoveByDifficulty(legalMoves);

                if (ShouldStopPlayingThisTurn(aiCardsPlayedThisTurn, (int)bestMove.evaluationScore))
                {
                    //Debug.Log($"AI: Choosing to hold after {aiCardsPlayedThisTurn} card(s) this turn.");
                    break;
                }
                
                //Debug.Log($"AI: Playing move {bestMove}");
                
                // Step 11-12: Execute the move
                if (!ExecuteMove(bestMove))
                {
                    break;
                }
                aiCardsPlayedThisTurn++;
                
                // Pacing delay between card plays for visual clarity
                yield return new WaitForSeconds(cardPlayDelaySeconds);
            }
            
            // Brief pause before ending turn
            yield return new WaitForSeconds(0.3f);
            
            if (isActiveAndEnabled && (GameManager.Instance == null || !GameManager.Instance.IsGameOver))
            {
                //Debug.Log("AI: Ending turn.");
                GameManager.Instance.EndTurn();
            }
        }
        finally
        {
            isAIThinking = false;
        }
    }
    
    // === PHASE 2: Get Legal Moves ===
    /// <summary>
    /// Returns all legal (card, slot) tuples the AI can play this turn.
    /// 
    /// Factors:
    /// - AI hand cards (from HandManager.GetHandCards())
    /// - Available opponent board slots (non-null, within bounds)
    /// - Board constraint: each slot must be unoccupied
    /// 
    /// Grid layout: 3 rows × 5 columns
    /// </summary>
    private List<AIMove> GetLegalMoves()
    {
        List<AIMove> legalMoves = new List<AIMove>();
        
        // Get AI's current hand
        List<CardData> aiHand = handManager.GetHandCards();
        
        if (aiHand == null || aiHand.Count == 0)
        {
            //Debug.Log("AI: No cards in hand.");
            return legalMoves;
        }
        
        int rows = opponentBoard.grid.GetLength(0);     // 3
        int columns = opponentBoard.grid.GetLength(1);  // 5
        
        // For each card in the AI's hand
        foreach (CardData card in aiHand)
        {
            if (card == null) continue;
            
            // For each slot on the opponent board
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    // Check if slot is empty (legal placement)
                    if (opponentBoard.GetCard(row, col) == null)
                    {
                        legalMoves.Add(new AIMove(row, col, card));
                    }
                }
            }
        }
        
        //Debug.Log($"AI: Found {legalMoves.Count} legal moves ({aiHand.Count} cards × empty slots)");
        
        return legalMoves;
    }
    
    // === PHASE 3: Evaluate & Pick Best ===
    /// <summary>
    /// Evaluates each candidate move and returns the best one.
    /// 
    /// Greedy strategy (v1):
    /// - For each move, simulate placing card at (row, col)
    /// - Net score = own row gain + opponent row loss from steal (same row, same rank)
    /// - Pick move with highest net score; random tie-breaking.
    /// </summary>
    private AIMove EvaluateAndPickMoveByDifficulty(List<AIMove> candidates)
    {
        if (candidates == null || candidates.Count == 0)
        {
            Debug.LogError("AI: EvaluateAndPickMoveByDifficulty called with no candidates.");
            return default;
        }

        if (aiDifficultyLevel <= 1)
        {
            // Easy: pick a random legal move.
            return candidates[UnityEngine.Random.Range(0, candidates.Count)];
        }

        List<AIMove> bestMoves = new List<AIMove>();
        List<AIMove> scoredMoves = new List<AIMove>(candidates.Count);
        int bestScore = int.MinValue;
        
        foreach (AIMove candidate in candidates)
        {
            int score = SimulateMove(candidate);
            
            AIMove scoredMove = candidate;
            scoredMove.evaluationScore = score;
            scoredMoves.Add(scoredMove);
            
            if (score > bestScore)
            {
                bestScore = score;
                bestMoves.Clear();
                bestMoves.Add(scoredMove);
            }
            else if (score == bestScore)
            {
                bestMoves.Add(scoredMove);
            }
        }

        if (aiDifficultyLevel == 2)
        {
            // Medium: choose randomly from the better half of scored moves.
            scoredMoves.Sort((a, b) => b.evaluationScore.CompareTo(a.evaluationScore));
            int selectionPoolSize = Mathf.Max(1, scoredMoves.Count / 2);
            return scoredMoves[UnityEngine.Random.Range(0, selectionPoolSize)];
        }
        
        // Hard (3): optimal play with random tie-breaking among equally scored moves.
        AIMove chosen = bestMoves[UnityEngine.Random.Range(0, bestMoves.Count)];
        //Debug.Log($"AI: Best move score={bestScore}, chose {chosen}");
        return chosen;
    }

    private void ResolveDifficultyFromGameState()
    {
        GameState state = GameManager.Instance != null ? GameManager.Instance.State : null;
        if (state == null)
        {
            aiDifficultyLevel = MaxDifficulty;
            Debug.LogWarning("AI: GameState not found. Falling back to hard difficulty (3).");
            return;
        }

        aiDifficultyLevel = Mathf.Clamp(state.AIDifficultyLevel, MinDifficulty, MaxDifficulty);
        //Debug.Log($"AI: Difficulty set to {aiDifficultyLevel}.");
    }

    private bool IsMultiplayerMode()
    {
        return GameManager.Instance != null
            && GameManager.Instance.State != null
            && GameManager.Instance.State.GameMode == GameMode.Multiplayer;
    }
    
    /// <summary>
    /// Simulates a move on snapshots of the board (no real board state is modified).
    /// Returns net score: own row gain + opponent row loss via steal.
    /// </summary>
    private int SimulateMove(AIMove move)
    {
        int row = move.row;
        int rank = move.card.RankValue;
        
        // --- Own board: score gain from placing the card ---
        CardData[] aiRow = GetRowSnapshot(opponentBoard, row);
        int aiScoreBefore = ScoringSystem.CalculateRowScore(aiRow);
        aiRow[move.col] = move.card;  // simulate placement (only on the copy)
        int aiScoreAfter = ScoringSystem.CalculateRowScore(aiRow);
        int ownGain = aiScoreAfter - aiScoreBefore;
        
        // --- Opponent board: score loss from steal (same row, same rank) ---
        CardData[] playerRow = GetRowSnapshot(playerBoard, row);
        int playerScoreBefore = ScoringSystem.CalculateRowScore(playerRow);
        
        for (int col = 0; col < playerRow.Length; col++)
        {
            if (playerRow[col] != null && playerRow[col].RankValue == rank)
            {
                playerRow[col] = null;  // simulate steal
            }
        }
        
        int playerScoreAfter = ScoringSystem.CalculateRowScore(playerRow);
        int opponentLoss = playerScoreBefore - playerScoreAfter;  // positive = opponent lost score

        int openingRankBias = GetOpeningLowCardBias(move.card);
        
        return ownGain + opponentLoss + openingRankBias;
    }

    private int GetOpeningLowCardBias(CardData card)
    {
        if (card == null || opponentBoard == null)
        {
            return 0;
        }

        int occupiedSlots = CountOccupiedSlots(opponentBoard);
        int totalSlots = opponentBoard.grid.GetLength(0) * opponentBoard.grid.GetLength(1);
        float fill = totalSlots > 0 ? (float)occupiedSlots / totalSlots : 1f;

        if (fill >= OpeningPhaseFillThreshold)
        {
            return 0;
        }

        // Early game: keep high cards slightly more often, prefer low cards.
        int rank = Mathf.Clamp(card.RankValue, 2, 14);
        int rawBias = Mathf.Clamp(8 - rank, -6, 6);
        float intensity = 1f - (fill / OpeningPhaseFillThreshold);

        float difficultyWeight = aiDifficultyLevel switch
        {
            1 => 0.5f,
            2 => 0.75f,
            _ => 1f
        };

        return Mathf.RoundToInt(rawBias * intensity * difficultyWeight);
    }

    private bool ShouldStopPlayingThisTurn(int cardsPlayedByAIThisTurn, int bestMoveScore)
    {
        if (cardsPlayedByAIThisTurn <= 0)
        {
            return false;
        }

        float stopChance = aiDifficultyLevel switch
        {
            1 => cardsPlayedByAIThisTurn switch
            {
                1 => 0.25f,
                2 => 0.45f,
                _ => 0.7f
            },
            2 => cardsPlayedByAIThisTurn switch
            {
                1 => 0.1f,
                2 => 0.25f,
                _ => 0.45f
            },
            _ => cardsPlayedByAIThisTurn switch
            {
                1 => 0.05f,
                2 => 0.12f,
                _ => 0.25f
            }
        };

        // If a move is very strong, AI is less likely to pass.
        if (bestMoveScore >= 20)
        {
            stopChance *= 0.4f;
        }
        else if (bestMoveScore <= 5)
        {
            stopChance *= 1.2f;
        }

        return UnityEngine.Random.value < Mathf.Clamp01(stopChance);
    }

    private int CountOccupiedSlots(GameBoard board)
    {
        if (board == null)
        {
            return 0;
        }

        int occupied = 0;
        int rows = board.grid.GetLength(0);
        int cols = board.grid.GetLength(1);
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (board.GetCard(row, col) != null)
                {
                    occupied++;
                }
            }
        }

        return occupied;
    }
    
    /// <summary>
    /// Returns a shallow copy of the given board row as a CardData array.
    /// Safe to modify without affecting the real board.
    /// </summary>
    private CardData[] GetRowSnapshot(GameBoard board, int row)
    {
        int columns = board.grid.GetLength(1);
        CardData[] snapshot = new CardData[columns];
        for (int col = 0; col < columns; col++)
        {
            snapshot[col] = board.GetCard(row, col);
        }
        return snapshot;
    }
    
    // === PHASE 4: Execute Move ===
    /// <summary>
    /// Executes a move on the opponent board:
    /// 1. Places card on board grid at (row, col)
    /// 2. Registers the play with GameManager (increments cardsPlayedThisTurn)
    /// 3. Fires CardDropped event to trigger steal logic
    /// 4. Updates hand tracking
    /// 5. Triggers UI refresh (BoardChanged event)
    /// 
    /// Implemented in Step 11-12.
    /// </summary>
    private bool ExecuteMove(AIMove move)
    {
        if (move.card == null)
        {
            Debug.LogError("AI: ExecuteMove called with null card.");
            return false;
        }
        
        // 1. Get the target CardSlot so we can reparent the card visually
        CardSlot targetSlot = opponentBoard.GetSlot(move.row, move.col);
        if (targetSlot == null)
        {
            Debug.LogError($"AI: No CardSlot found at ({move.row}, {move.col})");
            return false;
        }
        
        // 2. Remove from hand list BEFORE reparenting so turn-end cleanup ignores it
        handManager.RemoveCardFromHand(move.card);
        
        // 3. Reparent card to the slot (mirrors what OnDrop does)
        move.card.transform.SetParent(targetSlot.transform);
        move.card.transform.localPosition = Vector3.zero;
        move.card.transform.localRotation = Quaternion.identity;
        move.card.transform.localScale = Vector3.one;

        CardHover cardHover = move.card.GetComponent<CardHover>();
        if (cardHover != null)
        {
            cardHover.DisableHover();
        }

        CardDrag cardDrag = move.card.GetComponent<CardDrag>();
        if (cardDrag != null)
        {
            cardDrag.MarkDropped();
        }
        
        // 4. Update the logical board grid
        opponentBoard.PlaceCard(move.row, move.col, move.card);
        
        // 5. Register the play (increments cardsPlayedThisTurn)
        bool registered = GameManager.Instance.TryRegisterPlayedCard();
        if (!registered)
        {
            Debug.LogWarning("AI: Failed to register card play (over 4-card limit?)");
            return false;
        }

        handManager.HandDisplay.RevealCard(move.card.GetComponent<RectTransform>(), move.card.Identity);
        
        // 6. Fire event to trigger steal logic via GameplayManager
        EventManager.CardDropped(move.row, move.col, false);  // false = opponent played
        
        // 7. Trigger UI refresh (scores, deck/discard counts)
        EventManager.BoardChanged();
        
        //Debug.Log($"AI: Move executed at ({move.row}, {move.col})");
        return true;
    }
    
    // === REFERENCE CACHING ===
    private void CacheReferences()
    {
        gameplayManager = FindFirstObjectByType<GameplayManager>();
        if (gameplayManager == null)
        {
            Debug.LogError("AIManager: GameplayManager not found in scene.");
            return;
        }
        
        handManager = FindFirstObjectByType<HandManager>();
        if (handManager == null)
        {
            Debug.LogError("AIManager: HandManager not found in scene.");
            return;
        }
        
        playerBoard = gameplayManager.GetPlayerBoard();
        opponentBoard = gameplayManager.GetEnemyBoard();
        
        if (playerBoard == null || opponentBoard == null)
        {
            Debug.LogError("AIManager: Could not cache player/opponent boards from GameplayManager.");
            return;
        }
        
        Debug.Log("AIManager: References cached successfully.");
    }
}
