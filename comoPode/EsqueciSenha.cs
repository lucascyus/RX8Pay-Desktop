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
    public partial class EsqueciSenha : Form
    {
        public EsqueciSenha()
        {
            InitializeComponent();

            lbl_PreencherCampo1.Hide();
            lbl_PreencherCampo2.Hide();
            lbl_PreencherCampo3.Hide();

            lbl_DadosIncorretos.Hide();

            lbl_SenhaRedefinida.Hide();
        }

        private async void btn_RedefinirSenha_ClickAsync(object sender, EventArgs e)
        {
            if (txtb_login.Text == "")
            {
                lbl_PreencherCampo1.Show();
            }
            if (txtb_Email.Text == "")
            {
                lbl_PreencherCampo2.Show();
            }
            if (txtb_NovaSenha.Text == "")
            {
                lbl_PreencherCampo3.Show();
            }
            else
            {
                try
                {
                    SqlConnection con = new SqlConnection("Data Source=DESKTOP-7092F7U;Initial Catalog=loginform;Integrated Security=True");
                    con.Open();
                    string query = "update tbl_login set password = @password where username = @username and email = @email";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@password", txtb_NovaSenha.Text);
                        cmd.Parameters.AddWithValue("@username", txtb_login.Text);
                        cmd.Parameters.AddWithValue("@email", txtb_Email.Text);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            lbl_SenhaRedefinida.Show();
                            await Task.Delay(3000);
                            LoginForm f1 = new();
                            f1.StartPosition = FormStartPosition.Manual;
                            f1.Location = this.Location;
                            f1.Show();
                            this.Hide();
                        }
                        else
                        {
                            lbl_DadosIncorretos.Show();
                            await Task.Delay(10000);
                            lbl_DadosIncorretos.Hide();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("" + ex);
                }
            }
        }

        private void btn_Fechar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btn_LembreiSenha_Click(object sender, EventArgs e)
        {
            LoginForm f1 = new();
            f1.StartPosition = FormStartPosition.Manual;
            f1.Location = this.Location;
            f1.Show();
            this.Hide();
        }
    }
}
