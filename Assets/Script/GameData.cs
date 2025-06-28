using UnityEngine;

public static class GameData
{
    // 棋盘尺寸
    public const int mapWidth = 11;
    public const int mapHeight = 11;

    // 游戏平衡参数
    public const int initialNewPieceCountdown = 3;
    public const int maxTurnForDoubleEnemySpawn = 20;

    // 棋子移动次数范围
    public static class MovementCounts
    {
        public static readonly Vector2Int Pawn = new Vector2Int(3, 5);      // 卒：3-5
        public static readonly Vector2Int Cannon = new Vector2Int(1, 2);    // 炮：1-2
        public static readonly Vector2Int Elephant = new Vector2Int(2, 5);  // 象：2-5
        public static readonly Vector2Int Horse = new Vector2Int(1, 4);     // 马：1-4
        public static readonly Vector2Int Rook = new Vector2Int(1, 1);      // 车：1
        public static readonly Vector2Int General = new Vector2Int(0, 0);   // 将：0（不能移动）
    }

    // 心情等级
    public enum MoodLevel
    {
        Painful = 0,    // 痛苦
        Sad = 1,        // 悲伤
        Neutral = 2,    // 平淡
        Joyful = 3,     // 开心
        Active = 4      // 积极
    }

    // 获取指定棋子类型的随机移动次数
    public static int GetRandomMovementCount(Piece.PieceType pieceType)
    {
        Vector2Int range = GetMovementRange(pieceType);
        return Random.Range(range.x, range.y + 1);
    }

    // 获取棋子类型的移动次数范围
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

    // 心情变化辅助方法
    public static MoodLevel ChangeMood(MoodLevel currentMood, int change)
    {
        int newMoodValue = (int)currentMood + change;

        // 限制在有效范围内
        newMoodValue = Mathf.Clamp(newMoodValue, (int)MoodLevel.Painful, (int)MoodLevel.Active);

        return (MoodLevel)newMoodValue;
    }

    // 检查心情是否为最高（积极）
    public static bool IsMoodAtMax(MoodLevel mood)
    {
        return mood == MoodLevel.Active;
    }

    // 检查心情是否为最低（痛苦）
    public static bool IsMoodAtMin(MoodLevel mood)
    {
        return mood == MoodLevel.Painful;
    }

    // 计算新棋子倒计时
    public static int CalculateNewPieceCountdown(int totalNewPiecesGained)
    {
        return totalNewPiecesGained + 3;
    }

    // 计算敌人生成数量
    public static int CalculateEnemySpawnCount(int currentTurn)
    {
        if (currentTurn <= maxTurnForDoubleEnemySpawn)
        {
            // 前20回合：每两回合增加一个
            return 1 + (currentTurn - 1) / 2;
        }
        else
        {
            // 20回合后：每回合增加一个
            return 1 + (maxTurnForDoubleEnemySpawn - 1) / 2 + (currentTurn - maxTurnForDoubleEnemySpawn);
        }
    }

    // 棋子表情emoji间隔时间计算
    public static float CalculateEmojiInterval(int totalPieces)
    {
        return 20f / (totalPieces + 2);
    }
}