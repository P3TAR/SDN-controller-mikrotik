using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projeto1LTI
{
    public partial class Form1 : Form
    {
        

        public Form1()
        {
            InitializeComponent();

            //Ignorar erros de validação do SSL/TLS 
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            textBox3.UseSystemPasswordChar = true;

        }

        private void button1_Click(object sender, EventArgs e)
        {

            //Variáveis utilizadas para fazer o pedido Get
            string routerOSIpAddress = textBox1.Text;
            string username = textBox2.Text;
            string password = textBox3.Text;

            //Endpoint do pedido Get para verificar se a conexão ao router é sucedida ou não
            string baseUrl = "https://" + routerOSIpAddress + "/rest/system/identity"; ;

            //Verificar se as text box foram preenchidas
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text))
            {
                MessageBox.Show("Tem de preencher as credenciais", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    //Pedido request para ver se a conexão ao router foi bem sucedida 
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl);
                    request.Method = "GET";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // Conexão sucedida
                        Form2 newForm = new Form2(routerOSIpAddress, username, password);
                        newForm.Show();

                        textBox1.Text = "";
                        textBox2.Text = "";
                        textBox3.Text = "";
                    }
                    else
                    {
                        // Connection não foi sucedida
                        MessageBox.Show("Credenciais inválidas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (WebException ex)
                {
                    // Erro do pedido
                    MessageBox.Show("Credenciais inválidas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }


        }
    }
}
