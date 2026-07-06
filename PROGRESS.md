# پیشرفت: سایت شخصی محسن بحرزاده (PersonalSite)

## هدف
سایت شخصی با ASP.NET Core 10 (Razor Pages) شبیه به طراحی `../index.html` (تم تیره، RTL،
فونت Vazirmatn) که به‌جای کارت‌های استاتیک، مطالب (دوره‌ها) را از دیتابیس می‌خواند.
کاربران می‌توانند ثبت‌نام/ورود کنند، به هر مطلب امتیاز (۱ تا ۵ ستاره) بدهند و نظر بگذارند.
خود محسن (ادمین) از پنل `/Admin` مطالب را مدیریت می‌کند و نظرات را تأیید/حذف می‌کند.

## ساختار پروژه
این فایل و کد داخل همین ریشه‌ی ریپازیتوری (`PersonalSite/`) قرار دارند.

- `Data/` — `ApplicationUser` (IdentityUser + DisplayName)، `Post`، `Comment`، `Rating`،
  `ApplicationDbContext`، `SeedData` (ادمین + ۷ پست نمونه از دوره‌های موجود)
- `Pages/Index.cshtml(.cs)` — صفحه اصلی: hero + گرید پست‌ها از DB + درباره من + تماس
- `Pages/Posts/Detail.cshtml(.cs)` — نمایش یک مطلب + امتیازدهی (`OnPostRateAsync`) +
  نظرات (`OnPostCommentAsync`)، مسیر: `/Posts/{slug}`
- `Pages/Admin/` — `Index` (لیست/انتشار/حذف مطلب)، `Edit` (فرم ساخت/ویرایش، مسیر
  `/Admin/Edit/{id?}`)، `Comments` (تأیید/پنهان/حذف نظر) — همه با پالیسی Authorization
  به نام `"Admin"` محافظت شده‌اند (نگاه کن `Program.cs`: `AuthorizeFolder("/Admin", "Admin")`)
- `Areas/Identity/Pages/Account/Register.cshtml(.cs)` — override دستی صفحه ثبت‌نام
  پیش‌فرض Identity UI برای افزودن فیلد «نام نمایشی». صفحه Login از RCL پیش‌فرض
  Identity.UI استفاده می‌کند (نیازی به override نبود، تم از طریق CSS اعمال می‌شود).
- `wwwroot/css/site.css` — تم کامل (متغیرهای رنگ از `index.html` اصلی کپی شده) + شیم‌های
  کلاس‌های Bootstrap (`.form-control`، `.card`، `.container/.row/[class*="col-"]`...)
  چون صفحات Identity RCL از این کلاس‌ها استفاده می‌کنند ولی bootstrap.css خودش لینک نشده.

### دوره ویدئویی (Post.Kind == VideoCourse)
هر `Post` می‌تواند «مقاله» یا «دوره ویدئویی» باشد (`Data/PostKind.cs`). برای دوره ویدئویی:

- `Data/VideoEpisode.cs` — قسمت‌های دوره: `Title`، `Description` (توضیح این‌که این قسمت
  در مورد چیست)، `VideoFileName`، `DurationSeconds`، `Order`. `Post.Body` برای دوره
  ویدئویی به‌عنوان «معرفی کلی دوره» استفاده می‌شود (همون فیلد قبلی، فقط لیبل عوض شده).
- `Data/Mp4DurationReader.cs` — مدت زمان ویدیو را **خودکار** از فایل mp4 آپلودشده
  می‌خواند، بدون نیاز به ffmpeg یا هیچ ابزار خارجی. باکس‌های ISO-BMFF را پیمایش می‌کند
  تا `moov/mvhd` را پیدا کند و از `duration/timescale` مدت را حساب می‌کند. اگر فایل
  ساختار قابل‌تشخیص نداشت (خیلی نادر)، ادمین می‌تواند مدت را دستی وارد کند
  (`Input.ManualDurationSeconds` در `Admin/Episodes`).
- `Pages/Admin/Episodes.cshtml(.cs)` — مسیر `/Admin/Episodes/{postId}`: افزودن قسمت
  (آپلود ویدیو multipart + استخراج خودکار مدت)، حذف، جابه‌جایی ترتیب (بالا/پایین).
  فایل‌ها در `wwwroot/videos/{slug}/{guid}.mp4` ذخیره می‌شوند.
