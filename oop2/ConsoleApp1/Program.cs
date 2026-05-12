namespace CharacterBattle;

/// <summary>
/// Точка входа приложения WinForms.
/// Инициализирует графическую подсистему и запускает главное окно FormMain.
/// </summary>
static class Program
{
  /// <summary>
  /// Главная точка входа приложения.
  /// [STAThread] требуется для корректной работы WinForms.
  /// </summary>
  [STAThread]
  static void Main()
  {
    // Инициализация конфигурации WinForms (темы, стили и т.д.)
    ApplicationConfiguration.Initialize();
    // Запуск главной формы приложения
    Application.Run(new FormMain());
  }
  // Запуск проекта: dotnet run --project d:\oop1\oop2\ConsoleApp1\ConsoleApp1.csproj
}
