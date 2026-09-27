using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Sorting
{
  public partial class Form1 : Form
  {
    private const int MinRandomValue = 20;
    private const int MaxRandomValue = 50;
    private const int BogoSafeSize = 7;
    private const int BogoIterationLimit = 200000;

    private MenuStrip mainMenu;
    private ToolStripMenuItem runMenuItem;
    private ToolStripMenuItem clearMenuItem;
    private ToolStripMenuItem generateMenuItem;
    private ToolStripMenuItem loadExcelMenuItem;
    private ToolStripMenuItem loadGoogleMenuItem;
    private ToolStripMenuItem addRowMenuItem;
    private DataGridView inputGrid;
    private CheckBox bubbleCheckBox;
    private CheckBox insertionCheckBox;
    private CheckBox shakerCheckBox;
    private CheckBox quickCheckBox;
    private CheckBox bogoCheckBox;
    private RadioButton ascendingRadio;
    private RadioButton descendingRadio;
    private NumericUpDown elementsCountNumeric;
    private Panel visualizationPanel;
    private RichTextBox logBox;

    // --- Раскрывающиеся описания алгоритмов ---
    private LinkLabel bubbleInfoLink;
    private LinkLabel insertionInfoLink;
    private LinkLabel shakerInfoLink;
    private LinkLabel quickInfoLink;
    private LinkLabel bogoInfoLink;

    private Label bubbleInfoLabel;
    private Label insertionInfoLabel;
    private Label shakerInfoLabel;
    private Label quickInfoLabel;
    private Label bogoInfoLabel;

    private bool bubbleInfoVisible = false;
    private bool insertionInfoVisible = false;
    private bool shakerInfoVisible = false;
    private bool quickInfoVisible = false;
    private bool bogoInfoVisible = false;

    private readonly Dictionary<string, double[]> algorithmSnapshots =
      new Dictionary<string, double[]>();

    private readonly Dictionary<string, Color> algorithmColors =
      new Dictionary<string, Color>
      {
        { "Bubble", Color.Crimson },
        { "Insertion", Color.RoyalBlue },
        { "Shaker", Color.SeaGreen },
        { "Quick", Color.DarkOrange },
        { "Bogo", Color.MediumPurple }
      };

    private readonly Dictionary<string, string> algorithmRussianNames =
      new Dictionary<string, string>
      {
        { "Bubble", "Пузырьковая" },
        { "Insertion", "Вставками" },
        { "Shaker", "Шейкерная" },
        { "Quick", "Быстрая" },
        { "Bogo", "BOGO" }
      };

    public Form1()
    {
      InitializeComponent();
      BuildUserInterface();
      SortingAlgorithms.OnStepCompleted += HandleSortStep;
    }

    // ================= СОЗДАНИЕ ИНТЕРФЕЙСА =================
    private void BuildUserInterface()
    {
      // --- Меню ---
      mainMenu = new MenuStrip();
      mainMenu.Location = new Point(0, 0);
      mainMenu.Size = new Size(this.ClientSize.Width, 24);

      runMenuItem = new ToolStripMenuItem("Рассчитать");
      clearMenuItem = new ToolStripMenuItem("Очистить");
      generateMenuItem = new ToolStripMenuItem("Генерировать");
      addRowMenuItem = new ToolStripMenuItem("Добавить строку");
      loadExcelMenuItem = new ToolStripMenuItem("Загрузить из Excel");
      loadGoogleMenuItem = new ToolStripMenuItem("Загрузить из Google");

      mainMenu.Items.Add(runMenuItem);
      mainMenu.Items.Add(clearMenuItem);
      mainMenu.Items.Add(generateMenuItem);
      mainMenu.Items.Add(addRowMenuItem);
      mainMenu.Items.Add(loadExcelMenuItem);
      mainMenu.Items.Add(loadGoogleMenuItem);
      addRowMenuItem.Click += AddRowButton_Click;

      runMenuItem.Click += RunButton_Click;
      clearMenuItem.Click += ClearButton_Click;
      generateMenuItem.Click += GenerateButton_Click;
      loadExcelMenuItem.Click += LoadExcelButton_Click;
      loadGoogleMenuItem.Click += LoadGoogleButton_Click;

      this.MainMenuStrip = mainMenu;
      this.Controls.Add(mainMenu);

      // --- Таблица ---
      inputGrid = new DataGridView();
      inputGrid.Location = new Point(12, 35);
      inputGrid.Size = new Size(500, 260);
      inputGrid.AllowUserToAddRows = true;      // разрешаем добавление
      inputGrid.AllowUserToDeleteRows = true;   // разрешаем удаление
      inputGrid.EditMode = DataGridViewEditMode.EditOnEnter;
      inputGrid.ColumnHeadersHeightSizeMode =
        DataGridViewColumnHeadersHeightSizeMode.AutoSize;

      // Сразу создаём колонку — чтобы можно было вводить числа
      DataGridViewTextBoxColumn valueColumn = new DataGridViewTextBoxColumn();
      valueColumn.Name = "valueColumn";
      valueColumn.HeaderText = "Значение";
      valueColumn.Width = 120;
      inputGrid.Columns.Add(valueColumn);

      this.Controls.Add(inputGrid);

      

      // --- Чекбоксы с описаниями ---
      bubbleCheckBox = CreateCheckBox("Пузырьковая", 530, 35);
      bubbleInfoLink = CreateInfoLink(705, 35);
      bubbleInfoLabel = CreateInfoLabel(
        "Пузырьковая сортировка.\n\n" +
        "Идея: последовательно сравниваем соседние элементы. Если левый больше правого " +
        "(при сортировке по возрастанию) — меняем их местами. Проходим массив слева направо " +
        "много раз, пока не останется ни одной пары для обмена.\n\n" +
        "После первого прохода самый большой элемент «всплывает» в конец массива. " +
        "После второго — второй по величине встаёт на предпоследнее место. И так далее.\n\n" +
        "Сложность: O(n²) сравнений и обменов.\n" +
        "Плюсы: простой, устойчивый (не меняет порядок равных элементов).\n" +
        "Минусы: медленный на больших массивах.",
        195);

      insertionCheckBox = CreateCheckBox("Вставками", 530, 60);
      insertionInfoLink = CreateInfoLink(705, 60);
      insertionInfoLabel = CreateInfoLabel(
        "Сортировка вставками.\n\n" +
        "Идея: делим массив на две части — отсортированную (слева) и неотсортированную (справа). " +
        "Берём очередной элемент из неотсортированной части и «вставляем» его в нужное место " +
        "отсортированной, сдвигая все большие элементы вправо.\n\n" +
        "Начинаем с первого элемента (он считается отсортированным) и постепенно расширяем " +
        "отсортированную часть до всего массива.\n\n" +
        "Сложность: O(n²) в среднем и худшем случае, O(n) — на почти отсортированных данных.\n" +
        "Плюсы: эффективна на маленьких и почти упорядоченных массивах, устойчива.\n" +
        "Минусы: медленная на больших случайных массивах.",
        195);

      shakerCheckBox = CreateCheckBox("Шейкерная", 530, 85);
      shakerInfoLink = CreateInfoLink(705, 85);
      shakerInfoLabel = CreateInfoLabel(
        "Шейкерная сортировка (коктейльная).\n\n" +
        "Идея: улучшение пузырьковой сортировки. Проходим массив попеременно: " +
        "слева направо — тянем большие элементы в конец, затем справа налево — тянем " +
        "маленькие элементы в начало. Сужаем границы просмотра с обеих сторон.\n\n" +
        "Такой двусторонний проход решает проблему «черепах» пузырьковой сортировки — " +
        "маленьких элементов в конце массива, которые очень медленно всплывают.\n\n" +
        "Сложность: O(n²) в среднем, но на практике работает быстрее обычной пузырьковой.\n" +
        "Плюсы: быстрее пузырьковой на «перекошенных» данных.\n" +
        "Минусы: сложнее в реализации, всё ещё O(n²).",
        195);

      quickCheckBox = CreateCheckBox("Быстрая", 530, 110);
      quickInfoLink = CreateInfoLink(705, 110);
      quickInfoLabel = CreateInfoLabel(
        "Быстрая сортировка (QuickSort).\n\n" +
        "Идея (разделяй и властвуй): выбираем опорный элемент (pivot), обычно последний " +
        "в подмассиве. Разбиваем массив на две части: элементы меньше опорного — слева, " +
        "больше — справа. Затем рекурсивно применяем ту же процедуру к левой и правой частям.\n\n" +
        "В данной реализации используется схема разбиения по последнему элементу.\n\n" +
        "Сложность: O(n log n) в среднем, O(n²) — в худшем (если pivot всегда минимальный/максимальный).\n" +
        "Плюсы: самая быстрая на практике для случайных данных, рекурсивная и элегантная.\n" +
        "Минусы: неустойчива, худший случай O(n²), требует O(log n) стека.",
        195);

      bogoCheckBox = CreateCheckBox("BOGO", 530, 135);
      bogoInfoLink = CreateInfoLink(705, 135);
      bogoInfoLabel = CreateInfoLabel(
        "BOGO-сортировка (Bogosort, «дурацкая» сортировка).\n\n" +
        "Идея: пока массив не отсортирован, случайно переставляем все элементы " +
        "(перемешиваем за O(n) с помощью алгоритма Fisher–Yates). Так продолжается " +
        "до тех пор, пока случайно не получим отсортированный массив.\n\n" +
        "Сложность: O(n · n!) в среднем. Для n = 10 среднее число перестановок ≈ 3.6 млн.\n" +
        "Для n = 20 — практически бесконечность.\n\n" +
        "Плюсы: НЕТ. Это шуточный алгоритм для демонстрации «худшего из возможных».\n" +
        "Минусы: катастрофически медленный, неприменим на практике.\n\n" +
        "В программе ограничен по количеству итераций (200000), чтобы не «зависнуть».",
        195);

      // Привязка стрелок-описаний
      bubbleInfoLink.Click += BubbleInfoLink_Click;
      insertionInfoLink.Click += InsertionInfoLink_Click;
      shakerInfoLink.Click += ShakerInfoLink_Click;
      quickInfoLink.Click += QuickInfoLink_Click;
      bogoInfoLink.Click += BogoInfoLink_Click;

      // Добавляем на форму ссылки и метки описаний
      this.Controls.Add(bubbleInfoLink);
      this.Controls.Add(insertionInfoLink);
      this.Controls.Add(shakerInfoLink);
      this.Controls.Add(quickInfoLink);
      this.Controls.Add(bogoInfoLink);

      this.Controls.Add(bubbleInfoLabel);
      this.Controls.Add(insertionInfoLabel);
      this.Controls.Add(shakerInfoLabel);
      this.Controls.Add(quickInfoLabel);
      this.Controls.Add(bogoInfoLabel);

      // --- Радиокнопки ---
      ascendingRadio = new RadioButton();
      ascendingRadio.Text = "По возрастанию";
      ascendingRadio.Location = new Point(530, 170);
      ascendingRadio.Size = new Size(180, 22);
      ascendingRadio.Checked = true;
      this.Controls.Add(ascendingRadio);

      descendingRadio = new RadioButton();
      descendingRadio.Text = "По убыванию";
      descendingRadio.Location = new Point(530, 195);
      descendingRadio.Size = new Size(180, 22);
      this.Controls.Add(descendingRadio);

      // --- NumericUpDown ---
      elementsCountNumeric = new NumericUpDown();
      elementsCountNumeric.Location = new Point(530, 230);
      elementsCountNumeric.Size = new Size(80, 22);
      elementsCountNumeric.Minimum = 2;
      elementsCountNumeric.Maximum = 100;
      elementsCountNumeric.Value = 10;
      this.Controls.Add(elementsCountNumeric);

      // --- Панель визуализации ---
      visualizationPanel = new Panel();
      visualizationPanel.Location = new Point(12, 305);
      visualizationPanel.Size = new Size(700, 200);
      visualizationPanel.BackColor = Color.White;
      visualizationPanel.BorderStyle = BorderStyle.FixedSingle;
      visualizationPanel.Paint += VisualizationPanel_Paint;
      this.Controls.Add(visualizationPanel);

      // --- Лог ---
      logBox = new RichTextBox();
      logBox.Location = new Point(12, 515);
      logBox.Size = new Size(700, 150);
      logBox.ReadOnly = true;
      logBox.ScrollBars = RichTextBoxScrollBars.Vertical;
      this.Controls.Add(logBox);
    }

    private CheckBox CreateCheckBox(string text, int x, int y)
    {
      CheckBox checkBox = new CheckBox();
      checkBox.Text = text;
      checkBox.Location = new Point(x, y);
      checkBox.Size = new Size(180, 22);
      this.Controls.Add(checkBox);
      return checkBox;
    }

    // ИНФО-ССЫЛКИ И МЕТКИ
    private LinkLabel CreateInfoLink(int x, int y)
    {
      LinkLabel link = new LinkLabel();
      link.Text = "▶";
      link.Location = new Point(x, y);
      link.Size = new Size(20, 22);
      link.TextAlign = ContentAlignment.MiddleCenter;
      link.LinkBehavior = LinkBehavior.NeverUnderline;
      link.LinkColor = Color.DarkBlue;
      link.Font = new Font("Segoe UI", 9, FontStyle.Bold);
      return link;
    }

    private Label CreateInfoLabel(string text, int y)
    {
      Label label = new Label();
      label.Text = text;
      label.Location = new Point(12, y);
      label.Size = new Size(700, 0);
      label.Visible = false;
      label.BorderStyle = BorderStyle.FixedSingle;
      label.BackColor = Color.LightYellow;
      label.ForeColor = Color.Black;
      label.Font = new Font("Segoe UI", 8);
      label.Padding = new Padding(6);
      label.AutoSize = false;
      return label;
    }

    private void ToggleInfo(Label infoLabel, LinkLabel infoLink, ref bool isVisible)
    {
      isVisible = !isVisible;

      if (isVisible)
      {
        infoLabel.Height = infoLabel.PreferredHeight + 10;
        infoLabel.Visible = true;
        infoLabel.BringToFront();
        infoLink.Text = "▼";
      }
      else
      {
        infoLabel.Visible = false;
        infoLink.Text = "▶";
      }

      this.PerformLayout();
      this.Refresh();
    }

    private void BubbleInfoLink_Click(object sender, EventArgs e)
    {
      ToggleInfo(bubbleInfoLabel, bubbleInfoLink, ref bubbleInfoVisible);
    }

    private void InsertionInfoLink_Click(object sender, EventArgs e)
    {
      ToggleInfo(insertionInfoLabel, insertionInfoLink, ref insertionInfoVisible);
    }

    private void ShakerInfoLink_Click(object sender, EventArgs e)
    {
      ToggleInfo(shakerInfoLabel, shakerInfoLink, ref shakerInfoVisible);
    }

    private void QuickInfoLink_Click(object sender, EventArgs e)
    {
      ToggleInfo(quickInfoLabel, quickInfoLink, ref quickInfoVisible);
    }

    private void BogoInfoLink_Click(object sender, EventArgs e)
    {
      ToggleInfo(bogoInfoLabel, bogoInfoLink, ref bogoInfoVisible);
    }

    // РУЧНОЕ ДОБАВЛЕНИЕ СТРОКИ 
    private void AddRowButton_Click(object sender, EventArgs e)
    {
      if (inputGrid.Columns.Count == 0)
      {
        DataGridViewTextBoxColumn valueColumn = new DataGridViewTextBoxColumn();
        valueColumn.Name = "valueColumn";
        valueColumn.HeaderText = "Значение";
        valueColumn.Width = 120;
        inputGrid.Columns.Add(valueColumn);
      }

      inputGrid.Rows.Add();

      int lastRowIndex = inputGrid.Rows.Count - 1;
      if (lastRowIndex >= 0)
      {
        inputGrid.CurrentCell = inputGrid.Rows[lastRowIndex].Cells[0];
        inputGrid.BeginEdit(true);
      }
    }
    // ГЕНЕРАЦИЯ 
    private void GenerateButton_Click(object sender, EventArgs e)
    {
      elementsCountNumeric.Focus();
      elementsCountNumeric.ValidateChildren();
      Application.DoEvents();

      int elementsCount = (int)elementsCountNumeric.Value;

      if (elementsCount < 2)
      {
        MessageBox.Show(
          "Количество элементов должно быть не меньше 2.",
          "Некорректное значение",
          MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      if (elementsCount > 100)
      {
        MessageBox.Show(
          "Количество элементов должно быть не больше 100.",
          "Некорректное значение",
          MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      inputGrid.Rows.Clear();
      inputGrid.Columns.Clear();

      DataGridViewTextBoxColumn valueColumn = new DataGridViewTextBoxColumn();
      valueColumn.Name = "valueColumn";
      valueColumn.HeaderText = "Значение";
      valueColumn.Width = 120;
      inputGrid.Columns.Add(valueColumn);

      Random randomGenerator = new Random();
      for (int index = 0; index < elementsCount; index++)
      {
         // Дробное число в диапазоне [20.0; 50.0] с одним знаком после запятой
        double randomValue = Math.Round(MinRandomValue + randomGenerator.NextDouble() * (MaxRandomValue - MinRandomValue), 1);
        int rowIndex = inputGrid.Rows.Add();
        inputGrid.Rows[rowIndex].Cells[0].Value =
          randomValue.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture);
      }

      inputGrid.Refresh();
      inputGrid.Invalidate();

      logBox.Clear();
      logBox.AppendText("Сгенерировано " + elementsCount + " элементов в диапазоне [" +
        MinRandomValue + "; " + MaxRandomValue + "].\n");

      if (bogoCheckBox.Checked && elementsCount > BogoSafeSize)
      {
        MessageBox.Show(
          "BOGO при количестве элементов > " + BogoSafeSize +
          " может не завершиться за разумное время.",
          "Предупреждение",
          MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }

    // Чтение
     private double[] ReadValuesFromGrid()
    {
      List<double> parsedValues = new List<double>();
      var culture = System.Globalization.CultureInfo.InvariantCulture;

      foreach (DataGridViewRow gridRow in inputGrid.Rows)
      {
        if (gridRow.IsNewRow) continue;

        object cellValue = gridRow.Cells[0].Value;

        // Пропускаем пустые строки
        if (cellValue == null || string.IsNullOrWhiteSpace(cellValue.ToString()))
        {
          continue;
        }

        // Разрешаем и точку, и запятую как разделитель
        string raw = cellValue.ToString().Replace(',', '.');

        double parsedValue;
        if (!double.TryParse(raw, System.Globalization.NumberStyles.Float,
                             culture, out parsedValue))
        {
          throw new FormatException(
            "Некорректное значение в строке " + (gridRow.Index + 1) + ".");
        }

        parsedValues.Add(parsedValue);
      }

      if (parsedValues.Count < 2)
      {
        throw new InvalidOperationException("Необходимо минимум 2 элемента.");
      }

      return parsedValues.ToArray();
    }

    // Запуск
    private void RunButton_Click(object sender, EventArgs e)
    {
      double[] sourceValues;

      try
      {
        sourceValues = ReadValuesFromGrid();
      }
      catch (Exception exception)
      {
        MessageBox.Show(
          exception.Message,
          "Ошибка входных данных",
          MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      bool ascendingOrder = ascendingRadio.Checked;
      List<SortResult> results = new List<SortResult>();

      logBox.Clear();
      algorithmSnapshots.Clear();

      if (bubbleCheckBox.Checked)
        results.Add(SortingAlgorithms.BubbleSort(sourceValues, ascendingOrder));

      if (insertionCheckBox.Checked)
        results.Add(SortingAlgorithms.InsertionSort(sourceValues, ascendingOrder));

      if (shakerCheckBox.Checked)
        results.Add(SortingAlgorithms.ShakerSort(sourceValues, ascendingOrder));

      if (quickCheckBox.Checked)
        results.Add(SortingAlgorithms.QuickSort(sourceValues, ascendingOrder));

      if (bogoCheckBox.Checked)
      {
        if (sourceValues.Length > BogoSafeSize)
        {
          DialogResult userChoice = MessageBox.Show(
            "BOGO на " + sourceValues.Length + " элементах может не завершиться. Продолжить?",
            "BOGO",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

          if (userChoice == DialogResult.Yes)
          {
            results.Add(SortingAlgorithms.BogoSort(
              sourceValues, ascendingOrder, BogoIterationLimit));
          }
        }
        else
        {
          results.Add(SortingAlgorithms.BogoSort(
            sourceValues, ascendingOrder, BogoIterationLimit));
        }
      }

      PrintResults(sourceValues, ascendingOrder, results);
      visualizationPanel.Invalidate();
    }

    private void PrintResults(
      double[] sourceValues, bool ascendingOrder, List<SortResult> results)
    {
      string[] formattedSource = new string[sourceValues.Length];
      for (int i = 0; i < sourceValues.Length; i++)
      {
        formattedSource[i] = sourceValues[i].ToString("0.0");
      }
      logBox.AppendText("Исходный массив: [" + string.Join(", ", formattedSource) + "]\n");
      logBox.AppendText(
        "Порядок сортировки: " +
        (ascendingOrder ? "по возрастанию" : "по убыванию") + "\n\n");

      foreach (SortResult result in results)
      {
         string[] formattedValues = new string[result.SortedValues.Length];
        for (int i = 0; i < result.SortedValues.Length; i++)
        {
          formattedValues[i] = result.SortedValues[i].ToString("0.0");
        }

        logBox.AppendText(
          result.AlgorithmName.PadRight(14) + " | " +
          "Время: " + result.ElapsedMicroseconds.ToString().PadLeft(10) + " мкс (" +
          result.ElapsedTicks.ToString().PadLeft(10) + " тиков) | " +
          "Итераций: " + result.TotalIterations.ToString().PadLeft(8) + "\n" +
          "   Сравнений: " + result.ComparisonCount.ToString().PadLeft(8) + " | " +
          "Перестановок: " + result.SwapCount.ToString().PadLeft(8) + "\n" +
          "   Результат: [" + string.Join(", ", formattedValues) + "]\n\n");
      }

      if (results.Count > 0)
      {
        SortResult fastestResult = results.OrderBy(r => r.ElapsedTicks).First();
        logBox.AppendText(
          " Самый быстрый алгоритм: " + fastestResult.AlgorithmName +
          " (" + fastestResult.ElapsedMicroseconds + " мкс)\n");
      }
    }

    // Визуализация
    private void HandleSortStep(double[] snapshot, string algorithmName)
    {
      algorithmSnapshots[algorithmName] = snapshot;
    }

    private void VisualizationPanel_Paint(object sender, PaintEventArgs e)
    {
      if (algorithmSnapshots.Count == 0) return;

      Graphics graphics = e.Graphics;
      int panelWidth = visualizationPanel.Width;
      int panelHeight = visualizationPanel.Height;
      int rowHeight = panelHeight / Math.Max(1, algorithmSnapshots.Count);
      int rowIndex = 0;

       foreach (KeyValuePair<string, double[]> snapshotEntry in algorithmSnapshots)
      {
        double[] values = snapshotEntry.Value;
        if (values == null || values.Length == 0) continue;

        double maxValue = values.Max();
        if (maxValue <= 0) maxValue = 1;
        int barWidth = Math.Max(1, (panelWidth - 20) / values.Length);

        Color barColor;
        if (!algorithmColors.TryGetValue(snapshotEntry.Key, out barColor))
        {
          barColor = Color.Gray;
        }

        using (SolidBrush brush = new SolidBrush(barColor))
        using (Font labelFont = new Font("Segoe UI", 8))
        {
          string displayName;
          if (!algorithmRussianNames.TryGetValue(snapshotEntry.Key, out displayName))
          {
            displayName = snapshotEntry.Key;
          }

          graphics.DrawString(
            displayName, labelFont, Brushes.Black,
            4, rowIndex * rowHeight + 2);

          for (int valueIndex = 0; valueIndex < values.Length; valueIndex++)
          {
            int barHeight = (int)(
              (double)values[valueIndex] / maxValue * (rowHeight - 20));

            graphics.FillRectangle(
              brush,
              valueIndex * barWidth + 10,
              rowIndex * rowHeight + rowHeight - 5 - barHeight,
              barWidth - 2,
              barHeight);
          }
        }

        rowIndex++;
      }
    }

    // очистка
    private void ClearButton_Click(object sender, EventArgs e)
    {
      inputGrid.Rows.Clear();
      inputGrid.Columns.Clear();
      logBox.Clear();
      algorithmSnapshots.Clear();
      visualizationPanel.Invalidate();
    }

    // Exel
    private void LoadExcelButton_Click(object sender, EventArgs e)
    {
      MessageBox.Show(
        "Загрузка из Excel появится позже.",
        "Excel",
        MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    // Google
    private void LoadGoogleButton_Click(object sender, EventArgs e)
    {
      MessageBox.Show(
        "Загрузка из Google Sheets появится позже.",
        "Google Sheets",
        MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
  }
}