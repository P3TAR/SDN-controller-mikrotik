using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Security.Policy;
using System.Net.Http;
using System.Xml.Linq;
using System.Web;
using System.Security.Claims;
using System.Runtime.InteropServices.ComTypes;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using System.Data.Common;
using System.Net.Sockets;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace Projeto1LTI
{

    public partial class Form2 : Form
    {
        
        //Definição das variáveis utilizadas globalmente
        string routerOSIpAddress;
        string username;
        string password;
        string baseUrl;

        public Form2(string Ip, string Username, string Password)
        {
            InitializeComponent();

            //Ignorar erros de validação do SSL/TLS 
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            //Preencher variáveis passadas do Form 1
            routerOSIpAddress = Ip;
            username = Username;
            password = Password;
            baseUrl = "https://" + routerOSIpAddress + "/rest/";
        }


        //Cria todos os elemetos que necessitamos para executar todas as funções quando é iniciado o programa
        private void Form1_Load(object sender, EventArgs e)
        {
            //Começar com tudo invisível
            listBox1.Items.Clear();
            comboBox1.Visible = false;
            comboBox2.Visible = false;
            textBox4.Visible = false;
            textBox3.Visible = false;
            textBox2.Visible = false;
            textBox1.Visible = false;
            button1.Visible = false;
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            label10.Visible = false;
            label11.Visible = false;
            label12.Visible = false;
            button2.Visible = false;
            comboBox3.Visible = false;
        }


        #region Listar

        //Listar todas a interfaces do dispotivo
        private void listarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            limparForm();
            listBox1.Items.Clear();
            label5.Visible = true; label6.Visible = true; label7.Visible = true; label8.Visible = true;
            label5.Text = "id"; label6.Text = "name"; label7.Text = "type";label8.Text = "mtu";

            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "interface");
                request.Method = "GET";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));
            
                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JArray responseJson = JArray.Parse(reader.ReadToEnd());
                    foreach (JObject obj in responseJson)
                    {
                        string id = obj.Value<string>(".id");
                        string name = obj.Value<string>("name");
                        string type = obj.Value<string>("type");
                        string mtu = obj.Value<string>("mtu");
                        comboBox1.Items.Add(id);
                        listBox1.Items.Add(id + " - " + name + " - " + type + " - " + mtu);

                    }
                }
                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao listar as intefaces devido a: {responseText}");
                }
            }
        }
        
        //Listar todas as interface wireless
        private void interfacesWirelessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label6.Visible = true; label7.Visible = true; 
            label5.Text = "id"; label6.Text = "name"; label7.Text = "type"; 
            listarRedesWireless();
        }

        //Listar todas as interfaces bridge e as respetivas portas associadas
        private void interfacesBridgeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label6.Visible = true; label7.Visible = true; 
            label5.Text = "id"; label6.Text = "name"; label7.Text = "mtu"; 
            listarBridges();
        }

        //Listar todas as rotas estáticas
        private void rotasEstáticasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label7.Visible = true; label9.Visible = true; 
            label5.Text = "id"; label7.Text = "destiny"; label9.Text = "gateway"; 
            listarRotasEstaticas();
        }

        //Listar todos os endereços Ips
        private void endereçosIpsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label6.Visible = true; label7.Visible = true; label8.Visible = true; label9.Visible = true;label10.Visible = true;label11.Visible = true;label12.Visible = true;
            label5.Text = "id"; label6.Text = "nome"; label7.Text = "address"; label8.Text = "disabled";label9.Text = "dynamic";label10.Text = "interface";label11.Text = "invalid";label12.Text = "network"; 
            listarEndereçosIps();
            
        }

        //Listar todos os servidores DHCP
        private void servidoresDHCPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label6.Visible = true; label7.Visible = true;
            label5.Text = "id"; label6.Text = "address pool"; label7.Text = "interface";
            listarServidoresDHCP(); 
        }

        //Listar todas as regras da firewall
        private void regrasDeFirewallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label6.Visible = true; label7.Visible = true;
            label5.Text = "id"; label6.Text = "action"; label7.Text = "chain";
            listarRegrasFirewall();
        }

        //Listar Redes Wireless
        public void listarRedesWireless()
        {
            
            listBox1.Items.Clear();

            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "interface/wireless");
                request.Method = "GET";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));
            
                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JArray responseJson = JArray.Parse(reader.ReadToEnd());
                    comboBox1.Items.Clear();
                    foreach (JObject obj in responseJson)
                    {
                        string id = obj.Value<string>(".id");
                        string name = obj.Value<string>("name");
                        string mtu = obj.Value<string>("mtu");
                        listBox1.Items.Add(id + " - " + name + " - " + mtu);
                        comboBox1.Items.Add(id);
                    }
                }
                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao listar as intefaces wireless devido a: {responseText}");
                }
            }
        }

        //Listar apenas as interfaces bridge
        public void listarBridges()
        {
            
            listBox1.Items.Clear();

            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "interface/bridge");
                request.Method = "GET";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));
    
                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JArray responseJson = JArray.Parse(reader.ReadToEnd());
                    comboBox1.Items.Clear();
                    foreach (JObject obj in responseJson)
                    {
                        string id = obj.Value<string>(".id");
                        string name = obj.Value<string>("name");
                        string mtu = obj.Value<string>("mtu");
                        listBox1.Items.Add(id + " - " + name + " - " + mtu);
                        comboBox1.Items.Add(id);
                    }
                }
                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao listar interfaces bridge devido a: {responseText}");
                }
            }
        }

        //Lista todas as rotas estáticas
        public void listarRotasEstaticas()
        {
            
            listBox1.Items.Clear();

            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "ip/route");
                request.Method = "GET";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

            
                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JArray responseJson = JArray.Parse(reader.ReadToEnd());
                    comboBox1.Items.Clear();
                    foreach (JObject obj in responseJson)
                    {
                        string id = obj.Value<string>(".id");
                        string dst = obj.Value<string>("dst-address");
                        string gateway = obj.Value<string>("gateway");
                        listBox1.Items.Add(id + " - " + dst + " - " + gateway);
                        comboBox1.Items.Add(id);
                    }
                }
                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao listar as rotas estáticas devido a: {responseText}");
                }
            }
        }

        //Lista os endereços Ips
        public void listarEndereçosIps()
        {
            
            listBox1.Items.Clear();

            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "ip/address");
                request.Method = "GET";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

            
                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    comboBox1.Items.Clear();
                    JArray responseJson = JArray.Parse(reader.ReadToEnd());
                    foreach (JObject obj in responseJson)
                    {
                        string id = obj.Value<string>(".id");
                        string actualinterface = obj.Value<string>("namactual-interfacee");
                        string address = obj.Value<string>("address");
                        string disabled = obj.Value<string>("disabled");
                        string dynamic = obj.Value<string>("dynamic");
                        string interface1 = obj.Value<string>("interface");
                        string invalid = obj.Value<string>("invalid");
                        string network = obj.Value<string>("network");
                        listBox1.Items.Add(id + " - " + actualinterface + " - " + address + " - " + disabled + " - " + dynamic + " - " + interface1 + " - " + invalid + " - " + network);
                        comboBox1.Items.Add(id);
                    }
                }
                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao listar os endereços Ips devido a: {responseText}");
                }
            }
        }

        //Listar os servidores DHCP
        public void listarServidoresDHCP()
        {
            
            listBox1.Items.Clear();


            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "ip/dhcp-server");
                request.Method = "GET";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

            
                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JArray responseJson = JArray.Parse(reader.ReadToEnd());
                    comboBox1.Items.Clear();
                    foreach (JObject obj in responseJson)
                    {
                        string id = obj.Value<string>(".id");
                        string addresspool = obj.Value<string>("address-pool");
                        string interface1 = obj.Value<string>("interface");
                        listBox1.Items.Add(id + " - " + addresspool + " - " + interface1);
                        comboBox1.Items.Add(id);
                    }
                }
                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao listar os servidores DHCP devido a: {responseText}");
                }
            }
        }

        //Listar regras de firewall
        public void listarRegrasFirewall()
        {
         
            listBox1.Items.Clear();

            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "ip/firewall/filter");
                request.Method = "GET";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

            
                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JArray responseJson = JArray.Parse(reader.ReadToEnd());
                    comboBox1.Items.Clear();
                    foreach (JObject obj in responseJson)
                    {
                        string id = obj.Value<string>(".id");
                        string action = obj.Value<string>("action");
                        string chain = obj.Value<string>("chain");
                        listBox1.Items.Add(id + " - " + action + " - " + chain);
                        comboBox1.Items.Add(id);
                    }
                }
                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao listar as regras da firewall devido a: {responseText}");
                }
            }
        }

        //Listar os perfis de segurança
        public void listarPerfisSegurança()
        {
            
            listBox1.Items.Clear();

            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "interface/wireless/security-profiles");
                request.Method = "GET";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));
    
           
                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JArray responseJson = JArray.Parse(reader.ReadToEnd());
                    comboBox1.Items.Clear();
                    foreach (JObject obj in responseJson)
                    {
                        string id = obj.Value<string>(".id");
                        string authentication = obj.Value<string>("authentication-types");
                        string cyphers = obj.Value<string>("group-ciphers");
                        string tlsCertificate = obj.Value<string>("tls-certificate");
                        listBox1.Items.Add(id + " - " + authentication + " - " + cyphers +" - " + tlsCertificate);
                        comboBox1.Items.Add(id);
                    }
                }
                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao listar os perfis de segurança devido a: {responseText}");
                }
            }
        }

        //Listar Servidores DNS
        public void listarServidoresDNS()
        {
            listBox1.Items.Clear();

            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "ip/dns");
                request.Method = "GET";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));
            
                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject responseJson = JObject.Parse(reader.ReadToEnd());
                    string remoteRequets = responseJson.Value<string>("allow-remote-requests");
                    string cache = responseJson.Value<string>("cache-size");
                    string dynamicServers = responseJson.Value<string>("dynamic-servers");
                    string primaryDns = responseJson.Value<string>("servers");
                    listBox1.Items.Add(remoteRequets + " - " + cache + " - " + dynamicServers + " - " + primaryDns);
                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao listar o servidor DNS devido a: {responseText}");
                }
            }
        }

        //Listar Protocolos de encaminhamento
        public void listarProtocolosEncaminhamento()
        {
            listBox1.Items.Clear();

            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "ip/settings");
                request.Method = "GET";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

            
                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject responseJson = JObject.Parse(reader.ReadToEnd());
                    string icmpRedirects = responseJson.Value<string>("accept-redirects");
                    string icmpRateLimit = responseJson.Value<string>("icmp-rate-limit");
                    string ipForward = responseJson.Value<string>("ip-forward");
                    listBox1.Items.Add(icmpRedirects + " - " + icmpRateLimit + " - " + ipForward);
                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao listar os protocolos de encaminhamento devido a: {responseText}");
                }
            }
        }

        #endregion

        #region Criar

        //Chama a função para criar interfaces bridge quando se clica no botão criar
        private void bToolStripMenuItem_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label6.Visible = true; label7.Visible = true;
            label5.Text = "id"; label6.Text = "name"; label7.Text = "mtu";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            listBox1.Items.Clear();
            button1.Text = "Criar";
            comboBox1.Visible = false;
            comboBox2.Visible = false;
            textBox3.Visible = true;
            textBox4.Visible = false;
            label3.Visible = true;
            label4.Visible = false;
            label1.Text = "ID da Bridge";
            label2.Text = "Port 1";
            label3.Text = "Port 2";
            label1.Visible = true;
            label2.Visible = true;
            textBox1.Visible = true;
            textBox2.Visible = true;
            button1.Visible = true;
            button2.Visible = false;
            desassociarFuncoes();
            listarBridges();
            button1.Click += new EventHandler(criarinterfacebridge);
        }

        //Criar interface bridge
        private void criarinterfacebridge(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            string url = $"{baseUrl}interface/bridge/add";

            string name = textBox1.Text;
            string port1 = textBox2.Text;
            string port2 = textBox3.Text;
            string payload = "{\"name\":\"" + name + "\",\"ports\":[\"" + port1 + "\",\"" + port2 + "\"]}";

            try
            {
                //Criar um novo pedido POST Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                //Escrever o payload para o body do pedido
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(payload);
                }
           
                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseJson = reader.ReadToEnd();
                    listBox1.Items.Add($"A interface bridge foi criada com sucesso: {responseJson}");
                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao criar a interface bridge devido a: {responseText}");
                }
            }
        }

        //Chama a função para criar rotas estáticas quando se clica no botão criar
        private void rotasEstaticasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label7.Visible = true; label9.Visible = true;
            label5.Text = "id"; label7.Text = "destiny"; label9.Text = "gateway";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            listBox1.Items.Clear();
            button1.Text = "Criar";
            comboBox1.Visible = false;
            comboBox2.Visible = false;
            label2.Text = "Gateway";
            textBox3.Visible = false;
            textBox4.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            label1.Text = "Endereço destinatário";
            label2.Visible = true;
            label1.Visible = true;
            textBox1.Visible = true;
            textBox2.Visible = true;
            button1.Visible = true;
            button2.Visible = false;
            desassociarFuncoes();
            listarRotasEstaticas();
            button1.Click += new EventHandler(criarrotaestatica);
        }

        //Criar rotas estáticas
        private void criarrotaestatica(object sender, EventArgs e)
        {


            listBox1.Items.Clear();
            string url = $"{baseUrl}ip/route/add";
            string dstAddress = textBox1.Text;
            string gateway = textBox2.Text;
            string payload = "{\"dst-address\":\"" + dstAddress + "\",\"gateway\":\"" + gateway + "\"}";

            try
            {
                //Criar um novo pedido POST Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                //Escrever o payload para o body do pedido
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(payload);
                }

                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseJson = reader.ReadToEnd();
                    listBox1.Items.Add($"A rota estátiva foi criada com sucesso: {responseJson}");
                }

                //Limpar
                response.Close();

            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao criar a rota estática devido a: {responseText}");
                }
            }

        }


        //Chama a função para criar endereços IP quando se clica no botão criar
        private void endereçoIpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label6.Visible = true; label7.Visible = true; label8.Visible = true; label9.Visible = true; label10.Visible = true; label11.Visible = true; label12.Visible = true;
            label5.Text = "id"; label6.Text = "nome"; label7.Text = "address"; label8.Text = "disabled"; label9.Text = "dynamic"; label10.Text = "interface"; label11.Text = "invalid"; label12.Text = "network";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            listBox1.Items.Clear();
            button1.Text = "Criar";
            comboBox1.Visible = false;
            comboBox2.Visible = false;
            button1.Visible = true;
            textBox1.Visible = true;
            textBox2.Visible = true;
            textBox3.Visible = true;
            textBox4.Visible = false;
            label1.Visible = true;
            label1.Text = "Endereço IP";
            label2.Visible = true;
            label2.Text = "Interface";
            label3.Visible = true;
            label4.Visible = false;
            label3.Text = "Comentário";
            label4.Text = "";
            button2.Visible = false;
            desassociarFuncoes();
            listarEndereçosIps();
            button1.Click += new EventHandler(criarEndereçoIP);
        }

        //Criar endereços Ip
        private void criarEndereçoIP(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            string url = $"{baseUrl}ip/address/add";
            string ipAddress = textBox1.Text;
            string interface1 = textBox2.Text;
            string comment= textBox3.Text;
            string payload = "{\"address\":\"" + ipAddress + "\",\"interface\":\"" + interface1 + "\",\"comment\":\"" + comment + "\"}";

            try
            {
                //Criar um novo pedido POST Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                //Escrever o payload para o body do pedido
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(payload);
                }

                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseJson = reader.ReadToEnd();
                    listBox1.Items.Add($"O endereço Ip foi criado com sucesso: {responseJson}");
                }

                //Limpar
                response.Close();
            }
            catch (WebException error)
            {
                //Erro do pedido
                using (var stream = error.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao criar o Ip devido a: {responseText}");
                }
            }
        }

        //Chama a função para criar servidores DHCP quando se clica no botão criar
        private void dHCPServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label6.Visible = true; label7.Visible = true;
            label5.Text = "id"; label6.Text = "address pool"; label7.Text = "interface";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            listBox1.Items.Clear();
            button1.Text = "Criar";
            comboBox1.Visible = false;
            comboBox2.Visible = false;
            button1.Visible = true;
            textBox1.Visible = true;
            textBox2.Visible = true;
            textBox3.Visible = false;
            textBox4.Visible = false;
            label1.Visible = true;
            label1.Text = "Nome do Servidor DHCP";
            label2.Visible = true;
            label2.Text = "Interface do  servidor DHCP";
            label3.Visible = false;
            label4.Visible = false;
            label3.Text = "";
            label4.Text = "";
            desassociarFuncoes();
            button2.Visible = false;
            listarServidoresDHCP();
            button1.Click += new EventHandler(criarServerDHCP);
        }

        //Criar servidor DHCP
        private void criarServerDHCP(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            string url = $"{baseUrl}ip/dhcp-server/add";
            string name = textBox1.Text;
            string interface1 = textBox2.Text;

            //Criar um novo pedido POST Http
            string payload = "{\"name\":\"" + name + "\",\"interface\":\"" + interface1 + "\"}";

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                //Escrever o payload para o body do pedido
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(payload);
                }

                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseJson = reader.ReadToEnd();
                    listBox1.Items.Add($"DHCP server {name} created successfully: {responseJson}");
                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Error creating DHCP server {name}: {responseText}");
                }

            }
        }

        //Chama a função para criar regras de firewall quando se clica no botão criar
        private void regrasDeFirewallToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label6.Visible = true; label7.Visible = true;
            label5.Text = "id"; label6.Text = "action"; label7.Text = "chain";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            listBox1.Items.Clear();
            button1.Text = "Criar";
            button1.Visible = true;
            textBox1.Visible = false;
            comboBox1.Visible = true;
            textBox2.Visible = false;
            comboBox2.Visible = true;
            textBox3.Visible = true;
            textBox4.Visible = false;
            label1.Visible = true;
            label1.Text = "Chain";
            label2.Visible = true;
            label2.Text = "Action";
            label3.Visible = true;
            label4.Visible = false;
            label3.Text = "Endereço de Origem";
            label4.Text = "Endereço da Pool";
            textBox4.Visible = true;
            label4.Visible = true;
            button2.Visible = false;
            desassociarFuncoes();
            listarRegrasFirewall();
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(new string[] { "input", "output", "forward" });
            button1.Click += new EventHandler(criarFirewall);
        }


        //Criar regras de firewall
        private void criarFirewall(object sender, EventArgs e)
        {
            
            string url = $"{baseUrl}ip/firewall/filter/add";
            string jumpTarget = ""; 

            //Se a ação for jump, temos de atribuir jump target
            if (comboBox2.Text == "jump")
            {
                jumpTarget = textBox4.Text; 
            }
           
            
            string payload = "{\"chain\":\"" + comboBox1.Text + "\",\"action\":\"" + comboBox2.Text + "\",\"jump-target\":\"" + jumpTarget+"\"}";

            try
            {
                //Criar um novo pedido POST Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                //Escrever o payload para o body do pedido
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(payload);
                }

                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseJson = reader.ReadToEnd();
                    listBox1.Items.Add($"A regra da firewall foi criada com sucesso: {responseJson}");
                }

                //Limpar
                response.Close();

            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao criar a regra de firewall devido a: {responseText}");
                }
            }
        }


        //Chama a função para criar perfis de segurança quando se clica no botão criar
        private void perfilDeSegurançaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            limparForm();
            label5.Visible = true; label6.Visible = true; label7.Visible = true; label8.Visible = true;
            label5.Text = "id"; label6.Text = "autenticação"; label7.Text = "ciphers";label8.Text = "certificado";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            button1.Text = "Criar";
            comboBox1.Visible = false;
            comboBox2.Visible = false;
            button1.Visible = true;
            textBox1.Visible = true;
            textBox2.Visible = true;
            textBox3.Visible = false;
            textBox4.Visible = false;
            label1.Visible = true;
            label1.Text = "Name";
            label2.Visible = true;
            label2.Text = "Comment";
            label3.Visible = false;
            label4.Visible = false;
            label3.Text = "";
            label4.Text = "";
            button2.Visible = false;
            desassociarFuncoes();
            listarPerfisSegurança();
            button1.Click += new EventHandler(criarPerfilSegurança);

        }

        //Criar perfis de segurança
        private void criarPerfilSegurança(object sender, EventArgs e)
        {

            listBox1.Items.Clear();
            string url = $"{baseUrl}interface/wireless/security-profiles/add";
            string name = textBox1.Text;
            string comment = textBox2.Text;
            string payload = "{\"name\":\"" + name + "\",\"comment\":\"" + comment + "\"}";

            try
            {
                //Criar um novo pedido POST Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                //Escrever o payload para o body do pedido
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(payload);
                }


                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseJson = reader.ReadToEnd();
                    listBox1.Items.Add($"O perfil de segurança foi criado com sucesso: {responseJson}");
                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao criar o perfil de segurança devido a: {responseText}");
                }
            }
        }

        #endregion

        #region Editar

        //Chama a função para editar interfaces bridge quando se clica no botão editar
        private void interfaceBridgeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label6.Visible = true; label7.Visible = true;
            label5.Text = "id"; label6.Text = "name"; label7.Text = "mtu";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            comboBox1.Visible = true;
            comboBox2.Visible = false;
            label2.Text = "Nome da Interface Bridge";
            textBox3.Visible = true;
            textBox4.Visible = true;
            label3.Visible = true;
            label3.Text = "Desativado (true/false)";
            label4.Text = "Fast-Forward (true/false)";
            label4.Visible = true;
            label1.Text = "ID da Interface Bridge";
            label2.Visible = true;
            label1.Visible = true;
            textBox1.Visible = false;
            textBox2.Visible = true;
            button1.Visible = true;
            button1.Text = "Editar";
            button2.Visible = true;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            desassociarFuncoes();
            listarBridges();          
            button2.Click -= button2_click_configurarVPN;
            button2.Click -= button2_click_configurar_ProtocolosEncaminhamento;
            button2.Click -= button2_click_configurar_RedesWireless;
            button2.Click -= button2_click_configurar_ServidoresDNS;
            button2.Click -= button2_click_editar_EnderecosIP;
            button2.Click -= button2_click_editarBridge;
            button2.Click -= button2_click_editar_PerfilSeguranca;
            button2.Click -= button2_click_editar_RegraFirewall;
            button2.Click -= button2_click_editar_RotasEstaticas;
            button2.Click -= button2_click_editar_servidorDHCP;
            button2.Click += new EventHandler(button2_click_editarBridge);
            button1.Click += new EventHandler(editarInterfaceBridge);
        }

        //Editar interface bridge
        private void editarInterfaceBridge(object sender, EventArgs e)
        {
            //Verifica se a Combo Box tem um valor selecionado
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                listBox1.Items.Clear();
                string bridgeId = comboBox1.Text;
                string url = $"{baseUrl}interface/bridge/{bridgeId}";
                string name = textBox2.Text;
                string disabled = textBox3.Text;
                string fastforward = textBox4.Text;

                string payload = "{\"name\":\"" + name + "\",\"disabled\":\"" + disabled + "\",\"fast-forward\":\"" + fastforward + "\"}";

                try
                {
                    //Criar um pedido PATCH Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "PATCH";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                    //Escrever o payload para o body do pedido
                    using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                    {
                        writer.Write(payload);
                    }
              
                    //Enviar o pedido e receber a resposta
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler a resposta
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseJson = reader.ReadToEnd();
                        listBox1.Items.Add($"A interface bridge foi editada com sucesso: {responseJson}");
                    }

                    //Limpar
                    response.Close();

                }
                catch (WebException ex)
                {
                    //Erro do pedido
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        listBox1.Items.Add($"Erro ao editar a interface bridge devido a: {responseText}");
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        //Preenche os campos das interfaces bridge que se pretende editar
        public void button2_click_editarBridge(object sender, EventArgs e)
        {
            //Verifica se a Combo Box tem um valor selecionado
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                try
                {
                    //Criar um pedido GET Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "interface/bridge/" + comboBox1.Text);
                    request.Method = "GET";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                    //Enviar o pedido e obter a resposta
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler a resposta e passar para um array em JSON
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        JObject responseJson = JObject.Parse(reader.ReadToEnd());

                        string disabled = responseJson.Value<string>("disabled");
                        string name = responseJson.Value<string>("name");
                        string ff = responseJson.Value<string>("fast-forward");
                        textBox2.Text = name;
                        textBox3.Text = disabled;
                        textBox4.Text = ff;

                    }
                    //Limpar
                    response.Close();
                }
                catch (WebException ex)
                {
                    //Erro do pedido
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        listBox1.Items.Add($"Erro ao listar interfaces bridge devido a: {responseText}");
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        //Chama a função para editar rotas estáticas quando se clica no botão editar
        private void rotaEstáticaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label7.Visible = true; label9.Visible = true;
            label5.Text = "id"; label7.Text = "destiny"; label9.Text = "gateway";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            comboBox1.Visible = true;
            comboBox2.Visible = false;
            label2.Text = "Endereço de destino";
            textBox3.Visible = true;
            textBox4.Visible = false;
            label3.Visible = true;
            label3.Text = "Gateway";
            label4.Visible = false;
            label1.Text = "ID da rota estática";
            label2.Visible = true;
            label1.Visible = true;
            textBox1.Visible = false;
            textBox2.Visible = true;
            button1.Visible = true;
            button1.Text = "Editar";
            button2.Visible = true;
            comboBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            desassociarFuncoes();
            listarRotasEstaticas();
            button2.Click -= button2_click_configurarVPN;
            button2.Click -= button2_click_configurar_ProtocolosEncaminhamento;
            button2.Click -= button2_click_configurar_RedesWireless;
            button2.Click -= button2_click_configurar_ServidoresDNS;
            button2.Click -= button2_click_editar_EnderecosIP;
            button2.Click -= button2_click_editarBridge;
            button2.Click -= button2_click_editar_PerfilSeguranca;
            button2.Click -= button2_click_editar_RegraFirewall;
            button2.Click -= button2_click_editar_RotasEstaticas;
            button2.Click -= button2_click_editar_servidorDHCP;
            button2.Click += new EventHandler(button2_click_editar_RotasEstaticas);
            button1.Click += new EventHandler(editarRotaEstatica);
        }

        //Editar rotas estáticas
        private void editarRotaEstatica(object sender, EventArgs e)
        {
            //Verifica se a Combo Box tem um valor selecionado
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                listBox1.Items.Clear();
                string url = $"{baseUrl}ip/route/{comboBox1.Text}";
                string dstAddress = textBox2.Text;
                string gateway = textBox3.Text;
                // string payload = "{\"dst-address\":\"" + dstAddress + "\",\"gateway\":\"" + gateway + "\"}";
                string payload = "{\"dst-address\":\"" + dstAddress + "\",\"gateway\":\"" + gateway + "\"}";
                try
                {

                    //Criar um pedido PATCH Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "PATCH";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                    //Escrever o payload para o body do pedido
                    using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                    {
                        writer.Write(payload);
                    }

                    //Ler a resposta e passar para um array em JSON
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler a resposta
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseJson = reader.ReadToEnd();
                        listBox1.Items.Add($"A rota estátiva foi criada com sucesso: {responseJson}");
                    }

                    //Limpar
                    response.Close();

                }
                catch (WebException ex)
                {
                    //Erro do pedido
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        listBox1.Items.Add($"Erro ao criar a rota estática devido a: {responseText}");
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
        }

        //Preenche os campos das rotas estáticas que se pretende editar
        public void button2_click_editar_RotasEstaticas(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                try
                {
                    //Criar um pedido GET Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "ip/route/" + comboBox1.Text);
                    request.Method = "GET";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                    //Enviar o pedido e obter a resposta 
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler a resposta e passar para um array em JSON
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        JObject responseJson = JObject.Parse(reader.ReadToEnd());

                        string dst_address = responseJson.Value<string>("dst-address");
                        string gateway = responseJson.Value<string>("gateway");

                        textBox2.Text = dst_address;
                        textBox3.Text = gateway;
                    }
                    //Limpar
                    response.Close();
                }
                catch (WebException ex)
                {
                    //Erro do pedido
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        listBox1.Items.Add($"Erro ao listar interfaces bridge devido a: {responseText}");
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Chama a função para editar endereços Ips quando se clica no botão editar
        private void endereçoIPToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label6.Visible = true; label7.Visible = true; label8.Visible = true; label9.Visible = true; label10.Visible = true; label11.Visible = true; label12.Visible = true;
            label5.Text = "id"; label6.Text = "nome"; label7.Text = "address"; label8.Text = "disabled"; label9.Text = "dynamic"; label10.Text = "interface"; label11.Text = "invalid"; label12.Text = "network";

            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            comboBox1.Visible = true;
            comboBox2.Visible = false;
            label2.Text = "Endereço IP";
            textBox3.Visible = true;
            textBox4.Visible = true;
            label3.Visible = true;
            label3.Text = "Interface";
            label4.Visible = true;
            label4.Text = "Rede";
            label1.Text = "ID do endereço Ip";
            label2.Visible = true;
            label1.Visible = true;
            textBox1.Visible = false;
            textBox2.Visible = true;
            button1.Visible = true;
            button1.Text = "Editar";
            button2.Visible = true;
            comboBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            desassociarFuncoes();
            listarEndereçosIps();
            button2.Click -= button2_click_configurarVPN;
            button2.Click -= button2_click_configurar_ProtocolosEncaminhamento;
            button2.Click -= button2_click_configurar_RedesWireless;
            button2.Click -= button2_click_configurar_ServidoresDNS;
            button2.Click -= button2_click_editar_EnderecosIP;
            button2.Click -= button2_click_editarBridge;
            button2.Click -= button2_click_editar_PerfilSeguranca;
            button2.Click -= button2_click_editar_RegraFirewall;
            button2.Click -= button2_click_editar_RotasEstaticas;
            button2.Click -= button2_click_editar_servidorDHCP;
            button2.Click += new EventHandler(button2_click_editar_EnderecosIP);
            button1.Click += new EventHandler(editarEnderecoIP);
        }

        //Editar endereços Ips
        private void editarEnderecoIP(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                listBox1.Items.Clear();
                string url = $"{baseUrl}ip/address/{comboBox1.Text}";
                string address = textBox2.Text;
                string interface2 = textBox3.Text;
                string network = textBox4.Text;
                string payload = "{\"address\":\"" + address + "\",\"interface\":\"" + interface2 + "\",\"network\":\"" + network + "\"}";

                try
                {
                    //Criar um pedido PATCH Http
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "PATCH";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                    //Ler a resposta e passar para um array em JSON
                    using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                    {
                        writer.Write(payload);
                    }

                    //Ler a resposta e passar para um array em JSON
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler a resposta
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseJson = reader.ReadToEnd();
                        listBox1.Items.Add($"O endereço IP foi alterado com sucesso: {responseJson}");
                    }

                    //Limpar
                    response.Close();

                }
                catch (WebException ex)
                {
                    //Erro do pedido
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        listBox1.Items.Add($"Erro ao editar o endereço IP devido a: {responseText}");
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Preenche os campos dos endereços Ips que se pretende editar
        public void button2_click_editar_EnderecosIP(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                try
                {
                    //Criar um pedido GET Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "ip/address/" + comboBox1.Text);
                    request.Method = "GET";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));
                    
                    //Enviar o pedido e obter a resposta
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler a resposta e passar para um array em JSON
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        JObject responseJson = JObject.Parse(reader.ReadToEnd());

                        string address = responseJson.Value<string>("address");
                        string interface2 = responseJson.Value<string>("interface");
                        string network = responseJson.Value<string>("network");

                        textBox2.Text = address;
                        textBox3.Text = interface2;
                        textBox4.Text = network;
                    }
                    //Limpar
                    response.Close();
                }
                catch (WebException ex)
                {
                    //Erro do pedido
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        listBox1.Items.Add($"Erro ao listar os endereços Ip devido a: {responseText}");
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Chama a função para editar servidores DHCP quando se clica no botão editar
        private void servidorDHCPToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label6.Visible = true; label7.Visible = true;
            label5.Text = "id"; label6.Text = "address pool"; label7.Text = "interface";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Items.Clear();
            comboBox3.Visible = true;
            comboBox1.Visible = true;
            comboBox2.Visible = false;
            label2.Text = "Nome do servidor DHCP";
            textBox3.Visible = false;
            textBox4.Visible = true;
            label3.Visible = true;
            label3.Text = "Pool de endereços";
            label4.Visible = true;
            label4.Text = "Interface do servidor DHCP";
            label1.Text = "ID do servidor DHCP";
            label2.Visible = true;
            label1.Visible = true;
            textBox1.Visible = false;
            comboBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox2.Visible = true;
            button1.Visible = true;
            button1.Text = "Editar";
            button2.Visible = true;
            desassociarFuncoes();
            listarServidoresDHCP();
            button2.Click -= button2_click_configurar_ProtocolosEncaminhamento;
            button2.Click -= button2_click_configurar_RedesWireless;
            button2.Click -= button2_click_configurar_ServidoresDNS;
            button2.Click -= button2_click_editar_EnderecosIP;
            button2.Click -= button2_click_editarBridge;
            button2.Click -= button2_click_editar_PerfilSeguranca;
            button2.Click -= button2_click_editar_RegraFirewall;
            button2.Click -= button2_click_editar_RotasEstaticas;
            button2.Click -= button2_click_editar_servidorDHCP;
            button2.Click -= button2_click_configurarVPN;
            button2.Click += new EventHandler(button2_click_editar_servidorDHCP);
            button1.Click += new EventHandler(editarDHCP);
        }

        //Editar servidor DHCP
        private void editarDHCP(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                listBox1.Items.Clear();
                string url = $"{baseUrl}ip/dhcp-server/{comboBox1.Text}";
                string name = textBox2.Text;
                string interface2 = textBox4.Text;
                // string payload = "{\"dst-address\":\"" + dstAddress + "\",\"gateway\":\"" + gateway + "\"}";
                string payload = "{\"name\":\"" + name + "\",\"interface\":\"" + interface2 + "\",\"address-pool\":\"" + comboBox3.Text + "\"}";
                try
                {
                    //Criar um pedido PATCH Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "PATCH";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                    //Ler a resposta e passar para um array em JSON
                    using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                    {
                        writer.Write(payload);
                    }

                    //Ler a resposta e passar para um array em JSON
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler a resposta
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseJson = reader.ReadToEnd();
                        listBox1.Items.Add($"O servidor DHCP foi alterado com sucesso: {responseJson}");
                    }

                    //Limpar
                    response.Close();

                }
                catch (WebException ex)
                {
                    //Erro do pedido
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseJson = reader.ReadToEnd();
                        listBox1.Items.Add($"Erro ao editar o servidor DHCP devido a: {responseJson}");
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }           
        }

        //Preenche os campos do servidor DHCP que se pretende editar
        public void button2_click_editar_servidorDHCP(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                try
                {
                    //Criar um pedido GET Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "ip/dhcp-server/" + comboBox1.Text);
                    request.Method = "GET";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                    //Enviar o pedido e obter a resposta
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler a resposta e passar para um array em JSON
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        JObject responseJson = JObject.Parse(reader.ReadToEnd());

                        string name = responseJson.Value<string>("name");
                        string interface2 = responseJson.Value<string>("interface");
                        string address_pool = responseJson.Value<string>("address-pool");
                        textBox2.Text = name;
                        comboBox3.Text = address_pool;
                        textBox4.Text = interface2;
                    }
                    //Limpar
                    response.Close();
                }
                catch (WebException ex)
                {
                    //Erro do pedido
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        listBox1.Items.Add($"Erro ao listar os endereços Ip devido a: {responseText}");
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void listarDHCPpools()
        {
            listBox1.Items.Clear();
            try
            {
                //Criar um pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "ip/pool");
                request.Method = "GET";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JArray responseJson = JArray.Parse(reader.ReadToEnd());
                    comboBox3.Items.Clear();
                    foreach (JObject obj in responseJson)
                    {
                        
                        string name = obj.Value<string>("name");
                        comboBox3.Items.Add(name);
                    }
                }
                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao listar interfaces bridge devido a: {responseText}");
                }
            }
        }


        //Chama a função para editar regras de firewall quando se clica no botão editar
        private void regraFirewallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label6.Visible = true; label7.Visible = true;
            label5.Text = "id"; label6.Text = "action"; label7.Text = "chain";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Items.Clear();          
            comboBox3.Items.AddRange(new string[] { "input", "output", "forward" });
            comboBox3.Visible = true;
            button1.Text = "Editar";
            button1.Visible = true;
            textBox1.Visible = false;
            comboBox1.Visible = true;
            textBox2.Visible = false;
            comboBox2.Visible = true;
            textBox3.Visible = false;
            textBox4.Visible = false;
            label1.Visible = true;
            label1.Text = "ID da regra de Firewall";
            label2.Visible = true;
            label2.Text = "Ação";
            label3.Visible = true;
            label4.Visible = false;
            label3.Text = "Chain";
            label4.Text = "Endereço de Origem";
            textBox4.Visible = true;
            label4.Visible = true;
            button2.Visible = true;
            listarRegrasFirewall();
            desassociarFuncoes();
            button2.Click -= button2_click_configurar_ProtocolosEncaminhamento;
            button2.Click -= button2_click_configurar_RedesWireless;
            button2.Click -= button2_click_configurar_ServidoresDNS;
            button2.Click -= button2_click_editar_EnderecosIP;
            button2.Click -= button2_click_editarBridge;
            button2.Click -= button2_click_editar_PerfilSeguranca;
            button2.Click -= button2_click_editar_RegraFirewall;
            button2.Click -= button2_click_editar_RotasEstaticas;
            button2.Click -= button2_click_editar_servidorDHCP;
            button2.Click -= button2_click_configurarVPN;
            button2.Click += new EventHandler(button2_click_editar_RegraFirewall);        
            button1.Click += new EventHandler(editarRegraFirewall);
        }

        //Editar regra Firewall
        private void editarRegraFirewall(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                listBox1.Items.Clear();
                string firewallId = comboBox1.Text;
                string url = $"{baseUrl}ip/firewall/filter/{firewallId}";
                string srcAddress = textBox4.Text;
                string payload = "{\"action\":\"" + comboBox2.Text + "\",\"chain\":\"" + comboBox3.Text + "\",\"src-address\":\"" + srcAddress + "\"}";
                try
                {

                    //Criar um pedido PATCH Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "PATCH";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                    //Esvrebe o payload do Json para o body do pedido
                    using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                    {
                        writer.Write(payload);
                    }

                    //Ler a resposta e passar para um array em JSON
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    // Read the response body
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseJson = reader.ReadToEnd();
                        listBox1.Items.Add($"A regra de firewall foi alterada com sucesso: {responseJson}");
                    }

                    //Limpar
                    response.Close();

                }
                catch (WebException ex)
                {
                    //Erro do pedido
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseJson = reader.ReadToEnd();
                        listBox1.Items.Add($"Erro ao editar a regra da firewall devido a: {responseJson}");
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

        //Preenche os campos da regra firewall que se pretende editar
        public void button2_click_editar_RegraFirewall(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                try
                {
                    //Criar um pedido GET Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "ip/firewall/filter/" + comboBox1.Text);
                    request.Method = "GET";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                    //Ler a resposta e passar para um array em JSON
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler a resposta e passar para um array em JSON
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        JObject responseJson = JObject.Parse(reader.ReadToEnd());

                        string chain = responseJson.Value<string>("chain");
                        string action = responseJson.Value<string>("action");
                        string src_address = responseJson.Value<string>("src-address");
                        comboBox3.Text = chain;
                        comboBox2.Text = action;
                        textBox4.Text = src_address;
                    }
                    //Limpar
                    response.Close();
                }
                catch (WebException ex)
                {
                    //Erro do pedido
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        listBox1.Items.Add($"Erro ao listar as regras de firewall devido a: {responseText}");
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Chama a função para editar perfis de segurança quando se clica no botão editar
        private void perfilDeSegurançaToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label6.Visible = true; label7.Visible = true; label8.Visible = true;
            label5.Text = "id"; label6.Text = "autenticação"; label7.Text = "ciphers"; label8.Text = "certificado";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            button1.Text = "Editar";
            comboBox1.Visible = true;
            comboBox2.Visible = false;
            button1.Visible = true;
            textBox1.Visible = false;
            textBox2.Visible = true;
            textBox3.Visible = true;
            textBox4.Visible = false;
            label1.Visible = true;
            label1.Text = "ID do perfil de segurança";
            label2.Visible = true;
            label2.Text = "Nome";
            label3.Visible = true;
            label4.Visible = false;
            label3.Text = "Comentário";
            label4.Text = "";
            button2.Visible = true;
            listarPerfisSegurança();
            desassociarFuncoes();
            button2.Click -= button2_click_configurar_ProtocolosEncaminhamento;
            button2.Click -= button2_click_configurar_RedesWireless;
            button2.Click -= button2_click_configurar_ServidoresDNS;
            button2.Click -= button2_click_editar_EnderecosIP;
            button2.Click -= button2_click_editarBridge;
            button2.Click -= button2_click_editar_PerfilSeguranca;
            button2.Click -= button2_click_editar_RegraFirewall;
            button2.Click -= button2_click_editar_RotasEstaticas;
            button2.Click -= button2_click_editar_servidorDHCP;
            button2.Click -= button2_click_configurarVPN;
            button2.Click += new EventHandler(button2_click_editar_PerfilSeguranca);
            button1.Click += new EventHandler(editarPerfilSegurança);
        }


        //Editar perfil de segurança
        private void editarPerfilSegurança(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                listBox1.Items.Clear();
                string profileId = comboBox1.Text;
                string name = textBox2.Text;
                string comment = textBox3.Text;
                string url = $"{baseUrl}interface/wireless/security-profiles/{profileId}"; ;

                string payload = "{\"name\":\"" + name + "\",\"comment\":\"" + comment + "\"}";

                try
                {
                    //Criar um pedido PATCH Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "PATCH";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                    //Escrever o JSON do payload para o body do pedido
                    using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                    {
                        writer.Write(payload);
                    }

                    //Ler a resposta e passar para um array em JSON
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler a resposta
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseJson = reader.ReadToEnd();
                        listBox1.Items.Add($"O perfil de segurança foi alterado com sucesso: {responseJson}");
                    }

                    //Limpar
                    response.Close();

                }
                catch (WebException ex)
                {
                    //Erro do pedido
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseJson = reader.ReadToEnd();
                        listBox1.Items.Add($"Erro ao editar o perfil de segurança: {responseJson}");
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        //Preenche os campos do perfil de segurança que se pretende editar
        public void button2_click_editar_PerfilSeguranca(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                try
                {
                    //Criar um pedido GET Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "interface/wireless/security-profiles/" + comboBox1.Text);
                    request.Method = "GET";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                
                    //Enviar o pedido e obter a resposta
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler a resposta e passar para um array em JSON
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        JObject responseJson = JObject.Parse(reader.ReadToEnd());

                        string name = responseJson.Value<string>("name");
                        string comment = responseJson.Value<string>("comment");
                        textBox2.Text = name;
                        textBox3.Text = comment;

                    }
                    //Limpar
                    response.Close();
                }
                catch (WebException ex)
                {
                    //Erro do pedido
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        listBox1.Items.Add($"Erro ao listar os endereços Ip devido a: {responseText}");
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
        }

        #endregion

        #region Eliminar

        //Chama a função para eliminar interfaces bridge quando se clica no botão eliminar
        private void interfaceBridgeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label6.Visible = true; label7.Visible = true;
            label5.Text = "id"; label6.Text = "name"; label7.Text = "mtu";
            listarBridges();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            button1.Text = "Eliminar";
            button1.Visible = true;
            textBox1.Visible = false;
            comboBox1.Visible = true;
            textBox2.Visible = false;
            comboBox2.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            label1.Visible = true;
            label1.Text = "ID da Bridge";
            label2.Visible = false;
            label2.Text = "";
            label3.Visible = false;
            label4.Visible = false;
            label3.Text = "";
            label4.Text = "";
            button2.Visible = false;
            desassociarFuncoes();
            button1.Click += new EventHandler(eliminarBridge);

        }

        //Eliminar interface bridge
        private void eliminarBridge(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                listBox1.Items.Clear();
                string url = $"{baseUrl}interface/bridge/remove";
                string bridgeId = comboBox1.Text;

                string payload = "{\"numbers\": \"" + bridgeId + "\"}";

                try
                {
                    //Criar um novo pedido POST Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                    //Escreve o JSON do payload para o pedido do body
                    using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                    {
                        writer.Write(payload);
                    }

                    //Enviar o pedido e obter a resposta
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler a resposta
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseJson = reader.ReadToEnd();
                        listBox1.Items.Add($"A interface bridge foi eliminada com sucesso: {responseJson}");
                    }

                    //Limpar
                    response.Close();
                }
                catch (WebException error)
                {
                    //Erro do pedido
                    using (var stream = error.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        listBox1.Items.Add($"Erro ao eliminar a interface bridge devido a: {responseText}");
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        //Chama a função para eliminar rotas estáticas quando se clica no botão eliminar
        private void rotasEstáticasToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label7.Visible = true; label9.Visible = true;
            label5.Text = "id"; label7.Text = "destiny"; label9.Text = "gateway";
            listarRotasEstaticas();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            button1.Text = "Eliminar";
            button1.Visible = true;
            textBox1.Visible = false;
            comboBox1.Visible = true;
            textBox2.Visible = false;
            comboBox2.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            label1.Visible = true;
            label1.Text = "ID da Rota Estática";
            label2.Visible = false;
            label2.Text = "";
            label3.Visible = false;
            label4.Visible = false;
            label3.Text = "";
            label4.Text = "";
            button2.Visible = false;
            desassociarFuncoes();
            button1.Click += new EventHandler(eliminarRotasEstaticas);
        }

        //Eliminar rotas estáticas
        private void eliminarRotasEstaticas(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                listBox1.Items.Clear();
                string routeId = comboBox1.Text;
                string url = $"{baseUrl}ip/route/{routeId}";

                try
                {
                    //Criar um novo pedido DELETE Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "DELETE";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                    //Enviar o pedido e obter a resposta
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler a resposta
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseJson = reader.ReadToEnd();
                        listBox1.Items.Add($"A rota estática foi eliminada com sucesso: {responseJson}");
                    }

                    //Limpar
                    response.Close();
                }
                catch (WebException error)
                {
                    //Erro do pedido
                    using (var stream = error.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        listBox1.Items.Add($"Erro ao eliminar a rota estática devido a: {responseText}");
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        //Chama a função para eliminar endereços Ip quando se clica no botão eliminar 
        private void endereçoIpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label6.Visible = true; label7.Visible = true; label8.Visible = true; label9.Visible = true; label10.Visible = true; label11.Visible = true; label12.Visible = true;
            label5.Text = "id"; label6.Text = "nome"; label7.Text = "address"; label8.Text = "disabled"; label9.Text = "dynamic"; label10.Text = "interface"; label11.Text = "invalid"; label12.Text = "network";

            listarEndereçosIps();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            button1.Text = "Eliminar";
            button1.Visible = true;
            textBox1.Visible = false;
            comboBox1.Visible = true;
            textBox2.Visible = false;
            comboBox2.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            label1.Visible = true;
            label1.Text = "ID do endereço IP";
            label2.Visible = false;
            label2.Text = "";
            label3.Visible = false;
            label4.Visible = false;
            label3.Text = "";
            label4.Text = "";
            button2.Visible = false;
            desassociarFuncoes();
            button1.Click += new EventHandler(eliminarEndereçoIp);

        }

        //Eliminar endereços Ip
        private void eliminarEndereçoIp(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                listBox1.Items.Clear();
                string addressId = comboBox1.Text;
                string url = $"{baseUrl}ip/address/remove";
                string payload = $"{{\".id\":\"{addressId}\"}}";

                try
                {
                    //Criar um novo pedido POST Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                    //Escreve o JSON do payload para o body do pedido
                    using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                    {
                        writer.Write(payload);
                    }

                    //Enviar o pedido e obter a resposta
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler a resposta
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseJson = reader.ReadToEnd();
                        listBox1.Items.Add($"IP address with ID {addressId} deleted successfully: {responseJson}");
                    }

                    //Limpar
                    response.Close();
                }
                catch (WebException ex)
                {
                    //Erro do pedido
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        listBox1.Items.Add($"Error deleting IP address with ID {addressId}: {responseText}");
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Chama a função para eliminar servidores DHCP quando se clica no botão eliminar
        private void servidorDHCPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label6.Visible = true; label7.Visible = true;
            label5.Text = "id"; label6.Text = "address pool"; label7.Text = "interface";
            listarServidoresDHCP();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            button1.Text = "Eliminar";
            button1.Visible = true;
            textBox1.Visible = false;
            comboBox1.Visible = true;
            textBox2.Visible = false;
            comboBox2.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            label1.Visible = true;
            label1.Text = "ID do DCHP";
            label2.Visible = false;
            label2.Text = "";
            label3.Visible = false;
            label4.Visible = false;
            label3.Text = "";
            label4.Text = "";
            button2.Visible = false;
            desassociarFuncoes();
            button1.Click += new EventHandler(eliminarDHCP);

        }

        //Eliminar servidor DHCP
        private void eliminarDHCP(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                listBox1.Items.Clear();
                string dhcpId = comboBox1.Text;
                string url = $"{baseUrl}ip/dhcp-server/{dhcpId}";

                try
                {
                    //Criar um novo pedido DELETE Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "DELETE";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                    //Enviar o pedido e obter a resposta
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseJson = reader.ReadToEnd();
                        listBox1.Items.Add($"O servidor DHCP foi eliminado com sucesso: {responseJson}");
                    }

                    //Limpar
                    response.Close();
                }
                catch (WebException ex)
                {
                    //Erro do pedido
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        listBox1.Items.Add($"Erro ao eleminar o servidor DHCP devido a: {responseText}");
                    }
                } 
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Chama a função para eliminar regras de firewall quando se clica no botão eliminar
        private void regrasDeFirewallToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label6.Visible = true; label7.Visible = true;
            label5.Text = "id"; label6.Text = "action"; label7.Text = "chain";
            listarRegrasFirewall();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            button1.Text = "Eliminar";
            button1.Visible = true;
            textBox1.Visible = false;
            comboBox1.Visible = true;
            textBox2.Visible = false;
            comboBox2.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            label1.Visible = true;
            label1.Text = "ID da Firewall";
            label2.Visible = false;
            label2.Text = "";
            label3.Visible = false;
            label4.Visible = false;
            label3.Text = "";
            label4.Text = "";
            button2.Visible = false;
            desassociarFuncoes();
            button1.Click += new EventHandler(apagarRegrasFirewall);

        }

        //Eliminar regras de firewall
        private void apagarRegrasFirewall(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                listBox1.Items.Clear();

                string url = $"{baseUrl}ip/firewall/filter/remove";
                string payload = "{\"numbers\": \"" + comboBox1.Text + "\"}";

                try
                {
                    //Criar um novo pedido POST Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                    //Escreve o JSON do payload para o pedido do body
                    using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                    {
                        writer.Write(payload);
                    }
            
                    //Enviar o pedido e receber a resposta
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler a resposta
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseJson = reader.ReadToEnd();
                        listBox1.Items.Add($"A regra da firewall foi eliminada com sucesso: {responseJson}");
                    }

                    //Limpar
                    response.Close();
                }
                catch (WebException ex)
                {
                    //Erro do pedido
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        listBox1.Items.Add($"Erro ao eliminar a regra da firewall devido a: {responseText}");
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Chama a função para eliminar perfis de segurança quando se clica no botão eliminar
        private void perfilDeSegurançaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            limparForm();
            label5.Visible = true; label6.Visible = true; label7.Visible = true; label8.Visible = true;
            label5.Text = "id"; label6.Text = "autenticação"; label7.Text = "ciphers"; label8.Text = "certificado";
            listarPerfisSegurança();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            button1.Text = "Eliminar";
            comboBox1.Visible = true;
            comboBox2.Visible = false;
            button1.Visible = true;
            textBox1.Visible = false;
            textBox2.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            label1.Visible = true;
            label1.Text = "ID do Perfil";
            label2.Visible = true;
            label2.Text = "";
            label3.Visible = false;
            label4.Visible = false;
            label3.Text = "";
            label4.Text = "";
            button2.Visible = false;
            desassociarFuncoes();
            button1.Click += new EventHandler(eliminarPerfilSegurança);
        }

        //Eliminar perfis de segurança
        private void eliminarPerfilSegurança(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                listBox1.Items.Clear();
                string profileId = comboBox1.Text; // replace with the ID of the profile to be deleted
                string url = $"{baseUrl}interface/wireless/security-profiles/remove";
                string payload = $"{{\".id\":\"{profileId}\"}}";

                try
                {
                    //Criar um novo pedido POST Http
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                    //Escreve o JSON do payload para o pedido do body
                    using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                    {
                        writer.Write(payload);
                    }

                    //Enviar o pedido e receber a resposta
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler a resposta
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseJson = reader.ReadToEnd();
                        listBox1.Items.Add($"Security profile with ID {profileId} deleted successfully: {responseJson}");
                    }

                    //Limpar
                    response.Close();
                }
                catch (WebException ex)
                {
                    //Erro do pedido
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        listBox1.Items.Add($"Error deleting security profile with ID {profileId}: {responseText}");
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Ativar

        //Chama a função para ativar redes wireless quando se clica no botão ativar
        private void redesWirelessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            button1.Visible = true;
            button1.Text = "Ativar";
            textBox1.Visible = false;
            comboBox1.Visible = true;
            textBox2.Visible = false;
            comboBox2.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            label1.Visible = true;
            label1.Text = "ID da Rede Wireless";
            label2.Visible = false;
            label2.Text = "";
            label3.Visible = false;
            label4.Visible = false;
            label3.Text = "";
            label4.Text = "";
            button2.Visible = false;
            listarRedesWireless();
            desassociarFuncoes();
            button1.Click += new EventHandler(ativarRedesWireless);
        }

        //Ativar redes de wireless
        private void ativarRedesWireless(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                listBox1.Items.Clear();
                string id = comboBox1.Text;
                string url = $"{baseUrl}interface/wireless/{id}";
                string payload = "{\"disabled\":\"false\"}";

                try
                {
                    //Criar um novo pedido PATCH Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "PATCH";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                    //Escreve o JSON no payload para o body do pedido
                    using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                    {
                        writer.Write(payload);
                    }
            
                    //Enviar o pedido e receber a resposta
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler a resposta
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseJson = reader.ReadToEnd();
                        listBox1.Items.Add($"A rede wireless foi ativada com sucesso: {responseJson}");
                    }

                    //Limpar
                    response.Close();
                }
                catch (WebException ex)
                {
                    //Erro do pedido
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        listBox1.Items.Add($"Erro ao ativar a rede wireless devido a: {responseText}");
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Ativar o servidor DNS
        private void servidoresDNSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            button2.Visible = false;
            textBox1.Visible = false;
            comboBox1.Visible = false;
            label1.Visible = false;

            listBox1.Items.Clear();
            string url = $"{baseUrl}ip/dns/set";
            string payload = "{\"servers\": [{\"disabled\":\"no\"}]}";

            try
            {
                //Criar um novo pedido POST Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                //Escreve o JSON no payload para o body do pedido
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(payload);
                }

                //Enviar o pedido e receber a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler o pedido
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseJson = reader.ReadToEnd();
                    listBox1.Items.Add($"O servidor DNS foi ativado com sucesso: {responseJson}");
                }
                //Limpar
                response.Close();

            }
            catch (WebException error)
            {
                //Erro do pedido
                using (var stream = error.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao ativar o servidor DNS devido a: {responseText}");
                }
            }
        }

        //Ativar protocolos de encaminhamento
        private void protocolosDeEncaminhamentoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            button2.Visible = false;
            textBox1.Visible = false;
            comboBox1.Visible = false;
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            textBox2.Visible = false;
            textBox3.Visible = false;

            listBox1.Items.Clear();
            string url = $"{baseUrl}ip/settings/set";
            string payload = "{\"ip-forward\":\"true\"}";

            try
            {
                //Criar um novo pedido POST Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                //Escreve o JSON no payload para o body do pedido
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(payload);
                }

            
                //Enviar o pedido e receber a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler o pedido
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseJson = reader.ReadToEnd();
                    listBox1.Items.Add($"O protocolo de encaminhamento foi ativado com sucesso: {responseJson}");
                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao ativar o protocolo de encaminhamento devido a: {responseText}");
                }
            }
        }
        #endregion

        #region Desativar

        //Chama a função para ativar redes wireless quando se clica no botão desativar
        private void redesWirelessToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            button1.Visible = true;
            button1.Text = "Desativar";
            textBox1.Visible = false;
            comboBox1.Visible = true;
            textBox2.Visible = false;
            comboBox2.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            label1.Visible = true;
            label1.Text = "Wireless Network ID";
            label2.Visible = false;
            label2.Text = "";
            label3.Visible = false;
            label4.Visible = false;
            label3.Text = "";
            label4.Text = "";
            button2.Visible = false;
            listarRedesWireless();
            desassociarFuncoes();
            button1.Click += new EventHandler(desativarRedesWireless);
        }

        //Desativar redes wireless
        private void desativarRedesWireless(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                listBox1.Items.Clear();
                string id = comboBox1.Text;
                string url = $"{baseUrl}interface/wireless/{id}";
                string payload = "{\"disabled\":\"true\"}";

                try
                {
                    //Criar um novo pedido PATCH Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "PATCH";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                    //Escreve o JSON no payload para o body do pedido
                    using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                    {
                        writer.Write(payload);
                    }
            
                    //Enviar o pedido e receber a resposta
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler o pedido
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseJson = reader.ReadToEnd();
                        listBox1.Items.Add($"A rede wireless foi desativada com sucesso: {responseJson}");
                    }

                    //Limpar
                    response.Close();
                }
                catch (WebException ex)
                {
                    //Erro do pedido
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        listBox1.Items.Add($"Erro ao desativar a rede wireless devido a: {responseText}");
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Desativar o servidor DNS
        private void servidoresDNSToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            button2.Visible = false;
            textBox1.Visible = false;
            label1.Text = "";
            comboBox1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            textBox2.Visible = false;
            textBox3.Visible = false;

            listBox1.Items.Clear();

            string url = $"{baseUrl}ip/dns/set";
            string payload = "{\"servers\": [{\"disabled\":\"yes\"}]}";

            try
            {
                //Criar um novo pedido POST Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                //Escreve o JSON no payload para o body do pedido
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(payload);
                }
            
                //Enviar o pedido e receber a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler o pedido
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseJson = reader.ReadToEnd();
                    listBox1.Items.Add($"O servidor DNS foi desativado com sucesso: {responseJson}");
                }

                //Limpar
                response.Close();
            }
            catch (WebException error)
            {
                //Erro do pedido
                using (var stream = error.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao desativar o servidor DNS devido a: {responseText}");
                }
            }
        }

        //Desativar protocolos de encaminhamento
        private void protocolosDeEncaminhamentoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            button2.Visible = false;
            textBox1.Visible = false;
            label1.Text = "";
            comboBox1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            textBox2.Visible = false;
            textBox3.Visible = false;

            listBox1.Items.Clear();
            string url = $"{baseUrl}ip/settings/set";
            string payload = "{\"ip-forward\":\"false\"}";

            try
            {
                //Criar um novo pedido POST Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));


                //Escreve o JSON no payload para o body do pedido
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(payload);
                }

                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseJson = reader.ReadToEnd();
                    listBox1.Items.Add($"O protocolo de encaminhamento foi desativado com sucesso: {responseJson}");
                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao desativar o protocolo de encaminhamento devido a: {responseText}");
                }
            }
        }
        #endregion region

        #region Configurar


        //Chama a função para configurar redes wireless quando se clica no botão configurar
        private void redesWirelessToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            comboBox1.Visible = true;
            comboBox2.Visible = false;
            label2.Text = "Modo Bridge (Ativado/Desativado)";
            textBox3.Visible = true;
            textBox4.Visible = true;
            label3.Visible = true;
            label3.Text = "Frequência";
            label4.Text = "Perfil de segurança";
            label4.Visible = true;
            label1.Text = "ID da Rede Wireless";
            label2.Visible = true;
            label1.Visible = true;
            textBox1.Visible = false;
            textBox2.Visible = true;
            button1.Visible = true;
            button1.Text = "Configurar";
            button2.Visible = true;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            listarRedesWireless();
            button2.Click -= button2_click_configurar_ProtocolosEncaminhamento;
            button2.Click -= button2_click_configurar_RedesWireless;
            button2.Click -= button2_click_configurar_ServidoresDNS;
            button2.Click -= button2_click_editar_EnderecosIP;
            button2.Click -= button2_click_editarBridge;
            button2.Click -= button2_click_editar_PerfilSeguranca;
            button2.Click -= button2_click_editar_RegraFirewall;
            button2.Click -= button2_click_editar_RotasEstaticas;
            button2.Click -= button2_click_editar_servidorDHCP;
            desassociarFuncoes();
            button2.Click -= button2_click_configurarVPN;
            button2.Click += new EventHandler(button2_click_configurar_RedesWireless);
            button1.Click += new EventHandler(configurarRedesWireless);
        }

        //Configurar Interfaces Wireless
        private void configurarRedesWireless(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                string bridgeId = comboBox1.Text;
                string url = $"{baseUrl}interface/wireless/{bridgeId}";

                string bridgeMode = textBox2.Text;
                string frequency = textBox3.Text;
                string securityProfile = textBox4.Text;

            
                string payload = "{\"bridge-mode\":\"" + bridgeMode + "\",\"frequency\":\"" + frequency + "\",\"security-profile\":\"" + securityProfile + "\"}";

                try
                {
                    //Criar um novo pedido POST Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "PATCH";
                    request.ContentType = "application/json";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                    //Escreve o JSON do payload e enviar para o body do pedido
                    using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                    {
                        writer.Write(payload);
                    }
           
                    //Ler o pedido e obtem a resposta
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Ler a resposta
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string responseJson = reader.ReadToEnd();
                        listBox1.Items.Add($"A interface wireless foi configurada com sucesso: {responseJson}");
                    }

                    //Limpar
                    response.Close();

                }
                catch (WebException ex)
                {
                    //Erro do pedido
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        listBox1.Items.Add($"Erro ao configurar a interface wireless devido a: {responseText}");
                    }
                }
            }
            else
            {
                //A comboBox não tem um valor selecionado
                MessageBox.Show("Tem de preencher a comboBox com um valor válido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Preenche os campos do servidor DNS que se pretende editar
        public void button2_click_configurar_RedesWireless(object sender, EventArgs e)
        {
            //Verifica se a comboBox foi preenchida
            if (comboBox1.SelectedIndex >= 0 && comboBox1.SelectedItem != null)
            {
                try
                {
                    //Criar um novo pedido GET Http
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "interface/wireless/" + comboBox1.Text);
                    request.Method = "GET";
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                    //Envia o pedido e obtem a resposta
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    //Lê a reposta e passa para um array em JSON
                    using (Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        JObject responseJson = JObject.Parse(reader.ReadToEnd());

                        string bridgeMode = responseJson.Value<string>("bridge-mode");
                        string frequency = responseJson.Value<string>("frequency");
                        string securityProfile = responseJson.Value<string>("security-profile");
                        textBox2.Text = bridgeMode;
                        textBox3.Text = frequency;
                        textBox4.Text = securityProfile;

                    }

                    //Limpar
                    response.Close();
                }
                catch (WebException ex)
                {
                    //Erro do pedido
                    using (var stream = ex.Response.GetResponseStream())
                    using (var reader = new StreamReader(stream))
                    {
                        var responseText = reader.ReadToEnd();
                        listBox1.Items.Add($"Erro ao configurar a interface wireless devido a: {responseText}");
                    }
                }
            }
            else
            {
                //A comboBox não tem um item selecionado
                MessageBox.Show("Tem de selecionar um valor da comboBox", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }         
        }


        //Chama a função para configurar o servidor DNS quando se clica no botão configurar
        private void servidoresDNSToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            button1.Text = "Configurar";
            comboBox1.Visible = false;
            comboBox2.Visible = false;
            button1.Visible = true;
            textBox1.Visible = true;
            textBox2.Visible = true;
            textBox3.Visible = true;
            textBox4.Visible = true;
            label1.Visible = true;
            label1.Text = "Pedidos Remotos";
            label2.Visible = true;
            label2.Text = "Tamanho da Cache";
            label3.Visible = true;
            label4.Visible = true;
            label3.Text = "Doh timeout";
            label4.Text = "Servidores";
            button2.Visible = true;
            listarServidoresDNS();
            button2.Click -= button2_click_configurar_ProtocolosEncaminhamento;
            button2.Click -= button2_click_configurar_RedesWireless;
            button2.Click -= button2_click_configurar_ServidoresDNS;
            button2.Click -= button2_click_editar_EnderecosIP;
            button2.Click -= button2_click_editarBridge;
            button2.Click -= button2_click_editar_PerfilSeguranca;
            button2.Click -= button2_click_editar_RegraFirewall;
            button2.Click -= button2_click_editar_RotasEstaticas;
            button2.Click -= button2_click_editar_servidorDHCP;
            button2.Click -= button2_click_editar_servidorDHCP;
            button2.Click -= button2_click_configurarVPN;
            desassociarFuncoes();
            button2.Click += new EventHandler(button2_click_configurar_ServidoresDNS);            
            button1.Click += new EventHandler(configurarServidorDNS);
        }

        //Configurar Servidor Dns
        private void configurarServidorDNS(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            string remoteRequests = textBox1.Text;
            string dohTimeout = textBox3.Text;
            string cache = textBox2.Text;
            string primaryDns = textBox4.Text;

            string url = $"{baseUrl}ip/dns/set";

            string payload = "{\"allow-remote-requests\":\"" + remoteRequests + "\",\"servers\":\"" + primaryDns + "\",\"doh-timeout\":\"" + dohTimeout + "\",\"cache-size\":\"" + cache + "\"}";

            try
            {
                //Criar um novo pedido POST Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                //Escreve o JSON do payload e enviar para o body do pedido
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(payload);
                }
            
                //Envia o pedido e recebe a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler o pedido
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseJson = reader.ReadToEnd();
                    listBox1.Items.Add($"O servidor DNS foi configurado com sucesso: {responseJson}");
                }

                //Limpar
                response.Close();

            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseJson = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao configurar o servidor DNS: {responseJson}");
                }
            }
        }


        //Preenche os campos do servidor DNS que se pretende configurar
        public void button2_click_configurar_ServidoresDNS(object sender, EventArgs e)
        {
            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "ip/dns");
                request.Method = "GET";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

            
                //Envia o pedido e recebe a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Lê a reposta e passa para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject responseJson = JObject.Parse(reader.ReadToEnd());

                    string remoteRequets = responseJson.Value<string>("allow-remote-requests");
                    string cache = responseJson.Value<string>("cache-size");
                    string dynamicServers = responseJson.Value<string>("doh-timeout");
                    string primaryDns = responseJson.Value<string>("servers");

                    textBox1.Text = remoteRequets;
                    textBox2.Text = cache;
                    textBox3.Text= dynamicServers;
                    textBox4.Text= primaryDns;
                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao listar o servidor DNS devido a: {responseText}");
                }
            }
        }

        private void protocolosEncaminhamentoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            button1.Text = "Configurar";
            comboBox1.Visible = false;
            comboBox2.Visible = false;
            button1.Visible = true;
            textBox1.Visible = true;
            textBox2.Visible = true;
            textBox3.Visible = true;
            textBox4.Visible = true;
            label1.Visible = true;
            label1.Text = "Aceitar redirecionamentos";
            label2.Visible = true;
            label2.Text = "Tempo Limite Arp";
            label3.Visible = true;
            label4.Visible = true;
            label3.Text = "Taxa Icmp";
            label4.Text = "Encaminhamento do IP";
            button2.Visible = true;
            listarProtocolosEncaminhamento();
            button2.Click -= button2_click_configurar_ProtocolosEncaminhamento;
            button2.Click -= button2_click_configurar_RedesWireless;
            button2.Click -= button2_click_configurar_ServidoresDNS;
            button2.Click -= button2_click_editar_EnderecosIP;
            button2.Click -= button2_click_editarBridge;
            button2.Click -= button2_click_editar_PerfilSeguranca;
            button2.Click -= button2_click_editar_RegraFirewall;
            button2.Click -= button2_click_editar_RotasEstaticas;
            button2.Click -= button2_click_editar_servidorDHCP;
            button2.Click -= button2_click_configurarVPN;
            desassociarFuncoes();
            button2.Click += new EventHandler(button2_click_configurar_ProtocolosEncaminhamento);
            button1.Click += new EventHandler(configurarProtocoloEncaminhamento);
        }

        //Configurar Protolo Encaminhamento
        private void configurarProtocoloEncaminhamento(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            string acceptRedirects = textBox1.Text;
            string arpTimeout = textBox2.Text;
            string icmpRate = textBox3.Text;
            string ipForward = textBox4.Text;

            string url = $"{baseUrl}ip/settings/set";

            string payload = "{\"accept-redirects\":\"" + acceptRedirects + "\",\"arp-timeout\":\"" + arpTimeout + "\",\"icmp-rate-limit\":\"" + icmpRate + "\",\"ip-forward\":\"" + ipForward + "\"}";

            try
            {
                //Criar um novo pedido POST Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                //Escreve o JSON do payload e enviar para o body do pedido
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(payload);
                }

                //Envia o pedido e recebe a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler o pedido
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseJson = reader.ReadToEnd();
                    listBox1.Items.Add($"O protocolo de encaminhamento foi configurado com sucesso: {responseJson}");
                }

                //Limpar
                response.Close();

            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseJson = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao configurar o protocolo de encaminhamento: {responseJson}");
                }
            }
        }

        //Preenche os campos do protocolo de encaminhamento que se pretende configurar
        public void button2_click_configurar_ProtocolosEncaminhamento(object sender, EventArgs e)
        {
            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "ip/settings");
                request.Method = "GET";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                //Envia o pedido e recebe a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Lê a reposta e passa para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject responseJson = JObject.Parse(reader.ReadToEnd());

                    string acceptRedirects = responseJson.Value<string>("accept-redirects");
                    string arpTimeout = responseJson.Value<string>("arp-timeout");
                    string icmpRate = responseJson.Value<string>("icmp-rate-limit");
                    string ipForward = responseJson.Value<string>("ip-forward");

                    textBox1.Text = acceptRedirects;
                    textBox2.Text = arpTimeout;
                    textBox3.Text = icmpRate;
                    textBox4.Text = ipForward;
                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao listar o protocolo de encaminhamento devido a: {responseText}");
                }
            }
        }
        #endregion


        #region VPN
        private void vPNToolStripMenuItem1_Click(object sender, EventArgs e)
        {
          
        }
        private void configurarVPN(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            string enabled = comboBox1.Text;
            string ipsec_secret = textBox2.Text;
            string use_ipsec = comboBox3.Text;
            string max_sessions = textBox4.Text;

            string url = $"{baseUrl}interface/l2tp-server/server/set";

            string payload = "{\"enabled\":\"" + enabled + "\",\"ipsec-secret\":\"" + ipsec_secret + "\",\"use-ipsec\":\"" + use_ipsec + "\",\"max-sessions\":\"" + max_sessions + "\"}";

            try
            {
                //Criar um novo pedido POST Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                //Escreve o JSON do payload e enviar para o body do pedido
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(payload);
                }

                //Envia o pedido e recebe a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler o pedido
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseJson = reader.ReadToEnd();
                    listBox1.Items.Add($"A VPN foi configurado com sucesso: {responseJson}");
                }

                //Limpar
                response.Close();

            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseJson = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao configurar a VPN: {responseJson}");
                }
            }
        }
        public void button2_click_configurarVPN(object sender, EventArgs e)
        {
            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "interface/l2tp-server/server");
                request.Method = "GET";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                //Envia o pedido e recebe a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Lê a reposta e passa para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JObject responseJson = JObject.Parse(reader.ReadToEnd());

                    string enabled = responseJson.Value<string>("enabled");
                    string ipsec_secret = responseJson.Value<string>("ipsec-secret");
                    string use_ipsec = responseJson.Value<string>("use-ipsec");
                    string max_sessions = responseJson.Value<string>("max-sessions");

                    comboBox1.Text = enabled;
                    textBox2.Text = ipsec_secret;
                    comboBox3.Text = use_ipsec;
                    textBox4.Text = max_sessions;
                }

                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao listar a VPN devido a: {responseText}");
                }
            }
        }

        #endregion


        private void button1_Click(object sender, EventArgs e)
        {
            button1.Click -= button1_Click;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Click-=button2_Click;
        }

        private void desassociarFuncoes()
        {
            button1.Click -= criarinterfacebridge;
            button1.Click -= criarrotaestatica;
            button1.Click -= criarEndereçoIP;
            button1.Click -= criarServerDHCP;
            button1.Click -= criarFirewall;
            button1.Click -= criarPerfilSegurança;
            button1.Click -= editarInterfaceBridge;
            button1.Click -= editarEnderecoIP;
            button1.Click -= editarDHCP;
            button1.Click -= editarRotaEstatica;
            button1.Click -= editarRegraFirewall;
            button1.Click -= editarPerfilSegurança;
            button1.Click -= eliminarBridge;
            button1.Click -= eliminarDHCP;
            button1.Click -= eliminarEndereçoIp;
            button1.Click -= eliminarRotasEstaticas;
            button1.Click -= apagarRegrasFirewall;
            button1.Click -= eliminarPerfilSegurança;
            button1.Click -= ativarRedesWireless;
            button1.Click -= desativarRedesWireless;
            button1.Click -= configurarRedesWireless;
            button1.Click -= configurarServidorDNS;
            button1.Click -= configurarProtocoloEncaminhamento;
        }

        private void limparForm()
        {
            button1.Visible = false;
            button2.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible=false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            label1.Visible= false;
            label2.Visible=false;
            label3.Visible=false;
            label4.Visible=false;
            label5.Visible = false;
            label6.Visible=false;
            label7.Visible=false;
            label8.Visible=false;
            label9.Visible=false;
            label10.Visible=false;
            label11.Visible=false;
            label12.Visible=false;
            comboBox1.Visible=false;
            comboBox2.Visible=false;
            comboBox3.Visible=false;
        }

        private void configurarToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void configurarToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            limparForm();
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = true;
            comboBox3.Items.Clear();
            comboBox3.Items.AddRange(new string[] { "yes", "no", "required" });
            button1.Text = "Configurar";
            comboBox1.Visible = true;
            comboBox2.Visible = false;
            button1.Visible = true;
            textBox1.Visible = false;
            textBox2.Visible = true;
            textBox3.Visible = false;
            textBox4.Visible = true;
            label1.Visible = true;
            label1.Text = "Ativado";
            label2.Visible = true;
            label2.Text = "ipsec secret";
            label3.Visible = true;
            label4.Visible = true;
            label3.Text = "utilizar ipsec?";
            label4.Text = "Máximo de Sessões";
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(new string[] { "true", "false" });
            button2.Visible = true;
            desassociarFuncoes();
            
            button2.Click -= button2_click_configurar_ProtocolosEncaminhamento;
            button2.Click -= button2_click_configurar_RedesWireless;
            button2.Click -= button2_click_configurar_ServidoresDNS;
            button2.Click -= button2_click_editar_EnderecosIP;
            button2.Click -= button2_click_editarBridge;
            button2.Click -= button2_click_editar_PerfilSeguranca;
            button2.Click -= button2_click_editar_RegraFirewall;
            button2.Click -= button2_click_editar_RotasEstaticas;
            button2.Click -= button2_click_editar_servidorDHCP;
            button2.Click += new EventHandler(button2_click_configurarVPN);
            button1.Click += new EventHandler(configurarVPN);
        }

        private void adicionarUtilizadorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            comboBox1.Text = "";
            comboBox2.Text = "";
            comboBox3.Text = "";
            comboBox3.Visible = false;
            comboBox3.Items.Clear();
            limparForm();
            button1.Text = "Criar";
            comboBox1.Visible = false;
            comboBox2.Visible = false;
            button1.Visible = true;
            textBox1.Visible = true;
            textBox2.Visible = true;
            textBox3.Visible = true;
            textBox4.Visible = true;
            label1.Visible = true;
            label1.Text = "nome";
            label2.Visible = true;
            label2.Text = "password";
            label3.Visible = true;
            label4.Visible = true;
            label3.Text = "Endereço Local";
            label4.Text = "Endereço Remoto";
            comboBox1.Items.Clear();
           // comboBox1.Items.AddRange(new string[] { "true", "false" });
            button2.Visible = false;
            desassociarFuncoes();
            listarPPPSecrets();
            button1.Click += new EventHandler(AdicionarUtilizadorVPN);
        }
        private void AdicionarUtilizadorVPN(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            string name = textBox1.Text;
            string secret = textBox2.Text;
            string local_address = textBox3.Text;
            string remote_address= textBox4.Text;

            string url = $"{baseUrl}ppp/secret/add";

            string payload = "{\"name\":\"" + name + "\",\"password\":\"" + secret + "\",\"local-address\":\"" + local_address + "\",\"remote-address\":\"" + remote_address + "\"}";

            try
            {
                //Criar um novo pedido POST Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                //Escreve o JSON do payload e enviar para o body do pedido
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(payload);
                }

                //Envia o pedido e recebe a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler o pedido
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseJson = reader.ReadToEnd();
                    listBox1.Items.Add($"A VPN foi configurado com sucesso: {responseJson}");
                }

                //Limpar
                response.Close();

            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseJson = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao configurar a VPN: {responseJson}");
                }
            }
        }
        public void listarPPPSecrets()
        {

            listBox1.Items.Clear();

            try
            {
                //Criar um novo pedido GET Http
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(baseUrl + "ppp/secret");
                request.Method = "GET";
                request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password));

                //Enviar o pedido e obter a resposta
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Ler a resposta e passar a mesma para um array em JSON
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    JArray responseJson = JArray.Parse(reader.ReadToEnd());
                    comboBox1.Items.Clear();
                    foreach (JObject obj in responseJson)
                    {
                        
                        string name = obj.Value<string>("name");
                        string password = obj.Value<string>("password");
                        string local_address = obj.Value<string>("local-address");
                        string remote_address = obj.Value<string>("remote-address");
                        listBox1.Items.Add(name + " - " + password + " - " + local_address + " - " + remote_address);
                    }
                }
                //Limpar
                response.Close();
            }
            catch (WebException ex)
            {
                //Erro do pedido
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var responseText = reader.ReadToEnd();
                    listBox1.Items.Add($"Erro ao listar interfaces bridge devido a: {responseText}");
                }
            }
        }
    }
}
    

   

