using System;
using System.Drawing;
using System.Windows.Forms;

namespace CharacterBattle
{
  public class FormCharacter : Form
  {
    // Поля формы (доступны во всех методах)
    private readonly ICharacterRepository _repository; // Ссылка на репозиторий для сохранения/обновления персонажей
    private Character? _character; // Текущий редактируемый/создаваемый персонаж (null при создании)
    private readonly bool _isEditMode; // Флаг режима: true = редактирование, false = создание

    // Основные элементы управления (используются в обоих режимах)
    private ComboBox cmbType = null!; // Выпадающий список типа персонажа (Seeker/Templar)
    private TextBox txtName = null!; // Поле ввода имени персонажа
    private NumericUpDown nudLevel = null!; // Поле ввода уровня
    private NumericUpDown nudResource = null!; // Поле ввода энергии/веры
    private Button btnSave = null!; // Кнопка сохранения
    private Button btnCancel = null!; // Кнопка отмены
    private Label lblName = null!; // Подпись к полю имени
    private Label lblLevel = null!; // Подпись к полю уровня
    private Label lblResource = null!; // Подпись к полю ресурса (Энергия/Вера)

    // Элементы вкладки "Основное" (режим редактирования)
    private TabControl? tabControl; // Вкладки: Основное, Бой и реген, Класс
    private NumericUpDown? nudHealth; // Текущее здоровье персонажа
    private Label? lblMaxHealthInfo; // Отображение максимального HP по уровню
    private NumericUpDown? nudBaseDamage; // Базовый урон
    private NumericUpDown? nudRegenAmount; // Количество HP за тик регенерации
    private NumericUpDown? nudRegenInterval; // Интервал регенерации в секундах

    // Элементы вкладки "Бой и реген" и подсказок
    private Label? lblMeleeStats; // Метка с расчётом силы и ловкости
    private Label? lblClassKind; // Метка типа класса (Искатель/Храмовник)

    // Элементы панели Seeker (вкладка "Класс")
    private NumericUpDown? nudStrength; // Сила персонажа
    private NumericUpDown? nudAgility; // Ловкость персонажа
    private NumericUpDown? nudDodge; // Шанс уклонения (%)
    private CheckBox? chkStealth; // Флаг скрытности
    private NumericUpDown? nudStealthSec; // Оставшееся время скрытности (сек)

    // Элементы панели Templar (вкладка "Класс")
    private NumericUpDown? nudBlock; // Шанс блока (%)
    private NumericUpDown? nudArmor; // Значение брони
    private NumericUpDown? nudHolyPower; // Святая сила
    private CheckBox? chkImmobilized; // Флаг обездвиживания (эгида)

    // Панели классов для отображения специфичных атрибутов
    private Panel? panelSeeker; // Панель с атрибутами Искателя
    private Panel? panelTemplar; // Панель с атрибутами Храмовника

    /// Свойство для получения персонажа после закрытия формы (режим создания)
    public Character Character => _character ?? throw new InvalidOperationException();

    /// Конструктор формы создания/редактирования персонажа
    /// repository - репозиторий для сохранения персонажа
    /// character - персонаж для редактирования (null = режим создания)
    public FormCharacter(ICharacterRepository repository, Character? character = null)
    {
      _repository = repository;
      _character = character;
      _isEditMode = character != null;
      InitializeComponent();
      if (_character != null)
        LoadCharacterData();
    }

    /// Точка входа для построения UI - определяет какой режим использовать
    private void InitializeComponent()
    {
      if (!_isEditMode)
      {
        BuildCreateUi();
        return;
      }
      BuildEditUi();
    }

