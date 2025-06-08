using System.Text;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Lr.UI.AnsiConsoleUi.Renders;

/// <summary>
/// Украдено из интернетов, переписать
/// </summary>
public class MarkdownRenderable(string markdown) : Renderable
{
    protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
    {
        var ast = Markdown.Parse(markdown);
        foreach (var block in ast)
        {
            var r = RenderBlock(block).Render(options, maxWidth);
            foreach (var segment in r)
            {
                yield return segment;
            }

            yield return Segment.LineBreak;
        }
    }

    private IRenderable RenderBlock(Block block)
    {
        return block switch
        {
            HtmlBlock html when IsThinking(html) => BuildThinkControl(html),
            ParagraphBlock paragraph => CreateTextBlock(paragraph),
            HeadingBlock heading => BuildingHeadingBlock(heading),
            ListBlock list => BuildListItemControl(list),
            QuoteBlock quoteBlock => BuildQuoteControl(quoteBlock),
            FencedCodeBlock codeBlock => BuildCodeControl(codeBlock),
            CodeBlock codeBlock => BuildCodeControl(codeBlock),
            ThematicBreakBlock => new Grid() { Width = 120 }.AddColumn().AddRow(new Rule()),
            _ => new Paragraph()
        };
    }

    private static bool IsThinking(HtmlBlock htmlBlock)
    {
        var str = htmlBlock.Lines.ToString();
        return str.Contains("think") || str.Contains("</think>");
    }

    private IRenderable BuildingHeadingBlock(HeadingBlock heading)
    {
        var sb = new StringBuilder();
        if (heading.Inline != null)
        {
            AddInlineContent(sb, heading.Inline);
        }

        var content = sb.ToString().EscapeMarkup();

        switch (heading.Level)
        {
            case 1:
                return new FigletText(content);
            case > 4:
                return new Text(content, new Style(foreground: Color.Yellow));
        }

        var rowChar = heading.Level == 2 ? '=' : '-';

        var headerRow = new Text(content, new Style(foreground: Color.Yellow));
        var rowRow = new Text(new string(rowChar, content.Length), new Style(foreground: Color.Yellow));

        return new Rows(headerRow, rowRow);
    }

    private IRenderable CreateTextBlock(LeafBlock textBlock)
    {
        var sb = new StringBuilder();
        if (textBlock.Inline != null)
        {
            AddInlineContent(sb, textBlock.Inline);
        }

        var grid = new Grid { Width = 120, }
            .AddColumn()
            .AddRow(new Markup(sb.ToString()));

        return grid;
    }

    private IRenderable BuildCodeControl(CodeBlock codeBlock)
    {
        var sb = new StringBuilder();
        var lines = codeBlock.Lines.Lines;

        foreach (var line in lines)
        {
            sb.AppendLine(line.ToString());
        }

        var panel = new Panel(sb.ToString().EscapeMarkup().Trim())
        {
            Border = new LeftBorder(),
            BorderStyle = new Style(foreground: Color.Blue),
            Padding = new Padding(1, 0, 0, 0),
        };

        if (codeBlock is FencedCodeBlock fencedCodeBlock && !string.IsNullOrEmpty(fencedCodeBlock.Info))
        {
            panel.Header = new PanelHeader(fencedCodeBlock.Info.EscapeMarkup());
        }

        return panel;
    }

    private IRenderable BuildThinkControl(HtmlBlock htmlBlock)
    {
        var sb = new StringBuilder();
        var lines = htmlBlock.Lines.Lines;

        foreach (var line in lines)
        {
            var clean = line.ToString()
                .Replace("<think>", "")
                .Replace("</think>", "")
                .Trim();
            if (!String.IsNullOrEmpty(clean))
            {
                sb.AppendLine(clean);
            }
        }

        var panel = new Panel(new Markup(sb.ToString().Trim().EscapeMarkup(), new Style(Color.Grey50)))
        {
            Border = new LeftBorder(),
            BorderStyle = new Style(foreground: Color.Grey50),
            Padding = new Padding(1, 0, 0, 0),
        };
        return panel;
    }

    private IRenderable BuildQuoteControl(QuoteBlock quoteBlock)
    {
        var sb = new StringBuilder();

        foreach (var block in quoteBlock)
        {
            if (block is ParagraphBlock { Inline: not null } paragraphBlock)
            {
                AddInlineContent(sb, paragraphBlock.Inline);
            }
            else
            {
                var renderable = RenderBlock(block);
                sb.AppendLine(renderable.ToString());
            }
        }

        var panel = new Panel(sb.ToString().TrimEnd())
        {
            Border = new LeftBorder(),
            BorderStyle = new Style(Color.Green),
            Padding = new Padding(1, 1)
        };

        return panel;
    }

    private IRenderable BuildListItemControl(ListBlock listBlock)
    {
        var table = new Table().HideHeaders().Border(TableBorder.None);

        table.AddColumn(new TableColumn("Marker").Width(3));
        table.AddColumn(new TableColumn("Content"));

        var index = 1;

        foreach (var item in listBlock)
        {
            if (item is ListItemBlock listItem)
            {
                string marker = listBlock.IsOrdered ? $"{index}." : "\u25cb";
                index++;

                // Create a list of renderables for all blocks in the list item
                var contentRenderables = new List<IRenderable>();

                foreach (var block in listItem)
                {
                    // Recursively render each block and add it to the list
                    var blockRenderable = RenderBlock(block);
                    contentRenderables.Add(blockRenderable);
                }

                // Use Rows to combine all the renderables
                var contentRows = new Rows(contentRenderables);

                // Add the marker and content to the table
                table.AddRow(new Text(marker, new Style(foreground: Color.Green)), contentRows);
            }
        }

        return table;
    }

    private void AddInlineContent(StringBuilder sb, ContainerInline textBlockInline)
    {
        foreach (var inline in textBlockInline)
        {
            switch (inline)
            {
                case CodeInline code:
                    sb.Append("[fuchsia]`");
                    sb.Append(code.Content.EscapeMarkup());
                    sb.Append("`[/]");
                    break;
                case LiteralInline literal:
                    sb.Append(literal.Content.ToString().EscapeMarkup());
                    break;

                case EmphasisInline emphasis:
                    switch (emphasis.DelimiterCount)
                    {
                        case 1:
                            sb.Append("[yellow]");
                            break;
                        case 2:
                            sb.Append("[blue]");
                            break;
                    }

                    AddInlineContent(sb, emphasis);
                    sb.Append("[/]");
                    break;

                case LineBreakInline:
                    sb.Append(" ");
                    break;

                default:
                    if (inline is ContainerInline containerInline)
                    {
                        AddInlineContent(sb, containerInline);
                    }

                    break;
            }
        }
    }
}

class LeftBorder : BoxBorder
{
    public override string GetPart(BoxBorderPart part)
    {
        if (part is BoxBorderPart.Left)
        {
            return "\u2502";
        }

        return " ";
    }
}
