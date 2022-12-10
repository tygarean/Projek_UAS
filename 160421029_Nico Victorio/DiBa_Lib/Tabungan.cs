﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace DiBa_Lib
{
    public class Tabungan
    {
        #region data members
        private string noRekening;
        private Pengguna pengguna;
        private double saldo;
        private string status;
        private string keterangan;
        private DateTime tgl_buat;
        private DateTime tgl_perubahan;
        private Employee employee;
        #endregion

        #region constructors
        public Tabungan()
        {
            this.NoRekening = "";
            this.Pengguna = null;
            this.Saldo = 0.0;
            this.Status = "";
            this.Keterangan = "";
            this.Tgl_buat = DateTime.Now;
            this.Tgl_perubahan = DateTime.Now;
            this.Employee = null;
        }

        public Tabungan(string noRekening, Pengguna pengguna, double saldo, string status, string keterangan, DateTime tgl_buat, DateTime tgl_perubahan, Employee employee)
        {
            this.NoRekening = noRekening;
            this.Pengguna = pengguna;
            this.Saldo = saldo;
            this.Status = status;
            this.Keterangan = keterangan;
            this.Tgl_buat = tgl_buat;
            this.Tgl_perubahan = tgl_perubahan;
            this.Employee = employee;
        }
        #endregion

        #region properties
        public string NoRekening { get => noRekening; set => noRekening = value; }
        public double Saldo { get => saldo; set => saldo = value; }
        public string Status { get => status; set => status = value; }
        public string Keterangan { get => keterangan; set => keterangan = value; }
        public DateTime Tgl_buat { get => tgl_buat; set => tgl_buat = value; }
        public DateTime Tgl_perubahan { get => tgl_perubahan; set => tgl_perubahan = value; }
        public Employee Employee { get => employee; set => employee = value; }
        public Pengguna Pengguna { get => pengguna; set => pengguna = value; }
        #endregion

        #region methods
        public static string GenerateNoRek()
        {
            string sql = "SELECT RIGHT(no_rekening,2) as NoRek FROM tabungan WHERE " +
                " Date(tgl_perubahan) = Date(CURRENT_DATE) order by tgl_perubahan DESC limit 1";
            MySqlDataReader hasil = Koneksi.ambilData(sql);
            string hasilNoRek = "";
            if (hasil.Read())
            {
                if (hasil.GetString(0) != "")
                {
                    int noRek = hasil.GetInt32(0) + 1;
                    hasilNoRek = DateTime.Now.Year.ToString() +
                        DateTime.Now.Month.ToString().PadLeft(2, '0') +
                        DateTime.Now.Day.ToString().PadLeft(2, '0') +
                        noRek.ToString().PadLeft(2, '0');

                }
            }
            else
            {
                hasilNoRek = DateTime.Now.Year.ToString() +
                    DateTime.Now.Month.ToString().PadLeft(2, '0') +
                    DateTime.Now.Day.ToString().PadLeft(2, '0') +
                    "01";
            }
            return hasilNoRek;

        }
        public static List<Tabungan> BacaData(string kriteria, string nilaiKriteria)
        {
            string sql = "SELECT no_rekening, id_pengguna, saldo, status, IFNULL(keterangan,'') as jeterangan, " +
                         "tgl_buat, tgl_perubahan, IFNULL(verifikator,0) as verifikator " +
                         "FROM tabungan ";
            if (kriteria == "")
            {
                sql += ";";
            }
            else
            {
                sql += " WHERE " + kriteria + " LIKE '%" + nilaiKriteria + "%';";
            }

            MySqlDataReader hasil = Koneksi.ambilData(sql);

            List<Tabungan> listTabungan = new List<Tabungan>();

            //buat list untk menampung data 
            while (hasil.Read() == true)
            {
                Tabungan tab = new Tabungan();
                tab.NoRekening = hasil.GetString(0);
                tab.Saldo = hasil.GetDouble(2);
                tab.Status = hasil.GetString(3);
                tab.Keterangan = hasil.GetString(4);
                tab.Tgl_buat = DateTime.Parse(hasil.GetValue(5).ToString());
                tab.Tgl_perubahan = DateTime.Parse(hasil.GetValue(6).ToString());

                Pengguna tmpPengguna = new Pengguna();
                tmpPengguna.Nik = hasil.GetInt32(1);
                tab.Pengguna = tmpPengguna;

                Employee tmpEmployee = new Employee();
                tmpEmployee.Id = hasil.GetInt32(7);
                tab.Pengguna = tmpPengguna;

                listTabungan.Add(tab);
            }
            return listTabungan;
        }

        public bool TambahData()
        {
            string sql = "INSERT INTO tabungan (no_rekening, id_pengguna, saldo, status, " +
                                         "keterangan, tgl_buat, tgl_perubahan) " +
                         " VALUES ('" + this.NoRekening + "', " + this.Pengguna.Nik + ", " +
                         this.Saldo + ", '" + this.Status + "', '" + this.Keterangan + "', '" +
                         this.Tgl_buat.ToString("yyyy-MM-dd") + "', '" + 
                         this.Tgl_perubahan.ToString("yyyy-MM-dd") + "') ";
            bool result = Koneksi.executeDML(sql);
            return result;
        }
        public bool UbahData()
        {
            string sql = "UPDATE tabungan SET id_pengguna = " + this.Pengguna.Nik + 
                         ", saldo = " + this.Saldo + ", status = '" + this.Status +
                         "', keterangan = '" + this.Keterangan + "', tgl_buat = '" + this.Tgl_buat.ToString("yyyy-MM-dd") + 
                         "', tgl_perubahan = '" + this.Tgl_perubahan.ToString("yyyy-MM-dd") + "' " +
                         //"verifikator = " + this.Employee.Id + 
                         " WHERE no_rekening = '" + this.NoRekening + "';";
            bool result = Koneksi.executeDML(sql);
            return result;
        }

        public bool HapusData()
        {
            string sql = "DELETE from tabungan where no_rekening = " + this.NoRekening + ";";
            bool result = Koneksi.executeDML(sql);
            return result;
        }

        public static Tabungan tabunganByCode(string noRek)
        {
            string sql = "SELECT no_rekening, id_pengguna, saldo, status, IFNULL(jeterangan,'') as jeterangan, " +
                         "tgl_buat, tgl_perubahan, IFNULL(verifikator,0) as verifikator " +
                         "FROM tabungan WHERE no_rekening='" + noRek+"'";
            MySqlDataReader hasil = Koneksi.ambilData(sql);
            if (hasil.Read() == true)
            {
                Tabungan tab = new Tabungan();
                tab.NoRekening = hasil.GetString(0);
                tab.Saldo = hasil.GetDouble(2);
                tab.Status = hasil.GetString(3);
                tab.Keterangan = hasil.GetString(4);
                tab.Tgl_buat = DateTime.Parse(hasil.GetString(5));
                tab.Tgl_perubahan = DateTime.Parse(hasil.GetString(6));

                Pengguna tmpPengguna = new Pengguna();
                tmpPengguna.Nik = hasil.GetInt32(1);
                tab.Pengguna = tmpPengguna;

                Employee tmpEmployee = new Employee();
                tmpEmployee.Id = hasil.GetInt32(7);
                tab.Pengguna = tmpPengguna;
                return tab;
            }
            else
            {
                return null;
            }
        }

        //public bool UbahStatus()
        //{
        //    string sql = "UPDATE inbox SET status ='Terbuka' where id_pengguna=" + this.Pengguna.Nik + " and id_pesan=" + this.IdPesan + ";";
        //    bool result = Koneksi.executeDML(sql);
        //    return result;
        //}
        #endregion
    }
}