    /// Строит интерфейс для режима создания нового персонажа
    /// Включает: выбор типа (Seeker/Templar), имя, уровень, начальный ресурс
    private void BuildCreateUi()
    {
      Text = "Добавление персонажа";
      Size = new Size(400, 300);
      StartPosition = FormStartPosition.CenterParent;
      FormBorderStyle = FormBorderStyle.FixedDialog;
      MaximizeBox = false;
      MinimizeBox = false;
      cmbType = new ComboBox
      {
        Location = new Point(130, 20),
        Size = new Size(220, 25),
        DropDownStyle = ComboBoxStyle.DropDownList
      };
      cmbType.Items.AddRange(new[] { "Seeker (Искатель)", "Templar (Храмовник)" });
      cmbType.SelectedIndex = 0;

      lblName = new Label { Text = "Имя:", Location = new Point(20, 60), Size = new Size(100, 25) };
      txtName = new TextBox { Location = new Point(130, 60), Size = new Size(220, 25) };

      lblLevel = new Label { Text = "Уровень:", Location = new Point(20, 100), Size = new Size(100, 25) };
      nudLevel = new NumericUpDown
      {
        Location = new Point(130, 100),
        Size = new Size(220, 25),
        Minimum = 1,
        Maximum = 1000,
        Value = 1
      };

      lblResource = new Label { Text = "Энергия/Вера:", Location = new Point(20, 140), Size = new Size(100, 25) };
      nudResource = new NumericUpDown
      {
        Location = new Point(130, 140),
        Size = new Size(220, 25),
        Minimum = 0,
        Maximum = 100,
        Value = 80
      };

      btnSave = new Button
      {
        Text = "Сохранить",
        Location = new Point(180, 200),
        Size = new Size(100, 35),
        BackColor = Color.LightGreen
      };
      btnSave.Click += (_, _) => BtnSave_Click();

      btnCancel = new Button
      {
        Text = "Отмена",
        Location = new Point(290, 200),
        Size = new Size(80, 35),
        BackColor = Color.LightGray
      };
      btnCancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

      cmbType.SelectedIndexChanged += (_, _) =>
      {
        lblResource.Text = cmbType.SelectedIndex == 0 ? "Энергия:" : "Вера:";
      };

      Controls.AddRange(new Control[] {
        cmbType, lblName, txtName, lblLevel, nudLevel, lblResource, nudResource, btnSave, btnCancel
      });
    }

