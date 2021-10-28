using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Personel_Kontrol
{
    public class Sqlbağlantısı
    {
        public MySqlConnection bağlantı()
        {
            string bağlantıadresi = "server=127.0.0.1;user id=root;database=personel; Charset = utf8";

            MySqlConnection bağlantı = new MySqlConnection(bağlantıadresi);

            if (bağlantı.State == ConnectionState.Open)
            {
                Console.WriteLine("bağlanti başarılı");
                bağlantı.Close();
            }

            bağlantı.Open();
            if (bağlantı.State == ConnectionState.Open)
            {
                Console.WriteLine("bağlanti başarılı");
            }
            return bağlantı;
        }
    }
}
