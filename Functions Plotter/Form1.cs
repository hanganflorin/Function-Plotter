using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Functions_Plotter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Label[] labelFnc;
        TextBox[] textBoxFnc;
        CheckBox[] checkBoxFnc;
        Color[] colorFnc;
        Color BackgroundColor, AxisColor, GridColor;
        Expression[] E;
        bool pressed;
        const float resolution = 0.01F;
        int index;
        float Xmin, Xmax, Ymin, Ymax, Xzero, Yzero, Xdim, Ydim, W, H, Xstep, Ystep;
        float Xmin_initial, Xmax_initial, Ymin_initial, Ymax_initial;
        int cursorX_initial, cursorY_initial;
        float variable_a = 0, variable_b = 0, variable_c = 0, variable_d = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            E = new Expression[7];
            for (int i = 0; i < 7; ++i )
                E[i] = new Expression("n", 0);
            InsertControls();
            W = pictureBox_Surface.Width;
            H = pictureBox_Surface.Height;
            BackgroundColor = Color.Black;
            AxisColor = Color.White;
            GridColor = AxisColor;
            GridColor = Color.FromArgb(30, GridColor.R, GridColor.G, GridColor.B);
            Xstep = 1;
            Ystep = 2;
            this.DoubleBuffered = true;
            button_Apply.PerformClick();
            button_Apply2.PerformClick();
            

        }
        private void InsertControls()
        {
            labelFnc = new Label[7];
            textBoxFnc = new TextBox[7];
            checkBoxFnc = new CheckBox[7];
            colorFnc = new Color[10];
            colorFnc[0] = Color.Red;
            colorFnc[1] = Color.LawnGreen;
            colorFnc[2] = Color.Blue;
            colorFnc[3] = Color.Fuchsia;
            colorFnc[4] = Color.Pink;
            colorFnc[5] = Color.Coral;
            colorFnc[6] = Color.YellowGreen;


            for (int i = 0; i < 7; ++i)
            {
                //declarare
                labelFnc[i] = new Label();
                textBoxFnc[i] = new TextBox();
                checkBoxFnc[i] = new CheckBox();

                //text
                labelFnc[i].Text = string.Format("f{0}(x) = ", i + 1);
                textBoxFnc[i].Text = "0";
                checkBoxFnc[i].Text = "draw";


                //culoarea
                labelFnc[i].ForeColor = colorFnc[i];
                checkBoxFnc[i].ForeColor = colorFnc[i];
                textBoxFnc[i].Size = new System.Drawing.Size(130, 20);

                //pozitia
                labelFnc[i].Location = new Point(10, 30 * i + 10);
                textBoxFnc[i].Location = new Point(55, 30 * i + 7);
                checkBoxFnc[i].Location = new Point(190, 30 * i + 8);

                // afisare
                panel1.Controls.Add(labelFnc[i]);
                panel1.Controls.Add(textBoxFnc[i]);
                panel1.Controls.Add(checkBoxFnc[i]);

                //evenimente
                textBoxFnc[i].KeyDown += new KeyEventHandler(TextBox_KeyDownFnc);
                checkBoxFnc[i].CheckedChanged += new EventHandler(checkBox_CheckedChanged);

                //altele
                labelFnc[i].AutoSize = true;
                checkBoxFnc[i].AutoSize = true;
                checkBoxFnc[i].Checked = false;
                textBoxFnc[i].TabIndex = 1000 + i;
                labelFnc[i].ContextMenuStrip = contextMenuStrip1;

            }
            checkBoxFnc[0].Checked = true;
        }
        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            button_Draw.PerformClick();
        }
        private void TextBox_KeyDownFnc(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button_Draw.PerformClick();
        }
        private void Examples_Click(object sender, EventArgs e)
        {
            textBoxFnc[index].Text = sender.ToString();
            button_Draw.PerformClick();
        }
        
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            Control control_MenuStrip = contextMenuStrip1.SourceControl as Control;
            index = control_MenuStrip.Text[1] - 49;
        }

        private void changeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ( colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK )
            {
                labelFnc[index].ForeColor = colorDialog1.Color;
                checkBoxFnc[index].ForeColor = colorDialog1.Color;
                colorFnc[index] = colorDialog1.Color;
                pictureBox_Surface.Refresh();
            }
        }

        private void button_Apply_Click(object sender, EventArgs e)
        {
            try
            {
                Xmin = Convert.ToSingle(textBox_Xmin.Text);
                Xmax = Convert.ToSingle(textBox_Xmax.Text);
                Ymin = Convert.ToSingle(textBox_Ymin.Text);
                Ymax = Convert.ToSingle(textBox_Ymax.Text);
                UpdateSteps();
                pictureBox_Surface.Refresh();
            }
            catch
            {
                MessageBox.Show("Wrong Data!", "Error");
            }
        }

        private void pictureBox_Surface_Paint(object sender, PaintEventArgs e)
        {
            
            Xdim = W / (Xmax - Xmin);
            Ydim = H / (Ymax - Ymin);
            Xzero = -Xmin * Xdim;
            Yzero = Ymax * Ydim;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            if ( checkBox_Grid.Checked ) // desenam grid-ul
            {
                for (float i = 0; i <= Xmax; i += Xstep)
                    e.Graphics.DrawLine(new Pen(GridColor, 1), Xzero + i * Xdim, 0 , Xzero + i * Xdim, H);
                for (float i = 0; i >= Xmin; i -= Xstep)
                    e.Graphics.DrawLine(new Pen(GridColor, 1), Xzero + i * Xdim, 0, Xzero + i * Xdim, H);
                for (float i = 0; i <= Ymax; i += Ystep)
                    e.Graphics.DrawLine(new Pen(GridColor, 1), 0, Yzero - i * Ydim, W, Yzero - i * Ydim);
                for (float i = 0; i >= Ymin; i -= Ystep)
                    e.Graphics.DrawLine(new Pen(GridColor, 1), 0, Yzero - i * Ydim, W, Yzero - i * Ydim);
            }

            if ( radioButton_NoAxis.Checked == false )
            {
                if (Xmin <= 0 && Xmax >= 0)
                    e.Graphics.DrawLine(new Pen(AxisColor, 1), Xzero, 0, Xzero, H); //desenam cele doua axe
                if (Ymin <= 0 && Ymax >= 0)
                    e.Graphics.DrawLine(new Pen(AxisColor, 1), 0, Yzero, W, Yzero);
                if (radioButton_SimpleAxis.Checked == false) // desenam gradatiile
                {
                    for (float i = Xstep; i <= Xmax; i += Xstep)
                        e.Graphics.DrawLine(new Pen(AxisColor, 1), Xzero + i * Xdim, Yzero - 5, Xzero + i * Xdim, Yzero + 5);
                    for (float i = -Xstep; i >= Xmin; i -= Xstep)
                        e.Graphics.DrawLine(new Pen(AxisColor, 1), Xzero + i * Xdim, Yzero - 5, Xzero + i * Xdim, Yzero + 5);
                    for (float i = Ystep; i <= Ymax; i += Ystep)
                        e.Graphics.DrawLine(new Pen(AxisColor, 1), Xzero - 5, Yzero - i * Ydim, Xzero + 5, Yzero - i * Ydim);
                    for (float i = -Ystep; i >= Ymin; i -= Ystep)
                        e.Graphics.DrawLine(new Pen(AxisColor, 1), Xzero - 5, Yzero - i * Ydim, Xzero + 5, Yzero - i * Ydim); 
                }
            }


            if (radioButton_AxisMArksValues.Checked ) // desenam valorile
            {
                StringFormat str = new StringFormat();
                float y = Yzero-20;
                float x = Xzero + 10;
                float aux;
                if (y <= 3)
                    y = 3;
                if (y >= H - 15)
                    y = H - 15;
                if (x <= 3)
                    x = 3;
                str.Alignment = StringAlignment.Center;
                for (float i = Xstep; i <= Xmax; i += Xstep)
                {
                    aux = (float)Math.Round(i, 2);
                    e.Graphics.DrawString(String.Format("{0}", aux), new Font("Microsoft Sans Serif", 7), new SolidBrush(AxisColor), Xzero + i * Xdim, y, str);
                }
                for (float i = -Xstep; i >= Xmin; i -= Xstep)
                {
                    aux = (float)Math.Round(i, 2);
                    e.Graphics.DrawString(String.Format("{0}", aux), new Font("Microsoft Sans Serif", 7), new SolidBrush(AxisColor), Xzero + i * Xdim, y, str);
                }
                str.Alignment = StringAlignment.Near;
                for (float i = Ystep; i <= Ymax; i += Ystep)
                {
                    if (x >= W - 5)
                    {
                        x = W - 5;
                        str.Alignment = StringAlignment.Far;
                    }
                    else
                        str.Alignment = StringAlignment.Near;
                    aux = (float)Math.Round(i, 2);
                    e.Graphics.DrawString(String.Format("{0}", aux), new Font("Microsoft Sans Serif", 7), new SolidBrush(AxisColor), x, Yzero - i * Ydim - 7, str);
                }
                for (float i = -Ystep; i >= Ymin; i -= Ystep)
                {
                    if (x >= W - 5)
                    {
                        x = W - 5;
                        str.Alignment = StringAlignment.Far;
                    }
                    else
                        str.Alignment = StringAlignment.Near;
                    aux = (float)Math.Round(i, 2);
                    e.Graphics.DrawString(String.Format("{0}", aux), new Font("Microsoft Sans Serif", 7), new SolidBrush(AxisColor), x, Yzero - i * Ydim - 7, str);

                }
            }

            //desenam functiile
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; 
            float x1, y1, x2, y2;
            for ( int i = 0; i < 7; ++i )
                if ( checkBoxFnc[i].Checked )
                {
                    x1 = Xmin;
                    y1 = E[i].Calculate(x1, variable_a, variable_b, variable_c, variable_d);
                    for (x2 = Xmin + resolution; x2 <= Xmax + 50; x2 += resolution) // pentru fiecare functie parcurgem tot intervalul [Xmin, Xmax]
                    {                                                               //si calculam valoarea functiei in punctul respectiv
                        y2 = E[i].Calculate(x2, variable_a, variable_b, variable_c, variable_d);
                        if (!double.IsNaN(y2) && !double.IsInfinity(y2) )
                        {
                            if (y1 >= Ymin - 100 && y1 <= Ymax + 100 && y2 >= Ymin - 100 && y2 <= Ymax + 100)
                                e.Graphics.DrawLine(new Pen(colorFnc[i], 2), Xzero + x1 * Xdim, Yzero - y1 * Ydim, Xzero + x2 * Xdim, Yzero - y2 * Ydim);
                            x1 = x2;
                            y1 = y2;
                        }
                    }
                }
        }

        private void button_Draw_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 7; ++i )
                if ( checkBoxFnc[i].Checked )
                {
                    try
                    {
                        E[i] = E[i].FromString(textBoxFnc[i].Text);
                    }
                    catch
                    {
                        MessageBox.Show("Expression \"" + textBoxFnc[i].Text + "\" is incorrect");
                    }
                    
                }
            pictureBox_Surface.Refresh();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            W = pictureBox_Surface.Width;
            H = pictureBox_Surface.Height;
            UpdateSteps();
            pictureBox_Surface.Refresh();
        }

        private void radioButtons_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox_Surface.Refresh();
        }

       

        private void pictureBox_Surface_MouseUp(object sender, MouseEventArgs e)
        {
            pressed = false;
        }

        private void pictureBox_Surface_MouseDown(object sender, MouseEventArgs e)
        {
            //cand se apasa click se retin valorile
            cursorX_initial = e.X;
            cursorY_initial = e.Y;
            Xmin_initial = Xmin;
            Xmax_initial = Xmax;
            Ymin_initial = Ymin;
            Ymax_initial = Ymax;
            pressed = true;
        }
        private void pictureBox_Surface_MouseMove(object sender, MouseEventArgs e)
        {
            if (pressed)
            {
                //
                float x = cursorX_initial - e.X;
                float y = cursorY_initial - e.Y;
                Xmax = Xmax_initial + x / Xdim;
                Xmin = Xmin_initial + x / Xdim;
                Ymax = Ymax_initial - y / Ydim;
                Ymin = Ymin_initial - y / Ydim;

                textBox_Xmin.Text = Xmin.ToString();
                textBox_Xmax.Text = Xmax.ToString();
                textBox_Ymin.Text = Ymin.ToString();
                textBox_Ymax.Text = Ymax.ToString();
                pictureBox_Surface.Refresh();
            }
        }

        private void button_ToCentre_Click(object sender, EventArgs e)
        {
            float aux = Xmax - Xmin;
            Xmax = aux / 2;
            Xmin = -aux / 2;
            aux = Ymax - Ymin;
            Ymax = aux / 2;
            Ymin = -aux / 2;
            textBox_Xmin.Text = Xmin.ToString();
            textBox_Xmax.Text = Xmax.ToString();
            textBox_Ymin.Text = Ymin.ToString();
            textBox_Ymax.Text = Ymax.ToString();
            pictureBox_Surface.Refresh();
        }
        private void pictureBox_Surface__MouseWheel(object sender, MouseEventArgs e)
        {
            float Xval = 0, Yval = 0;
            int sign = 0;
            if (radioButton_Horizontal.Checked == false)
                Yval = (Ymax - Ymin) * 0.025F;
            if (radioButton_Vertical.Checked == false)
                Xval = (Xmax - Xmin) * 0.025F;
            if (e.Delta < 0)
                sign = -1;
            else if ( Xstep > 0.01F )
                sign = 1;
            if (sign != 0)
            {
                Xmax -= sign * Xval;
                Xmin += sign * Xval;
                Ymax -= sign * Yval;
                Ymin += sign * Yval;

                UpdateSteps();
                textBox_Xmin.Text = Xmin.ToString("F3");
                textBox_Xmax.Text = Xmax.ToString("F3");
                textBox_Ymin.Text = Ymin.ToString("F3");
                textBox_Ymax.Text = Ymax.ToString("F3");
                pictureBox_Surface.Refresh();
            }
        }
        private void UpdateSteps()
        {
            int min = 40, max = 60;
            if (Xstep * Xdim < min) // out
                Xstep += Xstep * 0.3F;
            if (Xstep * Xdim > max) // in
                Xstep -= Xstep * 0.3F;
            if (Ystep * Ydim < min)
                Ystep += Ystep * 0.3F;
            if (Ystep * Ydim > max)
                Ystep -= Ystep * 0.3F;

        }
        private void pictureBox_Surface_Click(object sender, EventArgs e)
        {
            pictureBox_Surface.Focus();
        }
        private void button_Reset_Click(object sender, EventArgs e)
        {
            Xmin = -10;
            Xmax = 10;
            Ymin = -10;
            Ymax = 10;
            Xstep = 1;
            Ystep = 2;
            textBox_Xmin.Text = Xmin.ToString();
            textBox_Xmax.Text = Xmax.ToString();
            textBox_Ymin.Text = Ymin.ToString();
            textBox_Ymax.Text = Ymax.ToString();
            pictureBox_Surface.Refresh();
        }

        private void button_Apply2_Click(object sender, EventArgs e)
        {
            trackBar_a.Maximum = 10 * Convert.ToInt32(textBox_Max_a.Text);
            trackBar_a.Minimum = 10 * Convert.ToInt32(textBox_Min_a.Text);
            trackBar_b.Maximum = 10 * Convert.ToInt32(textBox_Max_b.Text);
            trackBar_b.Minimum = 10 * Convert.ToInt32(textBox_Min_b.Text);
            trackBar_c.Maximum = 10 * Convert.ToInt32(textBox_Max_c.Text);
            trackBar_c.Minimum = 10 * Convert.ToInt32(textBox_Min_c.Text);
            trackBar_d.Maximum = 10 * Convert.ToInt32(textBox_Max_d.Text);
            trackBar_d.Minimum = 10 * Convert.ToInt32(textBox_Min_d.Text);
            trackBar_a.Value = Convert.ToInt32(Convert.ToSingle(textBox_Value_a.Text) * 10);
            trackBar_b.Value = Convert.ToInt32(Convert.ToSingle(textBox_Value_b.Text) * 10);
            trackBar_c.Value = Convert.ToInt32(Convert.ToSingle(textBox_Value_c.Text) * 10);
            trackBar_d.Value = Convert.ToInt32(Convert.ToSingle(textBox_Value_d.Text) * 10);
            pictureBox_Surface.Refresh();
        }

        private void TrackBar_ValueChange(object sender, EventArgs e)
        {
            variable_a = trackBar_a.Value * 0.1F;
            variable_b = trackBar_b.Value * 0.1F;
            variable_c = trackBar_c.Value * 0.1F;
            variable_d = trackBar_d.Value * 0.1F;
            textBox_Value_a.Text = variable_a.ToString("F1");
            textBox_Value_b.Text = variable_b.ToString("F1");
            textBox_Value_c.Text = variable_c.ToString("F1");
            textBox_Value_d.Text = variable_d.ToString("F1");
            
            pictureBox_Surface.Refresh();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button_Apply2.PerformClick();
        }

        private void pictureBox_BackgroundColor_Click(object sender, EventArgs e)
        {
            if ( colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK )
            {
                BackgroundColor = colorDialog1.Color;
                pictureBox_BackgroundColor.BackColor = BackgroundColor;
                pictureBox_Surface.BackColor = BackgroundColor;
                pictureBox_Surface.Refresh();
            }
        }

        private void pictureBox_AxisColor_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AxisColor = colorDialog1.Color;
                GridColor = AxisColor;
                GridColor = Color.FromArgb(30, GridColor.R, GridColor.G, GridColor.B);
                pictureBox_AxisColor.BackColor = AxisColor;
                pictureBox_Surface.Refresh();
            }
        }
   
    }
    
}
