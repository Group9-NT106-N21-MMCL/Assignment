using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
namespace baitapAPi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        class Task
        {
            public int userId { get; set; }
            public int Id { get; set; }
            public string Title { get; set; }
            public bool Completed { get; set; }
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            string url = "https://jsonplaceholder.typicode.com/todos";
            var httpClient = new HttpClient();

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string response_data = await response.Content.ReadAsStringAsync();
                var tasks = JsonConvert.DeserializeObject<Task[]>(response_data);
                dataGridView1.DataSource = tasks;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}