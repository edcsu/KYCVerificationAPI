using KYCVerificationAPI.Features.Vendors.Responses;
using KYCVerificationAPI.Features.Verifications.Responses;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace KYCVerificationAPI.Features.Verifications.Reports.Templates;

public class ComplianceDocument: IDocument
{
    public ComplianceResponse Model { get; }

    public ComplianceDocument(ComplianceResponse model)
    {
        Model = model;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.MarginHorizontal(30);
                page.MarginVertical(30);
            
                page.Header().Element(ComposeHeader);
                
                page.Content().Element(ComposeContent);

                page.Footer().Element(ComposeFooter);
            });
    }

    private void ComposeHeader(IContainer container)
    {
        var logo = File.ReadAllBytes(@"wwwroot/images/logo.jpeg");
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item()
                    .Text($"Compliance Verification Report")
                    .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                column.Item().Text(text =>
                {
                    text.Span("Issue date: ").SemiBold();
                    text.Span($"{DateTime.UtcNow:d}");
                });
                
                column.Item().Text(text =>
                {
                    text.Span("Issued By: ").SemiBold();
                    text.Span($"{Model.MadeBy}");
                });
                
                column.Item().Text(text =>
                {
                    text.Span("Total Requests: ").SemiBold();
                    text.Span($"{Model.VerificationResponses.Count()}");
                });
            });

            row.ConstantItem(100).Height(70).Image(logo);
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.PaddingVertical(40).Column(column =>
        {
            column.Spacing(5);

            column.Item().Element(ComposeTable);
        });
    }
    
    private void ComposeTable(IContainer tableContainer)
    {
        tableContainer.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(75);
                columns.ConstantColumn(240);
                columns.ConstantColumn(140);
                columns.ConstantColumn(60);
            });
        
            table.Header(header =>
            {
                header.Cell().Element(CellStyle).Text("#");
                header.Cell().Element(CellStyle).Text("Created by");
                header.Cell().Element(CellStyle).Text("Created at");
                header.Cell().Element(CellStyle).Text("Kyc Status");
                return;

                static IContainer CellStyle(IContainer container)
                {
                    return container
                        .DefaultTextStyle(x => x.SemiBold())
                        .PaddingVertical(5)
                        .BorderBottom(1)
                        .BorderColor(Colors.Black);
                }
            });
        
            foreach (var (index, response) in Model.VerificationResponses.Index())
            {
                table.Cell().Element(CellStyle).Text(response.TransactionId.ToString()[^10..]);
                table.Cell().Element(CellStyle).Text($"{response.CreatedBy}");
                table.Cell().Element(CellStyle).Text($"{response.CreatedAt}");
                switch (response.Data.KycStatus)
                {
                    case KycStatus.Ok:
                    {
                        table.Cell().Element(CellStyle).AlignCenter().Text($"{response.Data.KycStatus}").FontColor(Colors.Green.Medium);
                        break;
                    }
                    case KycStatus.Failed:
                    {
                        table.Cell().Element(CellStyle).AlignCenter().Text($"{response.Data.KycStatus}").FontColor(Colors.Orange.Medium);
                        break;
                    }
                    case KycStatus.Error:
                    {
                        table.Cell().Element(CellStyle).AlignCenter().Text($"{response.Data.KycStatus}").FontColor(Colors.Red.Medium);
                        break;
                    }
                    default:
                    {
                        table.Cell().Element(CellStyle).AlignCenter().Text($"{response.Data.KycStatus}");
                        break;
                    }
                }
                continue;

                IContainer CellStyle(IContainer container)
                {
                    var backgroundColor = index % 2 == 0 
                        ? Colors.White 
                        : Colors.Grey.Lighten2;
                    
                    return container
                        .Background(backgroundColor)
                        .BorderBottom(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .PaddingVertical(5)
                        .PaddingHorizontal(2);
                }
            }
        });
    }
    
    private static void ComposeFooter(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text(text =>
                {
                    text.Span($"© Copyright {DateTime.UtcNow.Year} uverify");
                });
            });

            row.ConstantItem(100).AlignRight().Text(x =>
            {
                x.CurrentPageNumber();
                x.Span(" / ");
                x.TotalPages();
            });
        });
    }
}