    /// Строит интерфейс для режима редактирования персонажа
    /// Включает три вкладки: Основное, Бой и реген, Класс
    private void BuildEditUi()
    {
      Text = "Редактирование персонажа";
      Size = new Size(560, 620);
      StartPosition = FormStartPosition.CenterParent;
      FormBorderStyle = FormBorderStyle.FixedDialog;
      MaximizeBox = false;
      MinimizeBox = false;

      lblClassKind = new Label
      {
        Location = new Point(16, 12),
        Size = new Size(500, 22),
        Font = new Font("Segoe UI", 10f, FontStyle.Bold)
      };

      txtName = new TextBox { Location = new Point(120, 40), Size = new Size(400, 25) };
      lblName = new Label { Text = "Имя:", Location = new Point(16, 42), Size = new Size(100, 25) };

      nudLevel = new NumericUpDown
      {
        Location = new Point(120, 72),
        Size = new Size(120, 25),
        Minimum = 1,
        Maximum = 1000
      };

      lblLevel = new Label { Text = "Уровень:", Location = new Point(16, 74), Size = new Size(100, 25) };
      nudLevel.ValueChanged += (_, _) =>
      {
        UpdateMeleeStatsLabel();
        UpdateLevelDerivedPreview();
      };

      lblMeleeStats = new Label
      {
        Location = new Point(260, 72),
        Size = new Size(280, 40),
        AutoSize = false
      };

      tabControl = new TabControl { Location = new Point(12, 110), Size = new Size(520, 400) };

      var tabBasic = new TabPage("Основное");
      var tabCombat = new TabPage("Бой и реген");
      var tabClass = new TabPage("Класс");

      nudHealth = new NumericUpDown { Location = new Point(140, 16), Size = new Size(120, 25), Minimum = 0, Maximum = 999999 };
      lblMaxHealthInfo = new Label
      {
        Location = new Point(140, 52),
        Size = new Size(120, 25),
        BorderStyle = BorderStyle.FixedSingle,
        TextAlign = ContentAlignment.MiddleLeft
      };
      lblResource = new Label { Text = "Энергия:", Location = new Point(16, 88), Size = new Size(120, 25) };
      nudResource = new NumericUpDown { Location = new Point(140, 86), Size = new Size(120, 25), Minimum = 0, Maximum = 999 };

      tabBasic.Controls.AddRange(new Control[] {
        new Label { Text = "Текущее HP:", Location = new Point(16, 18), Size = new Size(120, 25) },
        nudHealth,
        new Label { Text = "Макс. HP (по уровню):", Location = new Point(16, 54), Size = new Size(120, 40) },
        lblMaxHealthInfo,
        lblResource,
        nudResource
      });

      nudBaseDamage = new NumericUpDown { Location = new Point(180, 16), Size = new Size(120, 25), Minimum = 1, Maximum = 99999 };
      nudStrength = new NumericUpDown { Location = new Point(180, 52), Size = new Size(120, 25), Minimum = 0, Maximum = 99999 };
      nudAgility = new NumericUpDown { Location = new Point(180, 88), Size = new Size(120, 25), Minimum = 0, Maximum = 99999 };
      nudRegenAmount = new NumericUpDown { Location = new Point(180, 124), Size = new Size(120, 25), Minimum = 0, Maximum = 9999 };
      nudRegenInterval = new NumericUpDown { Location = new Point(180, 160), Size = new Size(120, 25), Minimum = 1, Maximum = 600, DecimalPlaces = 1, Increment = 0.5m };

      tabCombat.Controls.AddRange(new Control[] {
        new Label { Text = "Базовый урон:", Location = new Point(16, 18), Size = new Size(160, 25) },
        nudBaseDamage,
        new Label { Text = "Сила:", Location = new Point(16, 54), Size = new Size(160, 25) },
        nudStrength,
        new Label { Text = "Ловкость:", Location = new Point(16, 90), Size = new Size(160, 25) },
        nudAgility,
        new Label { Text = "Реген (HP за тик):", Location = new Point(16, 126), Size = new Size(160, 25) },
        nudRegenAmount,
        new Label { Text = "Интервал регена (сек):", Location = new Point(16, 162), Size = new Size(160, 25) },
        nudRegenInterval,
      });
      nudBaseDamage.ValueChanged += (_, _) => UpdateMeleeStatsLabel();
      nudStrength.ValueChanged += (_, _) => UpdateMeleeStatsLabel();
      nudAgility.ValueChanged += (_, _) => UpdateMeleeStatsLabel();

      panelSeeker = new Panel { Location = new Point(8, 8), Size = new Size(480, 340), Visible = false };
      nudDodge = new NumericUpDown { Location = new Point(160, 45), Size = new Size(120, 25), Minimum = 0, Maximum = 2000 };
      chkStealth = new CheckBox { Text = "В скрытности", Location = new Point(16, 70), Size = new Size(200, 24) };
      nudStealthSec = new NumericUpDown { Location = new Point(160, 105), Size = new Size(120, 25), Minimum = 0, Maximum = 300, DecimalPlaces = 1, Increment = 0.5m };

      panelSeeker.Controls.AddRange(new Control[] {
        new Label { Text = "Энергия задаётся на вкладке «Основное».", Location = new Point(16, 12), Size = new Size(400, 22) },
        new Label { Text = "Уклонение %:", Location = new Point(16, 44), Size = new Size(140, 25) },
        nudDodge,
        chkStealth,
        new Label { Text = "Скрытность (сек):", Location = new Point(16, 103), Size = new Size(140, 25) },
        nudStealthSec
      });

      panelTemplar = new Panel { Location = new Point(8, 8), Size = new Size(480, 340), Visible = false };
      nudBlock = new NumericUpDown { Location = new Point(160, 40), Size = new Size(120, 25), Minimum = 0, Maximum = 200 };
      nudArmor = new NumericUpDown { Location = new Point(160, 75), Size = new Size(120, 25), Minimum = 0, Maximum = 9999 };
      nudHolyPower = new NumericUpDown { Location = new Point(160, 110), Size = new Size(120, 25), Minimum = 0, Maximum = 99 };
      chkImmobilized = new CheckBox { Text = "Обездвижен (эгида)", Location = new Point(16, 140), Size = new Size(220, 24) };

      panelTemplar.Controls.AddRange(new Control[] {
        new Label { Text = "Вера задаётся на вкладке «Основное».", Location = new Point(16, 15), Size = new Size(390, 22) },
        new Label { Text = "Блок %:", Location = new Point(16, 44), Size = new Size(135, 25) },
        nudBlock,
        new Label { Text = "Броня:", Location = new Point(16, 80), Size = new Size(135, 25) },
        nudArmor,
        new Label { Text = "Святая сила:", Location = new Point(16, 116), Size = new Size(135, 25) },
        nudHolyPower,
        chkImmobilized
      });

      tabClass.Controls.Add(panelSeeker);
      tabClass.Controls.Add(panelTemplar);

      tabControl.TabPages.Add(tabBasic);
      tabControl.TabPages.Add(tabCombat);
      tabControl.TabPages.Add(tabClass);

      btnSave = new Button
      {
        Text = "Сохранить",
        Location = new Point(340, 530),
        Size = new Size(110, 36),
        BackColor = Color.LightGreen
      };
      btnSave.Click += (_, _) => BtnSave_Click();

      btnCancel = new Button
      {
        Text = "Отмена",
        Location = new Point(460, 530),
        Size = new Size(80, 36),
        BackColor = Color.LightGray
      };
      btnCancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

      Controls.AddRange(new Control[] {
        lblClassKind, lblName, txtName, lblLevel, nudLevel, lblMeleeStats, tabControl, btnSave, btnCancel
      });
    }

