using Guna.UI2.WinForms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO.Compression;
using System.Windows.Forms;
using Font = System.Drawing.Font;

namespace comoPode
{
    public partial class PagamentosForm : Form
    {
        private string connectionString = "Data Source=DESKTOP-7092F7U;Initial Catalog=SALARY_SYNC;Integrated Security=True";
        private SqlConnection connection;
        private DataTable dataTable;
        private Guna2DataGridView dataGridView;
        private decimal cotaMensalEmpresa = 10000;
        string perfil = UserContext.Username;



        public PagamentosForm()
        {
            InitializeComponent();

            pnl_Sair.Hide();
            btn_Ocultar.Hide();

        }

        private void PagamentosForm_Load(object sender, EventArgs e)
        {
            connection = new SqlConnection(connectionString);
            connection.Open();

            string sqlDataAdmissao = "select DataAdmissao, Nome, CPF, Id, Cargo, Salario, StatusPagamento from tbl_funcionario";
            using (SqlDataAdapter adapter = new SqlDataAdapter(sqlDataAdmissao, connection))
            {
                dataTable = new DataTable();
                adapter.Fill(dataTable);

                dataGridView = new Guna2DataGridView();
                dataGridView.Location = new Point(0, 108);
                dataGridView.Width = 1080;
                dataGridView.Height = 400;
                dataGridView.AllowUserToAddRows = false;
                dataGridView.AllowUserToDeleteRows = false;
                dataGridView.ReadOnly = true;
                dataGridView.RowHeadersVisible = false;
                dataGridView.ThemeStyle.BackColor = Color.White;


                dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.Indigo;
                dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView.Font, FontStyle.Bold);

                pnl_Pagamentos.Controls.Add(dataGridView);

                dataGridView.Columns.Add("DataAdmissao", "Data Admissão");
                dataGridView.Columns.Add("Nome", "Nome");
                dataGridView.Columns.Add("CPF", "CPF");
                dataGridView.Columns.Add("Id", "ID");
                dataGridView.Columns.Add("Cargo", "Cargo");
                dataGridView.Columns.Add("SalarioTotal", "Salário Total");
                dataGridView.Columns.Add("StatusPagamento", "Status Pagamento");

                dataGridView.CellDoubleClick += DataGridView_CellDoubleClick;


                foreach (DataRow row in dataTable.Rows)
                {
                    DateTime dataAdmissao = Convert.ToDateTime(row["DataAdmissao"]);
                    string dataFormatada = dataAdmissao.ToShortDateString();

                    decimal salario = Convert.ToDecimal(row["Salario"]);
                    decimal dsr = CalcularDSR(salario);
                    decimal valeTransporte = CalcularValeTransporte(salario);
                    decimal refeicao = CalcularRefeicao(salario);
                    decimal salarioTotal = salario;

                    string statusPagamento = row["StatusPagamento"].ToString();


                    // Verificar se o pagamento não foi feito e se é o dia 1 do mês
                    if (statusPagamento == "Não Pago" && DateTime.Now.Day == 1)
                    {
                        // Alterar o status para "Pendente" se for o dia 1
                        statusPagamento = "Pendente";
                        AtualizarStatusPagamento(row["Id"].ToString(), statusPagamento);
                    }
                    else if (statusPagamento == "Pago" && DateTime.Now.Day == 1)
                    {
                        statusPagamento = "Pendente";
                        AtualizarStatusPagamento(row["Id"].ToString(), statusPagamento);
                    }
                    else if (statusPagamento == "Pendente" && DateTime.Now.Day == 5)
                    {
                        statusPagamento = "Pago";
                        AtualizarStatusPagamento(row["Id"].ToString(), statusPagamento);
                    }

                    dataGridView.Rows.Add(dataFormatada,
                                     row["Nome"].ToString(),
                                     CensurarCpf(row["CPF"].ToString()),
                                     row["Id"].ToString(),
                                     row["Cargo"].ToString(),
                                     salarioTotal.ToString("C"),
                                     statusPagamento);
                }
            }

            connection.Close();
        }