- `Pages/Admin/Edit.cshtml(.cs)` — انتخاب نوع مطلب (مقاله/دوره ویدئویی). بعد از ذخیره‌ی
  یک دوره ویدئویی جدید، خودکار به `/Admin/Episodes/{id}` هدایت می‌شود.
- `Pages/Posts/Detail.cshtml` — برای دوره ویدئویی، بعد از معرفی کلی، لیست قسمت‌ها را
  به‌صورت `<details>` آکاردئونی نشان می‌دهد (عنوان + توضیح + مدت + پلیر `<video>`)،
  و در بالای صفحه «تعداد قسمت + مجموع مدت دوره» را نشان می‌دهد.
- `Pages/Index.cshtml` — کارت‌های دوره ویدئویی روی صفحه اصلی یک بج «دوره ویدئویی» +
  تعداد قسمت/مدت کل نشان می‌دهند.
- محدودیت آپلود در `Program.cs` تا ۱ گیگابایت افزایش یافته (`KestrelServerOptions`,
  `FormOptions`, و attribute های `[RequestSizeLimit]`/`[RequestFormLimits]` روی
  `EpisodesModel` — این attributeها فقط روی کلاس PageModel جواب می‌دهند، نه روی
  متد handler به‌تنهایی، همون محدودیتی که برای `[Authorize]` هم صادق بود).

## نحوه اجرا
```bash
cd PersonalSite
dotnet run
```
روی `http://localhost:5129` بالا می‌آید (نگاه کن `Properties/launchSettings.json`).
دیتابیس روی **SQL Server** (`Microsoft.EntityFrameworkCore.SqlServer`) با اولین اجرا
خودکار migrate و seed می‌شود (`SeedData.InitializeAsync` در `Program.cs`).

**نکته مهم:** پروژه اولاً با SQLite ساخته شد، بعد به‌درخواست کاربر به SQL Server سوییچ
شد. کانکشن استرینگ پیش‌فرض در `appsettings.json` از **LocalDB** استفاده می‌کند
(نصب‌شده روی این سیستم — تأیید شد با `sqllocaldb info`):
```
Server=(localdb)\mssqllocaldb;Database=PersonalSite;Trusted_Connection=True;MultipleActiveResultSets=true
```
اگر روی سیستم دیگری اجرا می‌شود و SQL Server واقعی (نه LocalDB) در دسترس است، این
کانکشن استرینگ را عوض کن (مثلاً `Server=localhost;Database=PersonalSite;User Id=sa;Password=...;TrustServerCertificate=True`).
LocalDB به‌صورت خودکار instance را در اولین اتصال می‌سازد، نیازی به کار دستی نیست.

**کاربر ادمین seed‌شده:**
- ایمیل: `mohsen.bahrzadeh@gmail.com`
- رمز: قبلاً به‌صورت ثابت `Admin@12345` در کد بود — چون پروژه قرار بود روی گیت‌هاب
  عمومی برود، این مورد **رفع شد**. الان `SeedData.InitializeAsync` مقدار
  `AdminSeed:Password` را از تنظیمات می‌خواند؛ اگر ست نشده باشد یک رمز تصادفی
  می‌سازد و در لاگ برنامه چاپ می‌کند (نگاه کن `Data/SeedData.cs`). برای تنظیم دستی:
  `dotnet user-secrets set "AdminSeed:Password" "<رمز دلخواه>"`

## مهاجرت دیتابیس
```bash
dotnet ef migrations add <Name>
dotnet ef database update
```
پوشه migrations در `Migrations/` (نه `Data/Migrations/`) است چون بدون `--output-dir`
تولید شده — همینجا نگه‌داشته شده، مهم نیست کجاست تا وقتی consistent بماند.

## قراردادهای نام‌گذاری
- Razor Page class names: `XxxModel` مطابق نام فایل (`EditModel`, `DetailModel`, ...)
- Handler methods: `OnGet/OnPostAsync` + نام handler برای اکشن‌های چندگانه در یک صفحه
  (مثلاً `OnPostRateAsync`, `OnPostCommentAsync`, `OnPostTogglePublishAsync`)
- متن UI فارسی، نام‌های کلاس CSS و شناسه‌ها انگلیسی (مثل `index.html` اصلی)
- رنگ‌بندی کارت‌ها با کلاس `c0`..`c6` بر اساس `Post.AccentIndex % 6`