    /// Обновляет значения UI-элементов из данных персонажа
    /// Вызывается при загрузке данных и после сброса характеристик
    private void RefreshFromCharacter()
    {
      if (_character == null || nudHealth == null) return;

      txtName!.Text = _character.Name;
      nudLevel!.Value = Math.Clamp(_character.Level, (int)nudLevel.Minimum, (int)nudLevel.Maximum);
      nudHealth!.Value = Math.Min(nudHealth.Maximum, Math.Max(nudHealth.Minimum, _character.Health));
      lblMaxHealthInfo!.Text = _character.MaxHealth.ToString();
      nudBaseDamage!.Value = Math.Max(nudBaseDamage!.Minimum, Math.Min(nudBaseDamage!.Maximum, _character.BaseDamage));
      if (_character is MeleeCharacter mm)
      {
        nudStrength!.Value = Math.Clamp(mm.Strength, (int)nudStrength!.Minimum, (int)nudStrength!.Maximum);
        nudAgility!.Value = Math.Clamp(mm.Agility, (int)nudAgility!.Minimum, (int)nudAgility!.Maximum);
      }
      nudRegenAmount!.Value = Math.Min(nudRegenAmount!.Maximum, _character.RegenAmount);
      nudRegenInterval!.Value = (decimal)Math.Min((double)nudRegenInterval!.Maximum, Math.Max((double)nudRegenInterval!.Minimum, _character.RegenIntervalSec));

      UpdateMeleeStatsLabel();

      if (_character is Seeker seeker)
      {
        lblResource!.Text = "Энергия:";
        nudResource!.Maximum = 1000;
        nudResource!.Value = seeker.Energy;
        nudDodge!.Value = seeker.DodgeChance;
        chkStealth!.Checked = seeker.IsStealthed;
        nudStealthSec!.Value = (decimal)seeker.StealthSeconds;
      }
      else if (_character is Templar templar)
      {
        lblResource!.Text = "Вера:";
        nudResource!.Maximum = 1000;
        nudResource!.Value = Math.Min(nudResource.Maximum, templar.Faith);
        nudBlock!.Value = templar.BlockChance;
        nudArmor!.Value = templar.Armor;
        nudHolyPower!.Value = templar.HolyPower;
        chkImmobilized!.Checked = templar.IsImmobilized;
      }
    }

