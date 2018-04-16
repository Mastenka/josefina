using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.IO;
using System.Net;
using System.Drawing;
using System.IO.Compression;
using Josefina.Entities;
using PdfSharp.Drawing.Layout;

namespace Josefina.Helpers
{
    public class TicketPDFGenerator 
    {
        private const int LINE_HEIGHT = 13;
        private const double PIXEL_CONVERT = 0.7;
        private const int QR_SIZE = 140;
        private const int LEFT_BOARDER = 35;

        public static void GetPDFTicket(List<TicketToGenerateWrapper> ticketsToExport, TicketSetting settings)
        {
            foreach (var ticketToExport in ticketsToExport)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PdfDocument document = new PdfDocument();
                    document.Info.Title = "";
                    PdfPage page = document.AddPage();

                    // Get an XGraphics object for drawing
                    XGraphics gfx = XGraphics.FromPdfPage(page);

                    //Logo 

                    double logoHeigh;
                    double logoWidth;

                    if (settings.Logo != null)
                    {
                        using (var ms = new MemoryStream(settings.Logo))
                        {
                            using (Image logoImg = Image.FromStream(ms))
                            {
                                XImage logo = XImage.FromGdiPlusImage(logoImg);
                                gfx.DrawImage(logo, LEFT_BOARDER, 0);

                                logoHeigh = logoImg.Height;
                                logoWidth = logoImg.Width;
                            }
                        }
                    }
                    else
                    {
                        logoHeigh = 0;
                        logoWidth = 600;
                    }

                    //QR
                    WebClient client = new WebClient();
                    client.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/535.1 (KHTML, like Gecko) Chrome/14.0.835.202 Safari/535.1";
                    client.Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                    string url = string.Format("http://chart.googleapis.com/chart?cht=qr&chs=184x184&chl={0}&choe=UTF-8&chld=L", ticketToExport.QRCode);
                    byte[] data = client.DownloadData(url);
                    var memoryStream2 = new MemoryStream(data);
                    Image qrImg = new Bitmap(memoryStream2);
                    memoryStream2.Close();
                    XImage qr = XImage.FromGdiPlusImage(qrImg);
                    gfx.DrawImage(qr, LEFT_BOARDER, logoHeigh * PIXEL_CONVERT);

                    double rowPointer = logoHeigh * PIXEL_CONVERT + 20;
                    XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
                    // Create a font
                    XFont font = new XFont("Verdana", 10, XFontStyle.Bold, options);
                    // Draw the text
                    if (!string.IsNullOrEmpty(ticketToExport.StartDate))
                    {
                        gfx.DrawString(ticketToExport.StartDate, font, XBrushes.Black, new XRect(LEFT_BOARDER + QR_SIZE, rowPointer, logoWidth - QR_SIZE, LINE_HEIGHT), XStringFormats.TopLeft);
                        rowPointer += LINE_HEIGHT;
                    }

                    if (!string.IsNullOrEmpty(ticketToExport.Location))
                    {
                        gfx.DrawString(ticketToExport.Location, font, XBrushes.Black, new XRect(LEFT_BOARDER + QR_SIZE, rowPointer, logoWidth - QR_SIZE, LINE_HEIGHT), XStringFormats.TopLeft);
                        rowPointer += LINE_HEIGHT;
                    }

                    gfx.DrawString(ticketToExport.CategoryName, font, XBrushes.Black, new XRect(LEFT_BOARDER + QR_SIZE, logoHeigh * PIXEL_CONVERT + 20 + (LINE_HEIGHT * 2), logoWidth - QR_SIZE, LINE_HEIGHT), XStringFormats.TopLeft);
                    rowPointer += LINE_HEIGHT * 2;

                    font = new XFont("Verdana", 10, XFontStyle.Regular, options);

                    if (!string.IsNullOrEmpty(ticketToExport.VisitorName))
                    {
                        gfx.DrawString(ticketToExport.VisitorName, font, XBrushes.Black, new XRect(LEFT_BOARDER + QR_SIZE, rowPointer, logoWidth - QR_SIZE, LINE_HEIGHT), XStringFormats.TopLeft);
                        rowPointer += LINE_HEIGHT;
                    }

                    gfx.DrawString(ticketToExport.VisitorEmail, font, XBrushes.Black, new XRect(LEFT_BOARDER + QR_SIZE, rowPointer, logoWidth - QR_SIZE, LINE_HEIGHT), XStringFormats.TopLeft);

                    rowPointer += LINE_HEIGHT * 2.5;

                    gfx.DrawString(ticketToExport.Code, font, XBrushes.Black, new XRect(LEFT_BOARDER + QR_SIZE, logoHeigh * PIXEL_CONVERT + QR_SIZE - LINE_HEIGHT - 20, logoWidth - QR_SIZE, LINE_HEIGHT), XStringFormats.TopLeft);
                    rowPointer += LINE_HEIGHT;

                    XTextFormatter tf = new XTextFormatter(gfx);
                    tf.Alignment = XParagraphAlignment.Left;

                    if (!string.IsNullOrEmpty(ticketToExport.Note))
                    {
                        tf.DrawString(ticketToExport.Note, font, XBrushes.Black, new XRect(LEFT_BOARDER, rowPointer + 10, 500, 600), XStringFormats.TopLeft);
                        rowPointer += LINE_HEIGHT;
                    }

                    document.Save(memoryStream);
                    ticketToExport.PDFTicket = memoryStream.ToArray();
                }
            }
        }

        public static byte[] GetZippedPdfTickets(List<TicketToGenerateWrapper> ticketsToExport, TicketSetting settings)
        {
            GetPDFTicket(ticketsToExport, settings);
            using (MemoryStream outStream = new MemoryStream())
            {
                using (ZipArchive archive = new ZipArchive(outStream, ZipArchiveMode.Create, false))
                {
                    foreach (var ticket in ticketsToExport)
                    {
                        ZipArchiveEntry ticketEntry = archive.CreateEntry(ticket.Code + ".pdf", CompressionLevel.Fastest);

                        //Get the stream of the attachment
                        using (var originalFileStream = new MemoryStream(ticket.PDFTicket))
                        {
                            using (var zipEntryStream = ticketEntry.Open())
                            {
                                //Copy the attachment stream to the zip entry stream
                                originalFileStream.CopyTo(zipEntryStream);
                            }
                        }
                    }
                }
                return outStream.ToArray();
            }
        }

        public class TicketToGenerateWrapper
        {
            public byte[] PDFTicket { get; set; }

            public string ProjectName { get; set; }

            public string StartDate { get; set; }

            public string Location { get; set; }

            public string CategoryName { get; set; }

            public string VisitorName { get; set; }

            public string VisitorEmail { get; set; }

            public string Code { get; set; }

            public string QRCode { get; set; }

            public string Note { get; set; }
        }
    }
}