using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SqlClient;

namespace comoPode
{
    public partial class LoginForm : Form
    {
        private SqlConnection conexao;
        private string stringConexao = (@"Data Source=DESKTOP-7092F7U;Initial Catalog=SALARY_SYNC;Integrated Security=True");
        public LoginForm()
        {
            InitializeComponent();
            lbl_PreencherCampo1.Hide();
            lbl_PreencherCampo2.Hide();
            lbl_DadosIncorretos.Hide();
        }

        private void guna2ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string username = txtb_login.Text;
            string password = txtb_Senha.Text;


            if (txtb_login.Text == "")
            {
                lbl_PreencherCampo1.Show();
            }
            if (txtb_Senha.Text == "")
            {
                lbl_PreencherCampo2.Show();
            }
            else
            {
                try
                {
                    SqlConnection con = new SqlConnection("Data Source=DESKTOP-7092F7U;Initial Catalog=loginform;Integrated Security=True");
                    SqlCommand cmd = new SqlCommand("select * from tbl_login where username = @username and password = @password", con);
                    cmd.Parameters.AddWithValue("@username", txtb_login.Text);
                    cmd.Parameters.AddWithValue("@password", txtb_Senha.Text);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();

                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        username = dt.Rows[0]["username"].ToString();
                        UserContext.SetUser(username, username);

                        PagamentosForm f1 = new();
                        f1.StartPosition = FormStartPosition.Manual;
                        f1.Location = this.Location;
                        f1.Show();
                        this.Hide();
                    }
                    else
                    {
                        lbl_DadosIncorretos.Show();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("" + ex);
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void guna2GradientPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2HtmlLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2GradientPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void lbl_PreencherCampo_Click(object sender, EventArgs e)
        {

        }

        private void txtb_Senha_TextChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel4_Click(object sender, EventArgs e)
        {

        }

        private async void guna2Button1_Click_1(object sender, EventArgs e)
        {
            CriarContaForm f1 = new();
            f1.Opacity = 0;
            f1.StartPosition = FormStartPosition.Manual;
            f1.Location = this.Location;
            f1.Show();
            while (f1.Opacity < 1)
            {
                await Task.Delay(10);
                f1.Opacity += 00.5;
            }
            this.Hide();
        }

        private async void btn_EsqueciSenha_ClickAsync(object sender, EventArgs e)
        {
            EsqueciSenha f1 = new();
            f1.Opacity = 0;
            f1.StartPosition = FormStartPosition.Manual;
            f1.Location = this.Location;
            f1.Show();
            while (f1.Opacity < 1)
            {
                await Task.Delay(10);
                f1.Opacity += 00.5;
            }
            this.Hide();
        }
    }
}
