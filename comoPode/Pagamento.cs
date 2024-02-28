using comoPode;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

public class PagamentoService
{
    private string connectionString = "Data Source=DESKTOP-7092F7U;Initial Catalog = loginform; Integrated Security = True";

    public PagamentoService(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public void RealizarPagamentos()
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT NomeFuncionario, Salario, DataPagamento, CotaMensal, Status FROM TabelaPagamentos";
            SqlCommand command = new SqlCommand(query, connection);

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                string nome = reader["Nome"].ToString();
                double salario = Convert.ToDouble(reader["Salario"]);
                DateTime dataPagamento = Convert.ToDateTime(reader["DataPagamento"]);
                double cotaMensal = Convert.ToDouble(reader["CotaMensal"]);
                string status = reader["Status"].ToString();

                if (dataPagamento.Day == DateTime.Now.Day)
                {
                    if (cotaMensal >= salario)
                    {
                        status = "Pago";
                    }
                    else
                    {
                        status = "Em Aberto";
                    }
                }
                else
                {
                    status = "Em Aberto";
                }

                // Atualize o status no banco de dados se necessário
                AtualizarStatusNoBanco(nome, status);
            }

            reader.Close();
        }
    }

    private void AtualizarStatusNoBanco(string nome, string novoStatus)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = "UPDATE TabelaPagamentos SET Status = @novoStatus WHERE NomeFuncionario = @nomeFuncionario";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@novoStatus", novoStatus);
            command.Parameters.AddWithValue("@nomeFuncionario", nome);

            command.ExecuteNonQuery();
        }
    }
}
