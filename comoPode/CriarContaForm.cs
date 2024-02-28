using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace comoPode
{
    public partial class CriarContaForm : Form
    {
        private SqlConnection conexao;
        private string stringConexao = (@"Data Source=DESKTOP-7092F7U;Initial Catalog=SALARY_SYNC;Integrated Security=True");
        public CriarContaForm()
        {
            InitializeComponent();

            lbl_PreencherCampo1.Hide();
            lbl_PreencherCampo2.Hide();
            lbl_PreencherCampo3.Hide();
            lbl_PreencherCampo4.Hide();

            lbl_SenhasDiferentes.Hide();

            lbl_CadastroSucesso.Hide();

            lbl_JaUtilizado.Hide();
            lbl_JaUtilizado2.Hide();
        }

        private void txtb_login_TextChanged(object sender, EventArgs e)
        {

        }

        private async void btn_Cadastrar_Click(object sender, EventArgs e)
        {
            if (txtb_login.Text == "")
            {
                lbl_PreencherCampo1.Show();
            }
            if (txtb_Email.Text == "")
            {
                lbl_PreencherCampo2.Show();
            }
            if (txtb_Senha1.Text == "")
            {
                lbl_PreencherCampo3.Show();
            }
            if (txtb_Senha2.Text == "")
            {
                lbl_PreencherCampo4.Show();
            }
            if (txtb_Senha1.Text != txtb_Senha2.Text)
            {
                lbl_SenhasDiferentes.Show();
            }

            else
            {
                try
                {
                    SqlConnection con = new("Data Source=DESKTOP-7092F7U;Initial Catalog=loginform;Integrated Security=True");
                    con.Open();
                    string query = "select count(*) from tbl_login where username = @username";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@username", txtb_login.Text);
                        int count = (int)cmd.ExecuteScalar();

                        if (count > 0)
                        {
                            lbl_JaUtilizado2.Show();
                            return;
                        }
                    }
                    string query1 = "select count(*) from tbl_login where email = @email";
                    using (SqlCommand cmd = new SqlCommand(query1, con))
                    {
                        cmd.Parameters.AddWithValue("@email", txtb_Email.Text);
                        int count = (int)cmd.ExecuteScalar();

                        if (count > 0)
                        {
                            lbl_JaUtilizado.Show();
                            return;
                        }
                    }
                    using (SqlCommand cmd = new("insert into tbl_login (username, password, email) values (@username, @password, @email)", con))
                    {
                        cmd.Parameters.AddWithValue("@username", txtb_login.Text);
                        cmd.Parameters.AddWithValue("@password", txtb_Senha1.Text);
                        cmd.Parameters.AddWithValue("@email", txtb_Email.Text);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        con.Close();
                        if (rowsAffected > 0)
                        {
                            lbl_CadastroSucesso.Show();

                        }
                        else
                        {
                            MessageBox.Show("Erro");
                        }
                    }



                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            await Task.Delay(3000);
            LoginForm f1 = new();
            f1.StartPosition = FormStartPosition.Manual;
            f1.Location = this.Location;
            f1.Show();
            this.Hide();
        }

        private void btn_TenhoConta_Click(object sender, EventArgs e)
        {
            LoginForm f1 = new();
            f1.StartPosition = FormStartPosition.Manual;
            f1.Location = this.Location;
            f1.Show();
            this.Hide();
        }

        private void btn_Fechar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void txtb_Email_TextChanged(object sender, EventArgs e)
        {

        }

        private void lbl_CadastroSucesso_Click(object sender, EventArgs e)
        {

        }
    }
}
