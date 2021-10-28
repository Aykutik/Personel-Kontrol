using MySql.Data.MySqlClient;
using System;
using System.IO;
using System.Windows.Forms;

namespace Personel_Kontrol
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        readonly static Sqlbağlantısı baglanti = new Sqlbağlantısı();

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public string G1 = "";
        public string C1 = "";
        public string G2= "";
        public string C2 = "";

        public static void dosyadanOku()
        {

            string dosya_yolu = @"C:\Users\aykut\20200710.txt";
            //Okuma işlem yapacağımız dosyanın yolunu belirtiyoruz.
            FileStream fs = new FileStream(dosya_yolu, FileMode.Open, FileAccess.Read);
            //Bir file stream nesnesi oluşturuyoruz. 1.parametre dosya yolunu,
            //2.parametre dosyanın açılacağını,
            //3.parametre dosyaya erişimin veri okumak için olacağını gösterir.
            StreamReader sw = new StreamReader(fs);
            //Okuma işlemi için bir StreamReader nesnesi oluşturduk.
            string yazi = sw.ReadLine();
            while (yazi != null)
            {

                String per_id = yazi.Substring(4, 5);
                String tarih = yazi.Substring(12,10);
                String saat = yazi.Substring(23,8);
                
                Console.WriteLine("id: "+ per_id + "");
                Console.WriteLine("tarih: " + tarih + "");
                Console.WriteLine("saat: " + saat + "");

                String kontrolGiris = "";
                String kontrolÇıkış = "";
                int kontrolTamam = 0;
                // Personelin o gün girişi var mı kontrol edilecek


                MySqlCommand komut_3 = new MySqlCommand("select * from personel_giriscikis where per_id=@per_id and tarih=@tarih and tamam=@tamam", baglanti.bağlantı());
                komut_3.Parameters.Clear();
                komut_3.Parameters.AddWithValue("@per_id", per_id);
                komut_3.Parameters.AddWithValue("@tarih", Convert.ToDateTime(tarih));
                komut_3.Parameters.AddWithValue("@tamam", 0);
                komut_3.ExecuteNonQuery();
                MySqlDataReader oku_3 = komut_3.ExecuteReader();
                if (oku_3.Read())
                {  
                    kontrolGiris = oku_3["giris"].ToString(); 
                    kontrolÇıkış = oku_3["cikis"].ToString();
                }
                oku_3.Close();

                MySqlCommand komut_4 = new MySqlCommand("select * from personel_giriscikis where per_id=@per_id and tarih=@tarih and tamam=@tamam", baglanti.bağlantı());
                komut_4.Parameters.Clear();
                komut_4.Parameters.AddWithValue("@per_id", per_id);
                komut_4.Parameters.AddWithValue("@tarih", Convert.ToDateTime(tarih));
                komut_4.Parameters.AddWithValue("@tamam", 1);
                komut_4.ExecuteNonQuery();
                MySqlDataReader oku_4 = komut_4.ExecuteReader();
                if (oku_4.Read())
                {
                    kontrolTamam = 1;
                }
                oku_4.Close();

                MySqlCommand komut_tk2 = new MySqlCommand("select * from personel_giriscikis where per_id=@per_id and tarih=@tarih and tamam=@tamam", baglanti.bağlantı());
                komut_tk2.Parameters.Clear();
                komut_tk2.Parameters.AddWithValue("@per_id", per_id);
                komut_tk2.Parameters.AddWithValue("@tarih", Convert.ToDateTime(tarih));
                komut_tk2.Parameters.AddWithValue("@tamam", 0);
                komut_tk2.ExecuteNonQuery();
                MySqlDataReader oku_tk = komut_tk2.ExecuteReader();
                if (oku_tk.Read())
                {
                    kontrolTamam = Convert.ToInt32(oku_tk["tamam"]);
                }
                oku_tk.Close();

                string girisNormal = "08:00:00";
                string cikisNormal = "18:20:00";
                string nullSaat = "00:00:00";
                DateTime girisSaat = Convert.ToDateTime(saat);
                DateTime girisSaatNormal = Convert.ToDateTime(girisNormal);
                DateTime cikisSaat = Convert.ToDateTime(saat);
                DateTime cikisSaatNormal = Convert.ToDateTime(cikisNormal);
                TimeSpan SonucGiris = girisSaat - girisSaatNormal;
                TimeSpan SonucCikis = cikisSaatNormal - cikisSaat;
                TimeSpan SonucNull = Convert.ToDateTime(nullSaat)- Convert.ToDateTime(nullSaat);
                int SonucDakika = SonucGiris.Minutes;
                int SonucSaat = SonucGiris.Hours;
                int SonucDakikaCikis = SonucCikis.Minutes;
                int SonucSaatCikis = SonucCikis.Hours;

                // Personelin o gün giriş var ise saat girişe değil çıkışa yazılacak, insert değil update işlemi gerçekleşecek

                if (kontrolGiris == "" && kontrolÇıkış == "" && kontrolTamam == 0)
                {
                    //Personel yeni giriş yapıyor
                    //giriş kısmına yazılacak

                    MySqlCommand komut_giris = new MySqlCommand("insert into personel_giriscikis" +
                                                                "(per_id,tarih,giris,tamam,giris_eksik,eksik_mesai) " +
                                                                "values(@per_id,@tarih,@giris,@tamam,@giris_eksik,@eksik_mesai)", baglanti.bağlantı());
                    komut_giris.Parameters.Clear();
                    komut_giris.Parameters.AddWithValue("@per_id", per_id);
                    komut_giris.Parameters.AddWithValue("@tarih", Convert.ToDateTime(tarih));
                    komut_giris.Parameters.AddWithValue("@giris", girisSaat);
                    komut_giris.Parameters.AddWithValue("@tamam", 0);

                    if (SonucDakika >= 11 || SonucSaat >= 1)
                    {
                        komut_giris.Parameters.AddWithValue("@giris_eksik", SonucGiris);
                        komut_giris.Parameters.AddWithValue("@eksik_mesai", SonucGiris);
                    }
                    else
                    {
                        komut_giris.Parameters.AddWithValue("@giris_eksik", null);
                        komut_giris.Parameters.AddWithValue("@eksik_mesai", null);
                    }

                    komut_giris.ExecuteNonQuery();
                    komut_giris.Dispose();
                }
                

                if (kontrolTamam==1)
                {   
                    // gün içinde tekrar giriş yapıyor
                    MySqlCommand komut_giris = new MySqlCommand("insert into personel_giriscikis" +
                                                                "(per_id,tarih,giris,tamam) " +
                                                                "values(@per_id,@tarih,@giris,@tamam)", baglanti.bağlantı());
                    komut_giris.Parameters.Clear();
                    komut_giris.Parameters.AddWithValue("@per_id", per_id);
                    komut_giris.Parameters.AddWithValue("@tarih", Convert.ToDateTime(tarih));
                    komut_giris.Parameters.AddWithValue("@giris", Convert.ToDateTime(saat));
                    komut_giris.Parameters.AddWithValue("@tamam", 0);
                    komut_giris.ExecuteNonQuery();
                    komut_giris.Dispose();

                    //bir önceki giriş çıkışın tk kolonunu 1 yapıyoruz
                    MySqlCommand komut_tk = new MySqlCommand("update personel_giriscikis set tk=@tk" +
                                                                " where tarih=@tarih and per_id=@per_id and tamam=1", baglanti.bağlantı());
                    komut_tk.Parameters.Clear();
                    komut_tk.Parameters.AddWithValue("@per_id", per_id);
                    komut_tk.Parameters.AddWithValue("@tarih", Convert.ToDateTime(tarih));
                    komut_tk.Parameters.AddWithValue("@tk", 1);
                    komut_tk.ExecuteNonQuery();
                    komut_tk.Dispose();


                    //önceki çıkış ile şimdiki giriş ara farkı giriş eksik kolonuna yazdırıyoruz
                    //ara fark hesaplama

                    //önceki çıkış bilgilerini getirelim
                    string tekrarCikis_Cikis = "";
                    string tekrarGirisEksik = "";
                    MySqlCommand komut_tk5 = new MySqlCommand("select * from personel_giriscikis where per_id=@per_id and tarih=@tarih and tamam=@tamam", baglanti.bağlantı());
                    komut_tk5.Parameters.Clear();
                    komut_tk5.Parameters.AddWithValue("@per_id", per_id);
                    komut_tk5.Parameters.AddWithValue("@tarih", Convert.ToDateTime(tarih));
                    komut_tk5.Parameters.AddWithValue("@tamam", 1);
                    komut_tk5.ExecuteNonQuery();
                    MySqlDataReader oku_tk5 = komut_tk5.ExecuteReader();
                    if (oku_tk5.Read())
                    {
                        tekrarCikis_Cikis = oku_tk5["cikis"].ToString();
                        tekrarGirisEksik = oku_tk5["giris_eksik"].ToString();
                    }
                    oku_tk5.Close();

                    TimeSpan araFark = Convert.ToDateTime(saat) - Convert.ToDateTime(tekrarCikis_Cikis);
                    //DateTime araFarkk = Convert.ToDateTime(tekrarGirisEksik) + araFark;

                    string mola1 = "10:10:00";
                    string mola2 = "13:15:00";
                    string mola3 = "15:15:00";
                    string mola4 = "17:00:00";

                    int _mola10dk = 10;
                    int _mola15dk = 15;
                    int _mola30dk = 30;
                    

                    DateTime ilkCikis = Convert.ToDateTime(tekrarCikis_Cikis);
                    DateTime giris = Convert.ToDateTime(saat);
                    

                    int _mola = 0;

                    if (ilkCikis < Convert.ToDateTime(mola1) && Convert.ToDateTime(mola1) < giris)
                    {
                        _mola = _mola + _mola15dk;
                    }

                    if (ilkCikis < Convert.ToDateTime(mola2) && Convert.ToDateTime(mola2) < giris)
                    {
                        _mola = _mola + _mola30dk;
                    }

                    if (ilkCikis < Convert.ToDateTime(mola3) && Convert.ToDateTime(mola3) < giris)
                    {
                        _mola = _mola + _mola15dk;
                    }

                    if (ilkCikis < Convert.ToDateTime(mola4) && Convert.ToDateTime(mola4) < giris)
                    {
                        _mola = _mola + _mola10dk;
                    }

                    string molaNull = "00:00:00";

                    if (_mola < 59)
                    {
                        molaNull = "00:"+_mola+":00";
                    }
                    else
                    {
                        molaNull = "01:00:00";
                    }

                    DateTime mola = Convert.ToDateTime(molaNull);
                    //TimeSpan araFarkk = Convert.ToDateTime(araFark) - mola;



                    //bulduğumuz fark eksiğini yeni satırda giris_eksik kolonuna yazdıralım
                    MySqlCommand komut_tkt = new MySqlCommand("update personel_giriscikis set giris_eksik=@giris_eksik" +
                                                             " where tarih=@tarih and per_id=@per_id and tamam=@tamam", baglanti.bağlantı());
                    komut_tkt.Parameters.Clear();
                    komut_tkt.Parameters.AddWithValue("@per_id", per_id);
                    komut_tkt.Parameters.AddWithValue("@tarih", Convert.ToDateTime(tarih));
                    komut_tkt.Parameters.AddWithValue("@tamam", 0);
                    komut_tkt.Parameters.AddWithValue("@giris_eksik", araFark-mola);
                    komut_tkt.ExecuteNonQuery();
                    komut_tkt.Dispose();

                }

                // *** ÇIKIŞ ***
                if (kontrolGiris != "" && kontrolÇıkış == "")
                {
                    string tekrarCikis_Tk = "";
                    string tekrarCikis_EksikMesai = "";
                    string tekrarCikis_GirisEksik = "";
                    string tekrarCikis_Cikis = "";

                    //Personel tekrar mı çıkış yapıyor?
                    MySqlCommand komut_tk5 = new MySqlCommand("select * from personel_giriscikis where per_id=@per_id and tarih=@tarih and tamam=@tamam", baglanti.bağlantı());
                    komut_tk5.Parameters.Clear();
                    komut_tk5.Parameters.AddWithValue("@per_id", per_id);
                    komut_tk5.Parameters.AddWithValue("@tarih", Convert.ToDateTime(tarih));
                    komut_tk5.Parameters.AddWithValue("@tamam", 1);
                    komut_tk5.ExecuteNonQuery();
                    MySqlDataReader oku_tk5 = komut_tk5.ExecuteReader();
                    if (oku_tk5.Read())
                    {
                        tekrarCikis_Tk = oku_tk5["tk"].ToString();
                        tekrarCikis_EksikMesai = oku_tk5["eksik_mesai"].ToString();
                        tekrarCikis_GirisEksik = oku_tk5["giris_eksik"].ToString();
                        tekrarCikis_Cikis = oku_tk5["cikis_eksik"].ToString();
                    }
                    oku_tk5.Close();


                    string arafark = "";

                    

                    if (tekrarCikis_Tk != "")
                    {
                        //orta çıkış ile giriş farkı getir
                        MySqlCommand komut_tk6 = new MySqlCommand("select * from personel_giriscikis where per_id=@per_id and tarih=@tarih and tamam=@tamam", baglanti.bağlantı());
                        komut_tk6.Parameters.Clear();
                        komut_tk6.Parameters.AddWithValue("@per_id", per_id);
                        komut_tk6.Parameters.AddWithValue("@tarih", Convert.ToDateTime(tarih));
                        komut_tk6.Parameters.AddWithValue("@tamam", 0);
                        komut_tk6.ExecuteNonQuery();
                        MySqlDataReader oku_tk6 = komut_tk6.ExecuteReader();
                        if (oku_tk6.Read())
                        {
                            arafark = oku_tk6["giris_eksik"].ToString();
                        }
                        oku_tk6.Close();

                        TimeSpan arafarkk = Convert.ToDateTime(arafark) - Convert.ToDateTime(nullSaat);

                        // Personel güniçinde tekrar çıkış yapıyor!
                        string cikisNull = "00:00:00";
                        TimeSpan cikisNullSonuc = Convert.ToDateTime(cikisNull) - Convert.ToDateTime(cikisNull);
                        DateTime tekarCikisToplamEksik;

                        // personel gün içinde tekrar çıkış yapıyor

                        // Eksik mesai hesaplaması

                        //persenel gün içinde ilk kez çıkış yapıyor
                        MySqlCommand komut_cikis_tekrar = new MySqlCommand("update personel_giriscikis set cikis=@cikis,tamam=@tamam,cikis_eksik=@cikis_eksik,eksik_mesai=@eksik_mesai" +
                                                                    " where tarih=@tarih and per_id=@per_id and tamam=0", baglanti.bağlantı());
                        komut_cikis_tekrar.Parameters.Clear();
                        komut_cikis_tekrar.Parameters.AddWithValue("@per_id", per_id);
                        komut_cikis_tekrar.Parameters.AddWithValue("@tarih", Convert.ToDateTime(tarih));
                        komut_cikis_tekrar.Parameters.AddWithValue("@cikis", Convert.ToDateTime(saat));
                        if (SonucCikis != SonucNull)
                        {
                            komut_cikis_tekrar.Parameters.AddWithValue("@cikis_eksik", SonucCikis);
                        }
                        else
                        {
                            komut_cikis_tekrar.Parameters.AddWithValue("@cikis_eksik", null);
                        }
                        komut_cikis_tekrar.Parameters.AddWithValue("@tamam", 1);
                        

                        //önceki girişte ve bu tekrar çıkışta eksik var
                        if (tekrarCikis_GirisEksik != "" && SonucCikis != cikisNullSonuc)
                        {
                            tekarCikisToplamEksik = Convert.ToDateTime(tekrarCikis_GirisEksik) + SonucCikis + arafarkk;
                            komut_cikis_tekrar.Parameters.AddWithValue("@eksik_mesai", tekarCikisToplamEksik);
                        }

                        //Sadece bu tekrar çıkışta eksik var
                        if (tekrarCikis_GirisEksik == "" && SonucCikis != cikisNullSonuc)
                        {
                            komut_cikis_tekrar.Parameters.AddWithValue("@eksik_mesai", SonucCikis+ arafarkk);
                        }

                        //Önceki girişte ve şmdiki tekrar çıkışta eksik yok
                        if (tekrarCikis_GirisEksik == "" && SonucCikis == cikisNullSonuc)
                        {
                            komut_cikis_tekrar.Parameters.AddWithValue("@eksik_mesai", arafarkk);
                        }

                        // komut_cikis_tekrar sonu

                        komut_cikis_tekrar.ExecuteNonQuery();
                        komut_cikis_tekrar.Dispose();


                        MySqlCommand komut_cikis_temizle = new MySqlCommand("update personel_giriscikis set eksik_mesai=@eksik_mesai,cikis_eksik=@cikis_eksik" +
                                                                           " where tarih=@tarih and per_id=@per_id and tk=@tk", baglanti.bağlantı());
                        komut_cikis_temizle.Parameters.Clear();
                        komut_cikis_temizle.Parameters.AddWithValue("@per_id", per_id);
                        komut_cikis_temizle.Parameters.AddWithValue("@tarih", Convert.ToDateTime(tarih));
                        komut_cikis_temizle.Parameters.AddWithValue("@eksik_mesai", null);
                        komut_cikis_temizle.Parameters.AddWithValue("@cikis_eksik", null);
                        komut_cikis_temizle.Parameters.AddWithValue("@tk", 1);
                        komut_cikis_temizle.ExecuteNonQuery();
                        komut_cikis_temizle.Dispose();
                    }
                    else
                    {
                        //persenel gün içinde ilk kez çıkış yapıyor
                        MySqlCommand komut_cikis = new MySqlCommand("update personel_giriscikis set cikis=@cikis,tamam=@tamam,cikis_eksik=@cikis_eksik,eksik_mesai=@eksik_mesai" +
                                                                " where tarih=@tarih and per_id=@per_id and tamam=0", baglanti.bağlantı());
                        komut_cikis.Parameters.Clear();
                        komut_cikis.Parameters.AddWithValue("@per_id", per_id);
                        komut_cikis.Parameters.AddWithValue("@tarih", Convert.ToDateTime(tarih));
                        komut_cikis.Parameters.AddWithValue("@cikis", Convert.ToDateTime(saat));


                        if (SonucDakikaCikis >= 11 || SonucSaatCikis >= 1)
                        {
                            komut_cikis.Parameters.AddWithValue("@cikis_eksik", SonucCikis);
                            string eksikmesai = "";
                            DateTime eksikmesaiToplam;
                            string eksikmesai_onceki = "";
                            DateTime eksikmesaiToplam_onceki;

                            string cikisNull = "00:00:00";
                            TimeSpan cikisNullSonuc = Convert.ToDateTime(cikisNull) - Convert.ToDateTime(cikisNull);

                            MySqlCommand komut_tk3 = new MySqlCommand("select * from personel_giriscikis where per_id=@per_id and tarih=@tarih and tamam=@tamam", baglanti.bağlantı());
                            komut_tk3.Parameters.Clear();
                            komut_tk3.Parameters.AddWithValue("@per_id", per_id);
                            komut_tk3.Parameters.AddWithValue("@tarih", Convert.ToDateTime(tarih));
                            komut_tk3.Parameters.AddWithValue("@tamam", 0);
                            komut_tk3.ExecuteNonQuery();
                            MySqlDataReader oku_tk2 = komut_tk3.ExecuteReader();
                            if (oku_tk2.Read())
                            {
                                eksikmesai = oku_tk2["giris_eksik"].ToString();
                            }
                            oku_tk2.Close();

                            //giriş ve çıkışda eksik var ise
                            if (eksikmesai != "" && SonucCikis != cikisNullSonuc)
                            {
                                eksikmesaiToplam = DateTime.Parse(eksikmesai) + SonucCikis;
                                komut_cikis.Parameters.AddWithValue("@eksik_mesai", eksikmesaiToplam);
                            }

                            //sadece çıkışta eksik var ise
                            if (eksikmesai == "" && SonucCikis != cikisNullSonuc)
                            {
                                komut_cikis.Parameters.AddWithValue("@eksik_mesai", SonucCikis);
                            }


                            //girişte ve çıkışta eksik yok ise
                            if (eksikmesai == "" && SonucCikis == cikisNullSonuc)
                            {
                                komut_cikis.Parameters.AddWithValue("@eksik_mesai", null);
                            }

                        }
                        else
                        {
                            komut_cikis.Parameters.AddWithValue("@cikis_eksik", null);
                            komut_cikis.Parameters.AddWithValue("@eksik_mesai", null);
                        }
                        komut_cikis.Parameters.AddWithValue("@tamam", 1);

                        komut_cikis.ExecuteNonQuery();
                        komut_cikis.Dispose();

                    }
                    
                }

                yazi = sw.ReadLine();
            }
            //Satır satır okuma işlemini gerçekleştirdik ve ekrana yazdırdık
            //Son satır okunduktan sonra okuma işlemini bitirdik
            sw.Close();
            fs.Close();
            //İşimiz bitince kullandığımız nesneleri iade ettik.
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            dosyadanOku();


        }
    }
}