        private void DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verifica se a célula clicada está dentro dos limites da tabela
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView.Rows.Count)
            {
                // Obtém os dados do funcionário selecionado
                DataGridViewRow selectedRow = dataGridView.Rows[e.RowIndex];
                string funcionarioId = selectedRow.Cells["Id"].Value.ToString();
                string nome = selectedRow.Cells["Nome"].Value.ToString();
                // Adicione outras propriedades conforme necessário

                // Exibe um diálogo de confirmação antes de excluir
                DialogResult result = MessageBox.Show($"Tem certeza de que deseja excluir o funcionário {nome}?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    // Chame o método para excluir o funcionário do banco de dados
                    ExcluirFuncionario(funcionarioId);

                    PagamentosForm f1 = new();
                    f1.StartPosition = FormStartPosition.Manual;
                    f1.Location = this.Location;
                    f1.Show();
                    this.Hide();
                }
            }
        }

        private void ExcluirFuncionario(string funcionarioId)
        {
            // Abra a conexão com o banco de dados
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Crie e execute a instrução SQL DELETE
                string sqlExclusao = "DELETE FROM tbl_funcionario WHERE Id = @FuncionarioId";

                using (SqlCommand command = new SqlCommand(sqlExclusao, connection))
                {
                    command.Parameters.AddWithValue("@FuncionarioId", funcionarioId);
                    command.ExecuteNonQuery();
                }

                // Feche a conexão com o banco de dados
                connection.Close();
            }
        }


        private decimal CalcularDSR(decimal salario)
        {
            decimal dsr = salario * 0.05m;
            return dsr;
        }

        private decimal CalcularValeTransporte(decimal salario)
        {
            decimal valeTransporte = salario * 0.06m;
            return valeTransporte;
        }

        private decimal CalcularRefeicao(decimal salario)
        {
            decimal refeicao = salario * 0.08M;
            return refeicao;
        }

        private string CensurarCpf(string cpf)
        {
            if (cpf.Length != 11)
            {
                return "CPF Inválido";
            }

            string cpfCensurado = $"{cpf.Substring(0, 3)}.***.***-{cpf.Substring(9)}";
            return cpfCensurado;
        }

        private void AtualizarStatusPagamento(string funcionarioId, string novoStatus)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sqlUpdate = "UPDATE tbl_funcionario SET StatusPagamento = @NovoStatus WHERE Id = @FuncionarioId";

                    using (SqlCommand cmd = new SqlCommand(sqlUpdate, conn))
                    {
                        cmd.Parameters.AddWithValue("@NovoStatus", novoStatus);
                        cmd.Parameters.AddWithValue("@FuncionarioId", funcionarioId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Status de pagamento atualizado com sucesso.");
                        }
                        else
                        {
                            MessageBox.Show("Não foi possível atualizar o status de pagamento.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocorreu um erro ao atualizar o status de pagamento: " + ex.Message);
            }
        }

        private void btn_Fechar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btn_Minimiza_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btn_Cadastro_Click(object sender, EventArgs e)
        {
            CadastroForm f1 = new();
            f1.StartPosition = FormStartPosition.Manual;
            f1.Location = this.Location;
            f1.Show();
            this.Hide();
        }

        private void btn_Pagamentos_Click(object sender, EventArgs e)
        {
            PagamentosForm f1 = new();
            f1.StartPosition = FormStartPosition.Manual;
            f1.Location = this.Location;
            f1.Show();
            this.Hide();
        }

        private void btn_Perfil_Click_1(object sender, EventArgs e)
        {
            pnl_Sair.Show();
            txtb_Usuario.Text = "@" + perfil;
            btn_Ocultar.Show();
        }

        private void btn_Sair_Click_1(object sender, EventArgs e)
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

        private void btn_exportarPdf_Click(object sender, EventArgs e)
        {
            try
            {
                GerarPdf.GerarPDFs(); // Chamando a função de geração de PDFs da classe RelatorioDespesas

                // Aqui você pode adicionar código para exibir uma mensagem de sucesso ao usuário
                MessageBox.Show("PDFs gerados com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Em caso de erro, exiba uma mensagem de erro ao usuário ou registre o erro em algum lugar
                MessageBox.Show("Ocorreu um erro ao gerar os PDFs: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_ExportarRelatorio_Click(object sender, EventArgs e)
        {
            try
            {
                RelatorioDespesas.GerarRelatorio(); // Chamando a função de geração de relatório da classe RelatorioDespesas

                // Aqui você pode adicionar código para exibir uma mensagem de sucesso ao usuário
                MessageBox.Show("Relatório exportado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Em caso de erro, exiba uma mensagem de erro ao usuário ou registre o erro em algum lugar
                MessageBox.Show("Ocorreu um erro ao exportar o relatório: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}

