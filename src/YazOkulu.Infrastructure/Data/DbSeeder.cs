using YazOkulu.Domain.Entities;

namespace YazOkulu.Infrastructure.Data;

public static class DbSeeder
{
    public static void SeedData(ApplicationDbContext context)
    {
        // Seed Courses
        if (!context.Courses.Any())
        {
            var courses = new List<Course>
            {
                new() { Code = "CS101", Name = "Bilgisayar Bilimine Giriş", Quota = 30, Department = "Bilgisayar Mühendisliği", Faculty = "Mühendislik Fakültesi", Instructor = "Prof. Dr. Ahmet Yılmaz", Credits = 3 },
                new() { Code = "CS201", Name = "Veri Yapıları ve Algoritmalar", Quota = 25, Department = "Bilgisayar Mühendisliği", Faculty = "Mühendislik Fakültesi", Instructor = "Doç. Dr. Ayşe Demir", Credits = 4 },
                new() { Code = "CS301", Name = "Veritabanı Yönetim Sistemleri", Quota = 20, Department = "Bilgisayar Mühendisliği", Faculty = "Mühendislik Fakültesi", Instructor = "Doç. Dr. Mehmet Kaya", Credits = 3 },
                new() { Code = "MAT101", Name = "Matematik I", Quota = 40, Department = "Matematik", Faculty = "Fen Fakültesi", Instructor = "Prof. Dr. Zeynep Arslan", Credits = 4 },
                new() { Code = "FIZ101", Name = "Fizik I", Quota = 35, Department = "Fizik", Faculty = "Fen Fakültesi", Instructor = "Doç. Dr. Can Özdemir", Credits = 4 },
                new() { Code = "ING201", Name = "İleri İngilizce", Quota = 15, Department = "İngilizce Dil ve Edebiyatı", Faculty = "Edebiyat Fakültesi", Instructor = "Dr. Öğr. Üyesi Elif Şahin", Credits = 2 }
            };

            context.Courses.AddRange(courses);
            context.SaveChanges();
        }
    }
}
