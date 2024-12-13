using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace labirint
{
    public partial class Form1 : Form
    {
        List<string> solutionRoutes = new List<string>();
        bool enter = false;
        bool exit = false;
        List<string> solutions = new List<string>();
        Dictionary<Point, Point> previousPoint = new Dictionary<Point, Point>();
        Dictionary<Point, Color> previousColors = new Dictionary<Point, Color>();

        public Form1()
        {
            InitializeComponent();
            int CountX = 10;
            int CountY = 10;
            dataGridView1.RowCount = CountY;
            dataGridView1.ColumnCount = CountX;
            for (int i = 0; i < CountX; i++) dataGridView1.Columns[i].Width = 30;
            for (int i = 0; i < CountY; i++) dataGridView1.Rows[i].Height = 30;
            for (int i = 0; i < CountX; i++)
            {
                for (int j = 0; j < CountY; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                }
            }
            listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
        }

        int cellRowRed = 10, cellColumnRed = 10, rowBlue = 0, columnBlue = 0, rowRed = 23 - 2, columnRed = 37 - 1, a = 10, b = 10;
        int[] stepX = { 0, 0, 1, -1 };
        int[] stepY = { 1, -1, 0, 0 };
        bool finish = false;

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1[e.ColumnIndex, e.RowIndex].Value != null)
            {
                if (dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString() == "2")
                {
                    columnBlue = dataGridView1.CurrentCell.ColumnIndex;
                    rowBlue = dataGridView1.CurrentCell.RowIndex;
                    dataGridView1.Rows[rowBlue].Cells[columnBlue].Style.BackColor = Color.Blue;
                    dataGridView1.Rows[rowBlue].Cells[columnBlue].Value = null;
                    enter = true;
                }
                else if (dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString() == "3")
                {
                    columnRed = dataGridView1.CurrentCell.ColumnIndex;
                    rowRed = dataGridView1.CurrentCell.RowIndex;
                    dataGridView1.Rows[rowRed].Cells[columnRed].Style.BackColor = Color.Red;
                    dataGridView1.Rows[rowRed].Cells[columnRed].Value = null;
                    exit = true;
                }
            }
        }

        private void dataGridView1_CellClick_2(object sender, DataGridViewCellEventArgs e)
        {
            int x = e.RowIndex;
            int y = e.ColumnIndex;
            if (dataGridView1.Rows[x].Cells[y].Style.BackColor == Color.White)
            {
                dataGridView1.Rows[x].Cells[y].Style.BackColor = Color.Black;
            }

            else if (dataGridView1.Rows[x].Cells[y].Style.BackColor == Color.Black)
            {
                dataGridView1.Rows[x].Cells[y].Style.BackColor = Color.White;
            }

            else if (dataGridView1.Rows[x].Cells[y].Style.BackColor == Color.Blue)
            {
                dataGridView1.Rows[x].Cells[y].Style.BackColor = Color.White;
            }

            else if (dataGridView1.Rows[x].Cells[y].Style.BackColor == Color.Red)
            {
                dataGridView1.Rows[x].Cells[y].Style.BackColor = Color.White;
            }
        }

        void SelectShortestRoute()
        {
            var shortestRoute = solutionRoutes
                .Zip(solutions, (route, solution) => new { Route = route, Solution = solution })
                .OrderBy(rs => int.Parse(rs.Solution.Split(':').Last()))
                .FirstOrDefault();

            if (shortestRoute != null)
            {
                foreach (string route in solutionRoutes)
                {
                    LoadRoute(route, Color.DarkBlue);
                }

                LoadRoute(shortestRoute.Route, Color.DarkBlue);

                listBox1.Items.Clear();
                listBox1.Items.Add(shortestRoute.Solution);
            }
            else
            {
                MessageBox.Show("Нет решения");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (!enter || !exit)
            {
                MessageBox.Show("Не задано начала или конца!");
            }
            else
            {
                dataGridView1.Rows[rowBlue].Cells[columnBlue].Value = 0;
                finish = false;

                AllRout(rowBlue, columnBlue, 0);

                SelectShortestRoute();
                for (int i = 0; i < a; i++)
                {
                    for (int j = 0; j < b; j++)
                    {
                        dataGridView1.Rows[i].Cells[j].Style.ForeColor = Color.Transparent;
                        if (dataGridView1.Rows[i].Cells[j].Style.BackColor == Color.Red)
                        {
                            dataGridView1.Rows[i].Cells[j].Style.ForeColor = Color.White;
                        }
                    }
                }
                listBox1.Items.Clear();
                foreach (string solution in solutions)
                {
                    listBox1.Items.Add(solution);
                }
                solutions.Clear();
            }
        }

        void AllRout(int row, int column, int shag)
        {
            if (dataGridView1.Rows[row].Cells[column].Style.BackColor == Color.Red)
            {
                dataGridView1.Rows[row].Cells[column].Value = shag;
                int steps = (int)dataGridView1.Rows[row].Cells[column].Value;
                solutions.Add($"Количество шагов: {steps}");
                Point current = new Point(row, column);
                List<Point> route = new List<Point>();

                if (!(row == rowRed && column == columnRed))
                {
                    route.Add(current);
                }

                while (previousPoint.ContainsKey(current))
                {
                    current = previousPoint[current];

                    if (!(current.X == rowBlue && current.Y == columnBlue))
                    {
                        route.Add(current);
                    }
                }

                route.Reverse();
                solutionRoutes.Add(string.Join(";", route.Select(p => $"{p.X},{p.Y}")));
            }
            else
            {
                for (int g = 0; g < 4; g++)
                {
                    int row1 = row + stepX[g];
                    int column1 = column + stepY[g];

                    if (row1 >= 0 && column1 >= 0 && row1 < a && column1 < b
                         && (dataGridView1.Rows[row1].Cells[column1].Style.BackColor == Color.Black || dataGridView1.Rows[row1].Cells[column1].Style.BackColor == Color.Red)
                         && dataGridView1.Rows[row1].Cells[column1].Value == null)
                    {
                        previousPoint[new Point(row1, column1)] = new Point(row, column);
                        dataGridView1.Rows[row1].Cells[column1].Value = shag + 1;
                        AllRout(row1, column1, shag + 1);
                        dataGridView1.Rows[row1].Cells[column1].Value = null;
                        previousPoint.Remove(new Point(row1, column1));
                    }
                }
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Текстовый документ (*.txt)|*.txt";
            if (save.ShowDialog() == DialogResult.OK)
            {
                StreamWriter SW = new StreamWriter(save.FileName);
                for (int i = 0; i < a; i++)
                {
                    string text = "";
                    for (int j = 0; j < b; j++)
                    {
                        if (dataGridView1.Rows[i].Cells[j].Style.BackColor == Color.White) text += 0;
                        if (dataGridView1.Rows[i].Cells[j].Style.BackColor == Color.Black) text += 1;
                        if (dataGridView1.Rows[i].Cells[j].Style.BackColor == Color.Blue) text += 2;
                        if (dataGridView1.Rows[i].Cells[j].Style.BackColor == Color.Red) text += 3;
                        if (dataGridView1.Rows[i].Cells[j].Style.BackColor == Color.Green) text += 1;
                    }
                    SW.WriteLine(text);
                }
                SW.Close();
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < a; i++)
            {
                for (int j = 0; j < b; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                    dataGridView1.Rows[i].Cells[j].Value = null;
                }
            }
            solutionRoutes.Clear();
            solutions.Clear();
            listBox1.Items.Clear();
            Application.Restart();
        }
        private void Button5_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < a; i++)
            {
                for (int j = 0; j < b; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = null;
                }
            }
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Текстовый документ (*.txt)|*.txt";
            if (open.ShowDialog() == DialogResult.OK)
            {
                StreamReader SW = new StreamReader(open.FileName);
                string[] text = File.ReadAllLines(open.FileName);
                for (int i = 0; i < a; i++)
                {
                    int j = 0;
                    foreach (char f in text[i])
                    {
                        if (f == '0') dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                        if (f == '1') dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Black;
                        if (f == '2')
                        {
                            dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Blue;
                            enter = true;
                            rowBlue = i;
                            columnBlue = j;
                        }
                        if (f == '3')
                        {
                            dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Red;
                            exit = true;
                        }
                        j++;
                    }
                }
                SW.Close();
            }
        }

        void LoadRoute(string solution, Color color)
        {
            foreach (var pair in previousColors)
            {
                dataGridView1.Rows[pair.Key.X].Cells[pair.Key.Y].Style.BackColor = pair.Value;
            }
            previousColors.Clear();

            foreach (string point in solution.Split(';'))
            {
                int row = Convert.ToInt32(point.Split(',')[0]);
                int column = Convert.ToInt32(point.Split(',')[1]);

                previousColors[new Point(row, column)] = dataGridView1.Rows[row].Cells[column].Style.BackColor;

                if (row == rowBlue && column == columnBlue)
                {
                    dataGridView1.Rows[row].Cells[column].Style.BackColor = Color.Blue;
                }
                else if (row == rowRed && column == columnRed)
                {
                    dataGridView1.Rows[row].Cells[column].Style.BackColor = Color.Red;
                }
                else
                {
                    dataGridView1.Rows[row].Cells[column].Style.BackColor = color;
                }
            }
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0 && listBox1.SelectedIndex < solutionRoutes.Count)
            {
                LoadRoute(solutionRoutes[listBox1.SelectedIndex], Color.DarkBlue);
            }
        }
    }
}
