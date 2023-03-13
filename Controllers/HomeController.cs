using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using iText.Html2pdf;
using iText.Html2pdf.Resolver.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout.Font;
using Microsoft.AspNetCore.Mvc;
using pdftohtml.Models;

namespace pdftohtml.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IWebHostEnvironment environment;

    public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        this.environment = environment;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult PdfFile(string html)
    {
        byte[] stringBytes = Convert.FromBase64String(html);
        html = Encoding.ASCII.GetString(stringBytes);

        FontProvider defaultFontProvider;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            defaultFontProvider = new DefaultFontProvider(true, true, false, "Arial");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            defaultFontProvider = new DefaultFontProvider(true, true, false, "Sans-serif");
        }
        else
        {
            defaultFontProvider = new DefaultFontProvider(true, true, false);
        }

        ConverterProperties props = new ConverterProperties();
        props.SetFontProvider(defaultFontProvider);

        
        var fileInfo = TempFile();
        using var pdfWriter = new PdfWriter(fileInfo);
        using PdfDocument pdfDocument = new PdfDocument(pdfWriter);
        pdfDocument.SetDefaultPageSize(PageSize.A4);

        HtmlConverter.ConvertToPdf(html, pdfDocument, props);

        return Content($"temp/{fileInfo.Name}");

        FileInfo TempFile()
        {
            string tempFileName = System.IO.Path.GetRandomFileName() + ".pdf";
            string tempPath = System.IO.Path.Combine(environment.WebRootPath, "temp");
            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            string tempFilePath = System.IO.Path.Combine(tempPath, tempFileName);

            var fileInfo = new FileInfo(tempFilePath);
            return fileInfo;
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
