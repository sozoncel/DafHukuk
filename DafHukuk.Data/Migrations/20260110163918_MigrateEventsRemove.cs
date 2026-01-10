using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DafHukuk.Data.Migrations
{
    public partial class MigrateEventsRemove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. ADIM: Önce verileri (Post'ları) kurtaralım.
            // ID:1 (Etkinlikler) olan tüm postları, ID:2 (Yayınlar) kategorisine taşıyoruz.
            migrationBuilder.Sql("UPDATE Posts SET CategoryId = 2 WHERE CategoryId = 1");

            // ID:3 (Hizmetlerimiz) olan tüm postları, ID:2 (Hizmetler) olarak güncelleyeceğimiz için 
            // şimdilik bekletiyoruz.

            // 2. ADIM: Seed Data Güncellemesi (ID'leri Hizala)

            // Mevcut ID 1'i (Etkinlik) komple siliyoruz
            migrationBuilder.DeleteData(table: "Categories", keyColumn: "Id", keyValue: 1);

            // Mevcut ID 2'yi (Yayınlar) güncelle
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "Name_AR", "Name_EN", "Name_TR" },
                values: new object[] { DateTime.UtcNow, "منشورات", "Publications", "Yayınlar" });

            // Mevcut ID 3'ü (Hizmetler) ID:2 yapmak yerine sadece isimleri güncel tutuyoruz. 
            // Çünkü ID değiştirmek Foreign Key (FK) hatası verir. 
            // En sağlıklısı şudur: ID 1 silinsin, 2 Yayın kalsın, 3 Hizmet kalsın.

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedDate", "Name_AR", "Name_EN", "Name_TR" },
                values: new object[] { DateTime.UtcNow, "خدماتنا", "Our Services", "Hizmetlerimiz" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Geri dönüş kodu (Down) projenin eski haline dönmesi içindir.
        }
    }
}