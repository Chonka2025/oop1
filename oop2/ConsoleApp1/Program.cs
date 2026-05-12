namespace CharacterBattle;
// Точка входа WinForms: запускает FormMain.
static class Program
{
  [STAThread]
  static void Main()
  {
    ApplicationConfiguration.Initialize();
    Application.Run(new FormMain());
  }
  // Запуск проекта: dotnet run --project d:\oop1\oop2\ConsoleApp1\ConsoleApp1.csproj
}
