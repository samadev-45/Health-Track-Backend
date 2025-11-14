using Health.Application.Common;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Health.Infrastructure.Utilities
{
    public class QuestPdfGenerator : IPdfGenerator
    {
        public async Task<byte[]> GeneratePrescriptionPdfAsync(PrescriptionPdfModel model, CancellationToken ct = default)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text($"Prescription — {model.Date:dd MMM yyyy}")
                        .SemiBold().FontSize(18);

                    page.Content()
                        .Column(col =>
                        {
                            col.Item().Text($"Patient: {model.PatientName}").Bold();
                            col.Item().Text($"Doctor: {model.DoctorName}");
                            col.Item().Text($"Diagnosis: {model.Diagnosis}");
                            col.Item().Text(" ");

                            if (model.Medicines.Count > 0)
                            {
                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(2);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Text("Medicine").Bold();
                                        header.Cell().Text("Dose").Bold();
                                        header.Cell().Text("Freq").Bold();
                                        header.Cell().Text("Days").Bold();
                                        header.Cell().Text("Notes").Bold();
                                    });

                                    foreach (var m in model.Medicines)
                                    {
                                        table.Cell().Text(m.Name);
                                        table.Cell().Text(m.Dose);
                                        table.Cell().Text(m.Frequency);
                                        table.Cell().Text(m.Duration);
                                        table.Cell().Text(m.Notes);
                                    }
                                });
                            }

                            col.Item().Text(" ");
                            col.Item().Text($"Advice: {model.Notes}");
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text($"Doctor Signature: {model.Signature}");
                });
            });

            using var ms = new MemoryStream();
            document.GeneratePdf(ms);
            return ms.ToArray();
        }
    }
}
