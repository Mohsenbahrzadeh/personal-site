# سایت شخصی محسن بحرزاده

سایت شخصی و پلتفرم دوره‌های آموزشی، ساخته‌شده با **ASP.NET Core 10 (Razor Pages)**.
طراحی تیره و راست‌به‌چپ، با امکان ثبت‌نام کاربران، امتیازدهی و نظردهی روی مطالب،
و پشتیبانی از دوره‌های ویدئویی چندقسمتی.

## امکانات

- صفحه اصلی با معرفی، لیست دوره‌ها/مقالات، بخش «درباره من» و راه‌های ارتباطی
- ثبت‌نام و ورود کاربران (ASP.NET Core Identity)
- امتیازدهی ستاره‌ای (۱ تا ۵) و نظردهی روی هر مطلب، برای کاربران واردشده
- **دوره ویدئویی**: هر دوره می‌تواند شامل چند قسمت ویدیویی باشد؛ مدت‌زمان هر قسمت
  به‌صورت **کاملاً خودکار** از فایل mp4 استخراج می‌شود (بدون نیاز به ffmpeg)
- پنل مدیریت (`/Admin`) برای ساخت/ویرایش/حذف مطالب، مدیریت قسمت‌های ویدیویی و
  تأیید/حذف نظرات کاربران — فقط برای نقش Admin

## پیش‌نیازها

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server یا SQL Server LocalDB (روی ویندوز معمولاً همراه Visual Studio نصب است)

## اجرا

```bash
cd PersonalSite
dotnet run
```

سایت روی `http://localhost:5129` بالا می‌آید. دیتابیس در اولین اجرا خودکار
migrate و seed می‌شود (چند مطلب نمونه + یک کاربر ادمین).

کانکشن استرینگ پیش‌فرض در `appsettings.json` از LocalDB استفاده می‌کند:

```
Server=(localdb)\mssqllocaldb;Database=PersonalSite;Trusted_Connection=True;MultipleActiveResultSets=true
```

برای استفاده از SQL Server واقعی، همین مقدار را در `appsettings.json` یا
`dotnet user-secrets` عوض کن.

### رمز اولیه کاربر ادمین

ایمیل ادمین seed‌شده: `mohsen.bahrzadeh@gmail.com`

رمز عبور **در کد قرار ندارد**. اگر مقدار `AdminSeed:Password` را در تنظیمات
(مثلاً با `dotnet user-secrets set "AdminSeed:Password" "<رمز دلخواه>"`) تنظیم نکنی،
یک رمز موقت تصادفی ساخته می‌شود و در لاگ اجرای برنامه چاپ می‌شود — همان‌جا آن را ببین
و بعد از اولین ورود عوضش کن.

## ساختار پروژه

- `Data/` — مدل‌ها (`Post`, `Comment`, `Rating`, `VideoEpisode`)، `ApplicationDbContext`،
  seed داده، و `Mp4DurationReader` (خواندن مدت ویدیو از باکس‌های ISO-BMFF فایل mp4)
- `Pages/` — صفحه اصلی، جزئیات مطلب/دوره، و پنل مدیریت (`Pages/Admin`)
- `Areas/Identity/` — override سفارشی صفحه ثبت‌نام (برای گرفتن نام نمایشی)

جزئیات کامل توسعه و چک‌لیست در [PROGRESS.md](PROGRESS.md).

## مجوز

این پروژه برای استفاده شخصی و آموزشی محسن بحرزاده ساخته شده است.