    /// Обновляет метку с расчётным уроном (сила + базовый урон + сила/5)
    /// Вызывается при изменении силы, ловкости или базового урона
    private void UpdateMeleeStatsLabel()
    {
      if (lblMeleeStats == null) return;
      if (_character is not MeleeCharacter) return;
      int strength = nudStrength != null ? (int)nudStrength.Value : 0;
      int agility = nudAgility != null ? (int)nudAgility.Value : 0;
      int baseDamage = nudBaseDamage != null ? (int)nudBaseDamage.Value : 0;
      int calculated = baseDamage + strength / 5;
      lblMeleeStats.Text = $"Сила: {strength}, Ловкость: {agility} | Расчётный урон: {calculated}";
    }

    /// Обновляет максимальное HP и корректирует текущее при изменении уровня
    /// Формула: MaxHealth = level * 100
    private void UpdateLevelDerivedPreview()
    {
      if (nudLevel == null || lblMaxHealthInfo == null) return;
      int predictedMaxHealth = 100 * (int)nudLevel.Value;
      lblMaxHealthInfo.Text = predictedMaxHealth.ToString();
      if (nudHealth != null && nudHealth.Value > predictedMaxHealth)
        nudHealth.Value = predictedMaxHealth;
    }

    /// Загружает данные персонажа в UI при открытии формы редактирования
    /// Определяет тип класса и показывает соответствующую панель
    private void LoadCharacterData()
    {
      if (_character == null || !_isEditMode) return;

      if (_character is Seeker)
      {
        lblClassKind!.Text = "Тип: Искатель (Seeker)";
        panelSeeker!.Visible = true;
        panelTemplar!.Visible = false;
      }
      else if (_character is Templar)
      {
        lblClassKind!.Text = "Тип: Храмовник (Templar)";
        panelSeeker!.Visible = false;
        panelTemplar!.Visible = true;
      }

      RefreshFromCharacter();
    }

    /// Обработчик кнопки "Сохранить"
    /// Проверяет имя и вызывает создание или редактирование
    private void BtnSave_Click()
    {
      if (string.IsNullOrWhiteSpace(txtName.Text))
      {
        MessageBox.Show("Введите имя персонажа!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      if (!_isEditMode)
      {
        SaveCreate();
        return;
      }

      SaveEdit();
    }

    /// Создаёт нового персонажа на основе введённых данных и сохраняет в репозиторий
    /// Вызывается только в режиме создания
    private void SaveCreate()
    {
      int level = (int)nudLevel!.Value;
      int resource = (int)nudResource!.Value;

      if (cmbType!.SelectedIndex == 0)
        _character = new Seeker(txtName!.Text, level, resource, _repository);
      else
        _character = new Templar(txtName!.Text, level, resource);

      _repository.Add(_character);
      DialogResult = DialogResult.OK;
      Close();
    }

    /// Сохраняет изменения редактируемого персонажа в репозиторий
    /// Обновляет все атрибуты, включая специфичные для класса
    private void SaveEdit()
    {
      if (_character == null || nudHealth == null) return;

      _character.Name = txtName!.Text.Trim();
      _character.Level = (int)nudLevel!.Value;
      _character.Health = (int)nudHealth!.Value;
      _character.BaseDamage = (int)nudBaseDamage!.Value;
      if (_character is MeleeCharacter melee)
      {
        melee.Strength = (int)nudStrength!.Value;
        melee.Agility = (int)nudAgility!.Value;
      }
      _character.RegenAmount = (int)nudRegenAmount!.Value;
      _character.RegenIntervalSec = (float)nudRegenInterval!.Value;

      if (_character.Health > _character.MaxHealth)
        _character.Health = _character.MaxHealth;

      if (_character is Seeker seeker)
      {
        seeker.Energy = (int)nudResource!.Value;
        seeker.DodgeChance = (int)nudDodge!.Value;
        seeker.IsStealthed = chkStealth!.Checked;
        seeker.StealthSeconds = (float)nudStealthSec!.Value;
      }
      else if (_character is Templar templar)
      {
        templar.Faith = (int)nudResource!.Value;
        templar.BlockChance = (int)nudBlock!.Value;
        templar.Armor = (int)nudArmor!.Value;
        templar.HolyPower = (int)nudHolyPower!.Value;
        templar.IsImmobilized = chkImmobilized!.Checked;
      }

      _repository.Update(_character);
      DialogResult = DialogResult.OK;
      Close();
    }
  }
}
