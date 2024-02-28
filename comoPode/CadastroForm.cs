using Guna.UI2.WinForms;
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
    public partial class CadastroForm : Form
    {
        private int lastInsertedId;
        private const string connectionString = "Data Source=DESKTOP-7092F7U;Initial Catalog=SALARY_SYNC;Integrated Security=True";
        private string codigoAleatorio;
        string perfil = UserContext.Username;


        public CadastroForm()
        {

            InitializeComponent();

            pnl_Sair.Hide();

            btn_Ocultar.Hide();

            lbl_JaVinculado1.Hide();
            lbl_JaVinculado2.Hide();
            lbl_JaVinculado3.Hide();
            lbl_JaVinculado4.Hide();
            lbl_JaVinculado5.Hide();
            lbl_JaVinculado6.Hide();

            lbl_PreencherCampo1.Hide();
            lbl_PreencherCampo2.Hide();
            lbl_PreencherCampo3.Hide();
            lbl_PreencherCampo4.Hide();
            lbl_PreencherCampo5.Hide();
            lbl_PreencherCampo6.Hide();
            lbl_PreencherCampo7.Hide();
            lbl_PreencherCampo9.Hide();
            lbl_PreencherCampo10.Hide();
            lbl_PreencherCampo11.Hide();
            lbl_PreencherCampo12.Hide();
            lbl_PreencherCampo13.Hide();
            lbl_PreencherCampo14.Hide();
            lbl_PreencherCampo15.Hide();
            lbl_PreencherCampo16.Hide();
            lbl_PreencherCampo17.Hide();
            lbl_PreencherCampo18.Hide();
        }

        private void btn_Fechar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pnl_Cadastro_Paint(object sender, PaintEventArgs e)
        {

        }



        private void btn_Salvar_Click(object sender, EventArgs e)
        {
            if (txtb_NomeCompleto.Text == "")
            {
                lbl_PreencherCampo1.Show();
            }
            if (txtb_Cpf.Text == "")
            {
                lbl_PreencherCampo3.Show();
            }
            if (txtb_Rg.Text == "")
            {
                lbl_PreencherCampo4.Show();
            }
            if (txtb_Sexo.Text == "")
            {
                lbl_PreencherCampo5.Show();
            }
            if (txtb_Ctps.Text == "")
            {
                lbl_PreencherCampo6.Show();
            }
            if (txtb_TituloEleitor.Text == "")
            {
                lbl_PreencherCampo7.Show();
            }
            if (txtb_Telefone.Text == "")
            {
                lbl_PreencherCampo9.Show();
            }
            if (txtb_Email.Text == "")
            {
                lbl_PreencherCampo10.Show();
            }
            if (txtb_Cep.Text == "")
            {
                lbl_PreencherCampo11.Show();
            }
            if (txtb_NomeBanco.Text == "")
            {
                lbl_PreencherCampo11.Show();
            }
            if (txtb_NumeroConta.Text == "")
            {
                lbl_PreencherCampo12.Show();
            }
            if (txtb_NumeroBanco.Text == "")
            {
                lbl_PreencherCampo13.Show();
            }
            if (txtb_AgenciaBancaria.Text == "")
            {
                lbl_PreencherCampo14.Show();
            }
            if (txtb_TipoConta.Text == "")
            {
                lbl_PreencherCampo15.Show();
            }
            if (cmbx_Cargo.Text == "")
            {
                lbl_PreencherCampo16.Show();
            }
            if (txtb_Salario.Text == "")
            {
                lbl_PreencherCampo17.Show();
            }
            if (CodigoExisteNoBancoDeDados(codigoAleatorio))
            {
                CadastroForm f1 = new();
                f1.Opacity = 0;
                f1.StartPosition = FormStartPosition.Manual;
                f1.Location = this.Location;
                f1.Show();
                this.Hide();
            }
            if (CpfVinculado(txtb_Cpf.Text))
            {
                lbl_JaVinculado1.Show();
            }
            if (RgVinculado(txtb_Rg.Text))
            {
                lbl_JaVinculado2.Show();
            }
            if (CtpsVinculado(txtb_Ctps.Text))
            {
                lbl_JaVinculado3.Show();
            }
            if (TituloVinculado(txtb_TituloEleitor.Text))
            {
                lbl_JaVinculado4.Show();
            }
            else
            {
                // Inserir o código gerado no banco de dados
                InserirCodigoNoBancoDeDados(codigoAleatorio);
                MessageBox.Show("Código inserido com sucesso no banco de dados.");
            }

        }

        private void InserirCodigoNoBancoDeDados(string codigo)
        {

            string escolha = cmbx_Cargo.SelectedItem.ToString();

            double salario = 0.00;

            if (escolha == "Assistente Administrativo")
            {
                salario = 2560.30;
            }
            if (escolha == "Assistente de Vendas")
            {
                salario = 1520.60;
            }

            if (escolha == "Atendente de Suporte ao Cliente")
            {
                salario = 2100.50;
            }

            if (escolha == "Auxiliar de Estoque")
            {
                salario = 1340.12;
            }

            if (escolha == "Recepcionista")
            {
                salario = 1403.00;
            }

            if (escolha == "Assistende de Marketing")
            {
                salario = 1660.90;
            }

            if (escolha == "Auxiliar de Contabilidade")
            {
                salario = 2433.21;
            }

            if (escolha == "Operador de Caixa")
            {
                salario = 1413.00;
            }

            if (escolha == "Assistente de Recursos Humanos")
            {
                salario = 1890.06;
            }

            if (escolha == "Auxiliar de Limpeza")
            {
                salario = 1320.00;
            }

            string nome = txtb_NomeCompleto.Text;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query1 = "INSERT INTO tbl_funcionario (Id, Nome, DataNascimento, Cpf, Rg, Sexo, Ctps, TituloEleitor, Reservista, Telefone, Email, Endereco, Cargo, Salario, DataAdmissao, Cep) VALUES (@Id, @Nome, @DataNascimento, @Cpf, @Rg, @Sexo, @Ctps, @TituloEleitor, @Reservista, @Telefone, @Email, @Endereco, @Cargo, @Salario, @DataAdmissao, @Cep)";
                using (SqlCommand command = new SqlCommand(query1, connection))
                {
                    command.Parameters.AddWithValue("@Id", codigo);
                    command.Parameters.AddWithValue("@Nome", txtb_NomeCompleto.Text);
                    command.Parameters.AddWithValue("@DataNascimento", dtm_DataNascimento.Value);
                    command.Parameters.AddWithValue("@Cpf", txtb_Cpf.Text);
                    command.Parameters.AddWithValue("@Rg", txtb_Rg.Text);
                    command.Parameters.AddWithValue("@Sexo", txtb_Sexo.Text);
                    command.Parameters.AddWithValue("@Ctps", txtb_Ctps.Text);
                    command.Parameters.AddWithValue("@TituloEleitor", txtb_TituloEleitor.Text);
                    command.Parameters.AddWithValue("@Reservista", txtb_Reservista.Text);
                    command.Parameters.AddWithValue("@Telefone", txtb_Telefone.Text);
                    command.Parameters.AddWithValue("@Email", txtb_Email.Text);
                    command.Parameters.AddWithValue("@Cep", txtb_Cep.Text);
                    command.Parameters.AddWithValue("@Endereco", txtb_Endereco.Text);
                    command.Parameters.AddWithValue("@Cargo", cmbx_Cargo.Text);
                    command.Parameters.AddWithValue("@Salario", salario);
                    command.Parameters.AddWithValue("@DataAdmissao", dtm_DataAdmissao.Value);
                    command.ExecuteNonQuery();
                }
                string query2 = "INSERT INTO tbl_banco (id, Nome, NumeroConta, NumeroBanco, AgenciaBancaria, TipoConta) values (@id, @Nome, @NumeroConta, @NumeroBanco, @AgenciaBancaria, @TipoConta)";
                using (SqlCommand command2 = new SqlCommand(query2, connection))
                {
                    command2.Parameters.AddWithValue("@Id", codigo);
                    command2.Parameters.AddWithValue("@Nome", txtb_NomeBanco.Text);
                    command2.Parameters.AddWithValue("@NumeroConta", txtb_NumeroConta.Text);
                    command2.Parameters.AddWithValue("@NumeroBanco", txtb_NumeroBanco.Text);
                    command2.Parameters.AddWithValue("@AgenciaBancaria", txtb_AgenciaBancaria.Text);
                    command2.Parameters.AddWithValue("@TipoConta", txtb_TipoConta.Text);
                    command2.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        private string GerarCodigoAleatorio(int tamanho)
        {
            const string caracteresPermitidos = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            char[] codigo = new char[tamanho];

            for (int i = 0; i < tamanho; i++)
            {
                int indice = random.Next(caracteresPermitidos.Length);
                codigo[i] = caracteresPermitidos[indice];
            }

            return new string(codigo);
        }

        private bool CodigoExisteNoBancoDeDados(string codigo)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM tbl_funcionario WHERE Id = @Id ";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id ", codigo);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private bool CpfVinculado(string cpf)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM tbl_funcionario WHERE Cpf = @Cpf";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Cpf", cpf);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private bool RgVinculado(string rg)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM tbl_funcionario WHERE Rg = @Rg";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Rg", rg);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        private bool CtpsVinculado(string ctps)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM tbl_Funcionario WHERE Ctps = @Ctps";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Ctps", ctps);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        private bool TituloVinculado(string tituloEleitor)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM tbl_Funcionario WHERE TituloEleitor = @TituloEleitor";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TituloEleitor", tituloEleitor);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private void CadastroForm_Load(object sender, EventArgs e)
        {
            codigoAleatorio = GerarCodigoAleatorio(5);
            txtb_CodigoChave.Text = codigoAleatorio;
        }


        private void cmbx_Cargo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string escolha = cmbx_Cargo.SelectedItem.ToString();

            double AssistenteAdministrativo = 2560.30;
            double AssistenteVendas = 1520.60;
            double SuporteCliente = 2100.50;
            double AuxiliarEstoque = 1340.12;
            double Recepcionista = 1403.00;
            double AssistenteMarketing = 1660.90;
            double AuxiliarContabilidade = 2433.21;
            double OperadorCaixa = 1413.00;
            double AssistenteRecursosHumanos = 1890.06;
            double AuxiliarLimpeza = 1320.00;

            if (escolha == "")
            {
                txtb_Salario.Text = "";
            }

            if (escolha == "Assistente Administrativo")
            {
                txtb_Salario.Text = "R$" + AssistenteAdministrativo.ToString("0.00");
            }

            if (escolha == "Assistente de Vendas")
            {
                txtb_Salario.Text = "R$" + AssistenteVendas.ToString("0.00");
            }

            if (escolha == "Atendente de Suporte ao Cliente")
            {
                txtb_Salario.Text = "R$" + SuporteCliente.ToString("0.00");
            }

            if (escolha == "Auxiliar de Estoque")
            {
                txtb_Salario.Text = "R$" + AuxiliarEstoque.ToString("0.00");
            }

            if (escolha == "Recepcionista")
            {
                txtb_Salario.Text = "R$" + Recepcionista.ToString("0.00");
            }

            if (escolha == "Assistende de Marketing")
            {
                txtb_Salario.Text = "R$" + AssistenteMarketing.ToString("0.00");
            }

            if (escolha == "Auxiliar de Contabilidade")
            {
                txtb_Salario.Text = "R$" + AuxiliarContabilidade.ToString("0.00");
            }

            if (escolha == "Operador de Caixa")
            {
                txtb_Salario.Text = "R$" + OperadorCaixa.ToString("0.00");
            }

            if (escolha == "Assistente de Recursos Humanos")
            {
                txtb_Salario.Text = "R$" + AssistenteRecursosHumanos.ToString("0.00");
            }

            if (escolha == "Auxiliar de Limpeza")
            {
                txtb_Salario.Text = "R$" + AuxiliarLimpeza.ToString("0.00");
            }
        }

        private void btn_Pagamentos_Click(object sender, EventArgs e)
        {
            PagamentosForm f1 = new();
            f1.StartPosition = FormStartPosition.Manual;
            f1.Location = this.Location;
            f1.Show();
            this.Hide();
        }

        private void btn_Perfil_Click(object sender, EventArgs e)
        {
            pnl_Sair.Show();
            txtb_Usuario.Text = "@" + perfil;
            btn_Ocultar.Show();

        }

        private void btn_Sair_Click(object sender, EventArgs e)
        {
            LoginForm f1 = new();
            f1.StartPosition = FormStartPosition.Manual;
            f1.Location = this.Location;
            f1.Show();
            this.Hide();
        }

        private void btn_Ocultar_Click(object sender, EventArgs e)
        {
            pnl_Sair.Hide();
            btn_Ocultar.Hide();
        }

        private void btn_Minimiza_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btn_Limpar_Click(object sender, EventArgs e)
        {
            CadastroForm f1 = new();
            f1.StartPosition = FormStartPosition.Manual;
            f1.Location = this.Location;
            f1.Show();
            this.Hide();
        }
    }
}