## تست دستی انجام‌شده (این سشن)
- ثبت‌نام کاربر عادی → ورود خودکار → صفحه اصلی ✅
- امتیازدهی به یک پست به‌عنوان کاربر لاگین‌شده ✅
- ثبت نظر روی یک پست ✅
- ورود به‌عنوان ادمین seed‌شده ✅
- `/Admin/Index` و `/Admin/Comments` فقط برای نقش Admin در دسترس‌اند (redirect به Login
  برای کاربر ناشناس، قبلاً یک باگ پالیسی داشت که رفع شد) ✅
- ویرایش یک پست از `/Admin/Edit/{id}` و مشاهده تغییر در صفحه عمومی ✅
- ساخت دوره ویدئویی از `/Admin/Edit` → ریدایرکت به `/Admin/Episodes/{id}` ✅
- آپلود قسمت (multipart/form-data) با فایل mp4 ساختگی ۳۰ ثانیه‌ای → مدت زمان به‌درستی
  و کاملاً خودکار `0:30` تشخیص داده شد (بدون ffmpeg) ✅
- نمایش قسمت‌ها + مدت کل در صفحه عمومی پست و بج «دوره ویدئویی» در صفحه اصلی ✅
- حذف پست دوره‌ای → cascade delete قسمت‌ها از دیتابیس + حذف کامل پوشه
  `wwwroot/videos/{slug}/` روی دیسک (در `Admin/Index.OnPostDeleteAsync`)
- سوییچ به SQL Server: migration تازه تولید و روی LocalDB اجرا شد، صفحه اصلی و seed
  (۷ پست + کاربر ادمین) درست از SQL Server خوانده شدند ✅

⚠️ نکته تست: هنگام تست دستی با curl روی چند فرم antiforgery در یک صفحه (مثلاً نویگیشن +
فرم اصلی)، حتماً `head -1` روی خروجی grep بزن — وگرنه دو توکن با newline به هم می‌چسبند و
درخواست با 400 رد می‌شود (این خودِ اپ باگ ندارد، مشکل تست دستی بود).

## چک‌لیست

- [x] اسکلت پروژه (Razor Pages + Identity + EF Core، net10.0)
- [x] سوییچ provider از SQLite به SQL Server (LocalDB) + migration تازه + تست end-to-end
- [x] مدل‌های Post / Comment / Rating + DbContext + Migration اول
- [x] تم تیره/RTL در `_Layout.cshtml` + `site.css` (پورت‌شده از `index.html`)
- [x] صفحه اصلی داینامیک (گرید پست‌ها از DB)
- [x] صفحه جزئیات پست + ستاره امتیاز + نظرات
- [x] ثبت‌نام سفارشی (با نام نمایشی) + Login پیش‌فرض themed
- [x] پنل ادمین: لیست/ساخت/ویرایش/حذف/انتشار پست + مدیریت نظرات
- [x] Seed داده (ادمین + ۷ پست) + build سبز + تست دستی end-to-end
- [x] این فایل PROGRESS.md
- [x] پشتیبانی از «دوره ویدئویی»: مدل VideoEpisode + PostKind
- [x] استخراج خودکار مدت زمان ویدیو از mp4 (بدون ffmpeg) + تست شده
- [x] پنل ادمین برای مدیریت قسمت‌های دوره (آپلود/حذف/ترتیب)
- [x] نمایش قسمت‌ها + مدت کل در صفحه پست و صفحه اصلی
- [x] افزایش محدودیت حجم آپلود (تا ۱ گیگابایت) + build و تست end-to-end
- [x] حذف رمز ثابت ادمین از کد (قبل از انتشار عمومی) + `.gitignore` + `README.md` فارسی
- [x] انتشار عمومی روی گیت‌هاب

## کارهای آتی (اختیاری، انجام‌نشده)
- [ ] صفحه Manage/Account (تغییر رمز و ...) هنوز از RCL پیش‌فرض است — تست نشد
  به‌صورت مجزا؛ چون از همان `_Layout` استفاده می‌کند احتمالاً OK است ولی چک نشد.
- [ ] deploy واقعی (IIS/Linux service/Docker) انجام نشده — فقط dev (`dotnet run`) تست شد.
- [ ] آواتار/عکس واقعی به‌جای ایموجی 👨‍💻 در hero (در صورت تمایل کاربر).
- [ ] فرمت‌های ویدیویی غیر از mp4/mov (مثلاً webm) توسط `Mp4DurationReader` پشتیبانی
  نمی‌شوند — برای آن‌ها مدت باید دستی وارد شود.
