using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
// Главное окно: таблица персонажей, кнопки CRUD, лечение, вызов методов.
// Здесь связываются UI и репозиторий.
namespace CharacterBattle
{
  public class FormMain : Form
  {
    private ICharacterRepository _repository = null!;
    private DataGridView dgvCharacters = null!;
    private Panel panelControls = null!;
    private Button btnAdd = null!;
    private Button btnEdit = null!;
    private Button btnDelete = null!;
    private Button btnRefresh = null!;
    private Button btnHeal= null!;
    private Button btnVizov = null!;
    private TextBox txtInfo = null!;
    private Label lblTitle = null!;
    private Label lblStatus = null!;
    private bool _isConnected;
    public static string DefaultConnectionString { get; set; } =
        "Host=localhost;Port=5432;Database=labb;Username=chonka;Password=chonka24211099;";

    public FormMain()
    {
      InitializeComponent();
      ConnectToDatabase();
    }

    private void ConnectToDatabase()
    {
      try
      {
        _repository = new CharacterRepository(DefaultConnectionString);
        _ = _repository.GetAll();
        _isConnected = true;

        lblStatus.Text = "Подключено к PostgreSQL";
        lblStatus.ForeColor = Color.LightGreen;

        LoadCharacters();
      }
      catch (Exception ex)
      {
        _isConnected = false;
        lblStatus.Text = "Ошибка подключения к БД";
        lblStatus.ForeColor = Color.Red;

        MessageBox.Show($"Не удалось подключиться к базе данных:\n{ex.Message}",
            "Ошибка подключения", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void InitializeComponent()
    {
      Text = "Character Battle";
      Size = new Size(1400, 800);
      StartPosition = FormStartPosition.CenterScreen;

      lblTitle = new Label
      {
        Text = "Управление персонажами",
        Font = new Font("Times New Roman", 16f, FontStyle.Bold),
        Location = new Point(20, 20),
        Size = new Size(400, 30)
      };

      lblStatus = new Label
      {
        Text = "Проверка подключения...",
        Location = new Point(700, 25),
        Size = new Size(230, 25),
        Font = new Font("Times New Roman", 9f, FontStyle.Italic),
        TextAlign = ContentAlignment.MiddleRight
      };

      dgvCharacters = new DataGridView
      {
        Location = new Point(20, 60),
        Size = new Size(1300, 300),
        ReadOnly = true,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        AllowUserToAddRows = false
      };

      panelControls = new Panel
      {
        Location = new Point(20, 370),
        Size = new Size(1500, 102)
      };

      btnAdd = new Button
      {
        Text = "Добавить",
        Location = new Point(10, 10),
        Size = new Size(120, 45),
        BackColor = Color.LightBlue,
        Font = new Font("Times New Roman", 10f)
      };
      btnAdd.Click += BtnAdd_Click;

      btnEdit = new Button
      {
        Text = "Редактировать",
        Location = new Point(140, 10),
        Size = new Size(140, 45),
        BackColor = Color.LightBlue,
        Font = new Font("Times New Roman", 10f)
      };
      btnEdit.Click += BtnEdit_Click;


      btnDelete = new Button
      {
        Text = "Удалить",
        Location = new Point(290, 10),
        Size = new Size(120, 45),
        BackColor = Color.LightBlue,
        Font = new Font("Times New Roman", 10f)
      };
      btnDelete.Click += BtnDelete_Click;

      btnRefresh = new Button
      {
        Text = "Обновить",
        Location = new Point(420, 10),
        Size = new Size(120, 45),
        BackColor = Color.LightBlue,
        Font = new Font("Times New Roman", 10f)
      };
      btnRefresh.Click += BtnRefresh_Click;

      btnHeal = new Button
      {
        Text = "Лечение",
        Location = new Point(550, 10),
        Size = new Size(130, 45),
        BackColor = Color.LightBlue,
        Font = new Font("Times New Roman", 10f)
      };
      btnHeal.Click += BtnHeal_Click;

      btnVizov = new Button
      {
        Text = "Вызвать методы",
        TextAlign = ContentAlignment.TopCenter,
        Location = new Point(690, 10),
        Size = new Size(130, 60),
        BackColor = Color.LightBlue,
        Font = new Font("Times New Roman", 10f)
      };
      btnVizov.Click += BtnVizov_Click;

      txtInfo = new TextBox
      {
        Location = new Point(20, 470),
        Size = new Size(1200, 270),
        Multiline = true,
        ReadOnly = true,
        Font = new Font("Times New Roman", 9f),
        BackColor = Color.Black,
        ForeColor = Color.LightGreen,
        ScrollBars = ScrollBars.Vertical
      };

      panelControls.Controls.AddRange(new Control[] {
        btnAdd, btnEdit, btnDelete, btnRefresh, btnHeal, btnVizov
      });
      Controls.AddRange(new Control[] { lblTitle, lblStatus, dgvCharacters, panelControls, txtInfo });
    }

    private void LoadCharacters()
    {
      if (!_isConnected) return;

      try
      {
        var characters = _repository.GetAll();
        dgvCharacters.DataSource = characters.Select(c => new
        {
          c.Id,
          c.Name,
          Тип = c.GetCharacterType() == "Seeker" ? "Искатель" : "Храмовник",
          Уровень = c.Level,
          Здоровье = $"{c.Health}/{c.MaxHealth}"
        }).ToList();

        txtInfo.AppendText($"[{DateTime.Now:HH:mm:ss}] Загружено {characters.Count} персонажей\n");
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
      if (!_isConnected) return;

      using var form = new FormCharacter(_repository);
      if (form.ShowDialog() == DialogResult.OK)
      {
        LoadCharacters();
        txtInfo.AppendText($"[{DateTime.Now:HH:mm:ss}] Добавлен персонаж «{form.Character.Name}»\n");
      }
    }

    private void BtnEdit_Click(object? sender, EventArgs e)
    {
      if (!_isConnected) return;

      if (dgvCharacters.SelectedRows.Count == 0)
      {
        MessageBox.Show("Выберите персонажа для редактирования", "Внимание",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      int id = (int)dgvCharacters.SelectedRows[0].Cells["Id"].Value!;
      var character = _repository.GetById(id);
      if (character == null)
      {
        MessageBox.Show("Персонаж не найден в базе.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      using var form = new FormCharacter(_repository, character);
      if (form.ShowDialog() == DialogResult.OK)
      {
        LoadCharacters();
        txtInfo.AppendText($"[{DateTime.Now:HH:mm:ss}] Обновлён персонаж «{form.Character.Name}»\n");
      }
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
      if (!_isConnected) return;

      if (dgvCharacters.SelectedRows.Count == 0)
      {
        MessageBox.Show("Выберите персонажа для удаления", "Внимание",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      int id = (int)dgvCharacters.SelectedRows[0].Cells["Id"].Value!;
      string? name = dgvCharacters.SelectedRows[0].Cells["Name"].Value?.ToString();

      var result = MessageBox.Show($"Удалить персонажа «{name}»?\nЭто действие нельзя отменить.",
          "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

      if (result == DialogResult.Yes)
      {
        _repository.Delete(id);
        LoadCharacters();
        txtInfo.AppendText($"[{DateTime.Now:HH:mm:ss}] Удалён персонаж «{name}»\n");
      }
    }

    private void BtnRefresh_Click(object? sender, EventArgs e)
    {
      LoadCharacters();
      txtInfo.AppendText($"[{DateTime.Now:HH:mm:ss}] Список обновлён\n");
    }

    private void BtnHeal_Click(object? sender, EventArgs e)
    {
      if (!_isConnected) return;

      if (dgvCharacters.SelectedRows.Count == 0)
      {
        MessageBox.Show("Выберите персонажа в таблице.", "Лечение", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }

      int id = (int)dgvCharacters.SelectedRows[0].Cells["Id"].Value!;
      var character = _repository.GetById(id);
      if (character == null)
      {
        MessageBox.Show("Персонаж не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      character.Health = character.MaxHealth;
      if (character is Seeker seeker)
        seeker.Energy = 1000;
      else if (character is Templar templar)
        templar.Faith = 1000;

      _repository.Update(character);
      LoadCharacters();
      txtInfo.AppendText($"[{DateTime.Now:HH:mm:ss}] Лечение: «{character.Name}» — HP до максимума, ресурс восстановлен.\n");
    }

    private void BtnVizov_Click(object? sender, EventArgs e)
    {
      if (!_isConnected) return;
      if (dgvCharacters.SelectedRows.Count == 0)
      {
        MessageBox.Show("Выберите персонажа в таблице.", "Вызвать метод",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }

      int id = (int)dgvCharacters.SelectedRows[0].Cells["Id"].Value!;
      var character = _repository.GetById(id);
      if (character == null)
      {
        MessageBox.Show("Персонаж не найден.", "Ошибка",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      ShowMethodPickerDialog(character);
    }

    private void ShowMethodPickerDialog(Character character)
    {
      using var dlg = new Form
      {
        Text = $"Выбор метода — {character.Name}",
        Size = new Size(520, 260),
        StartPosition = FormStartPosition.CenterParent,
        FormBorderStyle = FormBorderStyle.FixedDialog,
        MinimizeBox = false,
        MaximizeBox = false
      };

      var lbl = new Label
      {
        Text = $"Персонаж: {character.Name} ({character.GetCharacterType()})",
        Location = new Point(12, 12),
        Size = new Size(480, 24)
      };

      var cmb = new ComboBox
      {
        Location = new Point(12, 44),
        Size = new Size(480, 28),
        DropDownStyle = ComboBoxStyle.DropDownList
      };

      var btnRun = new Button
      {
        Text = "Выполнить",
        Location = new Point(12, 88),
        Size = new Size(120, 34),
        BackColor = Color.LightGray
      };

      var btnClose = new Button
      {
        Text = "Закрыть",
        Location = new Point(372, 88),
        Size = new Size(120, 34),
        BackColor = Color.LightGray
      };
      btnClose.Click += (_, _) => dlg.Close();

      // Общие методы
      cmb.Items.Add("PrintInfo()");
      cmb.Items.Add("Heal()");
      cmb.Items.Add("Update()");
      cmb.Items.Add("TakeDamage()");

      if (character is Seeker)
      {
        cmb.Items.Add("Seeker: UseAbility(-энергия + уворот)");
        cmb.Items.Add("Seeker: UseSpecial(скрытность + шанс уворота)");
        cmb.Items.Add("Seeker: TryDodge()");
        cmb.Items.Add("Seeker: DefaultStats()");
        cmb.Items.Add("Seeker: GetSeekerStats()");
      }
      else if (character is Templar)
      {
        cmb.Items.Add("Templar: UseAbility()");
        cmb.Items.Add("Templar: UseSpecial()");
        cmb.Items.Add("Templar: ProtectAlly(Templar, 20)");
        cmb.Items.Add("Templar: DefaultStats()");
        cmb.Items.Add("Templar: GetTemplarStats()");
      }

      if (cmb.Items.Count > 0) cmb.SelectedIndex = 0;

      btnRun.Click += (_, _) =>
      {
        if (cmb.SelectedItem == null) return;

        txtInfo.AppendText(Environment.NewLine + new string('=', 60) + Environment.NewLine);
        txtInfo.AppendText($"Вызов метода: {cmb.SelectedItem} для {character.Name} ({character.GetCharacterType()}){Environment.NewLine}");

        SelectedMethod(character, cmb.SelectedItem.ToString() ?? string.Empty);
        LoadCharacters();
        txtInfo.AppendText(new string('=', 60) + Environment.NewLine);
      };

      dlg.Controls.AddRange(new Control[] { lbl, cmb, btnRun, btnClose });
      dlg.ShowDialog(this);
    }

    private void SelectedMethod(Character character, string methodKey)
    {
      switch (methodKey)
      {
        case "PrintInfo()":
          txtInfo.AppendText($"PrintInfo(): {character.PrintInfo()}{Environment.NewLine}");
          return;

        case "Heal()":
          {
            int before = character.Health;
            character.Heal(10);
            txtInfo.AppendText($"Heal(10): HP {before}->{character.Health}/{character.MaxHealth}{Environment.NewLine}");
            _repository.Update(character);
            return;
          }

        case "Update()":
          {
            int before = character.Health;
            character.Update(5.0f);
            txtInfo.AppendText($"Update(5.0): HP {before}->{character.Health}/{character.MaxHealth}{Environment.NewLine}");
            _repository.Update(character);
            return;
          }

        case "TakeDamage()":
          {
            int before = character.Health;
            character.TakeDamage(33);
            txtInfo.AppendText($"TakeDamage(33): HP {before}->{character.Health}/{character.MaxHealth}{Environment.NewLine}");
            _repository.Update(character);
            return;
          }
      }

      if (character is Seeker seeker)
      {
        switch (methodKey)
        {
          case "Seeker: UseAbility(-энергия + уворот)":
            {
              int e0 = seeker.Energy;
              int d0 = seeker.DodgeChance;
              if (e0 >= 30)
              {
                seeker.UseAbility();
                txtInfo.AppendText($"UseAbility(): энергия {e0}->{seeker.Energy}, уворот {d0}->{seeker.DodgeChance} (+15%){Environment.NewLine}");
                _repository.Update(seeker);
              }
              else
              {
                txtInfo.AppendText($"UseAbility(): недостаточно энергии ({e0}/30 требуется){Environment.NewLine}");
              }
              return;
            }
          case "Seeker: UseSpecial(скрытность + шанс уворота)":
            {
              bool st0 = seeker.IsStealthed;
              float sec0 = seeker.StealthSeconds;
              int e0 = seeker.Energy;
              if (e0 >= 20 && !seeker.IsStealthed)
              {
                seeker.UseSpecial();
                txtInfo.AppendText($"UseSpecial(): вход в скрытность! энергия {e0}->{seeker.Energy}, скрыт {st0}->{seeker.IsStealthed}, таймер {sec0:0.0}->{seeker.StealthSeconds:0.0}, уворот {seeker.DodgeChance}%{Environment.NewLine}");

                _repository.Update(seeker);
              }
              else if (seeker.IsStealthed)
              {
                txtInfo.AppendText($"UseSpecial(): уже в скрытности (таймер: {seeker.StealthSeconds:0.0}){Environment.NewLine}");
              }
              else
              {
                txtInfo.AppendText($"UseSpecial(): недостаточно энергии ({e0}/20 требуется){Environment.NewLine}");
              }
              return;
            }
          case "Seeker: TryDodge()":
            {
              bool ok = seeker.TryDodge(out int roll, out int threshold);
              txtInfo.AppendText($"TryDodge(): roll={roll}, threshold={threshold}, result={ok}{Environment.NewLine}");
              return;
            }
          case "Seeker: DefaultStats()":
            seeker.DefaultStats();
            txtInfo.AppendText("DefaultStats(): выполнено" + Environment.NewLine);
            return;
          case "Seeker: GetSeekerStats()":
            txtInfo.AppendText(seeker.GetSeekerStats() + Environment.NewLine);
            return;
        }
      }
      else if (character is Templar templar)
      {
        switch (methodKey)
        {
          case "Templar: UseAbility()":
            {
              bool imm0 = templar.IsImmobilized;
              int faith0 = templar.Faith;
              int ticks0 = templar.AegisTicksRemaining;
              if (faith0 >= 50 && !templar.IsImmobilized)
              {
                templar.UseAbility();
                txtInfo.AppendText($"UseAbility(): активация эгиды! обездвижен {imm0}->{templar.IsImmobilized}, вера {faith0}->{templar.Faith}{Environment.NewLine}");
                _repository.Update(templar);
              }
              else if (templar.IsImmobilized && ticks0 > 0)
              {
                templar.UseAbility();
                txtInfo.AppendText($"UseAbility(): снятие эгиды! обездвижен {imm0}->{templar.IsImmobilized}{Environment.NewLine}");
                _repository.Update(templar);
              }
              else
              {
                txtInfo.AppendText($"UseAbility(): нельзя активировать (вера={faith0}/50 требуется, обездвижен={imm0}){Environment.NewLine}");
              }
              return;
            }
          case "Templar: UseSpecial()":
            {
              int a0 = templar.Armor;
              int b0 = templar.BlockChance;
              templar.UseSpecial();
              txtInfo.AppendText($"UseSpecial(): броня {a0}->{templar.Armor} (+10), блок {b0}->{templar.BlockChance} (+10){Environment.NewLine}");
              _repository.Update(templar);
              return;
            }
          case "Templar: ProtectAlly(Templar, 20)":
            {
              int hp0 = templar.Health;
              templar.ProtectAlly(templar, 20);
              txtInfo.AppendText($"ProtectAlly(Templar,20): HP {hp0}->{templar.Health}/{templar.MaxHealth}{Environment.NewLine}");
              _repository.Update(character);
              return;
            }
          case "Templar: DefaultStats()":
            templar.DefaultStats();
            txtInfo.AppendText("DefaultStats(): выполнено" + Environment.NewLine);
            return;
          case "Templar: GetTemplarStats()":
            txtInfo.AppendText(templar.GetTemplarStats() + Environment.NewLine);
            return;
        }
      }

      txtInfo.AppendText("Неизвестный метод/не поддерживается для этого типа." + Environment.NewLine);
    }
  }
}
