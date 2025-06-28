using UnityEngine;

public static class GameData
{
    // ���̳ߴ�
    public const int mapWidth = 11;
    public const int mapHeight = 11;

    // ��Ϸƽ�����
    public const int initialNewPieceCountdown = 3;
    public const int maxTurnForDoubleEnemySpawn = 20;

    // �����ƶ�������Χ
    public static class MovementCounts
    {
        public static readonly Vector2Int Pawn = new Vector2Int(3, 5);      // �䣺3-5
        public static readonly Vector2Int Cannon = new Vector2Int(1, 2);    // �ڣ�1-2
        public static readonly Vector2Int Elephant = new Vector2Int(2, 5);  // ��2-5
        public static readonly Vector2Int Horse = new Vector2Int(1, 4);     // ��1-4
        public static readonly Vector2Int Rook = new Vector2Int(1, 1);      // ����1
        public static readonly Vector2Int General = new Vector2Int(0, 0);   // ����0�������ƶ���
    }

    // ����ȼ�
    public enum MoodLevel
    {
        Painful = 0,    // ʹ��
        Sad = 1,        // ����
        Neutral = 2,    // ƽ��
        Joyful = 3,     // ����
        Active = 4      // ����
    }

    // ��ȡָ���������͵�����ƶ�����
    public static int GetRandomMovementCount(Piece.PieceType pieceType)
    {
        Vector2Int range = GetMovementRange(pieceType);
        return Random.Range(range.x, range.y + 1);
    }

    // ��ȡ�������͵��ƶ�������Χ
    public static Vector2Int GetMovementRange(Piece.PieceType pieceType)
    {
        return pieceType switch
        {
            Piece.PieceType.Pawn => MovementCounts.Pawn,
            Piece.PieceType.Cannon => MovementCounts.Cannon,
            Piece.PieceType.Elephant => MovementCounts.Elephant,
            Piece.PieceType.Horse => MovementCounts.Horse,
            Piece.PieceType.Rook => MovementCounts.Rook,
            Piece.PieceType.General => MovementCounts.General,
            _ => MovementCounts.Pawn
        };
    }

    // ����仯��������
    public static MoodLevel ChangeMood(MoodLevel currentMood, int change)
    {
        int newMoodValue = (int)currentMood + change;

        // ��������Ч��Χ��
        newMoodValue = Mathf.Clamp(newMoodValue, (int)MoodLevel.Painful, (int)MoodLevel.Active);

        return (MoodLevel)newMoodValue;
    }

    // ��������Ƿ�Ϊ��ߣ�������
    public static bool IsMoodAtMax(MoodLevel mood)
    {
        return mood == MoodLevel.Active;
    }

    // ��������Ƿ�Ϊ��ͣ�ʹ�ࣩ
    public static bool IsMoodAtMin(MoodLevel mood)
    {
        return mood == MoodLevel.Painful;
    }

    // ���������ӵ���ʱ
    public static int CalculateNewPieceCountdown(int totalNewPiecesGained)
    {
        return totalNewPiecesGained + 3;
    }

    // ���������������
    public static int CalculateEnemySpawnCount(int currentTurn)
    {
        if (currentTurn <= maxTurnForDoubleEnemySpawn)
        {
            // ǰ20�غϣ�ÿ���غ�����һ��
            return 1 + (currentTurn - 1) / 2;
        }
        else
        {
            // 20�غϺ�ÿ�غ�����һ��
            return 1 + (maxTurnForDoubleEnemySpawn - 1) / 2 + (currentTurn - maxTurnForDoubleEnemySpawn);
        }
    }

    // ���ӱ���emoji���ʱ�����
    public static float CalculateEmojiInterval(int totalPieces)
    {
        return 20f / (totalPieces + 2);
    }
}