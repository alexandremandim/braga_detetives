using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BackOffice
{
    public class TEmail
    {

        private System.Net.Mail.MailMessage Mensagem;
        private System.Net.Mail.SmtpClient Servidor;


        private const string EMAIL_SMTP = "smtp.gmail.com";
        private int EMAIL_PORT = 587;
        private int EMAIL_TIMEOUT = 10000;
        private string EMAIL_PASSWORD = "backoffice2016";
        private string EMAIL_CONTA = "li4backoffice@gmail.com";

        public bool Enviar(string Endereco, string Assunto, string Texto, string caminhoRelatorio)
        {
            try
            {

                Servidor = new System.Net.Mail.SmtpClient(EMAIL_SMTP, EMAIL_PORT);
                Servidor.Timeout = EMAIL_TIMEOUT;

                System.Net.NetworkCredential Credencial = new System.Net.NetworkCredential(EMAIL_CONTA, EMAIL_PASSWORD);
                Servidor.UseDefaultCredentials = false;
                Servidor.Credentials = Credencial;
                Servidor.EnableSsl = true;
                Servidor.Timeout = 50000;

                Mensagem = new System.Net.Mail.MailMessage();
                Mensagem.To.Add(Endereco);
                Mensagem.Subject = Assunto;
                Mensagem.From = new System.Net.Mail.MailAddress(EMAIL_CONTA);
                Mensagem.Body = Texto;
                Mensagem.Attachments.Add(new System.Net.Mail.Attachment(caminhoRelatorio));

                Servidor.Send(Mensagem);
                Mensagem.Dispose();
                Servidor.Dispose();
                MessageBox.Show("Email enviado com sucesso");
                return true;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Atenção");
                return false;
            }
        }
    }
}
