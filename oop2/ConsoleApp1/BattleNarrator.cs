using System;
// Вспомогательный класс логирования шагов боя в UI
namespace CharacterBattle
{
  /// <summary>
  /// Временный вывод шагов боя в UI (устанавливается на время SimulateBattle).
  /// </summary>
  public static class BattleNarrator
  {
    private static Action<string>? _sink;

    public static void SetSink(Action<string>? appendLine)
    {
      _sink = appendLine;
    }

    public static void Line(string message)
    {
      _sink?.Invoke(message);
    }
  }
}
